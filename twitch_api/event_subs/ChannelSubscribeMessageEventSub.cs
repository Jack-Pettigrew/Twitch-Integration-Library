using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

/// <summary>
/// A user shares a resubscription message subscription event.
/// </summary>
public partial class ChannelSubscribeMessageEventSub : IEventSub
{
    public Scopes.Requirement GetScopeRequirement()
    {
        return new(Scopes.Mode.All, [Scopes.CHANNEL_READ_SUBSCRIPTIONS]);
    }

    public string GetSubscriptionType()
    {
        return Subscriptions.CHANNEL_SUBSCRIPTION_MESSAGE;
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
        var subscribeMessage = JsonSerializer.Deserialize<ChannelSubscribeMessage>(
            twitchResponseJson["payload"]!["event"]
        );

        if (subscribeMessage is not null)
        {
            EventRegistry.TriggerChannelSubscribeMessageReceived(subscribeMessage);
        }
    }
}