namespace TIL.TwitchAPI;

/// <summary>
/// TwitchAPI Scopes for declaring integration intents.
/// </summary>
public static class Scopes
{
    public sealed record Requirement
    (
        Mode Mode,
        string[] Scopes
    );

    public enum Mode
    {
        All,
        Any
    }

    /// <summary>
    /// <para>API: Get Channel Followers</para>
    /// <para>EventSub: Channel Follow</para>
    /// </summary>
    public const string MODERATOR_READ_FOLLOWERS = "moderator:read:followers";

    /// <summary>
    /// <para>API: Delete Chat Messages</para>
    /// <para>API: Pin Chat Message</para>
    /// <para>API: Update Pinned Chat Message</para>
    /// <para>API: Unpin Chat Message</para>
    /// <para>EventSub: Channel Moderate</para>
    /// </summary>
    public const string MODERATOR_MANAGE_CHAT_MESSAGES = "moderator:manage:chat_messages";

    /// <summary>
    /// <para>API: Send Chat Announcement</para>
    /// </summary>
    public const string MODERATOR_MANAGE_ANNOUNCEMENTS = "moderator:manage:announcements";

    /// <summary>
    /// <para>API: Get Channel Points Predictions</para>
    /// <para>API: Create Channel Points Prediction</para>
    /// <para>API: End Channel Points Prediction</para>
    /// <para>EventSub: Channel Prediction Begin</para>
    /// <para>EventSub: Channel Prediction Progress</para>
    /// <para>EventSub: Channel Prediction Lock</para>
    /// <para>EventSub: Channel Prediction End</para>
    /// </summary>
    public const string MODERATOR_MANAGE_PREDICTIONS = "channel:manage:predictions";

    /// <summary>
    /// <para>API: Get Chatters</para>
    /// </summary>
    public const string MODERATOR_READ_CHATTERS = "moderator:read:chatters";

    /// <summary>
    /// <para>API: Send Chat Message</para>
    /// <para>EventSub: Channel Chat Clear</para>
    /// 
    /// <para>EventSub: Channel Chat Clear</para>
    /// <para>EventSub: Channel Chat Clear User Messages</para>
    /// <para>EventSub: Channel Chat Message</para>
    /// <para>EventSub: Channel Chat Message Delete</para>
    /// <para>EventSub: Channel Chat Notification</para>
    /// <para>EventSub: Channel Chat Settings Update</para>
    /// </summary>
    public const string CHANNEL_BOT = "channel:bot";

    /// <summary> 
    /// <para>API: Get Ad Schedule</para>
    /// <para>EventSub: Channel Ad Break Begin</para>
    /// </summary>
    public const string CHANNEL_READ_ADS = "channel:read:ads";

    /// <summary>
    /// <para>API: Get Broadcaster Subscriptions</para>
    /// <para>EventSub: Channel Subscribe</para>
    /// <para>EventSub: Channel Subscription End</para>
    /// <para>EventSub: Channel Subscription Gift</para>
    /// <para>EventSub: Channel Subscription Message</para>
    /// </summary>
    public const string CHANNEL_READ_SUBSCRIPTIONS = "channel:read:subscriptions";

    /// <summary>
    /// <para>API: Get VIPs</para>
    /// <para>EventSub: Channel VIP Add</para>
    /// <para>EventSub: Channel VIP Remove</para>
    /// </summary>
    public const string CHANNEL_READ_VIPS = "channel:read:vips";

    /// <summary>
    /// <para>API: Get Bits Leaderboard</para>
    /// <para>EventSub: Channel Bits Use</para>
    /// <para>EventSub: Channel Cheer</para>
    /// </summary>
    public const string CHANNEL_BITS_READ = "bits:read";

    /// <summary>
    /// <para>API: Get Custom Reward</para>
    /// <para>API: Get Custom Reward Redemption</para>
    /// <para>EventSub: Channel Points Automatic Reward Redemption</para>
    /// <para>EventSub: Channel Points Automatic Reward Redemption v2</para>
    /// <para>EventSub: Channel Points Custom Reward Add</para>
    /// <para>EventSub: Channel Points Custom Reward Update</para>
    /// <para>EventSub: Channel Points Custom Reward Remove</para>
    /// <para>EventSub: Channel Points Custom Reward Redemption Add</para>
    /// <para>EventSub: Channel Points Custom Reward Redemption Update</para>
    /// </summary>
    public const string CHANNEL_READ_REDEMPTIONS = "channel:read:redemptions";

    /// <summary>
    /// <para>API: Get Custom Reward</para>
    /// <para>API: Get Custom Reward Redemption</para>
    /// <para>API: Create Custom Rewards</para>
    /// <para>API: Delete Custom Rewards</para>
    /// <para>API: Update Custom Rewards</para>
    /// <para>API: Update Redemption Status</para>
    /// <para>EventSub: Channel Points Automatic Reward Redemption</para>
    /// <para>EventSub: Channel Points Custom Reward Add</para>
    /// <para>EventSub: Channel Points Custom Reward Update</para>
    /// <para>EventSub: Channel Points Custom Reward Remove</para>
    /// <para>EventSub: Channel Points Custom Reward Redemption Add</para>
    /// <para>EventSub: Channel Points Custom Reward Redemption Update</para>
    /// </summary>
    public const string CHANNEL_MANAGE_REDEMPTIONS = "channel:manage:redemptions";

    /// <summary>
    /// <para>API: Get Polls</para>
    /// <para>EventSub: Channel Poll Begin</para>
    /// <para>EventSub: Channel Poll Progress</para>
    /// <para>EventSub: Channel Poll End</para>
    /// </summary>
    public const string CHANNEL_READ_POLLS = "channel:read:polls";

    /// <summary>
    /// <para>API: Get Polls</para>
    /// <para>API: Create Poll</para>
    /// <para>API: End Poll</para>
    /// <para>EventSub: Channel Poll Begin</para>
    /// <para>EventSub: Channel Poll Progress</para>
    /// <para>EventSub: Channel Poll End</para>
    /// </summary>
    public const string CHANNEL_MANAGE_POLLS = "channel:manage:polls";

    /// <summary>
    /// <para>API: Get Channel Points Predictions</para>
    /// <para>EventSub: Channel Prediction Begin</para>
    /// <para>EventSub: Channel Prediction Progress</para>
    /// <para>EventSub: Channel Prediction Lock</para>
    /// <para>EventSub: Channel Prediction End</para>
    /// </summary>
    public const string CHANNEL_READ_PREDICTIONS = "channel:read:predictions";

    /// <summary>
    /// <para>API: Get Channel Points Predictions</para>
    /// <para>API: Create Channel Points Predictions</para>
    /// <para>API: End Channel Points Predictions</para>
    /// <para>EventSub: Channel Prediction Begin</para>
    /// <para>EventSub: Channel Prediction Progress</para>
    /// <para>EventSub: Channel Prediction Lock</para>
    /// <para>EventSub: Channel Prediction End</para>
    /// </summary>
    public const string CHANNEL_MANAGE_PREDICTIONS = "channel:manage:predictions";

    /// <summary>
    /// <para>API: Get Creator Goals</para>
    /// <para>EventSub: Goal Begin</para>
    /// <para>EventSub: Goal Progress</para>
    /// <para>EventSub: Goal End</para>
    /// </summary>
    public const string CHANNEL_READ_GOALS = "channel:read:goals";

    /// <summary>
    /// <para>API: Get Hype Train Events</para>
    /// <para>EventSub: Hype Train Begin</para>
    /// <para>EventSub: Hype Train Progress</para>
    /// <para>EventSub: Hype Train End</para>
    /// </summary>
    public const string CHANNEL_READ_HYPE_TRAIN = "channel:read:hype_train";

    /// <summary>
    /// <para>EventSub: Channel Chat Clear</para>
    /// <para>EventSub: Channel Chat Clear User Messages</para>
    /// <para>EventSub: Channel Chat Message</para>
    /// <para>EventSub: Channel Chat Message Delete</para>
    /// <para>EventSub: Channel Chat Notification</para>
    /// <para>EventSub: Channel Chat Settings Update</para>
    /// <para>EventSub: Channel Chat User Message Hold</para>
    /// <para>EventSub: Channel Chat User Message Update</para>
    /// </summary>
    public const string USER_READ_CHAT = "user:read:chat";

    /// <summary>
    /// <para>API: Send Chat Message</para>
    /// </summary>
    public const string USER_WRITE_CHAT = "user:write:chat";

    /// <summary>
    /// <para>API: Check User Subscription</para>
    /// </summary>
    public const string USER_READ_SUBSCRIPTIONS = "user:read:subscriptions";

    /// <summary>
    /// <para>API: Send Chat Message</para>
    /// <para>EventSub: Channel Chat Clear</para>
    /// <para>EventSub: Channel Chat Clear User Messages</para>
    /// <para>EventSub: Channel Chat Message</para>
    /// <para>EventSub: Channel Chat Message Delete</para>
    /// <para>EventSub: Channel Chat Notification</para>
    /// <para>EventSub: Channel Chat Settings Update</para>
    /// <para>EventSub: Channel Chat User Message Hold</para>
    /// <para>EventSub: Channel Chat User Message Update</para>
    /// </summary>
    public const string USER_BOT = "user:bot";
}