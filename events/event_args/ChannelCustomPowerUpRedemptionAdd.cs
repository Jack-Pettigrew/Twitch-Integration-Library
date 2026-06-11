using System;

namespace TIL.Events;

public class ChannelCustomPowerUpRedemptionAdd : EventArgs
{
    public string UserId { init; get; } = "";
    public string UserName { init; get; } = "";
    public string UserInput { init; get; } = "";
    public string Status { init; get; } = "";
    public string CustomPowerUpTitle { init; get; } = "";
    public int CustomPowerUpBits { init; get; } = 0;
    public string CustomPowerUpPrompt { init; get; } = "";
    public DateTime RedeemedAt { init; get; } = DateTime.Now;
}