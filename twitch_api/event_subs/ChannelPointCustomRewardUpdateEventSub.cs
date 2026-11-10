using System.Text.Json;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.TwitchAPI;
using TIL.TwitchAPI.EventSubs;

namespace TIL.Events;

public class ChannelPointCustomRewardUpdateEventSub : IEventSub
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
        return Subscriptions.CHANNEL_POINTS_CUSTOM_REWARD_UPDATE;
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
        var customRewardAdd = JsonSerializer.Deserialize<ChannelPointsCustomReward>(twitchResponseJson);

        if (customRewardAdd is not null) EventRegistry.TriggerChannelPointsCustomRewardUpdateRecieved(customRewardAdd);
    }
}