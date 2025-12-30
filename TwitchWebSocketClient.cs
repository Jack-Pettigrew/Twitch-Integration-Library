using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Nodes;

namespace TIL.Client;

class TwitchWebSocketClient : IDisposable
{
    private const string WEBSOCKET_ENDPOINT = "wss://eventsub.wss.twitch.tv/ws";

    private TwitchSessionContext twitchSessionContext;
    private ClientWebSocket websocket;
    private ArraySegment<byte> websocketResponseBuffer = new ArraySegment<byte>(new byte[1024]);

    public TwitchWebSocketClient(TwitchSessionContext twitchSessionContext)
    {
        this.twitchSessionContext = twitchSessionContext;

        websocket = new ClientWebSocket();
    }

    public async Task<bool> ConnectToTwitchAsync(CancellationToken cancellationToken)
    {
        await websocket.ConnectAsync(new Uri(WEBSOCKET_ENDPOINT), cancellationToken);

        if (websocket.State != WebSocketState.Open)
        {
            return false;
        }

        WebSocketReceiveResult result = await websocket.ReceiveAsync(websocketResponseBuffer, cancellationToken);

        JsonNode responseNode = JsonNode.Parse(Encoding.UTF8.GetString(websocketResponseBuffer.Array!, 0, result.Count))!;

        twitchSessionContext.session_id = responseNode!["payload"]!["session"]!["id"]?.ToString();

        return true;
    }

    public void Dispose()
    {
        websocket?.Dispose();
    }
}