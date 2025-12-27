namespace TIL.Network.Responses;

public record DeviceCodeRequestResponse
{
    public string? device_code { get; init; }
    public string? user_code { get; init; }
    public string? verification_uri { get; init; }
    public int expires_in { get; init; }
    public int interval { get; init; }
}
