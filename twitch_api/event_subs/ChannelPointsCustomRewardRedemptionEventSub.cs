using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

/// <summary>
/// A user redeemed a Streamer curated Channel Points Reward.
/// </summary>
public partial class ChannelPointsCustomRewardRedemptionEventSub : IEventSub
{
    public Scopes.Requirement GetScopeRequirement()
    {
        return new(Scopes.Mode.Any, [
            Scopes.CHANNEL_READ_REDEMPTIONS,
            Scopes.CHANNEL_MANAGE_REDEMPTIONS
        ]);
    }

    public string GetSubscriptionType()
    {
        return Subscriptions.CHANNEL_POINTS_CUSTOM_REWARD_REDEMPTION_ADD;
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
        var rewardRedemption = JsonSerializer.Deserialize<ChannelPointsCustomRewardRedemption>(
            twitchResponseJson["payload"]!["event"]
        );

        if (rewardRedemption is not null)
            EventRegistry.TriggerChannelPointsCustomRewardRedemptionReceived(rewardRedemption);
    }
}