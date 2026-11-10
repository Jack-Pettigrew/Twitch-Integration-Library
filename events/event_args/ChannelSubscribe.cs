using System;
using System.Text.Json.Serialization;
using TIL.DataContainers;

namespace TIL.Events;

public class ChannelSubscribe : EventArgs
{
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

    [JsonPropertyName("tier")]
    public string RawTier { get; init; } = "";

    [JsonIgnore]
    public Subscription.Tier Tier => Subscription.Parse(RawTier);

    [JsonPropertyName("is_gift")]
    public bool IsGift { get; init; } = false;
}