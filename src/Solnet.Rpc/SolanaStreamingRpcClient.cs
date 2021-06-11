using Microsoft.Extensions.Logging;
using Solnet.Rpc.Core;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Solnet.Rpc
{
    /// <summary>
    /// Implementation of the Solana streaming RPC API abstraction client.
    /// </summary>
    public class SolanaStreamingRpcClient : StreamingRpcClient, IStreamingRpcClient
    {
        /// <summary>
        /// Message Id generator.
        /// </summary>
        private readonly IdGenerator _idGenerator = new IdGenerator();

        /// <summary>
        /// Maps the internal ids to the unconfirmed subscription state objects.
        /// </summary>
        private readonly Dictionary<int, SubscriptionState> unconfirmedRequests = new Dictionary<int, SubscriptionState>();

        /// <summary>
        /// Maps the server ids to the confirmed subscription state objects.
        /// </summary>
        private readonly Dictionary<int, SubscriptionState> confirmedSubscriptions = new Dictionary<int, SubscriptionState>();

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="url">The url of the server to connect to.</param>
        /// <param name="logger">The possible ILogger instance.</param>
        /// <param name="websocket">The possible IWebSocket instance.</param>
        internal SolanaStreamingRpcClient(string url, ILogger logger = null, IWebSocket websocket = default) : base(url, logger, websocket)
        {
        }

        /// <inheritdoc cref="StreamingRpcClient.HandleNewMessage(Memory{byte})"/>
        protected override void HandleNewMessage(Memory<byte> messagePayload)
        {
            Utf8JsonReader jsonReader = new Utf8JsonReader(messagePayload.Span);
            jsonReader.Read();

            if (_logger?.IsEnabled(LogLevel.Information) ?? false)
            {
                var str = Encoding.UTF8.GetString(messagePayload.Span);
                _logger?.LogInformation($"[Received]{str}");
            }

            string prop = "", method = "";
            int id = -1, intResult = -1;
            bool handled = false;
            bool? boolResult = null;

            while (!handled && jsonReader.Read())
            {
                switch (jsonReader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        prop = jsonReader.GetString();
                        if (prop == "params")
                        {
                            HandleDataMessage(ref jsonReader, method);
                            handled = true;
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
        /// Handles and finishes parsing the contents of an error message.
        /// </summary>
        /// <param name="reader">The jsonReader that read the message so far.</param>
        private void HandleError(ref Utf8JsonReader reader)
        {
            JsonSerializerOptions opts = new JsonSerializerOptions() { MaxDepth = 64, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var err = JsonSerializer.Deserialize<ErrorContent>(ref reader, opts);

            reader.Read();

            //var prop = reader.GetString(); //don't care about property name

            reader.Read();

            var id = reader.GetInt32();

            var sub = RemoveUnconfirmedSubscription(id);

            sub?.ChangeState(SubscriptionStatus.ErrorSubscribing, err.Message, err.Code.ToString());
        }


        #region SubscriptionMapHandling
        /// <summary>
        /// Removes an unconfirmed subscription.
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
        /// Removes a given subscription object from the map and notifies the object of the unsubscription.
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
        /// Confirms a given subcription based on the internal subscription id and the newly received external id.
        /// Moves the subcription state object from the unconfirmed map to the confirmed map.
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
        /// Adds a new subscription state object into the unconfirmed subscriptions map.
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
        /// Safely retrieves a subscription state object from a given subscription id.
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
        /// <summary>
        /// Handles a notification message and finishes parsing the contents.
        /// </summary>
        /// <param name="reader">The current JsonReader being used to parse the message.</param>
        /// <param name="method">The method parameter already parsed within the message.</param>
        private void HandleDataMessage(ref Utf8JsonReader reader, string method)
        {
            JsonSerializerOptions opts = new JsonSerializerOptions() { MaxDepth = 64, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            switch (method)
            {
                case "accountNotification":
                    var accNotification = JsonSerializer.Deserialize<JsonRpcStreamResponse<ResponseValue<AccountInfo>>>(ref reader, opts);
                    if (accNotification == null) break;
                    NotifyData(accNotification.Subscription, accNotification.Result);
                    break;
                case "logsNotification":
                    var logsNotification = JsonSerializer.Deserialize<JsonRpcStreamResponse<ResponseValue<LogInfo>>>(ref reader, opts);
                    if (logsNotification == null) break;
                    NotifyData(logsNotification.Subscription, logsNotification.Result);
                    break;
                case "programNotification":
                    var programNotification = JsonSerializer.Deserialize<JsonRpcStreamResponse<ResponseValue<AccountKeyPair>>>(ref reader, opts);
                    if (programNotification == null) break;
                    NotifyData(programNotification.Subscription, programNotification.Result);
                    break;
                case "signatureNotification":
                    var signatureNotification = JsonSerializer.Deserialize<JsonRpcStreamResponse<ResponseValue<ErrorResult>>>(ref reader, opts);
                    if (signatureNotification == null) break;
                    NotifyData(signatureNotification.Subscription, signatureNotification.Result);
                    RemoveSubscription(signatureNotification.Subscription, true);
                    break;
                case "slotNotification":
                    var slotNotification = JsonSerializer.Deserialize<JsonRpcStreamResponse<SlotInfo>>(ref reader, opts);
                    if (slotNotification == null) break;
                    NotifyData(slotNotification.Subscription, slotNotification.Result);
                    break;
                case "rootNotification":
                    var rootNotification = JsonSerializer.Deserialize<JsonRpcStreamResponse<int>>(ref reader, opts);
                    if (rootNotification == null) break;
                    NotifyData(rootNotification.Subscription, rootNotification.Result);
                    break;
            }
        }

        /// <summary>
        /// Notifies a given subscription of a new data payload.
        /// </summary>
        /// <param name="subscription">The subscription ID received.</param>
        /// <param name="data">The parsed data payload to notify.</param>
        private void NotifyData(int subscription, object data)
        {
            var sub = RetrieveSubscription(subscription);

            sub.HandleData(data);
        }

        #region AccountInfo
        /// <inheritdoc cref="IStreamingRpcClient.SubscribeAccountInfoAsync(string, Action{SubscriptionState, ResponseValue{AccountInfo}})"/>
        public async Task<SubscriptionState> SubscribeAccountInfoAsync(string pubkey, Action<SubscriptionState, ResponseValue<AccountInfo>> callback)

        {
            var sub = new SubscriptionState<ResponseValue<AccountInfo>>(this, SubscriptionChannel.Account, callback, new List<object> { pubkey });

            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "accountSubscribe", new List<object> { pubkey, new Dictionary<string, string> { { "encoding", "base64" } } });

            return await Subscribe(sub, msg).ConfigureAwait(false);
        }

        /// <inheritdoc cref="IStreamingRpcClient.SubscribeAccountInfo(string, Action{SubscriptionState, ResponseValue{AccountInfo}})"/>
        public SubscriptionState SubscribeAccountInfo(string pubkey, Action<SubscriptionState, ResponseValue<AccountInfo>> callback)
            => SubscribeAccountInfoAsync(pubkey, callback).Result;
        #endregion

        #region Logs
        /// <inheritdoc cref="IStreamingRpcClient.SubscribeLogInfoAsync(string, Action{SubscriptionState, ResponseValue{LogInfo}})"/>
        public async Task<SubscriptionState> SubscribeLogInfoAsync(string pubkey, Action<SubscriptionState, ResponseValue<LogInfo>> callback)
        {
            var sub = new SubscriptionState<ResponseValue<LogInfo>>(this, SubscriptionChannel.Logs, callback, new List<object> { pubkey });

            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "logsSubscribe", new List<object> { new Dictionary<string, object> { { "mentions", new List<string> { pubkey } } } });
            return await Subscribe(sub, msg).ConfigureAwait(false);
        }

        /// <inheritdoc cref="IStreamingRpcClient.SubscribeLogInfo(string, Action{SubscriptionState, ResponseValue{LogInfo}})"/>
        public SubscriptionState SubscribeLogInfo(string pubkey, Action<SubscriptionState, ResponseValue<LogInfo>> callback)
            => SubscribeLogInfoAsync(pubkey, callback).Result;

        /// <inheritdoc cref="IStreamingRpcClient.SubscribeLogInfoAsync(LogsSubscriptionType, Action{SubscriptionState, ResponseValue{LogInfo}})"/>
        public async Task<SubscriptionState> SubscribeLogInfoAsync(LogsSubscriptionType subscriptionType, Action<SubscriptionState, ResponseValue<LogInfo>> callback)
        {
            var sub = new SubscriptionState<ResponseValue<LogInfo>>(this, SubscriptionChannel.Logs, callback, new List<object> { subscriptionType });


            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "logsSubscribe", new List<object> { subscriptionType });
            return await Subscribe(sub, msg).ConfigureAwait(false);
        }

        /// <inheritdoc cref="IStreamingRpcClient.SubscribeLogInfo(LogsSubscriptionType, Action{SubscriptionState, ResponseValue{LogInfo}})"/>
        public SubscriptionState SubscribeLogInfo(LogsSubscriptionType subscriptionType, Action<SubscriptionState, ResponseValue<LogInfo>> callback)
            => SubscribeLogInfoAsync(subscriptionType, callback).Result;
        #endregion

        #region Signature
        /// <inheritdoc cref="IStreamingRpcClient.SubscribeSignatureAsync(string, Action{SubscriptionState, ResponseValue{ErrorResult}})"/>
        public async Task<SubscriptionState> SubscribeSignatureAsync(string transactionSignature, Action<SubscriptionState, ResponseValue<ErrorResult>> callback)
        {
            var sub = new SubscriptionState<ResponseValue<ErrorResult>>(this, SubscriptionChannel.Signature, callback, new List<object> { transactionSignature });

            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "signatureSubscribe", new List<object> { transactionSignature });
            return await Subscribe(sub, msg).ConfigureAwait(false);
        }

        /// <inheritdoc cref="IStreamingRpcClient.SubscribeSignature(string, Action{SubscriptionState, ResponseValue{ErrorResult}})"/>
        public SubscriptionState SubscribeSignature(string transactionSignature, Action<SubscriptionState, ResponseValue<ErrorResult>> callback)
            => SubscribeSignatureAsync(transactionSignature, callback).Result;
        #endregion

        #region Program
        /// <inheritdoc cref="IStreamingRpcClient.SubscribeProgramAsync(string, Action{SubscriptionState, ResponseValue{ProgramInfo}})"/>
        public async Task<SubscriptionState> SubscribeProgramAsync(string transactionSignature, Action<SubscriptionState, ResponseValue<AccountKeyPair>> callback)
        {
            var sub = new SubscriptionState<ResponseValue<AccountKeyPair>>(this, SubscriptionChannel.Program, callback,
                new List<object> { transactionSignature, new Dictionary<string, string> { { "encoding", "base64" } } });

            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "programSubscribe", new List<object> { transactionSignature, new Dictionary<string, string> { { "encoding", "base64" } } });
            return await Subscribe(sub, msg).ConfigureAwait(false);
        }

        /// <inheritdoc cref="IStreamingRpcClient.SubscribeProgram(string, Action{SubscriptionState, ResponseValue{ProgramInfo}})"/>
        public SubscriptionState SubscribeProgram(string transactionSignature, Action<SubscriptionState, ResponseValue<AccountKeyPair>> callback)
            => SubscribeProgramAsync(transactionSignature, callback).Result;
        #endregion

        #region SlotInfo
        /// <inheritdoc cref="IStreamingRpcClient.SubscribeSlotInfoAsync(Action{SubscriptionState, SlotInfo})"/>
        public async Task<SubscriptionState> SubscribeSlotInfoAsync(Action<SubscriptionState, SlotInfo> callback)
        {
            var sub = new SubscriptionState<SlotInfo>(this, SubscriptionChannel.Slot, callback);

            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "slotSubscribe", null);
            return await Subscribe(sub, msg).ConfigureAwait(false);
        }

        /// <inheritdoc cref="IStreamingRpcClient.SubscribeSlotInfo(Action{SubscriptionState, SlotInfo})"/>
        public SubscriptionState SubscribeSlotInfo(Action<SubscriptionState, SlotInfo> callback)
            => SubscribeSlotInfoAsync(callback).Result;
        #endregion

        #region Root
        /// <inheritdoc cref="IStreamingRpcClient.SubscribeRootAsync(Action{SubscriptionState, int})"/>
        public async Task<SubscriptionState> SubscribeRootAsync(Action<SubscriptionState, int> callback)
        {
            var sub = new SubscriptionState<int>(this, SubscriptionChannel.Root, callback);

            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "rootSubscribe", null);
            return await Subscribe(sub, msg).ConfigureAwait(false);
        }

        /// <inheritdoc cref="IStreamingRpcClient.SubscribeRoot(Action{SubscriptionState, int})"/>
        public SubscriptionState SubscribeRoot(Action<SubscriptionState, int> callback)
            => SubscribeRootAsync(callback).Result;
        #endregion

        /// <summary>
        /// Internal subscribe function, finishes the serialization and sends the message payload.
        /// </summary>
        /// <param name="sub">The subscription state object.</param>
        /// <param name="msg">The message to be serialized and sent.</param>
        /// <returns>A task representing the state of the asynchronous operation-</returns>
        private async Task<SubscriptionState> Subscribe(SubscriptionState sub, JsonRpcRequest msg)
        {
            var json = JsonSerializer.SerializeToUtf8Bytes(msg, new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            });

            if (_logger?.IsEnabled(LogLevel.Information) ?? false)
            {
                var jsonString = Encoding.UTF8.GetString(json);
                _logger?.LogInformation(new EventId(msg.Id, msg.Method), $"[Sending]{jsonString}");
            }

            ReadOnlyMemory<byte> mem = new ReadOnlyMemory<byte>(json);

            try
            {
                await ClientSocket.SendAsync(mem, WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(false);
                AddSubscription(sub, msg.Id);
            }
            catch (Exception e)
            {
                sub.ChangeState(SubscriptionStatus.ErrorSubscribing, e.Message);
                _logger?.LogDebug(new EventId(msg.Id, msg.Method), e, $"Unable to send message");
            }

            return sub;
        }

        private string GetUnsubscribeMethodName(SubscriptionChannel channel) => channel switch
        {
            SubscriptionChannel.Account => "accountUnsubscribe",
            SubscriptionChannel.Logs => "logsUnsubscribe",
            SubscriptionChannel.Program => "programUnsubscribe",
            SubscriptionChannel.Root => "rootUnsubscribe",
            SubscriptionChannel.Signature => "signatureUnsubscribe",
            SubscriptionChannel.Slot => "slotUnsubscribe",
            _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, "invalid message type")
        };

        /// <inheritdoc cref="IStreamingRpcClient.UnsubscribeAsync(SubscriptionState)"/>
        public async Task UnsubscribeAsync(SubscriptionState subscription)
        {
            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), GetUnsubscribeMethodName(subscription.Channel), new List<object> { subscription.SubscriptionId });

            await Subscribe(subscription, msg).ConfigureAwait(false);
        }

        /// <inheritdoc cref="IStreamingRpcClient.Unsubscribe(SubscriptionState)"/>
        public void Unsubscribe(SubscriptionState subscription) => UnsubscribeAsync(subscription).Wait();
    }
}