using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

/// <summary>
/// A user uses Bits in the channel (currently Cheers, Power-ups, Custom Power-ups).
/// </summary>
public partial class ChannelBitsUseEventSub : IEventSub
{
    public Scopes.Requirement GetScopeRequirement()
    {
        return new(Scopes.Mode.All, [
            Scopes.CHANNEL_BITS_READ
        ]);
    }

    public string GetSubscriptionType()
    {
        return Subscriptions.CHANNEL_BITS_USE;
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
        BitsUse? bitsUse = JsonSerializer.Deserialize<BitsUse>(twitchResponseJson["payload"]!["event"]);

        if (bitsUse is not null) EventRegistry.TriggerBitsUseReceived(bitsUse);
    }
}