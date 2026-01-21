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
            string message;

            if (processorDictionary.ContainsKey(eventSub.GetSubscriptionType()))
            {
                message = "EventSub handler already registered for:";
            }
            else
            {
                message = "Unable to register event handler for:";
            }

            throw new UnableToRegisterEventSubHandlerException($"{message} {eventSub.GetSubscriptionType()}");
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