using System;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

/// <summary>
/// A new Hype Train has started event subscription.
/// </summary>
public partial class ChannelHypeTrainBeginEventSub : IEventSub
{
    public string GetScope()
    {
        return Scopes.CHANNEL_READ_HYPE_TRAIN;
    }

    public string GetSubscriptionType()
    {
        return Subscriptions.HYPE_TRAIN_BEGIN;
    }

    public object ToSubscriptionPayload(TwitchSessionContext twitchSessionContext)
    {
        return new
        {
            type = GetSubscriptionType(),
            version = "2",
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
        HypeTrainBegin hypeTrainBegin = new HypeTrainBegin
        {
            Type = twitchResponseJson["payload"]!["event"]!["type"]!.ToString(),
            Level = (int)twitchResponseJson["payload"]!["event"]!["level"]!,
            Goal = (int)twitchResponseJson["payload"]!["event"]!["goal"]!,
            TopContributions = twitchResponseJson["payload"]!["event"]!["top_contributions"]!.AsArray().Select(x => x!.AsObject()),
            StartedAt = DateTime.Parse(twitchResponseJson["payload"]!["event"]!["started_at"]!.ToString()),
            ExpiresAt = DateTime.Parse(twitchResponseJson["payload"]!["event"]!["expires_at"]!.ToString())
        };

        EventRegistry.TriggerHypeTrainBeginReceived(hypeTrainBegin);
    }
}