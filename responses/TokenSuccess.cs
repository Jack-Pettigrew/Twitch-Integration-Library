namespace TIL.Network.Responses;

public record TokenSuccess
{
    public string? access_token { get; init; }
    public int expires_in { get; init; }
    public string? refresh_token { get; init; }
    public string[]? scopes { get; init; }
    public string? token_type { get; init; }
}
