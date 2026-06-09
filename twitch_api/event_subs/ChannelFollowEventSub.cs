using System;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

public partial class ChannelFollowEventSub : IEventSub
{
    public string GetScope()
    {
        return Scopes.MODERATOR_READ_FOLLOWERS;
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
                broadcaster_user_id = twitchSessionContext.user_id,
                moderator_user_id = twitchSessionContext.user_id
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
        ChannelFollow channelFollow = new ChannelFollow
        {
            UserId = twitchResponseJson["payload"]!["event"]!["user_id"].ToString(),
            UserName = twitchResponseJson["payload"]!["event"]!["user_name"].ToString(),
            FollowedAt = DateTime.Parse(twitchResponseJson["payload"]!["event"]!["user_name"].ToString())
        };

        EventRegistry.TriggerChannelFollowReceived(channelFollow);
    }
}