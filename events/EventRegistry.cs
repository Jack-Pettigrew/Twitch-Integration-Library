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
    public static event TwitchEventHandler<PollEnd>? PollEndReceived;

    // REWARDS
    public static event TwitchEventHandler<ChannelPointsAutomaticRewardRedemption>? ChannelPointsAutomaticRewardRedemptionReceived;
    public static event TwitchEventHandler<ChannelPointsCustomRewardRedemption>? ChannelPointsCustomRewardRedemptionReceived;

    // BITS
    public static event TwitchEventHandler<BitsUse>? BitsUseReceived;

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

    public static void TriggerPollEndReceived(PollEnd pollEnd)
    {
        PollEndReceived?.Invoke(pollEnd);
    }

    public static void TriggerChannelPointsAutomaticRewardRedemptionReceived(ChannelPointsAutomaticRewardRedemption channelPointsAutomaticRewardRedemption)
    {
        ChannelPointsAutomaticRewardRedemptionReceived?.Invoke(channelPointsAutomaticRewardRedemption);
    }

    public static void TriggerChannelPointsCustomRewardRedemptionReceived(ChannelPointsCustomRewardRedemption channelPointsCustomRewardRedemption)
    {
        ChannelPointsCustomRewardRedemptionReceived?.Invoke(channelPointsCustomRewardRedemption);
    }

    public static void TriggerBitsUseReceived(BitsUse bitsUse)
    {
        BitsUseReceived?.Invoke(bitsUse);
    }
}