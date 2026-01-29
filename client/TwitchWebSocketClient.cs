using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using TIL.TwitchAPI;

namespace TIL.Client;

class TwitchWebSocketClient : IDisposable
{
    private const string WEBSOCKET_ENDPOINT = "wss://eventsub.wss.twitch.tv/ws";
    private readonly int? keepAliveTimeout = null;
    private CancellationTokenSource websocketCancellationTokenSource;

    private TwitchSessionContext twitchSessionContext;
    private ClientWebSocket websocket;
    private ArraySegment<byte> websocketResponseBuffer = new ArraySegment<byte>(new byte[1024 * 2]);

    // EVENTS
    public delegate void WebsocketClientEventHandler(TwitchWebSocketClient twitchWebSocketClient);
    public static event WebsocketClientEventHandler? OnWebsocketExperiencedException;

    public TwitchWebSocketClient(TwitchSessionContext twitchSessionContext)
    {
        this.twitchSessionContext = twitchSessionContext;
    }

    public TwitchWebSocketClient(int keepalive_timeout_seconds, TwitchSessionContext twitchSessionContext)
    {
        this.twitchSessionContext = twitchSessionContext;
        keepAliveTimeout = keepalive_timeout_seconds;
    }


    public async Task<bool> ConnectToTwitchAsync()
    {
        websocket = new ClientWebSocket();

        websocketCancellationTokenSource = new CancellationTokenSource();

        string websocket_endpoint_complete = keepAliveTimeout != null ? WEBSOCKET_ENDPOINT + $"?keepalive_timeout_seconds={keepAliveTimeout}" : WEBSOCKET_ENDPOINT;

        await websocket.ConnectAsync(new Uri(websocket_endpoint_complete), websocketCancellationTokenSource.Token);

        if (websocket.State != WebSocketState.Open)
        {
            return false;
        }

        WebSocketReceiveResult result = await websocket.ReceiveAsync(websocketResponseBuffer, websocketCancellationTokenSource.Token);

        JsonNode responseNode = JsonNode.Parse(Encoding.UTF8.GetString(websocketResponseBuffer.Array!, 0, result.Count))!;

        twitchSessionContext.session_id = responseNode!["payload"]!["session"]!["id"]?.ToString();

        return true;
    }

    public async Task DisconnectFromTwitchAsync()
    {
        if (websocket.State == WebSocketState.Open || websocket.State == WebSocketState.Connecting)
        {
            await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Completed websocket connection.", CancellationToken.None);
        }
    }

    public async Task StartHandlingWebsocket()
    {
        try
        {
            while (true)
            {
                websocketCancellationTokenSource.Token.ThrowIfCancellationRequested();

                WebSocketReceiveResult responseResult;
                string response = "";

                try
                {
                    do
                    {
                        responseResult = await websocket.ReceiveAsync(websocketResponseBuffer, websocketCancellationTokenSource.Token);

                        if (responseResult.MessageType == WebSocketMessageType.Close)
                        {
                            await DisconnectFromTwitchAsync();
                            return;
                        }

                        response += Encoding.UTF8.GetString(websocketResponseBuffer.Array!, 0, responseResult.Count);

                    } while (!responseResult.EndOfMessage);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"There was an issue receiving the next Socket Response: {e.Message}.");
                    Console.WriteLine("Skipping...");
                    continue;
                }

                JsonNode responseNode = JsonNode.Parse(response)!;

                switch (responseNode["metadata"]!["message_type"]!.ToString())
                {
                    case "notification":
                        TwitchEventNotificationProcessor.ProcessTwitchEvent(responseNode);
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
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"TwitchWebSocketClient was requested to stop via a websocketCancellationTokenSource.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"TwitchWebSocketClient experienced an error: {e.Message}");

            // Print stacktrace manually as we lose it in async outside main sync context
            Console.WriteLine($"{e.StackTrace}");

            OnWebsocketExperiencedException?.Invoke(this);
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