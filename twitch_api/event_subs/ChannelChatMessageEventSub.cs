using System.Text.Json;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

/// <summary>
/// Channel chat message received event.
/// </summary>
class ChannelChatMessageEventSub : IEventSub
{
    public Scopes.Requirement GetScopeRequirement()
    {
        return new(Scopes.Mode.All, [
            Scopes.USER_READ_CHAT
        ]);
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
                broadcaster_user_id = twitchSessionContext.userId,
                user_id = twitchSessionContext.userId
            },
            transport = new
            {
                method = "websocket",
                session_id = twitchSessionContext.sessionId
            }
        };
    }

    public void ProcessEvent(JsonNode twitchResponseJson)
    {
        ChatMessage? chatMessage = JsonSerializer.Deserialize<ChatMessage>(twitchResponseJson["payload"]!["event"]);

        if (chatMessage is not null) EventRegistry.TriggerChatMessageReceived(chatMessage);
    }
}