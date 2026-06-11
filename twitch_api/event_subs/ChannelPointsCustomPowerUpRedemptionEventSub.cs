using System;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

/// <summary>
/// A user redeemed a Twitch curated Channel Points Reward.
/// </summary>
public partial class ChannelPointsCustomPowerUpRedemptionEventSub : IEventSub
{
    public string GetScope()
    {
        return Scopes.CHANNEL_BITS_READ;
    }

    public string GetSubscriptionType()
    {
        return Subscriptions.CHANNEL_CUSTOM_POWER_UP_REDEMPTION_ADD;
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
        ChannelCustomPowerUpRedemptionAdd channelCustomPowerUpRedemptionAdd = new ChannelCustomPowerUpRedemptionAdd
        {
            UserId = twitchResponseJson["payload"]!["event"]!["user_id"]!.ToString(),
            UserName = twitchResponseJson["payload"]!["event"]!["user_name"]!.ToString(),
            UserInput = twitchResponseJson["payload"]!["event"]!["user_input"]!.ToString(),
            Status = twitchResponseJson["payload"]!["event"]!["status"]!.ToString(),
            CustomPowerUpTitle = twitchResponseJson["payload"]!["event"]!["custom_power_up"]!["title"]!.ToString(),
            CustomPowerUpBits = (int)twitchResponseJson["payload"]!["event"]!["custom_power_up"]!["bits"]!,
            CustomPowerUpPrompt = twitchResponseJson["payload"]!["event"]!["custom_power_up"]!["prompt"]!.ToString(),
            RedeemedAt = DateTime.Parse(twitchResponseJson["payload"]!["event"]!["redeemed_at"]!.ToString())
        };

        EventRegistry.TriggerChannelCustomPowerUpRedemptionAddReceived(channelCustomPowerUpRedemptionAdd);
    }
}