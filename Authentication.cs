using System.Diagnostics;
using System.Text.Json;
using TIL.Client;
using TIL.DataContainers;
using TIL.Exceptions;
using TIL.Network.Responses;
using TIL.Serialization;

namespace TIL.Auth;

static class Authentication
{
    private static HttpClient? httpClient = null;

    public static async Task<TwitchDeviceToken> GetDeviceAccessTokenAsync(TwitchSessionContext twitchSessionContext)
    {
        Console.WriteLine("Beginning Retrieving Device Token...");

        Console.WriteLine("Searching for saved Device Token...");
        TwitchDeviceToken? twitchDeviceToken = DeviceTokenSerializer.LoadDeviceToken();

        // Attempt to validate and refresh device token
        if (twitchDeviceToken != null)
        {
            Console.WriteLine("Checking saved Device Token...");

            // Valid Check
            if (DateTime.UtcNow > twitchDeviceToken.ExpiresAtUtc && twitchDeviceToken.RefreshToken != string.Empty)
            {
                Console.WriteLine("Device Token Expired. Attempting to refresh...");

                // Refresh Token
                try
                {
                    twitchDeviceToken = await RefreshDeviceToken(twitchSessionContext.client_id, twitchDeviceToken.RefreshToken);
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
                Console.WriteLine("Saved Device Token is valid.");
                return twitchDeviceToken;
            }
        }
        else
        {
            Console.WriteLine("No saved Device Token found.");
        }

        httpClient = new HttpClient();

        FormUrlEncodedContent postData = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = twitchSessionContext.client_id,
            ["scopes"] = string.Join(' ', twitchSessionContext.scopes)
        });

        Console.WriteLine("Requesting new Device Flow Auth...");

        using HttpResponseMessage response = await httpClient.PostAsync(new Uri("https://id.twitch.tv/oauth2/device"), postData);

        string responseJson = await response.Content.ReadAsStringAsync();

        // TODO handle exceptions
        response.EnsureSuccessStatusCode();

        DeviceCodeRequestResponse deviceResponse = JsonSerializer.Deserialize<DeviceCodeRequestResponse>(responseJson)!;

        // Open in browser
        using Process browserProcess = new Process();
        browserProcess.StartInfo.UseShellExecute = true;
        browserProcess.StartInfo.FileName = deviceResponse.verification_uri;
        browserProcess.Start();

        Console.WriteLine("Polling device code for user completion...");

        // TODO handle exceptions
        TokenSuccess tokenSuccess = await PollDeviceCodeFlowAsync(twitchSessionContext.client_id, deviceResponse, CancellationToken.None);

        twitchDeviceToken = new TwitchDeviceToken
        {
            AccessToken = tokenSuccess.access_token,
            RefreshToken = tokenSuccess.refresh_token,
            ExpiresAtUtc = DateTime.UtcNow.AddSeconds(tokenSuccess.expires_in),
            Scopes = tokenSuccess.scopes ?? Array.Empty<string>()
        };

        DeviceTokenSerializer.SaveDeviceToken(twitchDeviceToken);

        Cleanup();

        return twitchDeviceToken;
    }

    private static async Task<TokenSuccess> PollDeviceCodeFlowAsync(string clientId, DeviceCodeRequestResponse deviceCodeResponse, CancellationToken cancellationToken)
    {
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
                TokenSuccess authToken = JsonSerializer.Deserialize<TokenSuccess>(content);

                if (authToken?.access_token is null)
                {
                    throw new InvalidAccessTokenException("Device Flow Response is missing access_token.");
                }

                return authToken;
            }

            // Handle failed response
            TokenFailed tokenFailed = JsonSerializer.Deserialize<TokenFailed>(content);

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

    private static async Task<TwitchDeviceToken> RefreshDeviceToken(string clientId, string refreshToken)
    {
        FormUrlEncodedContent postData = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken
        });

        using HttpResponseMessage refreshResponse = await httpClient.PostAsync(new Uri("https://id.twitch.tv/oauth2/token"), postData);
        string content = await refreshResponse.Content.ReadAsStringAsync();

        Console.WriteLine(content);

        if (refreshResponse.IsSuccessStatusCode)
        {
            TokenSuccess tokenSuccess = JsonSerializer.Deserialize<TokenSuccess>(content);

            if (tokenSuccess?.access_token is null)
            {
                throw new InvalidAccessTokenException("Device Flow Response is missing access_token.");
            }

            TwitchDeviceToken twitchDeviceToken = new TwitchDeviceToken
            {
                AccessToken = tokenSuccess.access_token,
                RefreshToken = tokenSuccess.refresh_token,
                ExpiresAtUtc = DateTime.UtcNow.AddSeconds(tokenSuccess.expires_in),
                Scopes = tokenSuccess.scopes ?? Array.Empty<string>()
            };

            DeviceTokenSerializer.SaveDeviceToken(twitchDeviceToken);

            return twitchDeviceToken;
        }

        TokenFailed tokenFailed = JsonSerializer.Deserialize<TokenFailed>(content);

        throw new InvalidOperationException($"Device Flow polling failed: {(int)refreshResponse.StatusCode} {refreshResponse.ReasonPhrase}. Response: {content}");
    }

    private static void Cleanup()
    {
        httpClient?.Dispose();
    }
}