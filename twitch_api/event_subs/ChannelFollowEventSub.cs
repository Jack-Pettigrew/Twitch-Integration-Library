using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

/// <summary>
/// A new follower event.
/// </summary>
public partial class ChannelFollowEventSub : IEventSub
{
    public Scopes.Requirement GetScopeRequirement()
    {
        return new(Scopes.Mode.All, [
            Scopes.MODERATOR_READ_FOLLOWERS
        ]);
    }

    public string GetSubscriptionType()
    {
        return Subscriptions.CHANNEL_FOLLOW;
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
                moderator_user_id = twitchSessionContext.userId
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
        ChannelFollow? channelFollow = JsonSerializer.Deserialize<ChannelFollow>(twitchResponseJson);

        if (channelFollow is not null) EventRegistry.TriggerChannelFollowReceived(channelFollow);
    }
}