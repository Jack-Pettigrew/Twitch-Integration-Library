using System.Text.Json.Serialization;

namespace TIL.Events;

public class Poll : EventArgs
{
    public const int MinChoices = 2;
    public const int MaxChoices = 5;

    public const int MinDuration = 15;
    public const int MaxDuration = 1800;

    public const int MinChannelPointVote = 1;
    public const int MaxChannelPointVote = 1_000_000;

    public const int MaxTitleLength = 60;
    public const int MaxChoiceTitleLength = 25;

    public enum StatusEnum
    {
        Active,
        Completed,
        Terminated,
        Archived,
        Moderated,
        Invalid,
        Unknown
    };

    public sealed record Choice
    {
        [JsonPropertyName("id")]
        public string Id { get; init; } = "";

        [JsonPropertyName("title")]
        public string Title { get; init; } = "";

        [JsonPropertyName("votes")]
        public int Votes { get; init; } = 0;

        [JsonPropertyName("channel_points_votes")]
        public int ChannelPointVotes { get; init; } = 0;

        [Obsolete]
        [JsonPropertyName("bits_votes")]
        public int BitsVotes { get; init; } = 0;
    }

    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("broadcaster_id")]
    public string BroadcasterId { get; init; } = "";

    [JsonPropertyName("broadcaster_login")]
    public string BroadcasterLogin { get; init; } = "";

    [JsonPropertyName("broadcaster_name")]
    public string BroadcasterName { get; init; } = "";

    [JsonPropertyName("title")]
    public string Title { get; init; } = "";

    [JsonPropertyName("choices")]
    public Choice[] Choices { get; init; } = [];

    [Obsolete]
    [JsonPropertyName("bits_voting_enabled")]
    public bool BitsVotingEnabled { get; init; } = false;

    [Obsolete]
    [JsonPropertyName("bits_per_vote")]
    public int BitsPerVote { get; init; } = 0;

    [JsonPropertyName("channel_points_voting_enabled")]
    public bool ChannelPointsVotingEnabled { get; init; } = false;

    [JsonPropertyName("channel_points_per_vote")]
    public int ChannelPointsPerVote { get; init; } = 0;

    [JsonPropertyName("status")]
    public string RawStatus { get; init; } = "";

    [JsonIgnore]    
    public StatusEnum Status => RawStatus switch
    {
        "ACTIVE" => StatusEnum.Active,
        "COMPLETED" => StatusEnum.Completed,
        "TERMINATED" => StatusEnum.Terminated,
        "ARCHIVED" => StatusEnum.Archived,
        "MODERATED" => StatusEnum.Moderated,
        "INVALID" => StatusEnum.Invalid,
        _ => StatusEnum.Unknown
    };

    [JsonPropertyName("duration")]
    public int Duration { get; init; } = 0;

    [JsonPropertyName("started_at")]
    public DateTimeOffset? StartedAt { get; init; } = null;

    [JsonPropertyName("ended_at")]
    public DateTimeOffset? EndedAt { get; init; } = null;
}