using System;
using System.Text.Json.Nodes;

namespace TIL.Events;

public class HypeTrainProgress : EventArgs
{
    /// <summary>
    /// Types: treasure, golden_kappa, regular
    /// </summary>
    public string Type { init; get; } = "";
    public int Level { init; get; } = 0;
    public int Total { init; get; } = 0;
    public int Progress { init; get; } = 0;
    public int Goal { init; get; } = 0;
    public IEnumerable<JsonObject> TopContributions { init; get; } = Array.Empty<JsonObject>();
    public DateTime StartedAt { init; get; } = DateTime.Now;
    public DateTime ExpiresAt { init; get; } = DateTime.Now.AddMinutes(1);
}