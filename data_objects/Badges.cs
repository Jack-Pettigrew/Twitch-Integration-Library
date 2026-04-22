using System;
using System.Collections;
using System.Text.Json.Nodes;

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
        foreach (JsonNode node in jsonNode.AsArray())
        {
            JsonObject badge = node.AsObject();

            if (badge.ContainsKey("set_id"))
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

    public bool IsModerator() => badges.Contains("moderator");

    public bool IsSubscriber() => badges.Contains("subscriber") || badges.Contains("founder");

    public bool IsSubGifter() => badges.Contains("sub-gifter");
}