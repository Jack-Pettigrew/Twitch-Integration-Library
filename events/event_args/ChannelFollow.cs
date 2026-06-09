using System;

namespace TIL.Events;

public class ChannelFollow : EventArgs
{
    public string UserId { set; get; } = "";
    public string UserName { set; get; } = "";
    public DateTime FollowedAt { set; get; } = DateTime.Now;
}