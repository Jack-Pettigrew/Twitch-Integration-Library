using System.Text;
using System.Text.Json;
using TIL.Exceptions;
using TIL.TwitchAPI;
using TIL.TwitchAPI.EventSubs;
using TIL.DataContainers;
using TIL.Events;
using TIL.Auth;
using System.Text.Unicode;

namespace TIL.Client;

public sealed class TwitchHttpClient(TwitchSessionContext twitchSessionContext, HashSet<string> twitchScopes) : IDisposable
{
    private readonly HttpClient httpClient = new();
    private readonly SemaphoreSlim tokenRefreshLock = new(initialCount: 1, maxCount: 1);

    const string API_ENDPOINT = "https://api.twitch.tv/helix";
    const string URL_EVENTSUB_SUBSCRIPTIONS = $"{API_ENDPOINT}/eventsub/subscriptions";
    const string URL_POLLS = $"{API_ENDPOINT}/polls";
    const string URL_PREDICTIONS = $"{API_ENDPOINT}/predictions";
    const string URL_USERS = $"{API_ENDPOINT}/users";
    const string URL_SUBSCRIPTIONS = $"{API_ENDPOINT}/subscriptions";
    const string URL_SUBSCRIPTIONS_USER = $"{URL_SUBSCRIPTIONS}/user";
    const string URL_CHATTERS = $"{API_ENDPOINT}/chatters";
    const string URL_MESSAGES = $"{API_ENDPOINT}/chat/messages";
    const string URL_VIPS = $"{API_ENDPOINT}/channels/vips";

    private readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    private bool HasScope(string scope)
    {
        return twitchScopes.Contains(scope);
    }

    private void EnsureScopes(params string[] scopes)
    {
        foreach (string scope in scopes)
        {
            if (!HasScope(scope))
            {
                throw new MissingScopesException($"Missing scopes: ${scope}");
            }
        }
    }

    private void EnsureScopes(Scopes.Requirement scopeRequirement)
    {
        switch (scopeRequirement.Mode)
        {
            case Scopes.Mode.All:
                EnsureScopes(scopeRequirement.Scopes);
                break;
            
            case Scopes.Mode.Any:
                EnsureAnyScope(scopeRequirement.Scopes);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(scopeRequirement));
        }
    }

    private void EnsureAnyScope(params string[] scopes)
    {
        if (!scopes.Any(HasScope))
        {
            throw new MissingScopesException($"Needs one of these scopes: {scopes}");
        }
    }

    /// <summary>
    /// Renews the access token if the current token has expired.
    /// </summary>
    /// <exception cref="InvalidAccessTokenException"></exception>
    /// <exception cref="FailedToDeserializeTokenException"></exception>
    /// <exception cref="InvalidOperationException">Device Token Polling failed.</exception>
    public async Task EnsureAccessTokenAsync()
    {
        if (twitchSessionContext is null || twitchSessionContext.twitchDeviceToken is null)
        {
            throw new InvalidAccessTokenException();
        }

        await tokenRefreshLock.WaitAsync();

        try
        {
            var token = twitchSessionContext.twitchDeviceToken;

            if (DateTimeOffset.UtcNow < token.ExpiresAt) return;

            twitchSessionContext.twitchDeviceToken = await Authentication.RefreshDeviceToken
            (
                twitchSessionContext.clientId, token.RefreshToken
            );

            SetHttpAuth(twitchSessionContext);
        }
        finally
        {
            tokenRefreshLock.Release();
        }
    }

    public void SetHttpAuth(string clientId, string accessToken)
    {
        httpClient.DefaultRequestHeaders.Authorization = new
        (
            "Bearer", accessToken
        );

        httpClient.DefaultRequestHeaders.Add
        (
            "Client-Id", clientId
        );
    }

    public void SetHttpAuth(TwitchSessionContext twitchSessionContext)
    {
        if (twitchSessionContext.twitchDeviceToken is null)
        {
            throw new InvalidAccessTokenException();
        }

        SetHttpAuth
        (
            twitchSessionContext.clientId,
            twitchSessionContext.twitchDeviceToken.AccessToken
        );
    }

    private StringContent BodyToStringContent(object body, JsonSerializerOptions? options = default)
    {
        string json = JsonSerializer.Serialize(body, options ?? DefaultJsonSerializerOptions);

        return new StringContent
        (
            json,
            Encoding.UTF8,
            "application/json"
        );
    }

    private StringContent BodyToStringContent(byte[] bytes)
    {
        return new StringContent
        (
            Encoding.UTF8.GetString(bytes),
            Encoding.UTF8,
            "application/json"
        );
    }

    // Twitch is not consistent with these parameter names
    // and switches between id/login and user_id/user_login
    private List<string> CreateUserParams(string[] users, string idParam = "id", string loginParam = "login")
    {
        List<string> queryParams = [];

        if (users is null || users.Length == 0)
        {
            return queryParams;
        }

        foreach (string entry in users) {
            if (int.TryParse(entry, out int id))
            {
                queryParams.Add($"{idParam}={id}");
            }
            else
            {
                queryParams.Add($"{loginParam}={entry}");
            }
        }

        return queryParams;
    }

    private void PrintHttpError(string title, TwitchErrorResponse errorResponse)
    {
        Console.WriteLine($"{title}:");
        Console.WriteLine($"Status: {(int)errorResponse.Status}");
        Console.WriteLine($"Error: {errorResponse.Error}");
        if (errorResponse.Message is not null)
        {
            Console.WriteLine($"Message: {errorResponse.Message}");
        }
    }

    /// <summary>
    /// Subscribes an EventSub to the active <see cref="TwitchWebSocketClient"/>.
    /// Any EventSubs that are already subscribed to will be ignored.
    /// </summary>
    /// 
    /// <param name="eventSubs">
    ///     The EventSubs that should be subscribed to.
    /// </param>
    /// 
    /// <param name="cancellationToken">
    ///     A cancellation token.
    /// </param>
    public async Task SubscribeToEventSubAsync(IEventSub[] eventSubs, CancellationToken cancellationToken = default)
    {
        foreach (var eventSub in eventSubs)
        {
            EnsureScopes(eventSub.GetScopeRequirement());
            await EnsureAccessTokenAsync();

            byte[] payload = JsonSerializer.SerializeToUtf8Bytes(eventSub.ToSubscriptionPayload(twitchSessionContext));
            
            using HttpResponseMessage subscriptionResult = await httpClient.PostAsync(
                URL_EVENTSUB_SUBSCRIPTIONS, BodyToStringContent(payload), cancellationToken
            );

            string subscriptionResponse = await subscriptionResult.Content.ReadAsStringAsync(cancellationToken);

            // Add to registry
            try
            {
                TwitchEventNotificationProcessor.RegisterTwitchEventHandler(eventSub);
                Console.WriteLine($"Subscribed to EventSub: {eventSub.GetSubscriptionType()}");
            }
            catch (TwitchEventSubExistsException)
            {
                // Ignore these exceptions by default
            }
        }
    }

    /// <summary>
    ///     Subscribes all EventSubs in the <see cref="TwitchSessionContext"/> to the active websocket.
    /// </summary>
    /// 
    /// <param name="cancellationToken">
    ///     A cancellation token.
    /// </param>
    public async Task SubscribeToEventSubAsync(CancellationToken cancellationToken = default)
    {
        if (twitchSessionContext is null) return;

        await SubscribeToEventSubAsync(twitchSessionContext.eventSubs, cancellationToken);
    }

    public async Task SubscribeToEventSubAsync(IEventSub eventSub, CancellationToken cancellationToken = default)
    {
        await SubscribeToEventSubAsync([eventSub], cancellationToken);
    }

    public async Task<Poll?> CreatePollAsync(
        string title,
        string[] choices,
        int channelPointsPerVote,
        int duration,
        CancellationToken cancellationToken = default
    )
    {
        EnsureScopes(Scopes.CHANNEL_MANAGE_POLLS);
        await EnsureAccessTokenAsync();

        if (title.Length > Poll.MaxTitleLength)
        {
            throw new ArgumentException
            (
                $"Title can not be longer than {Poll.MaxTitleLength} characters"
            );
        }

        if (choices is null || choices.Length < Poll.MinChoices || choices.Length > Poll.MaxChoices) {
            throw new ArgumentException
            (
                $"A poll must have between {Poll.MinChoices} and {Poll.MaxChoices} choices", nameof(choices)
            );
        }

        foreach(string choice in choices)
        {
            if (choice.Length > Poll.MaxChoiceTitleLength)
            {
                throw new ArgumentException
                (
                    $"Choice title can not be longer than {Poll.MaxChoiceTitleLength} characters"
                );
            }
        }
        
        if (channelPointsPerVote < 0 || channelPointsPerVote > Poll.MaxChannelPointVote)
        {
            throw new ArgumentException
            (
                $"channelPointsPerVote must be between {0} and {Poll.MaxChannelPointVote}"
            );
        }

        if (duration < Poll.MinDuration || duration > Poll.MaxDuration)
        {
            throw new ArgumentException
            (
                $"duration must be between {Poll.MinDuration} and {Poll.MaxDuration}"
            );
        }
        
        var body = new
        {
            title,
            
            choices = choices.Select(choice => new
            {
                title = choice
            }),

            duration,
            broadcaster_id = twitchSessionContext.userId,
            channel_points_voting_enabled = channelPointsPerVote > 0,
            channel_points_per_vote = channelPointsPerVote > 0 ? channelPointsPerVote : (int?)null
        };
        
        using var response = await httpClient.PostAsync(URL_POLLS, BodyToStringContent(body), cancellationToken);

        string responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = JsonSerializer.Deserialize<TwitchErrorResponse>(responseJson);
            if (errorResponse is not null) {
                PrintHttpError("Failed to create poll", errorResponse);
            }

            return null;
        }

        var result = JsonSerializer.Deserialize<TwitchResponse<Poll>>(responseJson);

        if (result?.Data.FirstOrDefault() is not { } poll)
        {
            Console.WriteLine("Failed to parse Twitch Poll response");
            return null;
        }

        return poll;
    }

    public async Task<Poll?> EndPollAsync(
        string id,
        Poll.StatusEnum status = Poll.StatusEnum.Terminated,
        CancellationToken ct = default
    )
    {
        EnsureScopes(Scopes.CHANNEL_MANAGE_POLLS);
        await EnsureAccessTokenAsync();

        if (status != Poll.StatusEnum.Terminated || status != Poll.StatusEnum.Archived)
        {
            throw new ArgumentOutOfRangeException(
                "Status needs to be either TERMINATED or ARCHIVED"
            );
        }
        
        var body = new
        {
            id,
            status = status.ToString().ToUpper(),
            broadcaster_id = twitchSessionContext.userId
        };

        using var response = await httpClient.PatchAsync(URL_POLLS, BodyToStringContent(body), ct);

        string responseJson = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = JsonSerializer.Deserialize<TwitchErrorResponse>(responseJson);
            if (errorResponse is not null)
            {
                PrintHttpError("Failed to end poll", errorResponse);
            }

            return null;
        }

        var result = JsonSerializer.Deserialize<TwitchResponse<Poll>>(responseJson);

        if (result?.Data.FirstOrDefault() is not { } poll)
        {
            Console.WriteLine("Failed to parse Twitch Poll response");
            return null;
        }

        return poll;
    }

    public async Task<Prediction?> CreatePredictionAsync(
        string title,
        string[] outcomes,
        int predictionWindow,
        CancellationToken ct = default
    )
    {
        EnsureScopes(Scopes.MODERATOR_MANAGE_PREDICTIONS);
        await EnsureAccessTokenAsync();

        if (outcomes is null || outcomes.Length < Prediction.MinOutcomes || outcomes.Length > Prediction.MaxOutcomes)
        {
            throw new ArgumentException(
                $"A prediction must have between {Prediction.MinOutcomes} and {Prediction.MaxOutcomes} choices",
                nameof(outcomes)
            );
        }

        if (title.Length > Prediction.MaxTitleLength)
        {
            throw new ArgumentException(
                $"The title must be below {Prediction.MaxTitleLength} characters"
            );
        }

        foreach (var outcome in outcomes) {
            if (outcome.Length > Prediction.MaxPredictionLength)
            {
                throw new ArgumentException(
                    $"The title of an outcome must be under {Prediction.MaxPredictionLength} characters"
                );
            }
        }
        
        predictionWindow = Math.Clamp(
            predictionWindow,
            Prediction.MinPredictionWindow,
            Prediction.MaxPredictionWindow
        );

        var body = new
        {
            title,
            outcomes,
            prediction_window = predictionWindow,
            broadcaster_id = twitchSessionContext.userId
        };

        using var response = await httpClient.PostAsync(URL_PREDICTIONS, BodyToStringContent(body), ct);

        string responseJson = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = JsonSerializer.Deserialize<TwitchErrorResponse>(responseJson);
            if (errorResponse is not null)
            {
                PrintHttpError("Failed to create prediction", errorResponse);
            }

            return null;
        }

        var result = JsonSerializer.Deserialize<TwitchResponse<Prediction>>(responseJson);
        
        if (result?.Data.FirstOrDefault() is not { } prediction)
        {
            Console.WriteLine("Failed to parse Twitch prediction response");
            return null;
        }

        return prediction;
    }

    public async Task<Prediction?> EndPredictionAsync(
        string id,
        Prediction.StatusEnum status,
        string? winningOutcomeId,
        CancellationToken ct = default
    )
    {
        EnsureScopes(Scopes.MODERATOR_MANAGE_PREDICTIONS);
        await EnsureAccessTokenAsync();

        Prediction.StatusEnum[] RequiredStatusValues = [
            Prediction.StatusEnum.Resolved,
            Prediction.StatusEnum.Cancelled,
            Prediction.StatusEnum.Locked
        ];

        if (!RequiredStatusValues.Contains(status))
        {
            throw new ArgumentException(
                "Status must be set to RESOLVED, CANCELLED OR LOCKED"
            );
        }

        if (status == Prediction.StatusEnum.Resolved && winningOutcomeId is null)
        {
            throw new ArgumentException(
                "winningOutcomeId must be set if status is set to RESOLVED"
            );
        }

        var body = new
        {
            id,
            status = status.ToString().ToUpper(),
            winning_outcome_id = winningOutcomeId,
            broadcaster_id = twitchSessionContext.userId
        };

        using var response = await httpClient.PatchAsync(URL_PREDICTIONS, BodyToStringContent(body), ct);

        string responseJson = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = JsonSerializer.Deserialize<TwitchErrorResponse>(responseJson);

            if (errorResponse is not null) {
                PrintHttpError("Failed to end prediction", errorResponse);
            }

            return null;
        }

        var result = JsonSerializer.Deserialize<TwitchResponse<Prediction>>(responseJson);
        
        if (result?.Data.FirstOrDefault() is not { } prediction)
        {
            Console.WriteLine("Failed to parse Twitch prediction response");
            return null;
        }

        return prediction;
    }

    public async Task<Dictionary<string, TwitchUser>> GetUsersAsync(string[] users, CancellationToken ct = default)
    {  
        Dictionary<string, TwitchUser> twitchUsers = [];

        const int MaxUsersPerFetch = 100;

        for (int i = 0; i < users.Length; i += MaxUsersPerFetch)
        {
            await EnsureAccessTokenAsync();

            string[] batch = users[i..Math.Min(i + MaxUsersPerFetch, users.Length)];
            List<string> queryParams = CreateUserParams(batch);

            using var response = await httpClient.GetAsync(
                $"{URL_USERS}?{string.Join("&", queryParams)}", ct
            );

            string responseJson = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonSerializer.Deserialize<TwitchErrorResponse>(responseJson);
                if (errorResponse is not null)
                {
                    PrintHttpError("Failed to get users", errorResponse);
                }

                continue;
            }

            var result = JsonSerializer.Deserialize<TwitchResponse<TwitchUser>>(responseJson);

            if (result?.Data is null)
            {
                continue;
            }

            foreach (TwitchUser user in result.Data)
            {
                string key = batch.FirstOrDefault(
                    x => x.Equals(user.Id, StringComparison.OrdinalIgnoreCase) ||
                         x.Equals(user.Login, StringComparison.OrdinalIgnoreCase)
                ) ?? user.Id;

                twitchUsers[key] = user;
            }
        }

        return twitchUsers;
    }

    public async Task<Dictionary<string, TwitchUser>> GetUsersAsync(CancellationToken ct = default)
    {
        await EnsureAccessTokenAsync();
        
        Dictionary<string, TwitchUser> twitchUsers = [];

        using var response = await httpClient.GetAsync(URL_USERS, ct);

        string responseJson = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = JsonSerializer.Deserialize<TwitchErrorResponse>(responseJson);
            if (errorResponse is not null)
            {
                PrintHttpError("Failed to get users", errorResponse);
            }

            return twitchUsers;
        }

        var result = JsonSerializer.Deserialize<TwitchResponse<TwitchUser>>(responseJson);

        if (result?.Data is not null)
        {
            foreach (TwitchUser user in result.Data)
            {
                twitchUsers[user.Id] = user;
            }
        }

        return twitchUsers;
    }

    public async Task<UserSubscription?> CheckUserSubscriptionAsync
    (
        string broadcasterId,
        CancellationToken cancellationToken = default
    )
    {
        EnsureScopes(Scopes.USER_READ_SUBSCRIPTIONS);
        await EnsureAccessTokenAsync();

        using var response = await httpClient.GetAsync
        (
            $"{URL_SUBSCRIPTIONS_USER}?broadcaster_id={broadcasterId}&user_id={twitchSessionContext.userId}",
            cancellationToken
        );

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        string responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {

            var errorResponse = JsonSerializer.Deserialize<TwitchErrorResponse>(responseJson);
            if (errorResponse is not null)
            {
                PrintHttpError("Failed to check user subscription", errorResponse);
            }

            return null;
        }

        var result = JsonSerializer.Deserialize<TwitchResponse<UserSubscription>>(responseJson);

        if (result?.Data.FirstOrDefault() is not { } userSubscription)
        {
            Console.WriteLine("Failed to parse Twitch user subscription response");
            return null;
        }

        return userSubscription;
    }

    public async Task<List<Chatter>> GetChattersAsync(
        string broadcasterId,
        string moderatorId,
        CancellationToken cancellationToken = default
    )
    {
        EnsureScopes(Scopes.MODERATOR_READ_CHATTERS);

        List<Chatter> chatters = [];

        string? cursor = null;

        do
        {
            string url =
                URL_CHATTERS +
                $"?broadcaster_id={Uri.EscapeDataString(broadcasterId)}" +
                $"&moderator_id={Uri.EscapeDataString(moderatorId)}" +
                $"&first=100";
            
            if (!string.IsNullOrEmpty(cursor))
            {
                url += $"&after={Uri.EscapeDataString(cursor)}";
            }

            await EnsureAccessTokenAsync();

            using var response = await httpClient.GetAsync(url, cancellationToken);

            string responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonSerializer.Deserialize<TwitchErrorResponse>(responseJson);
                if (errorResponse is not null)
                {
                    PrintHttpError("Failed to get chatters", errorResponse);
                }

                throw new HttpRequestException
                (
                    $"Failed to get chatters: {(int)response.StatusCode} {response.ReasonPhrase}"
                );
            }

            var result = JsonSerializer.Deserialize<TwitchResponse<Chatter>>(
                responseJson,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                }
            );

            if (result is null)
            {
                break;
            }

            chatters.AddRange(result.Data);

            cursor = result.Pagination.Cursor;
        }
        while (!string.IsNullOrEmpty(cursor));

        return chatters;
    }

    public async Task<bool> SendChatMessageAsync(
        string broadcasterId,
        string senderId,
        string message,
        string? replyParentMessageId,
        bool pin,
        CancellationToken ct = default
    )
    {
        EnsureScopes(Scopes.USER_WRITE_CHAT);
        await EnsureAccessTokenAsync();

        if (pin && !HasScope(Scopes.MODERATOR_MANAGE_CHAT_MESSAGES))
        {
            throw new MissingScopesException(
                $"Pinning a message requires Scopes.MODERATOR_MANAGE_CHAT_MESSAGES"
            );
        }

        if (message.Length > 500)
        {
            throw new ArgumentOutOfRangeException("Messages cannot be over 500 characters long.");
        }

        var body = new
        {
            message,
            pin,
            broadcaster_id = broadcasterId,
            sender_id = senderId,
            reply_parent_message_id = replyParentMessageId
        };

        using var response = await httpClient.PostAsync(URL_MESSAGES, BodyToStringContent(body), ct);

        string responseJson = await response.Content.ReadAsStringAsync(ct);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = JsonSerializer.Deserialize<TwitchErrorResponse>(responseJson);
            if (errorResponse is not null)
            {
                PrintHttpError("Failed to send message", errorResponse);
            }

            return false;
        }

        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }

    public async Task<bool> SendChatAnnouncementAsync(
        string message,
        string color,
        bool forSourceOnly,
        string broadcasterId,
        string moderatorId,
        CancellationToken ct = default
    )
    {
        EnsureScopes(Scopes.MODERATOR_MANAGE_ANNOUNCEMENTS);
        await EnsureAccessTokenAsync();
        
        var body = new
        {
            message,
            color,
            for_source_only = forSourceOnly
        };

        using var response = await httpClient.PostAsync(
            $"{URL_MESSAGES}?broadcaster_id={broadcasterId}&moderator_id={moderatorId}",
            BodyToStringContent(body), ct
        );

        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            return true;
        }

        string responseJson = await response.Content.ReadAsStringAsync(ct);
        
        var errorResponse = JsonSerializer.Deserialize<TwitchErrorResponse>(responseJson);
        if (errorResponse is not null)
        {
            PrintHttpError("Failed to send message", errorResponse);
        }

        return false;
    }

    public async Task<List<Chatter>> GetVIPsAsync
    (
        string broadcasterId,
        CancellationToken cancellationToken
    )
    {
        EnsureScopes(Scopes.CHANNEL_READ_VIPS);

        List<Chatter> chatters = [];

        string? cursor = null;

        do
        {
            string url =
                URL_VIPS +
                $"&broadcaster_id={Uri.EscapeDataString(broadcasterId)}" +
                $"&first=100";
            
            if (!string.IsNullOrEmpty(cursor))
            {
                url += $"&after={Uri.EscapeDataString(cursor)}";
            }

            await EnsureAccessTokenAsync();

            using var response = await httpClient.GetAsync(url, cancellationToken);

            string responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonSerializer.Deserialize<TwitchErrorResponse>(responseJson);
                if (errorResponse is not null)
                {
                    PrintHttpError("Failed to get VIPs", errorResponse);
                }

                throw new HttpRequestException
                (
                    $"Failed to get VIPs: {(int)response.StatusCode} {response.ReasonPhrase}"
                );
            }

            var result = JsonSerializer.Deserialize<TwitchResponse<Chatter>>
            (
                responseJson,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                }
            );

            if (result is null)
            {
                break;
            }

            chatters.AddRange(result.Data);

            cursor = result.Pagination.Cursor;
        }
        while (!string.IsNullOrWhiteSpace(cursor));

        return chatters;
    }
    
    public void Dispose()
    {
        httpClient?.Dispose();
    }
}