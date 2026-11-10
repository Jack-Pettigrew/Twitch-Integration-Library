using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using TIL.DataContainers;

namespace TIL.Events;

public class HypeTrain : EventArgs
{
    public enum TypeEnum
    {
        Treasure,
        GoldenKappa,
        Regular
    }

    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("broadcaster_user_id")]
    public string BroadcasterUserId { get; init; } = "";

    [JsonPropertyName("broadcaster_user_login")]
    public string BroadcasterUserLogin { get; init; } = "";

    [JsonPropertyName("broadcaster_user_name")]
    public string BroadcasterUserName { get; init; } = "";

    [JsonPropertyName("total")]
    public int Total { get; init; } = 0;

    [JsonPropertyName("progress")]
    public int Progress { get; init; } = 0;

    [JsonPropertyName("goal")]
    public int Goal { get; init; } = 0;

    [JsonPropertyName("top_contributions")]
    public TopContributions[] TopContributions { get; init; } = [];

    [JsonPropertyName("shared_train_participants")]
    public IEnumerable<JsonObject> SharedTrainParticipants { get; init; } = [];

    [JsonPropertyName("level")]
    public int Level { get; init; } = 0;

    [JsonPropertyName("started_at")]
    public DateTimeOffset? StartedAt { get; init; } = null;

    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; init; } = null;

    [JsonPropertyName("ended_at")]
    public DateTimeOffset? EndedAt { get; init; } = null;

    [JsonPropertyName("cooldown_ends_at")]
    public DateTimeOffset? CooldownEndsAt { get; init; } = null;

    [JsonPropertyName("is_shared_train")]
    public bool IsSharedTrain { get; init; } = false;

    [JsonPropertyName("type")]
    public string RawType { get; init; } = "";

    [JsonIgnore]
    public TypeEnum Type => RawType switch
    {
        "treasure" => TypeEnum.Treasure,
        "golden_kappa" => TypeEnum.GoldenKappa,
        "regular" => TypeEnum.Regular,
        _ => TypeEnum.Regular
    };

    [JsonPropertyName("all_time_high_level")]
    public int AllTimeHighLevel { get; init; } = 0;

    [JsonPropertyName("all_time_high_total")]
    public int AllTimeHighTotal { get; init; } = 0;
}