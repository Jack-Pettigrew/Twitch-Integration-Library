using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TIL.Auth;
using TIL.Exceptions;
using TIL.TwitchAPI;
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
    private System.Net.Http.HttpClient httpClient;
    private TwitchWebSocketClient twitchWebSocketClient;
    public bool AutoReconnectWebsocketOnFailure { get; private set; } = false;

    // CANCELLATION TOKEN
    private CancellationTokenSource clientCancellationTokenSource;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="userId"></param>
    /// <param name="scopes"></param>
    /// <param name="ReconnectWebsocketOnFailure"></param>
    public TwitchClient(string clientId, string userId, IEventSub[] scopes, bool autoReconnectWebsocketOnFailure = false)
    {
        TwitchSessionContext = new TwitchSessionContext
        {
            client_id = clientId,
            user_id = userId,
            scopes = scopes,
        };

        // httpClient = new System.Net.Http.HttpClient();
        // twitchWebSocketClient = new TwitchWebSocketClient(30, TwitchSessionContext);

        SetAutoconnectWebsocketOnFailure(autoReconnectWebsocketOnFailure);
    }

    ~TwitchClient()
    {
        if (AutoReconnectWebsocketOnFailure)
        {
            TwitchWebSocketClient.OnWebsocketExperiencedException -= HandleTwitchWebSocketClientException;
        }
    }

    public async Task<bool> ConnectToTwitchAsync()
    {
        httpClient = new System.Net.Http.HttpClient();
        twitchWebSocketClient = new TwitchWebSocketClient(30, TwitchSessionContext);

        clientCancellationTokenSource = new CancellationTokenSource();

        TwitchSessionContext.twitch_device_token = await Authentication.GetDeviceAccessTokenAsync(TwitchSessionContext);

        Console.WriteLine($"Retrieved Device Token - {TwitchSessionContext.twitch_device_token}");

        if (!await twitchWebSocketClient.ConnectToTwitchAsync())
        {
            return false;
        }

        await SubscribeToEventSubAsync(TwitchSessionContext.scopes!);

        _ = twitchWebSocketClient.StartHandlingWebsocket();


        return true;
    }

    public async Task DisconnectFromTwitchAsync()
    {
        await clientCancellationTokenSource.CancelAsync();
        await twitchWebSocketClient.DisconnectFromTwitchAsync();
        TwitchEventNotificationProcessor.ClearEventHandlers();
    }

    /// <summary>
    /// Subscribes Client to the requested EventSubs.
    /// </summary>
    /// <param name="eventSubs">Array of Twitch EventSubs for the Client to subscribe to.</param>
    /// <exception cref="InvalidDeviceCodeException"=></exception>
    public async Task SubscribeToEventSubAsync(IEventSub[] eventSubs)
    {
        Console.WriteLine("Subbing to EventSubs...");

        if (TwitchSessionContext.twitch_device_token is null)
        {
            throw new InvalidDeviceCodeException("Unable to subscribe to EventSubs due to null device_token.");
        }

        foreach (var eventSub in eventSubs)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TwitchSessionContext.twitch_device_token.AccessToken);
            httpClient.DefaultRequestHeaders.Add("Client-Id", TwitchSessionContext.client_id);

            byte[] payload = JsonSerializer.SerializeToUtf8Bytes(eventSub.ToSubscriptionPayload(TwitchSessionContext));

            using StringContent httpContentJson = new StringContent(Encoding.UTF8.GetString(payload), Encoding.UTF8, "application/json");

            using HttpResponseMessage subscriptionResult = await httpClient.PostAsync(new Uri("https://api.twitch.tv/helix/eventsub/subscriptions"), httpContentJson, clientCancellationTokenSource.Token);
            string subscriptionResponse = await subscriptionResult.Content.ReadAsStringAsync(clientCancellationTokenSource.Token);

            // Add to registry
            TwitchEventNotificationProcessor.RegisterTwitchEventHandler(eventSub);

            Console.WriteLine(subscriptionResponse);
        }

        Console.WriteLine("Finished Subbing to EventSubs.");
    }

    public void SetAutoconnectWebsocketOnFailure(bool toggle)
    {
        if (toggle == AutoReconnectWebsocketOnFailure) return;

        AutoReconnectWebsocketOnFailure = toggle;

        if (AutoReconnectWebsocketOnFailure)
        {
            TwitchWebSocketClient.OnWebsocketExperiencedException += HandleTwitchWebSocketClientException;
        }
        else
        {
            TwitchWebSocketClient.OnWebsocketExperiencedException -= HandleTwitchWebSocketClientException;
        }
    }

    private void HandleTwitchWebSocketClientException(TwitchWebSocketClient twitchWebSocketClient)
    {
        Console.WriteLine("TwitchWebSocketClient threw exception - attempting to reconnect...");

        _ = twitchWebSocketClient.StartHandlingWebsocket();
    }

    public void Dispose()
    {
        twitchWebSocketClient?.Dispose();
        httpClient?.Dispose();
        clientCancellationTokenSource?.Dispose();
    }
}