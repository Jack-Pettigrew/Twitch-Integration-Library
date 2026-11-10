using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

/// <summary>
/// An active Poll was has progressed subscription event.
/// </summary>
public partial class ChannelPollProgressEventSub : IEventSub
{
    public Scopes.Requirement GetScopeRequirement()
    {
        return new(Scopes.Mode.Any, [
            Scopes.CHANNEL_READ_POLLS,
            Scopes.CHANNEL_MANAGE_POLLS
        ]);
    }

    public string GetSubscriptionType()
    {
        return Subscriptions.CHANNEL_POLL_PROGRESS;
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
        Poll? poll = JsonSerializer.Deserialize<Poll>(twitchResponseJson["payload"]!["event"]);

        if (poll is not null) EventRegistry.TriggerPollProgressReceived(poll);
    }
}