using System.Text.Json.Serialization;

namespace TIL.DataContainers;

public sealed record MessageContent
{
    [JsonPropertyName("text")]
    public string Text { get; init; } = "";

    [JsonPropertyName("fragments")]
    public MessageFragment[] Fragments { get; init; } = [];
}