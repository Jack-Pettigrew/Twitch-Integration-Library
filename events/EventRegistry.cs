namespace TIL.Events;

public static class EventRegistry
{
    public delegate void TwitchEventHandler<T>(T twitchEventArg) where T : EventArgs;

    public static event TwitchEventHandler<ChatMessage>? ChatMessageReceived;

    public static void TriggerChatMessageReceived(ChatMessage chatMessage)
    {
        ChatMessageReceived?.Invoke(chatMessage);
    }
}