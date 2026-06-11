using System;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

/// <summary>
/// A user redeemed a Streamer curated Channel Points Reward.
/// </summary>
public partial class ChannelPointsCustomRewardRedemptionEventSub : IEventSub
{
    public string GetScope()
    {
        return Scopes.CHANNEL_READ_REDEMPTIONS;
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
        ChannelPointsCustomRewardRedemption channelPointsCustomRewardRedemptionReceived = new ChannelPointsCustomRewardRedemption
        {
            UserId = twitchResponseJson["payload"]!["event"]!["user_id"]!.ToString(),
            UserName = twitchResponseJson["payload"]!["event"]!["user_name"]!.ToString(),
            UserInput = twitchResponseJson["payload"]!["event"]!["user_input"]!.ToString(),
            Status = twitchResponseJson["payload"]!["event"]!["status"]!.ToString(),
            Title = twitchResponseJson["payload"]!["event"]!["reward"]!["title"]!.ToString(),
            Cost = (int)twitchResponseJson["payload"]!["event"]!["reward"]!["cost"]!,
            RewardPrompt = twitchResponseJson["payload"]!["event"]!["reward"]!["prompt"]!.ToString(),
            RedeemedAt = DateTime.Parse(twitchResponseJson["payload"]!["event"]!["redeemed_at"]!.ToString())
        };

        EventRegistry.TriggerChannelPointsCustomRewardRedemptionReceived(channelPointsCustomRewardRedemptionReceived);
    }
}