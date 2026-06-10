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

    // GOAL
    public static event TwitchEventHandler<GoalBegin>? GoalBeginReceived;
    public static event TwitchEventHandler<GoalProgress>? GoalProgressReceived;
    public static event TwitchEventHandler<GoalEnd>? GoalEndReceived;

    // HYPE TRAIN
    public static event TwitchEventHandler<HypeTrainBegin>? HypeTrainBeginReceived;
    public static event TwitchEventHandler<HypeTrainProgress>? HypeTrainProgressReceived;
    public static event TwitchEventHandler<HypeTrainEnd>? HypeTrainEndReceived;

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

    public static void TriggersGoalBeginReceived(GoalBegin goalBegin)
    {
        GoalBeginReceived?.Invoke(goalBegin);
    }

    public static void TriggerGoalProgressReceived(GoalProgress goalProgress)
    {
        GoalProgressReceived?.Invoke(goalProgress);
    }

    public static void TriggerGoalEndReceived(GoalEnd goalEnd)
    {
        GoalEndReceived?.Invoke(goalEnd);
    }

    public static void TriggerHypeTrainBeginReceived(HypeTrainBegin hypeTrainBegin)
    {
        HypeTrainBeginReceived?.Invoke(hypeTrainBegin);
    }

    public static void TriggerHypeTrainProgressReceived(HypeTrainProgress hypeTrainProgress)
    {
        HypeTrainProgressReceived?.Invoke(hypeTrainProgress);
    }

    public static void TriggerHypeTrainEndReceived(HypeTrainEnd hypeTrainEnd)
    {
        HypeTrainEndReceived?.Invoke(hypeTrainEnd);
    }
}