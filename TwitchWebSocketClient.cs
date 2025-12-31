using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Nodes;
using TIL.TwitchAPI;

namespace TIL.Client;

class TwitchWebSocketClient : IDisposable
{
    private const string WEBSOCKET_ENDPOINT = "wss://eventsub.wss.twitch.tv/ws";
    private readonly int? keepalive_timeout = null;
    private CancellationToken cancellationToken;

    private TwitchSessionContext twitchSessionContext;
    private ClientWebSocket websocket;
    private ArraySegment<byte> websocketResponseBuffer = new ArraySegment<byte>(new byte[1024 * 2]);

    public TwitchWebSocketClient(TwitchSessionContext twitchSessionContext, CancellationToken cancellationToken)
    {
        this.twitchSessionContext = twitchSessionContext;
        this.cancellationToken = cancellationToken;

        websocket = new ClientWebSocket();
    }

    public TwitchWebSocketClient(int keepalive_timeout_seconds, TwitchSessionContext twitchSessionContext, CancellationToken cancellationToken)
    {
        this.twitchSessionContext = twitchSessionContext;
        this.cancellationToken = cancellationToken;
        keepalive_timeout = keepalive_timeout_seconds;

        websocket = new ClientWebSocket();
    }


    public async Task<bool> ConnectToTwitchAsync()
    {
        string websocket_endpoint_complete = keepalive_timeout != null ? WEBSOCKET_ENDPOINT + $"?keepalive_timeout_seconds={keepalive_timeout}" : WEBSOCKET_ENDPOINT;

        await websocket.ConnectAsync(new Uri(websocket_endpoint_complete), cancellationToken);

        if (websocket.State != WebSocketState.Open)
        {
            return false;
        }

        WebSocketReceiveResult result = await websocket.ReceiveAsync(websocketResponseBuffer, cancellationToken);

        JsonNode responseNode = JsonNode.Parse(Encoding.UTF8.GetString(websocketResponseBuffer.Array!, 0, result.Count))!;

        twitchSessionContext.session_id = responseNode!["payload"]!["session"]!["id"]?.ToString();

        return true;
    }

    public async Task DisconnectFromTwitchAsync()
    {
        if (websocket.State == WebSocketState.Open || websocket.State == WebSocketState.CloseReceived)
        {
            await websocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Completed websocket connection.", CancellationToken.None);
            // await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Completed websocket connection.", CancellationToken.None);
        }
    }

    public async Task StartHandlingWebsocket()
    {
        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                WebSocketReceiveResult responseResult;

                try
                {
                    responseResult = await websocket.ReceiveAsync(websocketResponseBuffer, cancellationToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"There was an issue receiving the next Socket Response: {e.Message}.");
                    Console.WriteLine("Skipping...");
                    continue;
                }

                if (responseResult.MessageType != WebSocketMessageType.Close)
                {
                    string response = Encoding.UTF8.GetString(websocketResponseBuffer.Array!, 0, responseResult.Count);

                    JsonNode responseNode = JsonNode.Parse(response)!;

                    switch (responseNode["metadata"]!["message_type"]!.ToString())
                    {
                        case "notification":
                            TwitchNotificationProcessor.ProcessTwitchEvent(responseNode);
                            break;

                        case "session_keepalive":
                            Console.WriteLine("Received 'session_keepalive'.");
                            break;

                        case "session_reconnect":
                            Console.WriteLine("Instructed to reconnect - disconnecting websocket...");
                            await DisconnectFromTwitchAsync();
                            Console.WriteLine("Handled Disconnect.");

                            Console.WriteLine("Reconnecting websocket to Twitch...");
                            bool success = await ConnectToTwitchAsync();

                            if (!success)
                            {
                                throw new Exception("Unable to reconnect websocket to Twitch post 'session_reconnect' instruction.");
                            }

                            Console.WriteLine("Successfully Reconnected websocket to Twitch.");

                            break;

                        case "revocation":
                            HandleRevocation(responseNode["payload"]!["subscription"]!);
                            break;

                        default:
                            Console.WriteLine($"Unknown message_type received: {responseNode["metadata"]!["message_type"]}");
                            break;
                    }
                }
                else
                {
                    await DisconnectFromTwitchAsync();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"TwitchWebSocketClient experienced an error: {e.Message}");

            // Print stacktrace manually as we lose it in async outside main sync context
            Console.WriteLine($"{e.StackTrace}");
        }
    }

    private void HandleRevocation(JsonNode subscriptionNode)
    {
        string status = subscriptionNode["status"]!.ToString();
        string type = subscriptionNode["type"]!.ToString();

        string errorMessage = $"Revocation with {status} on {type}:";

        switch (status)
        {
            case "authorization_revoked":
                errorMessage += " The user in the condition object revoked the authorization that let you get events on their behalf.";
                break;

            case "user_removed":
                errorMessage += " The user in the condition object is no longer a Twitch user.";
                break;

            case "version_removed":
                errorMessage += " The subscribed to subscription type and version is no longer supported.";
                break;

            default:
                errorMessage = $"An unknown revocation occurred: {status} on {type}";
                break;
        }

        Console.WriteLine(errorMessage);

    }

    public void Dispose()
    {
        websocket?.Dispose();
    }
}