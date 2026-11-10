using System;
using System.Text.Json.Serialization;
using TIL.DataContainers;

namespace TIL.Events;

public class ChannelSubscribeMessage : EventArgs
{
    public sealed record MessageContent
    {
        [JsonPropertyName("text")]
        public string Text { get; init; } = "";

        [JsonPropertyName("emotes")]
        public Emote[] Emotes { get; init; } = [];
    }

    public record Emote
    {
        [JsonPropertyName("begin")]
        public int Begin { get; init; } = 0;

        [JsonPropertyName("end")]
        public int End { get; init; } = 0;

        [JsonPropertyName("id")]
        public string Id { get; init; } = "";
    }

    [JsonPropertyName("user_id")]
    public string UserId { get; init; } = "";

    [JsonPropertyName("user_login")]
    public string UserLogin { get; init; } = "";

    [JsonPropertyName("user_name")]
    public string UserName { get; init; } = "";

    [JsonPropertyName("broadcaster_user_id")]
    public string BroadcasterId { get; init; } = "";

    [JsonPropertyName("broadcaster_user_login")]
    public string BroadcasterLogin { get; init; } = "";

    [JsonPropertyName("broadcaster_user_name")]
    public string BroadcasterName { get; init; } = "";

    [JsonPropertyName("tier")]
    public string RawTier { get; init; } = "";

    [JsonIgnore]
    public Subscription.Tier Tier => Subscription.Parse(RawTier);

    [JsonPropertyName("message")]
    public MessageContent? Message { get; init; }

    [JsonPropertyName("cumulative_months")]
    public int CumulativeMonths { get; init; } = 0;

    [JsonPropertyName("streak_months")]
    public int? StreakMonths { get; init; } = null; // null if not shared

    [JsonPropertyName("duration_months")]
    public int DurationMonths { get; init; } = 0;
}