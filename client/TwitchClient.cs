using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TIL.Auth;
using TIL.Exceptions;
using TIL.TwitchAPI.EventSubs;

namespace TIL.Client;

sealed class TwitchClient : IDisposable
{
    // REGULATION
    public bool IsConnectedToTwitch { get; private set; } = false;

    // CONTEXT
    public TwitchSessionContext TwitchSessionContext { get; private set; }

    // INTENTS
    public string[] scopes = Array.Empty<string>();

    // NETWORKING
    private HttpClient httpClient;
    private TwitchWebSocketClient twitchWebSocketClient;

    // CANCELLATION TOKEN
    private CancellationTokenSource cancellationTokenSource;

    public TwitchClient(string clientId, string userId, IEventSub[] scopes)
    {
        TwitchSessionContext = new TwitchSessionContext
        {
            client_id = clientId,
            user_id = userId,
            scopes = scopes,
        };

        cancellationTokenSource = new CancellationTokenSource();

        httpClient = new HttpClient();
        twitchWebSocketClient = new TwitchWebSocketClient(30, TwitchSessionContext, cancellationTokenSource.Token);
    }

    public async Task<bool> ConnectToTwitchAsync()
    {
        TwitchSessionContext.twitch_device_token = await Authentication.GetDeviceAccessTokenAsync(TwitchSessionContext);

        Console.WriteLine($"Retrieved Device Token - {TwitchSessionContext.twitch_device_token}");

        if (!await twitchWebSocketClient.ConnectToTwitchAsync())
        {
            return false;
        }

        await SubscribeToEventSubAsync(TwitchSessionContext.scopes!);

        return true;
    }

    public async Task DisconnectFromTwitchAsync()
    {
        await StopWebsocketListeningAsync();
    }

    public async Task SubscribeToEventSubAsync(IEventSub[] eventSubs)
    {
        Console.WriteLine("Subbing to EventSubs...");

        if (TwitchSessionContext.twitch_device_token is null)
        {
            throw new InvalidDeviceCodeException("Unable to subscribe to EventSubs due to null device_token.");
        }

        foreach (var eventsub in eventSubs)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TwitchSessionContext.twitch_device_token.AccessToken);
            httpClient.DefaultRequestHeaders.Add("Client-Id", TwitchSessionContext.client_id);

            byte[] payload = JsonSerializer.SerializeToUtf8Bytes(eventsub.ToSubscriptionPayload(TwitchSessionContext));

            using StringContent httpContentJson = new StringContent(Encoding.UTF8.GetString(payload), Encoding.UTF8, "application/json");

            using HttpResponseMessage subscriptionResult = await httpClient.PostAsync(new Uri("https://api.twitch.tv/helix/eventsub/subscriptions"), httpContentJson, cancellationTokenSource.Token);
            string subscriptionResponse = await subscriptionResult.Content.ReadAsStringAsync(cancellationTokenSource.Token);

            // JsonNode subscriptionNode = JsonNode.Parse(subscriptionResponse)!;

            // TODO handle subscription response

            Console.WriteLine(subscriptionResponse);
        }

        Console.WriteLine("Finished Subbing to EventSubs.");
    }

    public void StartWebsocketListening()
    {
        if (cancellationTokenSource.TryReset())
        {
            _ = twitchWebSocketClient.StartHandlingWebsocket();
        }
    }

    public async Task StopWebsocketListeningAsync()
    {
        await cancellationTokenSource.CancelAsync();
        await twitchWebSocketClient.DisconnectFromTwitchAsync();
    }

    public void Dispose()
    {
        twitchWebSocketClient?.Dispose();
        httpClient?.Dispose();
        cancellationTokenSource?.Dispose();
    }
}