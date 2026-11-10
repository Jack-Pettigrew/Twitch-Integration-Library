using System;
using System.Text.Json.Serialization;
using TIL.DataContainers;

namespace TIL.Events;

public class ChatMessage : EventArgs
{
    public sealed record ReplyObject
    {
        [JsonPropertyName("parent_message_id")]
        public string ParentMessageId { get; init; } = "";

        [JsonPropertyName("parent_message_body")]
        public string ParentMessageBody { get; init; } = "";

        [JsonPropertyName("parent_user_id")]
        public string ParentUserId { get; init; } = "";

        [JsonPropertyName("parent_user_name")]
        public string ParentUserName { get; init; } = "";

        [JsonPropertyName("parent_user_login")]
        public string ParentUserLogin { get; init; } = "";

        [JsonPropertyName("thread_message_id")]
        public string ThreadMessageId { get; init; } = "";

        [JsonPropertyName("thread_user_id")]
        public string ThreadUserId { get; init; } = "";

        [JsonPropertyName("thread_user_name")]
        public string ThreadUserName { get; init; } = "";

        [JsonPropertyName("thread_user_login")]
        public string ThreadUserLogin { get; init; } = "";
    }

    public sealed class CheerObject
    {
        [JsonPropertyName("bits")]
        public int Bits { get; init; } = 0;
    }

    public enum MessageTypeEnum
    {
        Text,
        ChannelPointsHighlighted,
        ChannelPointsSubOnly,
        UserIntro,
        PowerUpsMessageEffect,
        PowerUpsGigantifiedEmote
    }

    [JsonPropertyName("broadcaster_user_id")]
    public string BroadcasterUserId { get; init; } = "";

    [JsonPropertyName("broadcaster_user_login")]
    public string BroadcasterUserLogin { get; init; } = "";

    [JsonPropertyName("broadcaster_user_name")]
    public string BroadcasterUserName { get; init; } = "";

    [JsonPropertyName("chatter_user_id")]
    public string ChatterUserId { get; init; } = "";

    [JsonPropertyName("chatter_user_login")]
    public string ChatterUserLogin { get; init; } = "";

    [JsonPropertyName("chatter_user_name")]
    public string ChatterUserName { get; init; } = "";

    [JsonPropertyName("message_id")]
    public string MessageId { get; init; } = "";

    [JsonPropertyName("message")]
    public MessageContent Message { get; init; } = new();

    [JsonPropertyName("color")]
    public string Color { get; init; } = "";

    [JsonPropertyName("badges")]
    public Badges Badges { get; init; } = new();

    [JsonPropertyName("message_type")]
    public string RawMessageType { get; init; } = "";

    [JsonIgnore]
    public MessageTypeEnum MessageType => RawMessageType switch
    {
        "text" => MessageTypeEnum.Text,
        "channel_points_highlighted" => MessageTypeEnum.ChannelPointsHighlighted,
        "channel_points_sub_only" => MessageTypeEnum.ChannelPointsSubOnly,
        "user_intro" => MessageTypeEnum.UserIntro,
        "power_ups_message_effect" => MessageTypeEnum.PowerUpsMessageEffect,
        "power_ups_gigantified_emote" => MessageTypeEnum.PowerUpsGigantifiedEmote,
        _ => MessageTypeEnum.Text
    };

    [JsonPropertyName("cheer")]
    public CheerObject? Cheer { get; init; }

    [JsonPropertyName("reply")]
    public ReplyObject? Reply { get; init; }

    [JsonPropertyName("channel_points_custom_reward_id")]
    public string ChannelPointsCustomRewardId { get; init; } = "";

    [JsonPropertyName("source_broadcaster_user_id")]
    public string SourceBroadcasterUserId { get; init; } = "";

    [JsonPropertyName("source_broadcaster_user_login")]
    public string SourceBroadcasterUserLogin { get; init; } = "";

    [JsonPropertyName("source_broadcaster_user_name")]
    public string SourceBroadcasterUserName { get; init; } = "";

    [JsonPropertyName("source_message_id")]
    public string SourceMessageId { get; init; } = "";

    [JsonPropertyName("source_badges")]
    public Badges? SourceBadges { get; init; }
}