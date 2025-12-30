using TIL.Client;

namespace TIL.TwitchAPI.EventSubs;

public interface IEventSub
{
    string GetScope();
    string GetSubscriptionType();
    object ToSubscriptionPayload(TwitchSessionContext twitchSessionContext);
}