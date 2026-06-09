using System;

namespace TIL.Events;

public class ChannelPointsAutomaticRewardRedemption : EventArgs
{
    public string UserId { init; get; } = "";
    public string UserName { init; get; } = "";
    public string RewardType { init; get; } = "";
    public int ChannelPoints { init; get; } = 0;
    public string? Message { init; get; } = null;
    public DateTime RedeemedAt { init; get; } = DateTime.Now;
}