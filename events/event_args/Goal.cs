using System.Text.Json.Serialization;

namespace TIL.Events;

public class Goal : EventArgs
{
    public enum TypeEnum
    {
        Follow,
        Subscription,
        SubscriptionCount,
        NewSubscription,
        NewSubscriptionCount,
        NewBit,
        NewCheerer,
        Unknown
    }

    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("broadcaster_user_id")]
    public string BroadcasterUserId { get; init; } = "";

    [JsonPropertyName("broadcaster_user_login")]
    public string BroadcasterUserLogin { get; init; } = "";

    [JsonPropertyName("broadcaster_user_name")]
    public string BroadcasterUserName { get; init; } = "";

    [JsonPropertyName("type")]
    public string RawType { get; init; } = "";

    [JsonIgnore]
    public TypeEnum Type => RawType switch
    {
        "follow" => TypeEnum.Follow,
        "subscription" => TypeEnum.Subscription,
        "subscription_count" => TypeEnum.SubscriptionCount,
        "new_subscription" => TypeEnum.NewSubscription,
        "new_subscription_count" => TypeEnum.NewSubscriptionCount,
        "new_bit" => TypeEnum.NewBit,
        "new_cheerer" => TypeEnum.NewCheerer,
        _ => TypeEnum.Unknown
    };

    [JsonPropertyName("description")]
    public string Description { get; init; } = "";

    [JsonPropertyName("current_amount")]
    public int CurrentAmount { get; init; } = 0;

    [JsonPropertyName("target_amount")]
    public int TargetAmount { get; init; } = 0;

    [JsonPropertyName("is_achieved")]
    public bool IsAchieved { get; init; } = false;

    [JsonPropertyName("started_at")]
    public DateTimeOffset? StartedAt { get; init; } = null;

    [JsonPropertyName("ended_at")]
    public DateTimeOffset? StartedEnded { get; init; } = null;
}