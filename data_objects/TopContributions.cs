using System.Text.Json.Serialization;

namespace TIL.DataContainers;

public record TopContributions
{
    public enum TypeEnum
    {
        Bits,
        Subscription,
        Other
    }

    [JsonPropertyName("user_id")]
    public string UserId { get; init; } = "";

    [JsonPropertyName("user_login")]
    public string UserLogin { get; init; } = "";

    [JsonPropertyName("user_name")]
    public string UserName { get; init; } = "";

    [JsonPropertyName("type")]
    public string RawType { get; init; } = "";

    [JsonIgnore]
    public TypeEnum Type => RawType switch
    {
        "bits" => TypeEnum.Bits,
        "subscription" => TypeEnum.Subscription,
        "other" => TypeEnum.Other,
        _ => TypeEnum.Other
    };

    [JsonPropertyName("total")]
    public int Total { get; init; } = 0;
}