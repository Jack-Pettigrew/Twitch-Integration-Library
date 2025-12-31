namespace TIL.DataContainers;

public record TwitchDeviceToken
{
    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public DateTime ExpiresAtUtc { get; set; }
    public string[] Scopes { get; set; } = [];
}