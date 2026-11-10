using System.Text.Json.Serialization;

namespace TIL.DataContainers;

public class Reward
{
    public enum Status
    {
        Fulfulled,
        Unfulfilled,
        Canceled,
        Unknown
    }

    public static Status ParseStatus(string status) => status switch
    {
        "fulfulled" => Status.Fulfulled,
        "unfulfilled" => Status.Unfulfilled,
        "canceled" => Status.Canceled,
        "unknown" => Status.Unknown,
        _ => Status.Unknown
    };

    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("title")]
    public string Title { get; init; } = "";

    [JsonPropertyName("cost")]
    public int Cost { get; init; } = 0;

    [JsonPropertyName("prompt")]
    public string Prompt { get; init; } = "";
}