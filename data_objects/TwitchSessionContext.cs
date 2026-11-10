using TIL.DataContainers;
using TIL.TwitchAPI.EventSubs;

namespace TIL.Client;

public class TwitchSessionContext
{
    public string clientId = "";
    public string userId = "";
    public string sessionId = "";
    public string[] scopes = [];
    public IEventSub[] eventSubs = [];
    public TwitchDeviceToken? twitchDeviceToken;
}