using System;

namespace TIL.Events;

public class ChannelSubscribeMessage : EventArgs
{
    public string UserId { set; get; } = "";
    public string UserName { set; get; } = "";
    public string Tier { set; get; } = "";
    public string Message { set; get; } = "";
    public int CumulativeMonths { set; get; } = 0;
    public int StreakMonths { set; get; } = 0;
    public int DurationMonths { set; get; } = 0;
}