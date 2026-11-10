using System;
using System.Text.Json.Serialization;

namespace TIL.Events;

public class ChannelPointsCustomReward : EventArgs
{
    public sealed record MaxPerStreamObject
    {
        [JsonPropertyName("is_enabled")]
        public bool IsEnabled { get; init; } = false;

        [JsonPropertyName("value")]
        public int Value { get; init; } = 0;
    }

    public sealed record GlobalCooldownObject
    {
        [JsonPropertyName("is_enabled")]
        public bool IsEnabled { get; init; } = false;

        [JsonPropertyName("seconds")]
        public int Seconds { get; init; } = 0;
    }
    
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("broadcaster_user_id")]
    public string BroadcasterUserId { get; init; } = "";

    [JsonPropertyName("broadcaster_user_login")]
    public string BroadcasterUserLogin { get; init; } = "";

    [JsonPropertyName("broadcaster_user_name")]
    public string BroadcasterUserName { get; init; } = "";

    [JsonPropertyName("is_enabled")]
    public bool IsEnabled { get; init; } = false;

    [JsonPropertyName("is_paused")]
    public bool IsFalse { get; init; } = false;

    [JsonPropertyName("is_in_stock")]
    public bool IsInStock { get; init; } = false;

    [JsonPropertyName("title")]
    public string Title { get; init; } = "";

    [JsonPropertyName("cost")]
    public int Cost { get; init; } = 0;

    [JsonPropertyName("prompt")]
    public string Prompt { get; init; } = "";

    [JsonPropertyName("is_user_input_required")]
    public bool IsUserInputRequired { get; init; } = false;

    [JsonPropertyName("should_redemptions_skip_request_queue")]
    public bool ShouldRedemptionsSkipRequestQueue { get; init; } = false;

    [JsonPropertyName("cooldown_expires_at")]
    public DateTimeOffset? CooldownExpiresAt { get; init; } = null;

    [JsonPropertyName("redemptions_redeemed_current_stream")]
    public int? RedemptionsRedeemedCurrentStream { get; init; } = null;

    [JsonPropertyName("max_per_stream")]
    public MaxPerStreamObject MaxPerStream { get; init; } = new();

    [JsonPropertyName("max_per_user_per_stream")]
    public MaxPerStreamObject MaxPerUserPerStream { get; init; } = new();

    [JsonPropertyName("global_cooldown")]
    public GlobalCooldownObject GlobalCooldown { get; init; } = new();

    [JsonPropertyName("background_color")]
    public string BackgroundColor { get; init; } = "";

    [JsonPropertyName("image")]
    public Dictionary<string, string> Images { get; init; } = new();

    [JsonPropertyName("default_image")]
    public Dictionary<string, string> DefaultImages { get; init; } = new();
}