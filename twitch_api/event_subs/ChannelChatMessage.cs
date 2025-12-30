using TIL.Client;

namespace TIL.TwitchAPI.EventSubs;

class ChannelChatMessage : IEventSub
{
    public string GetScope()
    {
        return Scopes.USER_READ_CHAT;
    }

    public string GetSubscriptionType()
    {
        return Subscriptions.CHANNEL_CHAT_MESSAGE;
    }

    public object ToSubscriptionPayload(TwitchSessionContext twitchSessionContext)
    {
        return new
        {
            type = GetSubscriptionType(),
            version = "1",
            condition = new
            {
                broadcaster_user_id = twitchSessionContext.user_id,
                user_id = twitchSessionContext.user_id
            },
            transport = new
            {
                method = "websocket",
                session_id = twitchSessionContext.session_id
            }
        };
    }
}