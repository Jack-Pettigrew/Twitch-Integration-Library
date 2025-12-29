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

    // CLIENT DETAILS
    public readonly string? clientId = null;
    public int? userId { get; private set; } = null;
    public TwitchDeviceToken? twitchDeviceToken { get; private set; } = null;

    // INTENTS
    public string[] scopes = Array.Empty<string>();

    // NETWORKING
    private HttpClient httpClient;
    private TwitchWebSocketClient twitchWebSocketClient;

    // CANCELLATION TOKEN
    private CancellationTokenSource cancellationTokenSource;

    public TwitchClient(string clientId, int userId)
    {
        this.httpClient = new HttpClient();
        this.cancellationTokenSource = new CancellationTokenSource();
        this.clientId = clientId;
        this.userId = userId;
    }

    public TwitchClient(string clientId, int userId, string[] scopes)
    {
        this.httpClient = new HttpClient();
        this.cancellationTokenSource = new CancellationTokenSource();
        this.clientId = clientId;
        this.userId = userId;
        this.scopes = scopes;
    }

    public async Task<bool> ConnectToTwitchAsync()
    {
        twitchDeviceToken = await Authentication.GetDeviceAccessTokenAsync(clientId, scopes);

        Console.WriteLine($"Completed: Final access token - {twitchDeviceToken}");
        Console.ReadKey();

        twitchWebSocketClient = new TwitchWebSocketClient();

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