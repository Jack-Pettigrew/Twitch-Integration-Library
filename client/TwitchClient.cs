using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TIL.Auth;
using TIL.DataContainers;
using TIL.Exceptions;
using TIL.Events;
using TIL.TwitchAPI;
using TIL.TwitchAPI.EventSubs;

namespace TIL.Client;

/// <summary>
/// The class responsible for connecting to Twitch.
/// </summary>
public sealed class TwitchClient : IDisposable
{
    public enum ConnectionStateEnum
    {
        Connecting,
        Connected,
        Disconnecting,
        Disconnected
    }

    // REGULATION
    public ConnectionStateEnum ConnectionState { get; private set; } = ConnectionStateEnum.Disconnected;
    public bool Connected => ConnectionState == ConnectionStateEnum.Connected;

    // CONTEXT
    public TwitchSessionContext TwitchSessionContext { get; private set; }

    // INTENTS
    public string[] Scopes { get; private set; }

    // NETWORKING
    private TwitchWebSocketClient? twitchWebSocketClient;
    private TwitchHttpClient? twitchHttpClient;
    public bool AutoReconnectWebsocketOnFailure { get; private set; } = false;

    // CANCELLATION TOKEN
    private CancellationTokenSource? clientCancellationTokenSource;

    public TwitchClient(string clientId, string userId, IEventSub[] eventSubs, string[] scopes, bool autoReconnectWebsocketOnFailure = false)
    {
        Scopes = scopes;
        TwitchSessionContext = new TwitchSessionContext
        {
            clientId = clientId,
            userId = userId,
            eventSubs = eventSubs,
            scopes = scopes,
        };

        SetAutoconnectWebsocketOnFailure(autoReconnectWebsocketOnFailure);
    }

    ~TwitchClient()
    {
        if (AutoReconnectWebsocketOnFailure)
        {
            TwitchWebSocketClient.OnWebsocketExperiencedException -= HandleTwitchWebSocketClientException;
        }
    }

    public async Task<bool> ConnectToTwitchAsync()
    {
        if (ConnectionState != ConnectionStateEnum.Disconnected) return false;

        try
        {
            ConnectionState = ConnectionStateEnum.Connecting;

            twitchHttpClient = new(TwitchSessionContext, Scopes.ToHashSet());

            clientCancellationTokenSource = new CancellationTokenSource();

            TwitchSessionContext.twitchDeviceToken = await Authentication.GetDeviceAccessTokenAsync
            (
                TwitchSessionContext,
                clientCancellationTokenSource.Token
            );

            twitchHttpClient.SetHttpAuth(TwitchSessionContext);

            if (TwitchSessionContext.eventSubs.Length > 0)
            {
                twitchWebSocketClient = new TwitchWebSocketClient
                (
                    TwitchSessionContext,
                    keepalive_timeout_seconds: 30
                );

                if (!await twitchWebSocketClient.ConnectToTwitchAsync())
                {
                    await DisconnectFromTwitchAsync();
                    return false;
                }

                if (TwitchSessionContext.eventSubs.Length > 0)
                {
                    await twitchHttpClient.SubscribeToEventSubAsync();
                    Console.WriteLine("Finished Subbing to EventSubs.");
                }

                _ = twitchWebSocketClient.StartHandlingWebsocket();
            }

            ConnectionState = ConnectionStateEnum.Connected;
            return true;
        }
        catch (Exception) {
            ConnectionState = ConnectionStateEnum.Disconnecting;
            await DisconnectFromTwitchAsync();
            Dispose();
            ConnectionState = ConnectionStateEnum.Disconnected;
            throw;
        }
    }

    public async Task DisconnectFromTwitchAsync()
    {
        if (ConnectionState == ConnectionStateEnum.Disconnecting ||
            ConnectionState == ConnectionStateEnum.Disconnecting) return;

        ConnectionState = ConnectionStateEnum.Disconnecting;

        try
        {
            if (clientCancellationTokenSource is not null)
            {
                await clientCancellationTokenSource.CancelAsync();
            }

            if (twitchWebSocketClient is not null)
            {
                await twitchWebSocketClient.DisconnectFromTwitchAsync();
            }
            
            TwitchEventNotificationProcessor.ClearEventHandlers();
        }
        finally
        {
            ConnectionState = ConnectionStateEnum.Disconnected;
            twitchHttpClient = null;
            twitchWebSocketClient = null;
        }
    }

    /// <summary>
    ///     Sets whether the client will attempt to reconnect automatically if the websocket fails.
    /// </summary>
    public void SetAutoconnectWebsocketOnFailure(bool toggle)
    {
        if (toggle == AutoReconnectWebsocketOnFailure) return;

        AutoReconnectWebsocketOnFailure = toggle;

        if (AutoReconnectWebsocketOnFailure)
        {
            TwitchWebSocketClient.OnWebsocketExperiencedException += HandleTwitchWebSocketClientException;
        }
        else
        {
            TwitchWebSocketClient.OnWebsocketExperiencedException -= HandleTwitchWebSocketClientException;
        }
    }

    private void HandleTwitchWebSocketClientException(TwitchWebSocketClient twitchWebSocketClient)
    {
        Console.WriteLine("TwitchWebSocketClient threw exception - attempting to reconnect...");

        _ = twitchWebSocketClient.StartHandlingWebsocket();
    }

    /// <summary>
    /// Sends a message to the authenticated broadcaster's chat room.
    /// Requires <see cref="Scopes.USER_WRITE_CHAT"/>
    /// </summary>
    /// 
    /// <param name="message">
    ///     Must be 500 characters or lower.
    /// </param>
    /// 
    /// <param name="replyMessageId">
    ///     The message Id to reply to.
    /// </param>
    /// 
    /// <exception cref="TwitchNotConnectedException"/>
    /// <exception cref="MissingScopesException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public async Task<bool> SendChatMessageAsync(string message, string replyMessageId, CancellationToken cancellationToken = default)
    {
        ThrowIfNotConnected();

        return await twitchHttpClient!.SendChatMessageAsync(
            broadcasterId: TwitchSessionContext.userId,
            senderId: TwitchSessionContext.userId,
            message: message,
            replyParentMessageId: replyMessageId,
            pin: false,
            cancellationToken
        );
    }

    /// <summary>
    ///     Sends a message to the authenticated broadcaster's chat room.
    ///     Requires <see cref="Scopes.USER_WRITE_CHAT"/>
    /// </summary>
    /// 
    /// <param name="message">
    ///     Must be 500 characters or lower.
    /// </param>
    /// 
    /// <param name="pin">
    /// Can not be used with replies. Requires <see cref="Scopes.CHANNEL_MANAGE_POLLS"/>.
    /// </param>
    /// 
    /// <exception cref="TwitchNotConnectedException"/>
    /// <exception cref="MissingScopesException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public async Task<bool> SendChatMessageAsync
    (
        string message,
        bool pin = false,
        CancellationToken cancellationToken = default
    )
    {
        ThrowIfNotConnected();

        return await twitchHttpClient!.SendChatMessageAsync(
            broadcasterId: TwitchSessionContext.userId,
            senderId: TwitchSessionContext.userId,
            message: message,
            replyParentMessageId: null,
            pin: pin,
            cancellationToken
        );
    }

    /// <summary>
    ///     Gets the list of users that are connected to the authenticated broadcaster's chat session.
    ///     Requires <see cref="Scopes.MODERATOR_READ_CHATTERS"/>.
    /// </summary>
    /// <exception cref="TwitchNotConnectedException"/>
    /// <exception cref="MissingScopesException"/>
    public async Task<List<Chatter>> GetChattersAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfNotConnected();

        return await twitchHttpClient!.GetChattersAsync(
            broadcasterId: TwitchSessionContext.userId,
            moderatorId: TwitchSessionContext.userId,
            cancellationToken
        );
    }

    /// <summary>
    ///     Checks whether the authenticated broadcaster is subscribed to the specified user's channel.
    ///     Requires <see cref="Scopes.USER_READ_SUBSCRIPTIONS"/>.
    /// </summary>
    /// <param name="user">
    ///     The id of the user whose channel subscription status should be checked.
    /// </param>
    /// <returns>
    ///     The authenticated broadcaster's subscription information for the specified channel,
    ///     or <see langword="null"/> if they are not subscribed.
    /// </returns>
    public async Task<UserSubscription?> CheckUserSubscriptionAsync
    (
        string user,
        CancellationToken cancellationToken = default
    )
    {
        ThrowIfNotConnected();

        return await twitchHttpClient!.CheckUserSubscriptionAsync
        (
            broadcasterId: user,
            cancellationToken
        );
    }

    /// <summary>
    ///     Gets information about the specified users.
    ///     If none are provided, gets information about the user authenticated this session.
    /// </summary>
    /// 
    /// <param name="users">
    ///     User Ids or Logins
    /// </param>
    /// 
    /// <returns>
    ///     A dictionary of users, with each user's ID or login as the key
    ///     and their corresponding <see cref="TwitchUser"/> as the value.
    /// </returns>
    /// 
    /// <exception cref="TwitchNotConnectedException"/>
    public async Task<Dictionary<string, TwitchUser>> GetUsersAsync
    (
        string[] users,
        CancellationToken cancellationToken = default
    )
    {
        ThrowIfNotConnected();

        return await twitchHttpClient!.GetUsersAsync(users, cancellationToken);
    }

    /// <summary>
    ///     Gets information about the specified user.
    ///     If none are provided, gets information about the user authenticated this session.
    /// </summary>
    /// 
    /// <param name="user">
    ///     User Ids or Logins
    /// </param>
    /// 
    /// <returns>
    ///     A dictionary of users, with each user's ID or login as the key
    ///     and their corresponding <see cref="TwitchUser"/> as the value.
    /// </returns>
    /// 
    /// <exception cref="TwitchNotConnectedException"/>
    public async Task<TwitchUser?> GetUsersAsync
    (
        string user,
        CancellationToken cancellationToken = default
    )
    {
        ThrowIfNotConnected();

        var users = await twitchHttpClient!.GetUsersAsync([user], cancellationToken);
        return users.Values.FirstOrDefault();
    }

    /// <summary>
    ///     Gets information about the specified users.
    ///     If none are provided, gets information about the user authenticated this session.
    /// </summary>
    /// 
    /// <returns>
    ///     A dictionary of users, with each user's ID or login as the key
    ///     and their corresponding <see cref="TwitchUser"/> as the value.
    /// </returns>
    /// 
    /// <exception cref="TwitchNotConnectedException"/>
    public async Task<TwitchUser?> GetUsersAsync
    (
        CancellationToken cancellationToken = default
    )
    {
        ThrowIfNotConnected();

        var users = await twitchHttpClient!.GetUsersAsync(cancellationToken);
        return users.Values.FirstOrDefault();
    }

    /// <summary>
    ///     Creates a poll that viewers in the authenticated broadcaster's channel can vote on.
    ///     The poll begins as soon as it’s created. You may run only one poll at a time.
    ///     Requires <see cref="Scopes.CHANNEL_MANAGE_POLLS"/>.
    /// </summary>
    /// 
    /// <param name="title">
    /// Should not exceed a length of 60 characters.
    /// </param>
    /// 
    /// <param name="choices">
    /// Has to be between 2 and 5. Choice title should not exceed a length of 45 characters.
    /// </param>
    /// 
    /// <param name="channelPointsPerVote">
    /// Should not exceed 1 000 000.
    /// </param>
    /// 
    /// <param name="duration">
    /// Has to be between 15 and 1800 seconds.
    /// </param>
    public async Task<Poll?> CreatePollAsync
    (
        string title,
        string[] choices,
        int channelPointsPerVote = 0,
        int duration = 30,
        CancellationToken cancellationToken = default
    )
    {
        ThrowIfNotConnected();

        return await twitchHttpClient!.CreatePollAsync
        (
            title,
            choices,
            channelPointsPerVote,
            duration,
            cancellationToken
        );
    }

    /// <summary>
    ///     Ends an active poll on the authenticated broadcaster's channel.
    ///     You have the option to end it or end it and archive it.
    ///     Requires <see cref="Scopes.CHANNEL_MANAGE_POLLS"/>.
    /// </summary>
    /// 
    /// <param name="id">
    ///     The ID of the poll to update.
    /// </param>
    /// 
    /// <param name="status">
    ///     Has to be <see cref="Poll.StatusEnum.Terminated"/> or <see cref="Poll.StatusEnum.Archived"/>.
    /// </param>
    /// 
    /// <exception cref="TwitchNotConnectedException"/>
    /// <exception cref="MissingScopesException"/>
    public async Task<Poll?> EndPollAsync
    (
        string id,
        Poll.StatusEnum status = Poll.StatusEnum.Terminated,
        CancellationToken cancellationToken = default
    )
    {
        ThrowIfNotConnected();

        return await twitchHttpClient!.EndPollAsync(id, status, cancellationToken);
    }

    /// <summary>
    ///     Gets the list of users that are connected to the authenticated broadcaster’s chat session.
    ///     Requires <see cref="Scopes.MODERATOR_READ_CHATTERS"/>.
    /// </summary>
    /// 
    /// <returns>
    ///     A <see cref="List"/> of <see cref="Chatter"/>.
    /// </returns>
    /// 
    /// <exception cref="TwitchNotConnectedException"/>
    /// <exception cref="MissingScopesException"/>
    /// <exception cref="HttpRequestException"/>
    public async Task<List<Chatter>> GetChattersAsync
    (
        string broadcasterId,
        string moderatorId,
        CancellationToken cancellationToken = default
    )
    {
        ThrowIfNotConnected();

        return await twitchHttpClient!.GetChattersAsync(broadcasterId, moderatorId, cancellationToken);
    }

    /// <summary>
    ///     Gets a list of the broadcaster’s VIPs.
    ///     Requires <see cref="Scopes.CHANNEL_READ_VIPS"/>.
    /// </summary>
    /// 
    /// <returns>
    ///     A <see cref="List"/> of <see cref="Chatter"/>.
    /// </returns>
    /// 
    /// <exception cref="TwitchNotConnectedException"/>
    /// <exception cref="MissingScopesException"/>
    /// <exception cref="HttpRequestException">
    public async Task<List<Chatter>> GetVIPsAsync
    (
        CancellationToken cancellationToken = default
    )
    {
        ThrowIfNotConnected();

        return await twitchHttpClient!.GetVIPsAsync
        (
            broadcasterId: TwitchSessionContext.userId,
            cancellationToken
        );
    }

    /// <exception cref="TwitchNotConnectedException"/>
    public void ThrowIfNotConnected()
    {
        if (!Connected)
        {
            throw new TwitchNotConnectedException();
        }
    }

    public void Dispose()
    {
        twitchWebSocketClient?.Dispose();
        twitchHttpClient?.Dispose();
        clientCancellationTokenSource?.Dispose();
    }
}