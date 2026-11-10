using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

/// <summary>
/// A new Hype Train has started event subscription.
/// </summary>
public partial class ChannelHypeTrainBeginEventSub : IEventSub
{
    public Scopes.Requirement GetScopeRequirement()
    {
        return new(Scopes.Mode.All, [
            Scopes.CHANNEL_READ_HYPE_TRAIN
        ]);
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
        HypeTrain? hypeTrain = JsonSerializer.Deserialize<HypeTrain>(twitchResponseJson["payload"]!["event"]);

        if (hypeTrain is not null) EventRegistry.TriggerHypeTrainBeginReceived(hypeTrain);
    }
}