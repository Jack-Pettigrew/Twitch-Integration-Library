using TIL.DataContainers;

namespace TIL.Client;

public class TwitchSessionContext
{
    public string? client_id;
    public string? user_id;
    public string? session_id;
    public string[]? scopes;
    public TwitchDeviceToken? twitch_device_token;
}