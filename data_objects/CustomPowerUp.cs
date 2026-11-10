using System.Text.Json.Serialization;

namespace TIL.DataContainers;

public sealed record CustomPowerUp
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("title")]
    public string Title { get; init; } = "";

    [JsonPropertyName("bits")]
    public int Bits { get; init; } = 0;

    [JsonPropertyName("prompt")]
    public string Prompt { get; init; } = "";
}