using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

/// <summary>
/// A Goal has progressed subscription event.
/// </summary>
public partial class ChannelGoalEndEventSub : IEventSub
{
    public Scopes.Requirement GetScopeRequirement()
    {
        return new(Scopes.Mode.All, [
            Scopes.CHANNEL_READ_GOALS
        ]);
    }

    public string GetSubscriptionType()
    {
        return Subscriptions.GOAL_END;
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
        Goal? goal = JsonSerializer.Deserialize<Goal>(twitchResponseJson["payload"]!["event"]);

        if (goal is not null) EventRegistry.TriggerGoalEndReceived(goal);
    }
}