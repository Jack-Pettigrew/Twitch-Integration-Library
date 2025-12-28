using System.Diagnostics;
using System.Text.Json;
using TIL.DataContainers;
using TIL.Exceptions;
using TIL.Network.Responses;
using TIL.Serialization;

namespace TIL.Auth;

static class Authentication
{
    private static HttpClient? httpClient = null;

    public static async Task<string> GetDeviceAccessTokenAsync(string clientId, params string[] scopes)
    {
        // TODO find if we have access token stored

        // TODO if we have one, check its valid

        // TODO if its not, use refresh token

        // TODO if we don't, request new via auth flow

        httpClient = new HttpClient();

        FormUrlEncodedContent postData = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["scopes"] = string.Join(' ', scopes)
        });

        using HttpResponseMessage response = await httpClient.PostAsync(new Uri("https://id.twitch.tv/oauth2/device"), postData);

        string responseJson = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();

        DeviceCodeRequestResponse deviceResponse = JsonSerializer.Deserialize<DeviceCodeRequestResponse>(responseJson)!;

        // Open in browser
        using Process browserProcess = new Process();
        browserProcess.StartInfo.UseShellExecute = true;
        browserProcess.StartInfo.FileName = deviceResponse.verification_uri;
        browserProcess.Start();

        // TODO handle exceptions
        TokenSuccess tokenSuccess = await PollDeviceCodeFlowAsync(clientId, deviceResponse, CancellationToken.None);

        TwitchDeviceToken twitchDeviceToken = new TwitchDeviceToken
        {
            AccessToken = tokenSuccess.access_token,
            RefreshToken = tokenSuccess.refresh_token,
            ExpiresAtUtc = DateTime.UtcNow.AddSeconds(tokenSuccess.expires_in),
            Scopes = tokenSuccess.scopes ?? Array.Empty<string>()
        };

        DeviceTokenSerializer.SaveDeviceToken(twitchDeviceToken);

        Cleanup();

        return "";
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
                case "authorisation_pending":
                    await Task.Delay(deviceCodeResponse.interval, cancellationToken);
                    break;

                default:
                    throw new InvalidOperationException($"Device Flow polling failed: {(int)pollingResponse.StatusCode} {pollingResponse.ReasonPhrase}. Response: {content}");
            }
        }
    }

    private static void Cleanup()
    {
        httpClient?.Dispose();
    }
}