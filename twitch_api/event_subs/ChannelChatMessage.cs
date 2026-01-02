using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

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

    public void ProcessEvent(JsonNode twitchResponseJson)
    {
        ChatMessage chatMessage = new ChatMessage
        {
            MessageId = twitchResponseJson["payload"]!["event"]!["message_id"]!.ToString(),
            UserId = twitchResponseJson["payload"]!["event"]!["chatter_user_id"]!.ToString(),
            UserName = twitchResponseJson["payload"]!["event"]!["chatter_user_name"]!.ToString(),
            Message = twitchResponseJson["payload"]!["event"]!["message"]!["text"]!.ToString(),
            IsReply = twitchResponseJson["payload"]!["event"]!.AsObject().ContainsKey("reply")
        };

        EventRegistry.TriggerChatMessageReceived(chatMessage);
    }
}