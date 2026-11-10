using System.Text.Json.Serialization;

namespace TIL.Events;

public class Prediction : EventArgs
{
    public const int MinOutcomes = 2;
    public const int MaxOutcomes = 10;

    public const int MaxTitleLength = 45;
    public const int MaxPredictionLength = 25;

    public const int MinPredictionWindow = 30;
    public const int MaxPredictionWindow = 1800;

    public enum StatusEnum
    {
        Active,
        Cancelled,
        Locked,
        Resolved,
        Unknown
    }

    public enum ColorEnum
    {
        Blue,
        Pink,
        Unknown
    }

    public sealed record TopPredictors
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; init; } = "";

        [JsonPropertyName("user_login")]
        public string UserLogin { get; init; } = "";

        [JsonPropertyName("user_name")]
        public string UserName { get; init; } = "";

        [JsonPropertyName("channel_points_used")]
        public int ChannelPointUsed { get; init; } = 0;

        [JsonPropertyName("channel_points_won")]
        public int? ChannelPointWon { get; init; } = 0;
    }

    public sealed record Outcome
    {
        [JsonPropertyName("id")]
        public string Id { get; init; } = "";

        [JsonPropertyName("title")]
        public string Title { get; init; } = "";

        [JsonPropertyName("users")]
        public int Users { get; init; } = 0;

        [JsonPropertyName("channel_points")]
        public int ChannelPoints { get; init; } = 0;

        [JsonPropertyName("top_predictors")]
        public TopPredictors[] TopPredictors { get; init; } = [];

        [JsonPropertyName("color")]
        public string RawColor { get; init; } = "";

        [JsonIgnore]
        public ColorEnum Color => RawColor switch
        {
            "PINK" => ColorEnum.Pink,
            "BLUE" => ColorEnum.Blue,
            _ => ColorEnum.Unknown
        };
    }

    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("broadcaster_user_id")]
    public string BroadcasterId { get; init; } = "";

    [JsonPropertyName("broadcaster_user_login")]
    public string BroadcasterLogin { get; init; } = "";

    [JsonPropertyName("broadcaster_user_name")]
    public string BroadcasterName { get; init; } = "";

    [JsonPropertyName("title")]
    public string Title { get; init; } = "";

    [JsonPropertyName("winning_outcome_id")]
    public string WinningOutcomeId { get; init; } = "";

    [JsonPropertyName("outcomes")]
    public Outcome[] Outcomes { get; init; } = [];

    [JsonPropertyName("prediction_window")]
    public int PredictionWindow { get; init; } = 0;

    [JsonPropertyName("status")]
    public string RawStatus { get; init; } = "";

    [JsonIgnore]
    public StatusEnum Status => RawStatus switch
    {
        "ACTIVE" => StatusEnum.Active,
        "CANCELLED" => StatusEnum.Cancelled,
        "LOCKED" => StatusEnum.Locked,
        "RESOLVED" => StatusEnum.Resolved,
        _ => StatusEnum.Unknown
    };

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; } = null;

    [JsonPropertyName("started_at")]
    public DateTimeOffset? StartedAt { get; init; } = null;

    [JsonPropertyName("ended_at")]
    public DateTimeOffset? EndedAt { get; init; } = null;

    [JsonPropertyName("locks_at")]
    public DateTimeOffset? LocksAt { get; init; } = null;

    [JsonPropertyName("locked_at")]
    public DateTimeOffset? LockedAt { get; init; } = null;
}