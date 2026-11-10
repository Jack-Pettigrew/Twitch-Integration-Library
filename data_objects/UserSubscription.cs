using System.Text.Json.Serialization;

namespace TIL.DataContainers;

public sealed record UserSubscription
{
    [JsonPropertyName("broadcaster_id")]
    public string BroadcasterId { get; init; } = "";

    [JsonPropertyName("broadcaster_login")]
    public string BroadcasterLogin {get; init; } = "";

    [JsonPropertyName("broadcaster_name")]
    public string BroadcasterName { get; init; } = "";

    [JsonPropertyName("gifter_id")]
    public string GifterId { get; init; } = "";

    [JsonPropertyName("gifter_login")]
    public string GifterLogin { get; init; } = "";

    [JsonPropertyName("gifter_name")]
    public string GiferName { get; init; } = "";

    [JsonPropertyName("is_gift")]
    public bool IsGift { get; init; } = false;

    [JsonPropertyName("tier")]
    public string RawTier { get; init; } = "";
    
    [JsonIgnore]
    public Subscription.Tier Tier => Subscription.Parse(RawTier);
}