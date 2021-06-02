using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Solnet.Rpc.Core;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;

namespace Solnet.Rpc
{
    public class SolanaStreamingRpcClient : StreamingRpcClient
    {
        /// <summary>
        /// Message Id generator.
        /// </summary>
        IdGenerator _idGenerator = new IdGenerator();

        Dictionary<int, SubscriptionState> unconfirmedRequests = new Dictionary<int, SubscriptionState>();

        Dictionary<int, SubscriptionState> confirmedSubscriptions = new Dictionary<int, SubscriptionState>();


        public SolanaStreamingRpcClient(string url, IWebSocket websocket = default) : base(url, websocket)
        {
        }

        protected override void HandleNewMessage(Memory<byte> mem)
        {
            Utf8JsonReader asd = new Utf8JsonReader(mem.Span);
            asd.Read();

            //#TODO: remove and add proper logging
            var str = Encoding.UTF8.GetString(mem.Span);
            Console.WriteLine(str);

            string prop = "", method = "";
            int id = -1, intResult = -1;
            bool handled = false;
            bool? boolResult = null;

            while (!handled && asd.Read())
            {
                switch (asd.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        prop = asd.GetString();
                        if (prop == "params")
                        {
                            HandleDataMessage(ref asd, method);
                            handled = true;
                        }
                        break;
                    case JsonTokenType.String:
                        if (prop == "method")
                        {
                            method = asd.GetString();
                        }
                        break;
                    case JsonTokenType.Number:
                        if (prop == "id")
                        {
                            id = asd.GetInt32();
                        }
                        else if (prop == "result")
                        {
                            intResult = asd.GetInt32();
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
                            boolResult = asd.GetBoolean();
                        }
                        break;
                }
            }

            if (boolResult.HasValue)
            {
                RemoveSubscription(id, boolResult.Value);
            }
        }

        private void RemoveSubscription(int id, bool value)
        {
            SubscriptionState sub;
            lock (this)
            {
                if (!confirmedSubscriptions.Remove(id, out sub))
                {
                    // houston, we might have a problem?
                }
            }
            if (value)
            {
                sub?.ChangeState(SubscriptionStatus.Unsubscribed);
            }
            else
            {
                sub?.ChangeState(sub.State, "Subscription doesnt exists");
            }
        }

        #region SubscriptionMapHandling

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

        private void AddSubscription(SubscriptionState subscription, int internalId)
        {
            lock (this)
            {
                unconfirmedRequests.Add(internalId, subscription);
            }
        }

        private SubscriptionState RetrieveSubscription(int subscriptionId)
        {
            lock (this)
            {
                return confirmedSubscriptions[subscriptionId];
            }
        }
        #endregion

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
                    var programNotification = JsonSerializer.Deserialize<JsonRpcStreamResponse<ResponseValue<ProgramInfo>>>(ref reader, opts);
                    if (programNotification == null) break;
                    NotifyData(programNotification.Subscription, programNotification.Result);
                    break;
                case "signatureNotification":
                    var signatureNotification = JsonSerializer.Deserialize<JsonRpcStreamResponse<ErrorResult>>(ref reader, opts);
                    if (signatureNotification == null) break;
                    NotifyData(signatureNotification.Subscription, signatureNotification.Result);
                    // remove subscription from map
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

        private void NotifyData(int subscription, object data)
        {
            var sub = RetrieveSubscription(subscription);

            sub.HandleData(data);
        }

        #region AccountInfo
        public async Task<SubscriptionState> SubscribeAccountInfoAsync(string pubkey, Action<SubscriptionState, ResponseValue<AccountInfo>> callback)

        {
            var sub = new SubscriptionState<ResponseValue<AccountInfo>>(this, SubscriptionChannel.Account, callback, new List<object> { pubkey });

            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "accountSubscribe", new List<object> { pubkey, new Dictionary<string, string> { { "encoding", "jsonParsed" } } });

            return await Subscribe(sub, msg).ConfigureAwait(false);
        }

        public SubscriptionState SubscribeAccountInfo(string pubkey, Action<SubscriptionState, ResponseValue<AccountInfo>> callback)
            => SubscribeAccountInfoAsync(pubkey, callback).Result;
        #endregion

        #region Logs
        public async Task<SubscriptionState> SubscribeLogInfoAsync(string pubkey, Action<SubscriptionState, ResponseValue<LogInfo>> callback)
        {
            var sub = new SubscriptionState<ResponseValue<LogInfo>>(this, SubscriptionChannel.Logs, callback, new List<object> { pubkey });

            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "logsSubscribe", new List<object> { new Dictionary<string, object> { { "mentions", new List<string> { pubkey } } } });
            return await Subscribe(sub, msg).ConfigureAwait(false);
        }
        public SubscriptionState SubscribeLogInfo(string pubkey, Action<SubscriptionState, ResponseValue<LogInfo>> callback)
            => SubscribeLogInfoAsync(pubkey, callback).Result;

        public async Task<SubscriptionState> SubscribeLogInfoAsync(LogsSubscriptionType subscriptionType, Action<SubscriptionState, ResponseValue<LogInfo>> callback)
        {
            var sub = new SubscriptionState<ResponseValue<LogInfo>>(this, SubscriptionChannel.Logs, callback, new List<object> { subscriptionType });


            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "logsSubscribe", new List<object> { subscriptionType });
            return await Subscribe(sub, msg).ConfigureAwait(false);
        }
        public SubscriptionState SubscribeLogInfo(LogsSubscriptionType subscriptionType, Action<SubscriptionState, ResponseValue<LogInfo>> callback)
            => SubscribeLogInfoAsync(subscriptionType, callback).Result;
        #endregion

        #region Signature
        public async Task<SubscriptionState> SubscribeSignatureAsync(string transactionSignature, Action<SubscriptionState, ResponseValue<ErrorResult>> callback)
        {
            var sub = new SubscriptionState<ResponseValue<ErrorResult>>(this, SubscriptionChannel.Signature, callback, new List<object> { transactionSignature });

            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "signatureSubscribe", new List<object> { transactionSignature });
            return await Subscribe(sub, msg).ConfigureAwait(false);
        }
        public SubscriptionState SubscribeSignature(string transactionSignature, Action<SubscriptionState, ResponseValue<ErrorResult>> callback)
            => SubscribeSignatureAsync(transactionSignature, callback).Result;
        #endregion

        #region Program
        public async Task<SubscriptionState> SubscribeProgramAsync(string transactionSignature, Action<SubscriptionState, ResponseValue<ProgramInfo>> callback)
        {
            var sub = new SubscriptionState<ResponseValue<ProgramInfo>>(this, SubscriptionChannel.Program, callback,
                new List<object> { transactionSignature/*, new Dictionary<string, string> { { "encoding", "base64" } }*/ });

            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "programSubscribe", new List<object> { transactionSignature });
            return await Subscribe(sub, msg).ConfigureAwait(false);
        }
        public SubscriptionState SubscribeProgram(string transactionSignature, Action<SubscriptionState, ResponseValue<ProgramInfo>> callback)
            => SubscribeProgramAsync(transactionSignature, callback).Result;
        #endregion

        #region SlotInfo

        public async Task<SubscriptionState> SubscribeSlotInfoAsync(Action<SubscriptionState, SlotInfo> callback)
        {
            var sub = new SubscriptionState<SlotInfo>(this, SubscriptionChannel.Slot, callback);

            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "slotSubscribe", null);
            return await Subscribe(sub, msg).ConfigureAwait(false);
        }
        public SubscriptionState SubscribeSlotInfo(Action<SubscriptionState, SlotInfo> callback)
            => SubscribeSlotInfoAsync(callback).Result;
        #endregion


        #region Root
        public async Task<SubscriptionState> SubscribeRootAsync(Action<SubscriptionState, int> callback)
        {
            var sub = new SubscriptionState<int>(this, SubscriptionChannel.Root, callback);

            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "rootSubscribe", null);
            return await Subscribe(sub, msg).ConfigureAwait(false);
        }
        public SubscriptionState SubscribeRoot(Action<SubscriptionState, int> callback)
            => SubscribeRootAsync(callback).Result;
        #endregion

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
            Console.WriteLine($"\tRequest: {Encoding.UTF8.GetString(json)}");


            ReadOnlyMemory<byte> mem = new ReadOnlyMemory<byte>(json);
            await ClientSocket.SendAsync(mem, WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(false);

            AddSubscription(sub, msg.Id);
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

        public async Task UnsubscribeAsync(SubscriptionState subscription)
        {
            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), GetUnsubscribeMethodName(subscription.Channel), new List<object> { subscription.SubscriptionId });

            await Subscribe(subscription, msg).ConfigureAwait(false);
        }

        public void Unsubscribe(SubscriptionState subscription) => UnsubscribeAsync(subscription).Wait();
    }
}