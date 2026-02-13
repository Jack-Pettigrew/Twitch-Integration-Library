using TIL.Client;
using TIL.Events;
using TIL.TwitchAPI.EventSubs;

using TwitchClient twitchClient = new TwitchClient(TIL.Env.CLIENT_ID, TIL.Env.USER_ID, new IEventSub[]
{
    new ChannelChatMessage()
});

var success = await twitchClient.ConnectToTwitchAsync();

void Test(ChatMessage chatMessage)
{
    Console.WriteLine($"{chatMessage.UserName} said: {chatMessage.Message}");
}

EventRegistry.ChatMessageReceived += Test;

if (success)
{
    Console.ReadKey();
}
else
{
    throw new Exception("Unable to connect to Twitch.");
}


await twitchClient.DisconnectFromTwitchAsync();

Console.WriteLine("End of Program");