namespace TIL.DataContainers;

public static class Subscription
{
    public enum Tier
    {
        Tier1,
        Tier2,
        Tier3
    }

    public static Tier Parse(string tier) => tier switch
    {
        "3000" => Tier.Tier3,
        "2000" => Tier.Tier2,
        "1000" => Tier.Tier1,
        _ => Tier.Tier1
    };
}