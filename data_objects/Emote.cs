using System.Text.Json.Serialization;

namespace TIL.DataContainers;

public sealed class Emote
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("emote_set_id")]
    public string EmoteSetId { get; init; } = "";

    [JsonPropertyName("owner_id")]
    public string OwnerId { get; init; } = "";

    [JsonPropertyName("format")]
    public string[] Format { get; init; } = [];
}