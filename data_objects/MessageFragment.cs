using System.Text.Json.Serialization;
using static TIL.Events.ChatMessage;

namespace TIL.DataContainers;

public class MessageFragment
{
    public enum TypeEnum
    {
        Text,
        Cheermote,
        Emote,
        Mention,
        Gif
    }

    [JsonPropertyName("type")]
    public string RawType { get; init; } = "";

    [JsonIgnore]
    public TypeEnum Type => RawType switch
    {
        "text" => TypeEnum.Text,
        "cheermote" => TypeEnum.Cheermote,
        "emote" => TypeEnum.Emote,
        "mention" => TypeEnum.Mention,
        "gif" => TypeEnum.Gif,
        _ => TypeEnum.Text
    };

    [JsonPropertyName("text")]
    public string Text { get; init; } = "";

    [JsonPropertyName("cheermote")]
    public CheermoteObject? Cheermote { get; init; }
    
    [JsonPropertyName("emote")]
    public Emote? Emote { get; init; }

    [JsonPropertyName("mention")]
    public MentionObject? Mention { get; init; }
}