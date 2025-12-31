using System.Text.Json.Nodes;
using TIL.Events;

namespace TIL.TwitchAPI;

public static class TwitchEventNotificationProcessor
{
    public static void ProcessTwitchEvent(JsonNode twitchResponseJson)
    {
        Console.WriteLine(twitchResponseJson.ToString());

        switch (twitchResponseJson["payload"]!["subscription"]!["type"]!.ToString())
        {
            case "channel.chat.message":
                ChatMessage chatMessage = new ChatMessage
                {
                    MessageId = twitchResponseJson["payload"]!["event"]!["message_id"]!.ToString(),
                    UserId = twitchResponseJson["payload"]!["event"]!["chatter_user_id"]!.ToString(),
                    UserName = twitchResponseJson["payload"]!["event"]!["chatter_user_name"]!.ToString(),
                    Message = twitchResponseJson["payload"]!["event"]!["message"]!["text"]!.ToString(),
                    IsReply = twitchResponseJson["payload"]!["event"]!.AsObject().ContainsKey("reply")
                };

                EventRegistry.TriggerChatMessageReceived(chatMessage);
                break;
            default:
                Console.WriteLine($"No handler registered for the received event: {twitchResponseJson}");
                break;
        }
    }
}