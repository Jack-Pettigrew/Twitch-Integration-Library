using System;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

/// <summary>
/// A user redeemed a Twitch curated Channel Points Reward.
/// </summary>
public partial class ChannelPointsAutomaticRewardRedemptionEventSub : IEventSub
{
    public string GetScope()
    {
        return Scopes.CHANNEL_READ_REDEMPTIONS;
    }

    public string GetSubscriptionType()
    {
        return Subscriptions.CHANNEL_POINTS_AUTOMATIC_REWARD_REDEMPTION_ADD_V2;
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
        ChannelPointsAutomaticRewardRedemption channelPointsAutomaticRewardRedemptionReceived = new ChannelPointsAutomaticRewardRedemption
        {
            UserId = twitchResponseJson["payload"]!["event"]!["user_id"]!.ToString(),
            UserName = twitchResponseJson["payload"]!["event"]!["user_name"]!.ToString(),
            RewardType = twitchResponseJson["payload"]!["event"]!["reward"]!["type"]!.ToString(),
            ChannelPoints = (int)twitchResponseJson["payload"]!["event"]!["reward"]!["channel_points"]!,
            Message = twitchResponseJson["payload"]!["event"]!["message"]!["text"]?.ToString() ?? null,
            RedeemedAt = DateTime.Parse(twitchResponseJson["payload"]!["event"]!["redeemed_at"]!.ToString())
        };

        EventRegistry.TriggerChannelPointsAutomaticRewardRedemptionReceived(channelPointsAutomaticRewardRedemptionReceived);
    }
}