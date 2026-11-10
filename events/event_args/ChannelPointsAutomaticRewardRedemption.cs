using System;
using System.Text.Json.Serialization;
using TIL.DataContainers;

namespace TIL.Events;

public class ChannelPointsAutomaticRewardRedemption : EventArgs
{
    public sealed record RewardObject
    {
        [JsonPropertyName("type")]
        public string Type { get; init; } = "";

        [JsonPropertyName("channel_points")]
        public int ChannelPoints { get; init; } = 0;

        [JsonPropertyName("emote")]
        public Emote? Emote { get; init; } = null;
    }

    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("broadcaster_user_id")]
    public string BroadcasterUserId { get; init; } = "";

    [JsonPropertyName("broadcaster_user_login")]
    public string BroadcasterUserLogin { get; init; } = "";

    [JsonPropertyName("broadcaster_user_name")]
    public string BroadcasterUserName { get; init; } = "";

    [JsonPropertyName("user_id")]
    public string UserId { get; init; } = "";

    [JsonPropertyName("user_login")]
    public string UserLogin { get; init; } = "";

    [JsonPropertyName("user_name")]
    public string UserName { get; init; } = "";

    [JsonPropertyName("reward")]
    public RewardObject? Reward { get; init; } = null;

    [JsonPropertyName("message")]
    public MessageContent? Message { get; init; } = null;

    [JsonPropertyName("redeemed_at")]
    public DateTimeOffset? RedeemedAt { get; init; } = null;
}