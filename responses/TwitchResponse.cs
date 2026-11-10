using System.Text.Json.Serialization;

public record TwitchResponse<T>
{
    [JsonPropertyName("data")]
    public T[] Data { get; set; } = [];

    [JsonPropertyName("pagination")]
    public TwitchPagination Pagination { get; set; } = new();

    [JsonPropertyName("total")]
    public int? Total { get; set; }
}

public record TwitchPagination
{
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }
}