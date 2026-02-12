namespace TIL;

public static class Env
{
    /// <summary>
    /// <para>The CLIENT_SECRET of your registered appication - supplied by Twitch.</para>
    /// <para>This is largely optional but included in case you require it.</para>
    /// </summary>
    public static string CLIENT_SECRET { get; } = "CLIENT_SECRET_IF_NEEDED";

    /// <summary>
    /// The CLIENT_ID of your registered application - supplied by Twitch.
    /// </summary>
    public static string CLIENT_ID { get; } = "CLIENT_ID";

    /// <summary>
    /// The USER_ID for the application to act as - this is usually your Twitch account ID.
    /// </summary>
    public static string USER_ID { get; } = "USER_ID";
}
