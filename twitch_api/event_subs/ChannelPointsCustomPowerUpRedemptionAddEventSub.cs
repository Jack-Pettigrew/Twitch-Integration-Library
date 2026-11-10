using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

/// <summary>
/// A user redeemed a Twitch curated Channel Points Reward.
/// </summary>
public partial class ChannelPointsCustomPowerUpRedemptionAddEventSub : IEventSub
{
    public Scopes.Requirement GetScopeRequirement()
    {
        return new(Scopes.Mode.All, [
            Scopes.CHANNEL_BITS_READ
        ]);
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
        var redemption = JsonSerializer.Deserialize<ChannelCustomPowerUpRedemption>(twitchResponseJson);

        if (redemption is not null) EventRegistry.TriggerChannelCustomPowerUpRedemptionAddReceived(redemption);
    }
}