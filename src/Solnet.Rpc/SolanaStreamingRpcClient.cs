using Microsoft.Extensions.Logging;
using Solnet.Rpc.Converters;
using Solnet.Rpc.Core;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Solnet.Rpc;

/// <summary>
///     Implementation of the Solana streaming RPC API abstraction client.
/// </summary>
[DebuggerDisplay("Cluster = {" + nameof(NodeAddress) + "}")]
internal class SolanaStreamingRpcClient : StreamingRpcClient, IStreamingRpcClient
{
    /// <summary>
    ///     Message Id generator.
    /// </summary>
    private readonly IdGenerator _idGenerator = new();

    /// <summary>
    ///     Maps the server ids to the confirmed subscription state objects.
    /// </summary>
    private readonly Dictionary<int, SubscriptionState> confirmedSubscriptions = new();

    /// <summary>
    ///     Maps the internal ids to the unconfirmed subscription state objects.
    /// </summary>
    private readonly Dictionary<int, SubscriptionState> unconfirmedRequests = new();

    /// <summary>
    ///     Internal constructor.
    /// </summary>
    /// <param name="url">The url of the server to connect to.</param>
    /// <param name="logger">The possible ILogger instance.</param>
    /// <param name="websocket">The possible IWebSocket instance.</param>
    /// <param name="clientWebSocket">The possible ClientWebSocket instance.</param>
    internal SolanaStreamingRpcClient(string url, ILogger logger = null, IWebSocket websocket = default,
        ClientWebSocket clientWebSocket = default) : base(url, logger, websocket, clientWebSocket)
    {
    }

    /// <inheritdoc cref="IStreamingRpcClient.UnsubscribeAsync(SubscriptionState)" />
    public async Task UnsubscribeAsync(SubscriptionState subscription)
    {
        JsonRpcRequest msg = new JsonRpcRequest(_idGenerator.GetNextId(),
            GetUnsubscribeMethodName(subscription.Channel), new List<object> {subscription.SubscriptionId});

        await Subscribe(subscription, msg).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IStreamingRpcClient.Unsubscribe(SubscriptionState)" />
    public void Unsubscribe(SubscriptionState subscription)
    {
        UnsubscribeAsync(subscription).Wait();
    }

    /// <inheritdoc cref="StreamingRpcClient.CleanupSubscriptions" />
    protected override void CleanupSubscriptions()
    {
        foreach (KeyValuePair<int, SubscriptionState> sub in confirmedSubscriptions)
        {
            sub.Value.ChangeState(SubscriptionStatus.Unsubscribed, "Connection terminated");
        }

        foreach (KeyValuePair<int, SubscriptionState> sub in unconfirmedRequests)
        {
            sub.Value.ChangeState(SubscriptionStatus.Unsubscribed, "Connection terminated");
        }

        unconfirmedRequests.Clear();
        confirmedSubscriptions.Clear();
    }


    /// <inheritdoc cref="StreamingRpcClient.HandleNewMessage(Memory{byte})" />
    protected override void HandleNewMessage(Memory<byte> messagePayload)
    {
        Utf8JsonReader jsonReader = new(messagePayload.Span);
        jsonReader.Read();

        if (_logger?.IsEnabled(LogLevel.Information) ?? false)
        {
            string str = Encoding.UTF8.GetString(messagePayload.Span);
            _logger?.LogInformation($"[Received]{str}");
        }

        string prop = "", method = "";
        int id = -1, intResult = -1;
        bool handled = false;
        bool? boolResult = null;

        Utf8JsonReader savedState = default;

        while (!handled && jsonReader.Read())
        {
            switch (jsonReader.TokenType)
            {
                case JsonTokenType.PropertyName:
                    prop = jsonReader.GetString();
                    if (prop == "params")
                    {
                        savedState = jsonReader;
                        jsonReader.Read();
                        jsonReader.Read();
                        jsonReader.Skip();
                    }
                    else if (prop == "error")
                    {
                        HandleError(ref jsonReader);
                        handled = true;
                    }

                    break;
                case JsonTokenType.String:
                    if (prop == "method")
                    {
                        method = jsonReader.GetString();
                    }

                    break;
                case JsonTokenType.Number:
                    if (prop == "id")
                    {
                        id = jsonReader.GetInt32();
                    }
                    else if (prop == "result")
                    {
                        intResult = jsonReader.GetInt32();
                    }
                    else if (prop == "subscription")
                    {
                        id = jsonReader.GetInt32();
                        HandleDataMessage(ref savedState, method, id);
                        handled = true;
                    }

                    if (id != -1 && intResult != -1)
                    {
                        ConfirmSubscription(id, intResult);
                        handled = true;
                    }

                    break;
                case JsonTokenType.True:
                case JsonTokenType.False:
                    if (prop == "result")
                    {
                        // this is the result of an unsubscription
                        // I don't think its supposed to ever be false if we correctly manage the subscription ids
                        // maybe future followup
                        boolResult = jsonReader.GetBoolean();
                    }

                    break;
            }
        }

        if (boolResult.HasValue)
        {
            RemoveSubscription(id, boolResult.Value);
        }
    }

    /// <summary>
    ///     Handles and finishes parsing the contents of an error message.
    /// </summary>
    /// <param name="reader">The jsonReader that read the message so far.</param>
    private void HandleError(ref Utf8JsonReader reader)
    {
        JsonSerializerOptions opts = new() {MaxDepth = 64, PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
        ErrorContent err = JsonSerializer.Deserialize<ErrorContent>(ref reader, opts);

        reader.Read();

        //var prop = reader.GetString(); //don't care about property name

        reader.Read();

        int id = reader.GetInt32();

        SubscriptionState sub = RemoveUnconfirmedSubscription(id);

        sub?.ChangeState(SubscriptionStatus.ErrorSubscribing, err.Message, err.Code.ToString());
    }

    /// <summary>
    ///     Handles a notification message and finishes parsing the contents.
    /// </summary>
    /// <param name="reader">The current JsonReader being used to parse the message.</param>
    /// <param name="method">The method parameter already parsed within the message.</param>
    /// <param name="subscriptionId">The subscriptionId for this message.</param>
    private void HandleDataMessage(ref Utf8JsonReader reader, string method, int subscriptionId)
    {
        JsonSerializerOptions opts = new() {MaxDepth = 64, PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

        SubscriptionState sub = RetrieveSubscription(subscriptionId);

        object result = null;

        switch (method)
        {
            case "accountNotification":
                {
                    if (sub.Channel == SubscriptionChannel.TokenAccount)
                    {
                        //var newReader = new Utf8JsonReader()
                        JsonRpcStreamResponse<ResponseValue<TokenAccountInfo>> tokenAccNotification =
                            JsonSerializer.Deserialize<JsonRpcStreamResponse<ResponseValue<TokenAccountInfo>>>(
                                ref reader, opts);
                        result = tokenAccNotification.Result;
                    }
                    else
                    {
                        JsonRpcStreamResponse<ResponseValue<AccountInfo>> accNotification =
                            JsonSerializer.Deserialize<JsonRpcStreamResponse<ResponseValue<AccountInfo>>>(ref reader,
                                opts);
                        result = accNotification.Result;
                    }

                    break;
                }
            case "logsNotification":
                JsonRpcStreamResponse<ResponseValue<LogInfo>> logsNotification =
                    JsonSerializer.Deserialize<JsonRpcStreamResponse<ResponseValue<LogInfo>>>(ref reader, opts);
                result = logsNotification.Result;
                break;
            case "programNotification":
                JsonRpcStreamResponse<ResponseValue<AccountKeyPair>> programNotification =
                    JsonSerializer.Deserialize<JsonRpcStreamResponse<ResponseValue<AccountKeyPair>>>(ref reader, opts);
                result = programNotification.Result;
                break;
            case "signatureNotification":
                JsonRpcStreamResponse<ResponseValue<ErrorResult>> signatureNotification =
                    JsonSerializer.Deserialize<JsonRpcStreamResponse<ResponseValue<ErrorResult>>>(ref reader, opts);
                result = signatureNotification.Result;
                RemoveSubscription(signatureNotification.Subscription, true);
                break;
            case "slotNotification":
                JsonRpcStreamResponse<SlotInfo> slotNotification =
                    JsonSerializer.Deserialize<JsonRpcStreamResponse<SlotInfo>>(ref reader, opts);
                result = slotNotification.Result;
                break;
            case "rootNotification":
                JsonRpcStreamResponse<int> rootNotification =
                    JsonSerializer.Deserialize<JsonRpcStreamResponse<int>>(ref reader, opts);
                result = rootNotification.Result;
                break;
        }

        sub.HandleData(result);
    }

    /// <summary>
    ///     Internal subscribe function, finishes the serialization and sends the message payload.
    /// </summary>
    /// <param name="sub">The subscription state object.</param>
    /// <param name="msg">The message to be serialized and sent.</param>
    /// <returns>A task representing the state of the asynchronous operation-</returns>
    private async Task<SubscriptionState> Subscribe(SubscriptionState sub, JsonRpcRequest msg)
    {
        byte[] json = JsonSerializer.SerializeToUtf8Bytes(msg,
            new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = {new EncodingConverter(), new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)}
            });

        if (_logger?.IsEnabled(LogLevel.Information) ?? false)
        {
            string jsonString = Encoding.UTF8.GetString(json);
            _logger?.LogInformation(new EventId(msg.Id, msg.Method), $"[Sending]{jsonString}");
        }

        ReadOnlyMemory<byte> mem = new(json);

        try
        {
            await ClientSocket.SendAsync(mem, WebSocketMessageType.Text, true, CancellationToken.None)
                .ConfigureAwait(false);
            AddSubscription(sub, msg.Id);
        }
        catch (Exception e)
        {
            sub.ChangeState(SubscriptionStatus.ErrorSubscribing, e.Message);
            _logger?.LogDebug(new EventId(msg.Id, msg.Method), e, "Unable to send message");
        }

        return sub;
    }

    private string GetUnsubscribeMethodName(SubscriptionChannel channel)
    {
        return channel switch
        {
            SubscriptionChannel.Account => "accountUnsubscribe",
            SubscriptionChannel.Logs => "logsUnsubscribe",
            SubscriptionChannel.Program => "programUnsubscribe",
            SubscriptionChannel.Root => "rootUnsubscribe",
            SubscriptionChannel.Signature => "signatureUnsubscribe",
            SubscriptionChannel.Slot => "slotUnsubscribe",
            _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, "invalid message type")
        };
    }


    #region SubscriptionMapHandling

    /// <summary>
    ///     Removes an unconfirmed subscription.
    /// </summary>
    /// <param name="id">The subscription id.</param>
    /// <returns>Returns the subscription object if it was found.</returns>
    private SubscriptionState RemoveUnconfirmedSubscription(int id)
    {
        SubscriptionState sub;
        lock (this)
        {
            if (!unconfirmedRequests.Remove(id, out sub))
            {
                _logger.LogDebug(new EventId(), $"No unconfirmed subscription found with ID:{id}");
            }
        }

        return sub;
    }

    /// <summary>
    ///     Removes a given subscription object from the map and notifies the object of the unsubscription.
    /// </summary>
    /// <param name="id">The subscription id.</param>
    /// <param name="shouldNotify">Whether or not to notify that the subscription was removed.</param>
    private void RemoveSubscription(int id, bool shouldNotify)
    {
        SubscriptionState sub;
        lock (this)
        {
            if (!confirmedSubscriptions.Remove(id, out sub))
            {
                _logger.LogDebug(new EventId(), $"No subscription found with ID:{id}");
            }
        }

        if (shouldNotify)
        {
            sub?.ChangeState(SubscriptionStatus.Unsubscribed);
        }
    }

    /// <summary>
    ///     Confirms a given subcription based on the internal subscription id and the newly received external id.
    ///     Moves the subcription state object from the unconfirmed map to the confirmed map.
    /// </summary>
    /// <param name="internalId"></param>
    /// <param name="resultId"></param>
    private void ConfirmSubscription(int internalId, int resultId)
    {
        SubscriptionState sub;
        lock (this)
        {
            if (unconfirmedRequests.Remove(internalId, out sub))
            {
                sub.SubscriptionId = resultId;
                confirmedSubscriptions.Add(resultId, sub);
            }
        }

        sub?.ChangeState(SubscriptionStatus.Subscribed);
    }

    /// <summary>
    ///     Adds a new subscription state object into the unconfirmed subscriptions map.
    /// </summary>
    /// <param name="subscription">The subcription to add.</param>
    /// <param name="internalId">The internally generated id of the subscription.</param>
    private void AddSubscription(SubscriptionState subscription, int internalId)
    {
        lock (this)
        {
            unconfirmedRequests.Add(internalId, subscription);
        }
    }

    /// <summary>
    ///     Safely retrieves a subscription state object from a given subscription id.
    /// </summary>
    /// <param name="subscriptionId">The subscription id.</param>
    /// <returns>The subscription state object.</returns>
    private SubscriptionState RetrieveSubscription(int subscriptionId)
    {
        lock (this)
        {
            return confirmedSubscriptions[subscriptionId];
        }
    }

    #endregion

    #region AccountInfo

    /// <inheritdoc
    ///     cref="IStreamingRpcClient.SubscribeAccountInfoAsync(string, Action{SubscriptionState, ResponseValue{AccountInfo}}, Commitment)" />
    public async Task<SubscriptionState> SubscribeAccountInfoAsync(string pubkey,
        Action<SubscriptionState, ResponseValue<AccountInfo>> callback, Commitment commitment = Commitment.Finalized)

    {
        List<object> parameters = new List<object> {pubkey};
        Dictionary<string, object> configParams = new Dictionary<string, object> {{"encoding", "base64"}};

        if (commitment != Commitment.Finalized)
        {
            configParams.Add("commitment", commitment);
        }

        parameters.Add(configParams);

        SubscriptionState<ResponseValue<AccountInfo>> sub =
            new SubscriptionState<ResponseValue<AccountInfo>>(this, SubscriptionChannel.Account, callback, parameters);

        JsonRpcRequest msg = new JsonRpcRequest(_idGenerator.GetNextId(), "accountSubscribe", parameters);

        return await Subscribe(sub, msg).ConfigureAwait(false);
    }

    /// <inheritdoc
    ///     cref="IStreamingRpcClient.SubscribeAccountInfo(string, Action{SubscriptionState, ResponseValue{AccountInfo}}, Commitment)" />
    public SubscriptionState SubscribeAccountInfo(string pubkey,
        Action<SubscriptionState, ResponseValue<AccountInfo>> callback, Commitment commitment = Commitment.Finalized)
    {
        return SubscribeAccountInfoAsync(pubkey, callback, commitment).Result;
    }

    #endregion

    #region TokenAccount

    /// <inheritdoc
    ///     cref="IStreamingRpcClient.SubscribeTokenAccountAsync(string, Action{SubscriptionState, ResponseValue{TokenAccountInfo}}, Commitment)" />
    public async Task<SubscriptionState> SubscribeTokenAccountAsync(string pubkey,
        Action<SubscriptionState, ResponseValue<TokenAccountInfo>> callback,
        Commitment commitment = Commitment.Finalized)

    {
        List<object> parameters = new List<object> {pubkey};
        Dictionary<string, object> configParams = new Dictionary<string, object> {{"encoding", "jsonParsed"}};

        if (commitment != Commitment.Finalized)
        {
            configParams.Add("commitment", commitment);
        }

        parameters.Add(configParams);

        SubscriptionState<ResponseValue<TokenAccountInfo>> sub =
            new SubscriptionState<ResponseValue<TokenAccountInfo>>(this, SubscriptionChannel.TokenAccount, callback,
                parameters);

        JsonRpcRequest msg = new JsonRpcRequest(_idGenerator.GetNextId(), "accountSubscribe", parameters);

        return await Subscribe(sub, msg).ConfigureAwait(false);
    }

    /// <inheritdoc
    ///     cref="IStreamingRpcClient.SubscribeTokenAccount(string, Action{SubscriptionState, ResponseValue{TokenAccountInfo}}, Commitment)" />
    public SubscriptionState SubscribeTokenAccount(string pubkey,
        Action<SubscriptionState, ResponseValue<TokenAccountInfo>> callback,
        Commitment commitment = Commitment.Finalized)
    {
        return SubscribeTokenAccountAsync(pubkey, callback, commitment).Result;
    }

    #endregion

    #region Logs

    /// <inheritdoc
    ///     cref="IStreamingRpcClient.SubscribeLogInfoAsync(string, Action{SubscriptionState, ResponseValue{LogInfo}}, Commitment)" />
    public async Task<SubscriptionState> SubscribeLogInfoAsync(string pubkey,
        Action<SubscriptionState, ResponseValue<LogInfo>> callback, Commitment commitment = Commitment.Finalized)
    {
        List<object> parameters =
            new List<object> {new Dictionary<string, object> {{"mentions", new List<string> {pubkey}}}};

        if (commitment != Commitment.Finalized)
        {
            Dictionary<string, Commitment> configParams =
                new Dictionary<string, Commitment> {{"commitment", commitment}};
            parameters.Add(configParams);
        }

        SubscriptionState<ResponseValue<LogInfo>> sub =
            new SubscriptionState<ResponseValue<LogInfo>>(this, SubscriptionChannel.Logs, callback, parameters);

        JsonRpcRequest msg = new JsonRpcRequest(_idGenerator.GetNextId(), "logsSubscribe", parameters);
        return await Subscribe(sub, msg).ConfigureAwait(false);
    }

    /// <inheritdoc
    ///     cref="IStreamingRpcClient.SubscribeLogInfo(string, Action{SubscriptionState, ResponseValue{LogInfo}}, Commitment)" />
    public SubscriptionState SubscribeLogInfo(string pubkey, Action<SubscriptionState, ResponseValue<LogInfo>> callback,
        Commitment commitment = Commitment.Finalized)
    {
        return SubscribeLogInfoAsync(pubkey, callback, commitment).Result;
    }

    /// <inheritdoc
    ///     cref="IStreamingRpcClient.SubscribeLogInfoAsync(LogsSubscriptionType, Action{SubscriptionState, ResponseValue{LogInfo}}, Commitment)" />
    public async Task<SubscriptionState> SubscribeLogInfoAsync(LogsSubscriptionType subscriptionType,
        Action<SubscriptionState, ResponseValue<LogInfo>> callback, Commitment commitment = Commitment.Finalized)
    {
        List<object> parameters = new List<object> {subscriptionType};

        if (commitment != Commitment.Finalized)
        {
            Dictionary<string, Commitment> configParams =
                new Dictionary<string, Commitment> {{"commitment", commitment}};
            parameters.Add(configParams);
        }

        SubscriptionState<ResponseValue<LogInfo>> sub =
            new SubscriptionState<ResponseValue<LogInfo>>(this, SubscriptionChannel.Logs, callback, parameters);

        JsonRpcRequest msg = new JsonRpcRequest(_idGenerator.GetNextId(), "logsSubscribe", parameters);
        return await Subscribe(sub, msg).ConfigureAwait(false);
    }

    /// <inheritdoc
    ///     cref="IStreamingRpcClient.SubscribeLogInfo(LogsSubscriptionType, Action{SubscriptionState, ResponseValue{LogInfo}}, Commitment)" />
    public SubscriptionState SubscribeLogInfo(LogsSubscriptionType subscriptionType,
        Action<SubscriptionState, ResponseValue<LogInfo>> callback, Commitment commitment = Commitment.Finalized)
    {
        return SubscribeLogInfoAsync(subscriptionType, callback, commitment).Result;
    }

    #endregion

    #region Signature

    /// <inheritdoc
    ///     cref="IStreamingRpcClient.SubscribeSignatureAsync(string, Action{SubscriptionState, ResponseValue{ErrorResult}}, Commitment)" />
    public async Task<SubscriptionState> SubscribeSignatureAsync(string transactionSignature,
        Action<SubscriptionState, ResponseValue<ErrorResult>> callback, Commitment commitment = Commitment.Finalized)
    {
        List<object> parameters = new List<object> {transactionSignature};

        if (commitment != Commitment.Finalized)
        {
            Dictionary<string, Commitment> configParams =
                new Dictionary<string, Commitment> {{"commitment", commitment}};
            parameters.Add(configParams);
        }

        SubscriptionState<ResponseValue<ErrorResult>> sub =
            new SubscriptionState<ResponseValue<ErrorResult>>(this, SubscriptionChannel.Signature, callback,
                parameters);

        JsonRpcRequest msg = new JsonRpcRequest(_idGenerator.GetNextId(), "signatureSubscribe", parameters);
        return await Subscribe(sub, msg).ConfigureAwait(false);
    }

    /// <inheritdoc
    ///     cref="IStreamingRpcClient.SubscribeSignature(string, Action{SubscriptionState, ResponseValue{ErrorResult}}, Commitment)" />
    public SubscriptionState SubscribeSignature(string transactionSignature,
        Action<SubscriptionState, ResponseValue<ErrorResult>> callback, Commitment commitment = Commitment.Finalized)
    {
        return SubscribeSignatureAsync(transactionSignature, callback, commitment).Result;
    }

    #endregion

    #region Program

    /// <inheritdoc
    ///     cref="IStreamingRpcClient.SubscribeProgramAsync(string, Action{SubscriptionState, ResponseValue{AccountKeyPair}}, Commitment)" />
    public async Task<SubscriptionState> SubscribeProgramAsync(string programPubkey,
        Action<SubscriptionState, ResponseValue<AccountKeyPair>> callback, Commitment commitment = Commitment.Finalized)
    {
        List<object> parameters = new List<object> {programPubkey};
        Dictionary<string, object> configParams = new Dictionary<string, object> {{"encoding", "base64"}};

        if (commitment != Commitment.Finalized)
        {
            configParams.Add("commitment", commitment);
        }

        parameters.Add(configParams);

        SubscriptionState<ResponseValue<AccountKeyPair>> sub =
            new SubscriptionState<ResponseValue<AccountKeyPair>>(this, SubscriptionChannel.Program, callback,
                parameters);

        JsonRpcRequest msg = new JsonRpcRequest(_idGenerator.GetNextId(), "programSubscribe", parameters);
        return await Subscribe(sub, msg).ConfigureAwait(false);
    }

    /// <inheritdoc
    ///     cref="IStreamingRpcClient.SubscribeProgram(string, Action{SubscriptionState, ResponseValue{AccountKeyPair}}, Commitment)" />
    public SubscriptionState SubscribeProgram(string programPubkey,
        Action<SubscriptionState, ResponseValue<AccountKeyPair>> callback, Commitment commitment = Commitment.Finalized)
    {
        return SubscribeProgramAsync(programPubkey, callback, commitment).Result;
    }

    #endregion

    #region SlotInfo

    /// <inheritdoc cref="IStreamingRpcClient.SubscribeSlotInfoAsync(Action{SubscriptionState, SlotInfo})" />
    public async Task<SubscriptionState> SubscribeSlotInfoAsync(Action<SubscriptionState, SlotInfo> callback)
    {
        SubscriptionState<SlotInfo> sub = new SubscriptionState<SlotInfo>(this, SubscriptionChannel.Slot, callback);

        JsonRpcRequest msg = new JsonRpcRequest(_idGenerator.GetNextId(), "slotSubscribe", null);
        return await Subscribe(sub, msg).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IStreamingRpcClient.SubscribeSlotInfo(Action{SubscriptionState, SlotInfo})" />
    public SubscriptionState SubscribeSlotInfo(Action<SubscriptionState, SlotInfo> callback)
    {
        return SubscribeSlotInfoAsync(callback).Result;
    }

    #endregion

    #region Root

    /// <inheritdoc cref="IStreamingRpcClient.SubscribeRootAsync(Action{SubscriptionState, int})" />
    public async Task<SubscriptionState> SubscribeRootAsync(Action<SubscriptionState, int> callback)
    {
        SubscriptionState<int> sub = new SubscriptionState<int>(this, SubscriptionChannel.Root, callback);

        JsonRpcRequest msg = new JsonRpcRequest(_idGenerator.GetNextId(), "rootSubscribe", null);
        return await Subscribe(sub, msg).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IStreamingRpcClient.SubscribeRoot(Action{SubscriptionState, int})" />
    public SubscriptionState SubscribeRoot(Action<SubscriptionState, int> callback)
    {
        return SubscribeRootAsync(callback).Result;
    }

    #endregion
}