using System;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using TIL.DataContainers;

namespace TIL.Events;

public class BitsUse : EventArgs
{
    public enum TypeEnum
    {
        Text,
        Cheermote,
        Emote
    }

    [JsonPropertyName("user_id")]
    public string UserId { get; init; } = "";

    [JsonPropertyName("user_login")]
    public string UserLogin { get; init; } = "";

    [JsonPropertyName("user_name")]
    public string UserName { get; init; } = "";

    [JsonPropertyName("broadcaster_user_id")]
    public string BroadcasterUserId { get; init; } = "";

    [JsonPropertyName("broadcaster_user_login")]
    public string BroadcasterUserLogin { get; init; } = "";

    [JsonPropertyName("broadcaster_user_name")]
    public string BroadcasterUserName { get; init; } = "";

    [JsonPropertyName("bits")]
    public int Bits { get; init; } = 0;

    [JsonPropertyName("type")]
    public string RawType { get; init; } = "";

    [JsonIgnore]
    public TypeEnum Type => RawType switch
    {
        "text" => TypeEnum.Text,
        "cheermote" => TypeEnum.Cheermote,
        "emote" => TypeEnum.Emote,
        _ => TypeEnum.Text
    };

    // Dont know what these powerup objects are
    [JsonPropertyName("power_up")]
    public IEnumerable<JsonObject> PowerUp { get; init; } = [];

    [JsonPropertyName("custom_power_up")]
    public IEnumerable<JsonObject> CustomPowerUp { get; init; } = [];

    [JsonPropertyName("message")]
    public MessageContent? Message { get; init; }
}