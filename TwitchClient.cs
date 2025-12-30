using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TIL.Auth;
using TIL.DataContainers;
using TIL.TwitchAPI;
using TIL.TwitchAPI.EventSubs;

namespace TIL.Client;

class TwitchClient : IDisposable
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

    public TwitchClient(string clientId, string userId, string[] scopes)
    {
        TwitchSessionContext = new TwitchSessionContext
        {
            client_id = clientId,
            user_id = userId,
            scopes = scopes,
        };

        httpClient = new HttpClient();
        twitchWebSocketClient = new TwitchWebSocketClient(TwitchSessionContext);
        cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task<bool> ConnectToTwitchAsync()
    {
        TwitchSessionContext.twitch_device_token = await Authentication.GetDeviceAccessTokenAsync(TwitchSessionContext);

        Console.WriteLine($"Completed: Final access token - {TwitchSessionContext.twitch_device_token}");
        Console.ReadKey();

        if (!await twitchWebSocketClient.ConnectToTwitchAsync(cancellationTokenSource.Token))
        {
            return false;
        }


        return true;
    }

    public async Task SubscribeToEventSubAsync(IEventSub[] eventSubs)
    {
        Console.WriteLine("Subbing to EventSubs...");

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
    public void Dispose()
    {
        twitchWebSocketClient?.Dispose();
        httpClient?.Dispose();
    }
}