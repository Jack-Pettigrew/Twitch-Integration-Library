using TIL.DataContainers;
using TIL.TwitchAPI.EventSubs;

namespace TIL.Client;

public class TwitchSessionContext
{
    public string? client_id;
    public string? user_id;
    public string? session_id;
    public IEventSub[]? scopes;
    public TwitchDeviceToken? twitch_device_token;
}