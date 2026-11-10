using System.Text.Json.Serialization;

namespace TIL.DataContainers;

public sealed record TwitchUser
{
    public enum UserTypeEnum
    {
        Admin,
        GlobalMod,
        Staff,
        Normal
    }

    public enum BroadcasterTypeEnum
    {
        Affiliate,
        Partner,
        Normal
    }

    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("login")]
    public string Login { get; init; } = "";

    [JsonPropertyName("display_name")]
    public string DisplayName { get; init; } = "";

    [JsonInclude]
    [JsonPropertyName("type")]
    private string RawType { get; init; } = "";

    [JsonIgnore]
    public UserTypeEnum Type => RawType switch
    {
        "admin" => UserTypeEnum.Admin,
        "global_mod" => UserTypeEnum.GlobalMod,
        "staff" => UserTypeEnum.Staff,
        _ => UserTypeEnum.Normal
    };

    [JsonInclude]
    [JsonPropertyName("broadcaster_type")]
    private string RawBroadcasterType { get; init; } = "";

    [JsonIgnore]
    public BroadcasterTypeEnum BroadcasterType => RawBroadcasterType switch
    {
        "affiliate" => BroadcasterTypeEnum.Affiliate,
        "partner" => BroadcasterTypeEnum.Partner,
        _ => BroadcasterTypeEnum.Normal
    };

    [JsonPropertyName("description")]
    public string Description { get; init; } = "";

    [JsonPropertyName("profile_image_url")]
    public string ProfileImageUrl { get; init; } = "";

    [JsonPropertyName("offline_image_url")]
    public string OfflineImageUrl { get; init; } = "";

    [Obsolete]
    [JsonPropertyName("view_count")]
    public int ViewCount { get; init; } = 0;

    [JsonPropertyName("email")]
    public string Email { get; init; } = "";

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.Now;
}