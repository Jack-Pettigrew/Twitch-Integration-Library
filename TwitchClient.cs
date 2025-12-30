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
        httpClient = new HttpClient();
        cancellationTokenSource = new CancellationTokenSource();

        TwitchSessionContext = new TwitchSessionContext
        {
            client_id = clientId,
            user_id = userId,
            scopes = scopes,
        };
    }

    public async Task<bool> ConnectToTwitchAsync()
    {
        TwitchSessionContext.twitch_device_token = await Authentication.GetDeviceAccessTokenAsync(TwitchSessionContext);

        Console.WriteLine($"Completed: Final access token - {TwitchSessionContext.twitch_device_token}");
        Console.ReadKey();

        twitchWebSocketClient = new TwitchWebSocketClient(TwitchSessionContext);

        if (!await twitchWebSocketClient.ConnectToTwitchAsync(cancellationTokenSource.Token))
        {
            return false;
        }


        return true;
    }

    public void Dispose()
    {
        twitchWebSocketClient?.Dispose();
        httpClient?.Dispose();
    }
}