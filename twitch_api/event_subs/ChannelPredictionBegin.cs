
using System.Text.Json;
using System.Text.Json.Nodes;
using TIL.Client;
using TIL.Events;
using TIL.TwitchAPI;
using TIL.TwitchAPI.EventSubs;

public partial class ChannelPredictionBegin : IEventSub
{
    public Scopes.Requirement GetScopeRequirement()
    {
        return new(Scopes.Mode.Any, [
            Scopes.CHANNEL_READ_PREDICTIONS,
            Scopes.CHANNEL_MANAGE_PREDICTIONS
        ]);
    }

    public string GetSubscriptionType()
    {
        return Subscriptions.CHANNEL_PREDICTION_BEGIN;
    }

    public object ToSubscriptionPayload(TwitchSessionContext twitchSessionContext)
    {
        return new
        {
            type = GetSubscriptionType(),
            version = "1",
            condition = new
            {
                broadcaster_user_id = twitchSessionContext.userId,
            },
            transport = new
            {
                method = "websocket",
                session_id = twitchSessionContext.sessionId
            }
        };
    }

    public void ProcessEvent(JsonNode twitchResponseJson)
    {
        Prediction? prediction = JsonSerializer.Deserialize<Prediction>(twitchResponseJson["payload"]!["event"]);

        if (prediction is not null) EventRegistry.TriggerPredictionBegin(prediction);
    }
}