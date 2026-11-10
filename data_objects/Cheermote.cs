using System.Text.Json.Serialization;

namespace TIL.DataContainers;

public sealed class CheermoteObject
{
    [JsonPropertyName("prefix")]
    public string Prefix { get; init; } = "";

    [JsonPropertyName("bits")]
    public int Bits { get; init; } = 0;

    [JsonPropertyName("tier")]
    public int Tier { get; init; } = 0;
}