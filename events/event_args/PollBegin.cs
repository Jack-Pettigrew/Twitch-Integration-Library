using System;
using System.Text.Json.Nodes;

namespace TIL.Events;

public class PollBegin : EventArgs
{
    public string Title { init; get; } = "";
    public IEnumerable<JsonObject> Choices { init; get; } = Array.Empty<JsonObject>();
    public bool BitsVotingEnabled { init; get; } = false;
    public int BitsVotingAmount { init; get; } = 0;
    public bool ChannelPointsVotingEnabled { init; get; } = false;
    public int ChannelPointsVotingAmount { init; get; } = 0;
    public DateTime StartedAt { init; get; } = DateTime.Now;
    public DateTime? EndsAt { init; get; } = null;
}