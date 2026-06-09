using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using TIL.Client;
using TIL.DataContainers;
using TIL.Exceptions;
using TIL.Network.Responses;
using TIL.Serialization;

namespace TIL.Auth;

static class Authentication
{
    private static HttpClient httpClient;

    static Authentication()
    {
        httpClient = new HttpClient();
    }

    /// <summary>
    /// Requests Device Access Token from Twitch or attempts to use locally saved token.
    /// </summary>
    /// <param name="twitchSessionContext">Current Session Context</param>
    /// <returns>A working Device Token.</returns>
    /// <exception cref="FailedToDeserializeTokenException"></exception>
    /// <exception cref="InvalidAccessTokenException"></exception>
    /// <exception cref="HttpRequestException"></exception>
    public static async Task<TwitchDeviceToken> GetDeviceAccessTokenAsync(TwitchSessionContext twitchSessionContext)
    {
        Console.WriteLine("Beginning Retrieving Device Token...");

        Console.WriteLine("Searching for saved Device Token...");
        TwitchDeviceToken? twitchDeviceToken = DeviceTokenSerializer.LoadDeviceToken();

        // Attempt to validate and refresh device token
        if (twitchDeviceToken is not null)
        {
            Console.WriteLine("Checking saved Device Token...");

            // Valid Check
            if (!DetectNewScopesAdded(twitchSessionContext, twitchDeviceToken))
            {
                if (DateTime.UtcNow >= twitchDeviceToken.ExpiresAtUtc && twitchDeviceToken.RefreshToken != string.Empty)
                {
                    Console.WriteLine("Device Token Expired. Attempting to refresh...");

                    try
                    {
                        // Refresh Token
                        twitchDeviceToken = await RefreshDeviceToken(twitchSessionContext.client_id!, twitchDeviceToken.RefreshToken);
                        return twitchDeviceToken;
                    }
                    catch (Exception e)
                    {
                        // Let it go into to device auth flow
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    // TODO final check with Twitch if token is valid (could have been revoked)

                    Console.WriteLine("Saved Device Token is valid.");
                    Console.WriteLine($"Using device token: {twitchDeviceToken}");
                    return twitchDeviceToken;
                }
            }
            else
            {
                Console.WriteLine("New scopes have since been added - requesting a new Token...");
            }
        }
        else
        {
            Console.WriteLine("No saved Device Token found.");
        }

        httpClient = new HttpClient();

        FormUrlEncodedContent postData = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = twitchSessionContext.client_id!,
            ["scopes"] = string.Join(" ", twitchSessionContext.scopes!.Select(e => e.GetScope()))
        });

        Console.WriteLine("Requesting new Device Flow Auth...");

        using HttpResponseMessage response = await httpClient.PostAsync(new Uri("https://id.twitch.tv/oauth2/device"), postData);

        string responseJson = await response.Content.ReadAsStringAsync();

        // Guard no success - exception exit early
        response.EnsureSuccessStatusCode();

        DeviceCodeRequestResponse deviceResponse = JsonSerializer.Deserialize<DeviceCodeRequestResponse>(responseJson)!;

        // Open in browser
        using Process browserProcess = new Process();
        browserProcess.StartInfo.UseShellExecute = true;
        browserProcess.StartInfo.FileName = deviceResponse.verification_uri;
        browserProcess.Start();

        Console.WriteLine("Polling device code for user completion...");

        // TODO handle exceptions
        TokenSuccess tokenSuccess = await PollDeviceCodeFlowAsync(twitchSessionContext.client_id!, deviceResponse, CancellationToken.None);

        if (tokenSuccess is null)
        {
            throw new FailedToDeserializeTokenException("");
        }

        DateTime tokenExpiry = DateTime.UtcNow.AddSeconds(tokenSuccess.expires_in);

        if (tokenSuccess.access_token is null)
        {
            throw new InvalidAccessTokenException("Device Flow Response is missing access_token.");
        }

        if (tokenSuccess.refresh_token is null)
        {
            Console.WriteLine($"Refresh successful but refresh token was not provided. User will need to reauth at: {tokenExpiry}");
        }

        twitchDeviceToken = new TwitchDeviceToken
        {
            AccessToken = tokenSuccess.access_token,
            RefreshToken = tokenSuccess.refresh_token ?? "",
            ExpiresAtUtc = tokenExpiry,
            Scopes = tokenSuccess.scope ?? Array.Empty<string>()
        };

        DeviceTokenSerializer.SaveDeviceToken(twitchDeviceToken);

        Cleanup();

        return twitchDeviceToken;
    }

    /// <exception cref="FailedToDeserializeTokenException"></exception>
    /// <exception cref="InvalidAccessTokenException"></exception>
    /// <exception cref="InvalidOperationException">Device Token Polling failed.</exception>
    private static async Task<TokenSuccess> PollDeviceCodeFlowAsync(string clientId, DeviceCodeRequestResponse deviceCodeResponse, CancellationToken cancellationToken)
    {
        if (deviceCodeResponse.device_code is null)
        {
            throw new ArgumentNullException("Device Code Response contains a null device_code.");
        }

        DateTime startedPollingAt = DateTime.UtcNow;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if ((DateTime.UtcNow - startedPollingAt).TotalSeconds >= deviceCodeResponse.expires_in)
            {
                throw new DeviceCodeExpiredExceptionException("Device code expired (timed out). Start the device flow again.");
            }

            using FormUrlEncodedContent postData = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["device_code"] = deviceCodeResponse.device_code,
                ["grant_type"] = "urn:ietf:params:oauth:grant-type:device_code"

            });

            using HttpResponseMessage pollingResponse = await httpClient.PostAsync("https://id.twitch.tv/oauth2/token", postData, cancellationToken);
            string content = await pollingResponse.Content.ReadAsStringAsync(cancellationToken);

            // Handle successful response
            if (pollingResponse.IsSuccessStatusCode)
            {
                TokenSuccess? authToken = JsonSerializer.Deserialize<TokenSuccess>(content);

                if (authToken is null)
                {
                    throw new FailedToDeserializeTokenException("Unable to deserialize device code polling success response");
                }

                if (authToken.access_token is null)
                {
                    throw new InvalidAccessTokenException("Device Flow Response is missing access_token.");
                }

                return authToken;
            }

            // Handle failed response
            TokenFailed? tokenFailed = JsonSerializer.Deserialize<TokenFailed>(content);

            if (tokenFailed is null)
            {
                throw new FailedToDeserializeTokenException("Unable to deserialize device code polling failure response.");
            }

            switch (tokenFailed.message)
            {
                case "authorization_pending":
                    await Task.Delay(deviceCodeResponse.interval, cancellationToken);
                    break;

                default:
                    throw new InvalidOperationException($"Device Flow polling failed: {(int)pollingResponse.StatusCode} {pollingResponse.ReasonPhrase}. Response: {content}");
            }
        }
    }

    /// <exception cref="FailedToDeserializeTokenException"></exception>
    /// <exception cref="InvalidAccessTokenException"></exception>
    /// <exception cref="InvalidOperationException">Device Token Polling failed.</exception>
    private static async Task<TwitchDeviceToken> RefreshDeviceToken(string clientId, string refreshToken)
    {
        FormUrlEncodedContent postData = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["client_secret"] = Env.CLIENT_SECRET,
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken
        });

        using HttpResponseMessage refreshResponse = await httpClient.PostAsync(new Uri("https://id.twitch.tv/oauth2/token"), postData);
        string content = await refreshResponse.Content.ReadAsStringAsync();

        if (refreshResponse.IsSuccessStatusCode)
        {
            Console.WriteLine("Refreshed Token Successfully");

            TokenSuccess? tokenSuccess = JsonSerializer.Deserialize<TokenSuccess>(content);

            if (tokenSuccess is null)
            {
                throw new FailedToDeserializeTokenException("");
            }

            DateTime tokenExpiry = DateTime.UtcNow.AddSeconds(tokenSuccess.expires_in);

            if (tokenSuccess.access_token is null)
            {
                throw new InvalidAccessTokenException("Device Flow Response is missing access_token.");
            }

            if (tokenSuccess.refresh_token is null)
            {
                Console.WriteLine($"Refresh successful but refresh token was not provided. User will need to reauth at: {tokenExpiry}");
            }

            TwitchDeviceToken twitchDeviceToken = new TwitchDeviceToken
            {
                AccessToken = tokenSuccess.access_token,
                RefreshToken = tokenSuccess.refresh_token ?? "",
                ExpiresAtUtc = tokenExpiry,
                Scopes = tokenSuccess.scope ?? Array.Empty<string>()
            };

            DeviceTokenSerializer.SaveDeviceToken(twitchDeviceToken);

            return twitchDeviceToken;
        }

        TokenFailed? tokenFailed = JsonSerializer.Deserialize<TokenFailed>(content)!;

        if (tokenFailed is null)
        {
            throw new FailedToDeserializeTokenException("");
        }

        throw new InvalidOperationException($"Device Flow polling failed: {(int)refreshResponse.StatusCode} {refreshResponse.ReasonPhrase}. Response: {tokenFailed.message}");
    }

    private static bool DetectNewScopesAdded(TwitchSessionContext twitchSessionContext, TwitchDeviceToken twitchDeviceToken)
    {
        List<string> additionalScopes = new List<string>(twitchSessionContext.scopes.Select(x => x.GetScope()).Except(twitchDeviceToken.Scopes));

        return additionalScopes.Count > 0;
    }

    private static void Cleanup()
    {
        httpClient?.Dispose();
    }
}