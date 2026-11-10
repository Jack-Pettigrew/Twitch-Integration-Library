using System.Text.Json.Serialization;

namespace TIL.DataContainers;

public sealed record Chatter
{
    [JsonPropertyName("user_id")]
    public string UserId { get; init; } = "";

    [JsonPropertyName("user_login")]
    public string UserLogin { get; init; } = "";

    [JsonPropertyName("user_name")]
    public string UserName { get; init; } = "";
}