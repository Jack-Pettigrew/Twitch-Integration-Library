using System;

namespace TIL.Events;

public static class EventRegistry
{
    public delegate void TwitchEventHandler<T>(T twitchEventArg) where T : EventArgs;

    // CHANNEL
    public static event TwitchEventHandler<ChatMessage>? ChatMessageReceived;
    public static event TwitchEventHandler<ChannelFollow>? ChannelFollowReceived;
    public static event TwitchEventHandler<ChannelSubscribe>? ChannelSubscribeReceived;
    public static event TwitchEventHandler<ChannelSubscribeMessage>? ChannelSubscribeMessageReceived;

    // POLL
    public static event TwitchEventHandler<PollBegin>? PollBeginReceived;
    public static event TwitchEventHandler<PollProgress>? PollProgressReceived;

    public static void TriggerChatMessageReceived(ChatMessage chatMessage)
    {
        ChatMessageReceived?.Invoke(chatMessage);
    }

    public static void TriggerChannelFollowReceived(ChannelFollow channelFollow)
    {
        ChannelFollowReceived?.Invoke(channelFollow);
    }

    public static void TriggerChannelSubscribeReceived(ChannelSubscribe channelSubscribe)
    {
        ChannelSubscribeReceived?.Invoke(channelSubscribe);
    }

    public static void TriggerChannelSubscribeMessageReceived(ChannelSubscribeMessage channelSubscribeMessage)
    {
        ChannelSubscribeMessageReceived?.Invoke(channelSubscribeMessage);
    }

    public static void TriggerPollBeginReceived(PollBegin pollBegin)
    {
        PollBeginReceived?.Invoke(pollBegin);
    }

    public static void TriggerPollProgressReceived(PollProgress pollProgress)
    {
        PollProgressReceived?.Invoke(pollProgress);
    }
}