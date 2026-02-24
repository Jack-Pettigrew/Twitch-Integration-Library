using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using TIL.Events;
using TIL.TwitchAPI.EventSubs;

namespace TIL.TwitchAPI;

public static class TwitchEventNotificationProcessor
{
    // <type, eventsub>
    private static Dictionary<string, IEventSub> processorDictionary = new Dictionary<string, IEventSub>();

    public static void RegisterTwitchEventHandler(IEventSub eventSub)
    {
        if (!processorDictionary.TryAdd(eventSub.GetSubscriptionType(), eventSub))
        {
            if (processorDictionary.ContainsKey(eventSub.GetSubscriptionType()))
            {
                throw new TwitchEventSubExistsException($"EventSub handler already registered for: {eventSub.GetSubscriptionType()}");
            }

            throw new UnableToRegisterEventSubHandlerException($"Unable to register event handler for: {eventSub.GetSubscriptionType()}");
        }
    }

    public static void ClearEventHandlers()
    {
        processorDictionary.Clear();
    }

    public static void ProcessTwitchEvent(JsonNode twitchResponseJson)
    {
        if (processorDictionary.TryGetValue(twitchResponseJson["payload"]!["subscription"]!["type"]!.ToString(), out IEventSub? eventSub))
        {
            eventSub.ProcessEvent(twitchResponseJson);
        }
    }
}