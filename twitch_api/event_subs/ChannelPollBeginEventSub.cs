using System;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;

namespace TIL.TwitchAPI.EventSubs;

/// <summary>
/// A new Poll was created subscription event.
/// </summary>
public partial class ChannelPollBeginEventSub : IEventSub
{
    public string GetScope()
    {
        return Scopes.CHANNEL_READ_POLLS;
    }

    public string GetSubscriptionType()
    {
        return Subscriptions.CHANNEL_POLL_BEGIN;
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
        PollBegin pollBegin = new PollBegin
        {
            Title = twitchResponseJson["payload"]!["event"]!["title"]!.ToString(),
            Choices = twitchResponseJson["payload"]!["event"]!["choices"]!.AsArray().Select(x => x!.AsObject()),
            BitsVotingEnabled = (bool)twitchResponseJson["payload"]!["event"]!["bits_voting"]!["is_enabled"]!,
            BitsVotingAmount = (int)twitchResponseJson["payload"]!["event"]!["bits_voting"]!["amount_per_vote"]!,
            ChannelPointsVotingEnabled = (bool)twitchResponseJson["payload"]!["event"]!["channel_points_voting"]!["is_enabled"]!,
            ChannelPointsVotingAmount = (int)twitchResponseJson["payload"]!["event"]!["channel_points_voting"]!["amount_per_vote"]!,
            StartedAt = DateTime.Parse(twitchResponseJson["payload"]!["event"]!["channel_points_voting"]!["amount_per_vote"]!.ToString()),
            EndsAt = DateTime.Parse(twitchResponseJson["payload"]!["event"]!["channel_points_voting"]!["amount_per_vote"]!.ToString())
        };

        EventRegistry.TriggerPollBeginReceived(pollBegin);
    }
}