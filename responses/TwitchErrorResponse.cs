using System.Net;
using System.Text.Json.Serialization;

public record TwitchErrorResponse
{
    [JsonPropertyName("error")]
    public string Error { get; init; } = "";
    
    [JsonPropertyName("status")]
    public HttpStatusCode Status { get; init; }

    [JsonPropertyName("message")]
    public string Message { get; init; } = "";
}