using System.Text.Json.Serialization;

namespace TIL.Network.Responses;

public record DeviceCodeRequestResponse
{
    [JsonPropertyName("device_code")]
    public string DeviceCode { get; init; } = "";

    [JsonPropertyName("user_code")]
    public string UserCode { get; init; } = "";

    [JsonPropertyName("verification_uri")]
    public string? VerificationUri { get; init; } = "";

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; } = 0;

    [JsonPropertyName("interval")]
    public int Interval { get; init; } = 0;
}
