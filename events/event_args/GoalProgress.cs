using System;
using System.Text.Json.Nodes;

namespace TIL.Events;

public class GoalProgress : EventArgs
{
    public string Type { init; get; } = "";
    public string Description { init; get; } = "";
    public int CurrentAmount { init; get; } = 0;
    public int TargetAmount { init; get; } = 0;
    public DateTime StartedAt { init; get; } = DateTime.Now;
}