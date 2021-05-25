using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Solnet.Rpc.Core.Sockets
{
    public class SolanaStreamingClient
    {
        IWebSocket _clientSocket;

        string _socketUri;

        private int _id;

        Dictionary<int, SubscriptionState> unconfirmedRequests = new Dictionary<int, SubscriptionState>();

        Dictionary<int, SubscriptionState> confirmedSubscriptions = new Dictionary<int, SubscriptionState>();
        private int GetNextId()
        {
            lock (this)
            {
                return _id++;
            }
        }

        public SolanaStreamingClient(string nodeUri, IWebSocket socket = default)
        {
            _clientSocket = socket ?? new WebSocketWrapper(new ClientWebSocket());
            _socketUri = nodeUri ?? "wss://";
        }

        public async Task Init()
        {
            await _clientSocket.ConnectAsync(new Uri(_socketUri), CancellationToken.None).ConfigureAwait(false);
            _ = Task.Run(StartListening);
        }

        private async Task StartListening()
        {
            while (_clientSocket.State == WebSocketState.Open)
            {
                try
                {
                    await ReadNextMessage().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Console.WriteLine("uh oh");
                }
            }
        }

        private async Task ReadNextMessage(CancellationToken cancellationToken = default)
        {
            var buffer = new byte[32768];
            Memory<byte> mem = new Memory<byte>(buffer);
            var messageParts = new StringBuilder();
            int count = 0;

            ValueWebSocketReceiveResult result = await _clientSocket.ReceiveAsync(mem, cancellationToken).ConfigureAwait(false);
            count = result.Count;
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await _clientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
            }
            else
            {
                if (!result.EndOfMessage)
                {
                    MemoryStream ms = new MemoryStream();
                    ms.Write(mem.Span);


                    while (!result.EndOfMessage)
                    {
                        result = await _clientSocket.ReceiveAsync(mem, cancellationToken).ConfigureAwait(false);
                        ms.Write(mem.Span);
                        count += result.Count;
                    }

                    mem = new Memory<byte>(ms.ToArray());
                }
                else
                {
                    mem = mem.Slice(0, count);
                }
                Console.WriteLine("\n\n[" + DateTime.UtcNow.ToLongTimeString() + "][Received]\n" + UTF8Encoding.UTF8.GetString(mem.ToArray(), 0, count));


                HandleNewMessage(mem);
            }
        }

        private void HandleNewMessage(Memory<byte> mem)
        {
            Utf8JsonReader asd = new Utf8JsonReader(mem.Span);

            asd.Read();

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

            SubscriptionState sub = null;
            lock (this)
            {
                if (!unconfirmedRequests.Remove(id, out sub))
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
            SubscriptionState sub = null;
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
                    var accNotif = JsonSerializer.Deserialize<JsonRpcStreamResponse<AccountInfo>>(ref reader, opts);
                    NotifyData(accNotif.Subscription, accNotif.Result);
                    break;
                case "logsNotification":
                    var logsNotif = JsonSerializer.Deserialize<JsonRpcStreamResponse<LogsInfo>>(ref reader, opts);
                    NotifyData(logsNotif.Subscription, logsNotif.Result);
                    break;
                case "programNotification":
                    var programNotif = JsonSerializer.Deserialize<JsonRpcStreamResponse<ProgramInfo>>(ref reader, opts);
                    NotifyData(programNotif.Subscription, programNotif.Result);
                    break;
                case "signatureNotification":
                    var signatureNotif = JsonSerializer.Deserialize<JsonRpcStreamResponse<string>>(ref reader, opts);
                    NotifyData(signatureNotif.Subscription, signatureNotif.Result);
                    // remove subscription from map
                    break;
                case "slotNotification":
                    var slotNotif = JsonSerializer.Deserialize<JsonRpcStreamResponse<SlotInfo>>(ref reader, opts);
                    NotifyData(slotNotif.Subscription, slotNotif.Result);
                    break;
                case "rootNotification":
                    var rootNotif = JsonSerializer.Deserialize<JsonRpcStreamResponse<int>>(ref reader, opts);
                    NotifyData(rootNotif.Subscription, rootNotif.Result);
                    break;
            }
        }

        private void NotifyData(int subscription, object data)
        {
            var sub = RetrieveSubscription(subscription);

            sub.HandleData(data);
        }

        private Type GetTypeFromMethod(string method) => method switch
        {
            "accountNotification" => typeof(JsonRpcStreamResponse<ResponseValue<AccountInfo>>)
        };


        public async Task<SubscriptionState> SubscribeAccountInfoAsync(string pubkey, Action<SubscriptionState, ResponseValue<AccountInfo>> callback)
        {
            var sub = new SubscriptionState<ResponseValue<AccountInfo>>(this, SubscriptionChannel.Account, callback, new List<object>() { pubkey });

            var msg = new JsonRpcRequest(GetNextId(), "accountSubscribe", new List<object>() { pubkey, new Dictionary<string, string>() { { "encoding", "base64" } } });

            var json = JsonSerializer.SerializeToUtf8Bytes(msg, new JsonSerializerOptions() { WriteIndented = false, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });


            ReadOnlyMemory<byte> mem = new ReadOnlyMemory<byte>(json);
            await _clientSocket.SendAsync(mem, WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(false);

            AddSubscription(sub, msg.Id);
            return sub;
        }

        public SubscriptionState SubscribeAccountInfo(string pubkey, Action<SubscriptionState, ResponseValue<AccountInfo>> callback)
            => SubscribeAccountInfoAsync(pubkey, callback).Result;

        public async Task UnsubscribeAsync(SubscriptionState subscription)
        {
            var req = new JsonRpcRequest(GetNextId(), GetUnsubscribeMethodName(subscription.Channel), new List<object>() { subscription.SubscriptionId });

            var json = JsonSerializer.SerializeToUtf8Bytes(req, new JsonSerializerOptions() { WriteIndented = false, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            Console.WriteLine("\n\n[" + DateTime.UtcNow.ToLongTimeString() + "][Received]\n" + UTF8Encoding.UTF8.GetString(json, 0, json.Length));

            AddSubscription(subscription, req.Id);

            ReadOnlyMemory<byte> mem = new ReadOnlyMemory<byte>(json);
            await _clientSocket.SendAsync(mem, WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(false);
        }

        private string GetUnsubscribeMethodName(SubscriptionChannel channel) => channel switch
        {
            SubscriptionChannel.Account => "accountUnsubscribe",
            SubscriptionChannel.Logs => "logsUnsubscribe",
            SubscriptionChannel.Program => "programUnsubscribe",
            SubscriptionChannel.Root => "rootUnsubscribe",
            SubscriptionChannel.Signature => "signatureUnsubscribe",
            SubscriptionChannel.Slot => "slotUnsubscribe"
        };

        public void Unsubscribe(SubscriptionState subscription) => UnsubscribeAsync(subscription).Wait();
    }
}
