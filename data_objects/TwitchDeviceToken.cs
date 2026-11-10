using System;
using System.Text.Json.Serialization;

namespace TIL.DataContainers;

public record TwitchDeviceToken
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = "";

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; } = "";

    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = "";

    [JsonPropertyName("scope")]
    public string[] Scopes { get; init; } = [];

    [JsonInclude]
    [JsonPropertyName("expires_in")]
    private int ExpiresIn { get; init; } = 0;

    [JsonIgnore]
    private DateTimeOffset RecievedAt { get; init; } = DateTimeOffset.UtcNow;

    [JsonIgnore]
    public DateTimeOffset ExpiresAt => RecievedAt.AddSeconds(ExpiresIn);
}