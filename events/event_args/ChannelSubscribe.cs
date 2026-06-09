using System;

namespace TIL.Events;

public class ChannelSubscribe : EventArgs
{
    public string UserId { set; get; } = "";
    public string UserName { set; get; } = "";
    public string Tier { set; get; } = "";
    public bool IsGift { set; get; } = false;
}