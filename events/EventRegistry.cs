using System;

namespace TIL.Events;

public static class EventRegistry
{
    public delegate void TwitchEventHandler<T>(T twitchEventArg) where T : EventArgs;

    public static event TwitchEventHandler<ChatMessage>? ChatMessageReceived;
    public static event TwitchEventHandler<ChannelFollow> ChannelFollowReceived;
    public static event TwitchEventHandler<ChannelSubscribe> ChannelSubscribeReceived;

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
}