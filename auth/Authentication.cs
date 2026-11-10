using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TIL.Client;
using TIL.DataContainers;
using TIL.Exceptions;
using TIL.Network.Responses;
using TIL.Serialization;
using TIL.TwitchAPI;
using TIL.TwitchAPI.EventSubs;

namespace TIL.Auth;

static class Authentication
{
    private static HttpClient httpClient = new();

    /// <summary>
    /// Requests Device Access Token from Twitch or attempts to use locally saved token.
    /// </summary>
    /// <param name="twitchSessionContext">Current Session Context</param>
    /// <returns>A working Device Token.</returns>
    /// <exception cref="FailedToDeserializeTokenException"></exception>
    /// <exception cref="InvalidAccessTokenException"></exception>
    /// <exception cref="HttpRequestException"></exception>
    public static async Task<TwitchDeviceToken?> GetDeviceAccessTokenAsync(
        TwitchSessionContext twitchSessionContext,
        CancellationToken cancellationToken = default
    )
    {
        if (HasMissingScopes(twitchSessionContext, out IEventSub[] missingEventSubs))
        {
            throw new MissingScopesException(
                "Missing scopes for the following EventSubs:\n" +
                string.Join("\n", missingEventSubs.Select(e =>
                {
                    var requirement = e.GetScopeRequirement();

                    var missingScopes = requirement.Scopes
                        .Where(scope => !twitchSessionContext.scopes.Contains(scope));

                    string separator = requirement.Mode switch
                    {
                        Scopes.Mode.All => " and ",
                        Scopes.Mode.Any => " or ",
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    return $"- {e}: {string.Join(separator, missingScopes)}";
                }))
            );
        }

        Console.WriteLine("Beginning Retrieving Device Token...");

        TwitchDeviceToken? twitchDeviceToken = null;

        try
        {
            Console.WriteLine("Searching for saved Device Token...");
            twitchDeviceToken = DeviceTokenSerializer.LoadDeviceToken();
        }
        catch (UnauthorizedAccessException e)
        {
            Console.WriteLine($"Permission denied loading device token: ${e.Message}");
        }
        catch (IOException e)
        {
            Console.WriteLine($"File system error loading device token: {e.Message}");
        }
        catch (JsonException e)
        {
            Console.WriteLine($"JSON serialization error loading device token: {e.Message}");
        }

        // Attempt to validate and refresh device token
        if (twitchDeviceToken is not null)
        {
            Console.WriteLine("Checking saved Device Token...");

            if (DetectNewScopesAdded(twitchSessionContext, twitchDeviceToken))
            {
                Console.WriteLine("New scopes have since been added - requesting a new Token...");
            }
            else
            {
                if (DateTimeOffset.UtcNow >= twitchDeviceToken.ExpiresAt)
                {
                    Console.WriteLine("Device token has expired.");
                }
                else
                {
                    if (await ValidateToken(twitchDeviceToken.AccessToken, cancellationToken))
                    {
                        Console.WriteLine("Saved Device Token is valid.");
                        return twitchDeviceToken;
                    }

                    Console.WriteLine("Saved Device Token is invalid or revoked.");
                }

                if (!string.IsNullOrEmpty(twitchDeviceToken.RefreshToken))
                {
                    Console.WriteLine("Attempting to refresh Device Token...");

                    try
                    {
                        twitchDeviceToken = await RefreshDeviceToken
                        (
                            twitchSessionContext.clientId,
                            twitchDeviceToken.RefreshToken,
                            cancellationToken
                        );

                        return twitchDeviceToken;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Token refresh failed: {e.Message}");
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("No saved Device Token found.");
        }

        httpClient = new HttpClient();

        FormUrlEncodedContent postData = new(new Dictionary<string, string>
        {
            ["client_id"] = twitchSessionContext.clientId!,
            ["scopes"] = string.Join(" ", twitchSessionContext.scopes)
        });

        Console.WriteLine("Requesting new Device Flow Auth...");

        using HttpResponseMessage response = await httpClient.PostAsync(
            new Uri("https://id.twitch.tv/oauth2/device"), postData, cancellationToken
        );

        string responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

        // Guard no success - exception exit early
        response.EnsureSuccessStatusCode();

        DeviceCodeRequestResponse deviceResponse = JsonSerializer.Deserialize<DeviceCodeRequestResponse>(responseJson)!;

        // Open in browser
        using Process browserProcess = new();
        browserProcess.StartInfo.UseShellExecute = true;
        browserProcess.StartInfo.FileName = deviceResponse.VerificationUri;
        browserProcess.Start();

        Console.WriteLine("Polling device code for user completion...");

        // TODO handle exceptions
        twitchDeviceToken = await PollDeviceCodeFlowAsync(
            twitchSessionContext.clientId!,
            deviceResponse,
            cancellationToken
        );

        if (string.IsNullOrWhiteSpace(twitchDeviceToken.AccessToken))
        {
            throw new InvalidAccessTokenException("Device Flow Response is missing access_token.");
        }

        if (string.IsNullOrWhiteSpace(twitchDeviceToken.RefreshToken))
        {
            Console.WriteLine($"Refresh successful but refresh token was not provided. User will need to reauth at: {twitchDeviceToken.ExpiresAt}");
        }

        DeviceTokenSerializer.SaveDeviceToken(twitchDeviceToken);

        return twitchDeviceToken;
    }

    /// <exception cref="FailedToDeserializeTokenException"></exception>
    /// <exception cref="InvalidAccessTokenException"></exception>
    /// <exception cref="InvalidOperationException">Device Token Polling failed.</exception>
    private static async Task<TwitchDeviceToken> PollDeviceCodeFlowAsync(
        string clientId, DeviceCodeRequestResponse
        deviceCodeResponse,
        CancellationToken cancellationToken
    )
    {
        if (deviceCodeResponse.DeviceCode is null)
        {
            throw new ArgumentNullException("Device Code Response contains a null device_code.");
        }

        DateTime startedPollingAt = DateTime.UtcNow;
        int pollingRateSeconds = deviceCodeResponse.Interval;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if ((DateTime.UtcNow - startedPollingAt).TotalSeconds >= deviceCodeResponse.ExpiresIn)
            {
                throw new DeviceCodeExpiredExceptionException("Device code expired (timed out). Start the device flow again.");
            }

            using FormUrlEncodedContent postData = new(new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["device_code"] = deviceCodeResponse.DeviceCode,
                ["grant_type"] = "urn:ietf:params:oauth:grant-type:device_code"

            });

            using HttpResponseMessage pollingResponse = await httpClient.PostAsync(
                "https://id.twitch.tv/oauth2/token", postData, cancellationToken
            );
            string content = await pollingResponse.Content.ReadAsStringAsync(cancellationToken);

            // Handle successful response
            if (pollingResponse.IsSuccessStatusCode)
            {
                TwitchDeviceToken? twitchDeviceToken = JsonSerializer.Deserialize<TwitchDeviceToken>(content);

                if (twitchDeviceToken is null)
                {
                    throw new FailedToDeserializeTokenException("Unable to deserialize device code polling success response");
                }

                if (twitchDeviceToken.AccessToken is null)
                {
                    throw new InvalidAccessTokenException("Device Flow Response is missing access_token.");
                }

                return twitchDeviceToken;
            }

            // Handle failed response
            TwitchErrorResponse? tokenFailed = JsonSerializer.Deserialize<TwitchErrorResponse>(content);

            if (tokenFailed is null)
            {
                throw new FailedToDeserializeTokenException("Unable to deserialize device code polling failure response.");
            }

            switch (tokenFailed.Message)
            {
                case "authorization_pending":
                    break;
                
                case "slow_down":
                    pollingRateSeconds = checked(pollingRateSeconds + 1);
                    break;

                default:
                    throw new InvalidOperationException($"Device Flow polling failed: {(int)pollingResponse.StatusCode} {pollingResponse.ReasonPhrase}. Response: {content}");
            }

            await Task.Delay(pollingRateSeconds * 1000, cancellationToken);
        }
    }

    /// <exception cref="FailedToDeserializeTokenException"></exception>
    /// <exception cref="InvalidAccessTokenException"></exception>
    /// <exception cref="InvalidOperationException">Device Token Polling failed.</exception>
    public static async Task<TwitchDeviceToken> RefreshDeviceToken(
        string clientId,
        string refreshToken,
        CancellationToken cancellationToken = default
    )
    {
        FormUrlEncodedContent postData = new(new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["client_secret"] = Env.CLIENT_SECRET,
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken
        });

        using HttpResponseMessage refreshResponse = await httpClient.PostAsync(
            new Uri("https://id.twitch.tv/oauth2/token"), postData, cancellationToken
        );
        string content = await refreshResponse.Content.ReadAsStringAsync(cancellationToken);

        if (refreshResponse.IsSuccessStatusCode)
        {
            Console.WriteLine("Refreshed Token Successfully");

            TwitchDeviceToken? twitchDeviceToken = JsonSerializer.Deserialize<TwitchDeviceToken>(content);

            if (twitchDeviceToken is null)
            {
                throw new FailedToDeserializeTokenException("");
            }

            if (twitchDeviceToken.AccessToken is null)
            {
                throw new InvalidAccessTokenException("Device Flow Response is missing access_token.");
            }

            if (twitchDeviceToken.RefreshToken is null)
            {
                Console.WriteLine($"Refresh successful but refresh token was not provided. User will need to reauth at: {twitchDeviceToken.ExpiresAt}");
            }

            DeviceTokenSerializer.SaveDeviceToken(twitchDeviceToken);

            return twitchDeviceToken;
        }

        TwitchErrorResponse? tokenFailed = JsonSerializer.Deserialize<TwitchErrorResponse>(content);

        if (tokenFailed is null)
        {
            throw new FailedToDeserializeTokenException("");
        }

        throw new InvalidOperationException($"Device Flow polling failed: {(int)refreshResponse.StatusCode} {refreshResponse.ReasonPhrase}. Response: {tokenFailed.Message}");
    }

    public static async Task<bool> ValidateToken(string accessToken, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://id.twitch.tv/oauth2/validate"
        );

        request.Headers.Authorization = new("OAuth", accessToken);

        using var response = await httpClient.SendAsync(request, cancellationToken);

        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }

    private static bool DetectNewScopesAdded(TwitchSessionContext twitchSessionContext, TwitchDeviceToken twitchDeviceToken)
    {
        List<string> additionalScopes = new(twitchSessionContext.scopes.Except(twitchDeviceToken.Scopes));
        return additionalScopes.Count > 0;
    }

    private static bool HasMissingScopes(TwitchSessionContext sessionContext, out IEventSub[] missingEventSubs)
    {
        missingEventSubs = sessionContext.eventSubs.Where(e =>
        {
            Scopes.Requirement requirement = e.GetScopeRequirement();

            var missing = requirement.Scopes
                .Where(scope => !sessionContext.scopes.Contains(scope))
                .ToArray();

            return requirement.Mode switch
            {
                Scopes.Mode.All => missing.Length > 0,
                Scopes.Mode.Any => missing.Length == requirement.Scopes.Length,
                _ => throw new ArgumentOutOfRangeException()
            };
        }).ToArray();

        return missingEventSubs.Length > 0;
    }
}