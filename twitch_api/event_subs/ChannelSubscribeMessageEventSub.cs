using System;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

/// <summary>
/// A user shares a resubscription message subscription event.
/// </summary>
public partial class ChannelSubscribeMessageEventSub : IEventSub
{
    public string GetScope()
    {
        return Scopes.CHANNEL_READ_SUBSCRIPTIONS;
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
        ChannelSubscribeMessage channelSubscribeMessage = new ChannelSubscribeMessage
        {
            UserId = twitchResponseJson["payload"]!["event"]!["user_id"]!.ToString(),
            UserName = twitchResponseJson["payload"]!["event"]!["user_name"]!.ToString(),
            Tier = twitchResponseJson["payload"]!["event"]!["tier"]!.ToString(),
            Message = twitchResponseJson["payload"]!["event"]!["message"]!["text"]!.ToString(),
            CumulativeMonths = (int)twitchResponseJson["payload"]!["event"]!["cumulative_months"]!,
            StreakMonths = (int)twitchResponseJson["payload"]!["event"]!["streak_months"]!,
            DurationMonths = (int)twitchResponseJson["payload"]!["event"]!["duration_months"]!
        };

        EventRegistry.TriggerChannelSubscribeMessageReceived(channelSubscribeMessage);
    }
}