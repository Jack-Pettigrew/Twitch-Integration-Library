using System;

namespace TIL.Events;

public class ChannelPointsCustomRewardRedemption : EventArgs
{
    public string UserId { init; get; } = "";
    public string UserName { init; get; } = "";
    public string UserInput { init; get; } = "";
    public string Status { init; get; } = "";
    public string Title { init; get; } = "";
    public int Cost { init; get; } = 0;
    public string RewardPrompt { init; get; } = "";
    public DateTime RedeemedAt { init; get; } = DateTime.Now;
}