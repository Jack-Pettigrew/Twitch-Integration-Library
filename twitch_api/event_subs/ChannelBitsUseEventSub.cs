using System;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

/// <summary>
/// A user uses Bits in the channel (currently Cheers, Power-ups, Custom Power-ups).
/// </summary>
public partial class ChannelBitsUseEventSub : IEventSub
{
    public string GetScope()
    {
        return Scopes.CHANNEL_BITS_READ;
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
        BitsUse bitsUse = new BitsUse
        {
            UserId = twitchResponseJson["payload"]!["event"]!["user_id"]!.ToString(),
            UserName = twitchResponseJson["payload"]!["event"]!["user_name"]!.ToString(),
            Bits = (int)twitchResponseJson["payload"]!["event"]!["bits"]!,
            Type = twitchResponseJson["payload"]!["event"]!["type"]!.ToString(),
            Message = twitchResponseJson["payload"]!["event"]!["message"]!["text"]!.ToString()
        };

        EventRegistry.TriggerBitsUseReceived(bitsUse);
    }
}