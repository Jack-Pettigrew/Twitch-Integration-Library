using System;
using System.Text.Json.Serialization;
using TIL.DataContainers;

namespace TIL.Events;

public class ChannelCustomPowerUpRedemption : EventArgs
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("user_id")]
    public string UserId { get; init; } = "";

    [JsonPropertyName("user_login")]
    public string UserLogin { get; init; } = "";

    [JsonPropertyName("user_name")]
    public string UserName { get; init; } = "";

    [JsonPropertyName("broadcaster_user_id")]
    public string BroadcasterUserId { get; init; } = "";

    [JsonPropertyName("broadcaster_user_login")]
    public string BroadcasterUserLogin { get; init; } = "";

    [JsonPropertyName("broadcaster_user_name")]
    public string BroadcasterUserName { get; init; } = "";

    [JsonPropertyName("user_input")]
    public string UserInput { get; init; } = "";

    [JsonPropertyName("status")]
    public string RawStatus { get; init; } = "";

    [JsonIgnore]
    public Reward.Status Status => Reward.ParseStatus(RawStatus);

    [JsonPropertyName("custom_power_up")]
    public CustomPowerUp? CustomPowerUp { get; init; } = null;

    [JsonPropertyName("redeemed_at")]
    public DateTimeOffset? RedeemedAt { get; init; } = null;
}