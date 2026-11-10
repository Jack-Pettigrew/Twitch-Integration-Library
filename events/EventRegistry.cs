using System;
using TIL.DataContainers;

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
    public static event TwitchEventHandler<Poll>? PollBeginReceived;
    public static event TwitchEventHandler<Poll>? PollProgressReceived;
    public static event TwitchEventHandler<Poll>? PollEndReceived;

    // REWARDS
    public static event TwitchEventHandler<ChannelPointsAutomaticRewardRedemption>? ChannelPointsAutomaticRewardRedemptionAddReceived;
    public static event TwitchEventHandler<ChannelPointsCustomRewardRedemption>? ChannelPointsCustomRewardRedemptionReceived;
    public static event TwitchEventHandler<ChannelPointsCustomReward>? ChannelPointsCustomRewardAdd;
    public static event TwitchEventHandler<ChannelPointsCustomReward>? ChannelPointsCustomRewardUpdate;
    public static event TwitchEventHandler<ChannelPointsCustomReward>? ChannelPointsCustomRewardRemove;

    // POWER UPS
    public static event TwitchEventHandler<ChannelCustomPowerUpRedemption>? ChannelCustomPowerUpRedemptionAddReceived;

    // BITS
    public static event TwitchEventHandler<BitsUse>? BitsUseReceived;

    // GOAL
    public static event TwitchEventHandler<Goal>? GoalBeginReceived;
    public static event TwitchEventHandler<Goal>? GoalProgressReceived;
    public static event TwitchEventHandler<Goal>? GoalEndReceived;

    // HYPE TRAIN
    public static event TwitchEventHandler<HypeTrain>? HypeTrainBeginReceived;
    public static event TwitchEventHandler<HypeTrain>? HypeTrainProgressReceived;
    public static event TwitchEventHandler<HypeTrain>? HypeTrainEndReceived;

    // PREDICTION
    public static event TwitchEventHandler<Prediction>? PredictionBeginReceived;
    public static event TwitchEventHandler<Prediction>? PredictionProgressReceived;
    public static event TwitchEventHandler<Prediction>? PredictionLockReceived;
    public static event TwitchEventHandler<Prediction>? PredictionEndRecieved;

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

    public static void TriggerPollBeginReceived(Poll poll)
    {
        PollBeginReceived?.Invoke(poll);
    }

    public static void TriggerPollProgressReceived(Poll poll)
    {
        PollProgressReceived?.Invoke(poll);
    }

    public static void TriggerPollEndReceived(Poll poll)
    {
        PollEndReceived?.Invoke(poll);
    }

    public static void TriggerChannelPointsAutomaticRewardRedemptionAddReceived(ChannelPointsAutomaticRewardRedemption rewardRedemptionAdd)
    {
        ChannelPointsAutomaticRewardRedemptionAddReceived?.Invoke(rewardRedemptionAdd);
    }

    public static void TriggerChannelPointsCustomRewardRedemptionReceived(ChannelPointsCustomRewardRedemption channelPointsCustomRewardRedemption)
    {
        ChannelPointsCustomRewardRedemptionReceived?.Invoke(channelPointsCustomRewardRedemption);
    }

    public static void TriggerChannelPointsCustomRewardAddRecieved(ChannelPointsCustomReward customReward)
    {
        ChannelPointsCustomRewardAdd?.Invoke(customReward);
    }

    public static void TriggerChannelPointsCustomRewardUpdateRecieved(ChannelPointsCustomReward customReward)
    {
        ChannelPointsCustomRewardUpdate?.Invoke(customReward);
    }

    public static void TriggerChannelPointsCustomRewardRemoveRecieved(ChannelPointsCustomReward customReward)
    {
        ChannelPointsCustomRewardRemove?.Invoke(customReward);
    }

    public static void TriggerBitsUseReceived(BitsUse bitsUse)
    {
        BitsUseReceived?.Invoke(bitsUse);
    }

    public static void TriggerGoalBeginReceived(Goal goal)
    {
        GoalBeginReceived?.Invoke(goal);
    }

    public static void TriggerGoalProgressReceived(Goal goal)
    {
        GoalProgressReceived?.Invoke(goal);
    }

    public static void TriggerGoalEndReceived(Goal goal)
    {
        GoalEndReceived?.Invoke(goal);
    }

    public static void TriggerHypeTrainBeginReceived(HypeTrain hypeTrain)
    {
        HypeTrainBeginReceived?.Invoke(hypeTrain);
    }

    public static void TriggerHypeTrainProgressReceived(HypeTrain hypeTrain)
    {
        HypeTrainProgressReceived?.Invoke(hypeTrain);
    }

    public static void TriggerHypeTrainEndReceived(HypeTrain hypeTrain)
    {
        HypeTrainEndReceived?.Invoke(hypeTrain);
    }

    public static void TriggerChannelCustomPowerUpRedemptionAddReceived(ChannelCustomPowerUpRedemption channelCustomPowerUpRedemption)
    {
        ChannelCustomPowerUpRedemptionAddReceived?.Invoke(channelCustomPowerUpRedemption);
    }

    public static void TriggerPredictionBegin(Prediction prediction)
    {
        PredictionBeginReceived?.Invoke(prediction);
    }

    public static void TriggerPredictionProgress(Prediction prediction)
    {
        PredictionProgressReceived?.Invoke(prediction);
    }

    public static void TriggerPredictionLock(Prediction prediction)
    {
        PredictionLockReceived?.Invoke(prediction);
    }

    public static void TriggerPredictionEnd(Prediction prediction)
    {
        PredictionEndRecieved?.Invoke(prediction);
    }
}