using System;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

/// <summary>
/// A brand new Twitch subscription event.
/// </summary>
public partial class ChannelSubscribeEventSub : IEventSub
{
    public string GetScope()
    {
        return Scopes.CHANNEL_READ_SUBSCRIPTIONS;
    }

    public string GetSubscriptionType()
    {
        return Subscriptions.CHANNEL_SUBSCRIBE;
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
        ChannelSubscribe channelSubscribe = new ChannelSubscribe
        {
            UserId = twitchResponseJson["payload"]!["event"]!["user_id"].ToString(),
            UserName = twitchResponseJson["payload"]!["event"]!["user_name"].ToString(),
            Tier = twitchResponseJson["payload"]!["event"]!["tier"].ToString(),
            IsGift = (bool)twitchResponseJson["payload"]!["event"]!["is_gift"]
        };

        EventRegistry.TriggerChannelSubscribeReceived(channelSubscribe);
    }
}