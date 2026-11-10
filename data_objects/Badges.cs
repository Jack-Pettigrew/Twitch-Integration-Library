using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace TIL;

[JsonConverter(typeof(BadgesConverter))]
public class Badges
{
    private readonly HashSet<string> badges = new HashSet<string>();

    public Badges()
    { }

    public Badges(string[] badgeIds)
    {
        foreach (string badge in badgeIds)
        {
            badges.Add(badge);
        }
    }

    public Badges(JsonNode jsonNode)
    {
        foreach (JsonNode? node in jsonNode.AsArray())
        {
            JsonObject? badge = node?.AsObject();

            if (badge is not null && badge.ContainsKey("set_id"))
            {
                badges.Add(badge["set_id"]!.ToString());
            }
        }
    }

    public override string ToString()
    {
        string toString = "";

        foreach (var item in badges)
        {
            if (toString.Length == 0)
            {
                toString += item;
            }
            else
            {
                toString += $", {item}";
            }
        }

        return toString;
    }

    public void AddBadge(string badgeId)
    {
        badges.Add(badgeId);
    }

    public string[] GetBadges() => badges.ToArray();

    public bool IsBroadcaster() => badges.Contains("broadcaster");

    public bool IsModerator() => badges.Contains("moderator");

    public bool IsSubscriber() => badges.Contains("subscriber") || badges.Contains("founder");

    public bool IsFounder() => badges.Contains("founder");

    public bool IsSubGifter() => badges.Contains("sub-gifter");
}

public class BadgesConverter : JsonConverter<Badges>
{
    public override Badges Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Badges result = new();

        using JsonDocument document = JsonDocument.ParseValue(ref reader);

        foreach (JsonElement badge in document.RootElement.EnumerateArray())
        {
            if (badge.TryGetProperty("set_id", out JsonElement setId))
            {
                result.AddBadge(setId.GetString() ?? "");
            }
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, Badges value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (string badge in value.GetBadges())
        {
            writer.WriteStartObject();
            writer.WriteString("set_id", badge);
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
    }
}