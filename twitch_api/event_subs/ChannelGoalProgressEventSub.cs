using System;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

/// <summary>
/// A Goal has progressed subscription event.
/// </summary>
public partial class ChannelGoalProgressEventSub : IEventSub
{
    public string GetScope()
    {
        return Scopes.CHANNEL_READ_GOALS;
    }

    public string GetSubscriptionType()
    {
        return Subscriptions.GOAL_PROGRESS;
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
        GoalProgress goalProgress = new GoalProgress
        {
            Type = twitchResponseJson["payload"]!["event"]!["type"]!.ToString(),
            Description = twitchResponseJson["payload"]!["event"]!["Description"]!.ToString(),
            CurrentAmount = (int)twitchResponseJson["payload"]!["event"]!["current_amount"]!,
            TargetAmount = (int)twitchResponseJson["payload"]!["event"]!["target_amount"]!,
            StartedAt = DateTime.Parse(twitchResponseJson["payload"]!["event"]!["message"]!["started_at"]!.ToString())
        };

        EventRegistry.TriggerGoalProgressReceived(goalProgress);
    }
}