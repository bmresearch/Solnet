using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Solnet.Rpc.Types;
using System.Threading;
using System.Threading.Tasks;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class SolanaStreamingClientTest
    {
        private Mock<IWebSocket> _socketMock;
        private ManualResetEvent _notificationEvent;
        private ManualResetEvent _subConfirmEvent;
        private bool _isSubConfirmed;
        private bool _hasNotified;
        private bool _hasEnded;
        private ValueTask<ValueWebSocketReceiveResult> _valueTaskConfirmation;
        private ValueTask<ValueWebSocketReceiveResult> _valueTaskNotification;
        private ValueTask<ValueWebSocketReceiveResult> _valueTaskEnd;

        private void SetupAction<T>(out Action<SubscriptionState, T> action, Action<T> resultCaptureCallback, Action<ReadOnlyMemory<byte>> sentPayloadCaptureCallback, byte[] subConfirmContent, byte[] notificationContents)
        {

            var actionMock = new Mock<Action<SubscriptionState, T>>();
            actionMock.Setup(_ => _(It.IsAny<SubscriptionState>(), It.IsAny<T>())).Callback<SubscriptionState, T>((_, notifValue) =>
            {
                resultCaptureCallback(notifValue);
                _notificationEvent.Set();
            });
            action = actionMock.Object;

            _valueTaskConfirmation = new ValueTask<ValueWebSocketReceiveResult>(
                                            new ValueWebSocketReceiveResult(subConfirmContent.Length, WebSocketMessageType.Text, true));
            _valueTaskNotification = new ValueTask<ValueWebSocketReceiveResult>(
                                                        new ValueWebSocketReceiveResult(notificationContents.Length, WebSocketMessageType.Text, true));

            _valueTaskEnd = new ValueTask<ValueWebSocketReceiveResult>(
                                                        new ValueWebSocketReceiveResult(0, WebSocketMessageType.Close, true));

            _socketMock.Setup(_ => _.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Callback(() => _socketMock.SetupGet(s => s.State).Returns(WebSocketState.Open));

            _socketMock.Setup(_ => _.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()))
                .Callback<ReadOnlyMemory<byte>, WebSocketMessageType, bool, CancellationToken>((mem, _, _, _) => sentPayloadCaptureCallback(mem));

            _socketMock.Setup(_ => _.ReceiveAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>())).
                Callback<Memory<byte>, CancellationToken>((mem, _) =>
                {
                    if (!_isSubConfirmed)
                    {
                        if (!_subConfirmEvent.WaitOne(TimeSpan.FromSeconds(10))) throw new TimeoutException("Subscription confirmation was not released.");
                        subConfirmContent.CopyTo(mem);
                        _isSubConfirmed = true;
                    }
                    else if (!_hasNotified)
                    {
                        notificationContents.CopyTo(mem);
                        _hasNotified = true;

                        _socketMock.SetupGet(s => s.State).Returns(WebSocketState.Closed);
                    }
                    else if (!_hasEnded)
                    {
                        _hasEnded = true;
                    }

                }).Returns(() => _hasEnded ? _valueTaskEnd : _hasNotified ? _valueTaskNotification : _valueTaskConfirmation);

        }

        [TestInitialize]
        public void SetupTest()
        {
            _socketMock = new Mock<IWebSocket>();
            _notificationEvent = new ManualResetEvent(false);
            _subConfirmEvent = new ManualResetEvent(false);
            _isSubConfirmed = false;
            _hasNotified = false;
            _hasEnded = false;
        }

        [TestMethod]
        public void SubscribeAccountInfoTest()
        {
            var expected = File.ReadAllText("Resources/Streaming/Account/AccountSubscribe.json");
            var subConfirmContent = File.ReadAllBytes("Resources/Streaming/SubscribeConfirm.json");
            var notificationContents = File.ReadAllBytes("Resources/Streaming/Account/AccountSubscribeNotification.json");
            ResponseValue<AccountInfo> resultNotification = null;
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<AccountInfo>> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                notificationContents);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            const string pubKey = "CM78CPUeXjn8o3yroDHxUtKsZZgoy4GPkPPXfouKNH12";

            sut.ConnectAsync().Wait();
            _ = sut.SubscribeAccountInfo(pubKey, action);
            _subConfirmEvent.Set();

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(_notificationEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");
            Assert.AreEqual(5199307UL, resultNotification.Context.Slot);
            Assert.AreEqual("11111111111111111111111111111111", resultNotification.Value.Owner);
            Assert.AreEqual(33594UL, resultNotification.Value.Lamports);
            Assert.AreEqual(635UL, resultNotification.Value.RentEpoch);
            Assert.IsFalse(resultNotification.Value.Executable);
        }

        [TestMethod]
        public void SubscribeTokenAccountTest()
        {
            var expected = File.ReadAllText("Resources/Streaming/Account/TokenAccountSubscribe.json");
            var subConfirmContent = File.ReadAllBytes("Resources/Streaming/SubscribeConfirm.json");
            var notificationContents = File.ReadAllBytes("Resources/Streaming/Account/TokenAccountSubscribeNotification.json");
            ResponseValue<TokenAccountInfo> resultNotification = null;
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<TokenAccountInfo>> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                notificationContents);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            const string pubKey = "CM78CPUeXjn8o3yroDHxUtKsZZgoy4GPkPPXfouKNH12";

            sut.ConnectAsync().Wait();
            _ = sut.SubscribeTokenAccount(pubKey, action);
            _subConfirmEvent.Set();

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(_notificationEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");
            Assert.AreEqual(99118135UL, resultNotification.Context.Slot);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", resultNotification.Value.Owner);
            Assert.AreEqual("F8Vyqk3unwxkXukZFQeYyGmFfTG3CAX4v24iyrjEYBJV", resultNotification.Value.Data.Parsed.Info.Owner);
            Assert.AreEqual("9830001302037", resultNotification.Value.Data.Parsed.Info.TokenAmount.Amount);
            Assert.AreEqual("9830001.302037", resultNotification.Value.Data.Parsed.Info.TokenAmount.UiAmountString);
            Assert.AreEqual(6, resultNotification.Value.Data.Parsed.Info.TokenAmount.Decimals);
            Assert.AreEqual(2039280UL, resultNotification.Value.Lamports);
        }

        [TestMethod]
        public void SubscribeAccountInfoTestProcessed()
        {
            var expected = File.ReadAllText("Resources/Streaming/Account/AccountSubscribeProcessed.json");
            var subConfirmContent = File.ReadAllBytes("Resources/Streaming/SubscribeConfirm.json");
            var notificationContents = File.ReadAllBytes("Resources/Streaming/Account/AccountSubscribeNotification.json");
            ResponseValue<AccountInfo> resultNotification = null;
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<AccountInfo>> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                notificationContents);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            const string pubKey = "CM78CPUeXjn8o3yroDHxUtKsZZgoy4GPkPPXfouKNH12";

            sut.ConnectAsync().Wait();
            _ = sut.SubscribeAccountInfo(pubKey, action, Types.Commitment.Processed);
            _subConfirmEvent.Set();

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(_notificationEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");
            Assert.AreEqual(5199307UL, resultNotification.Context.Slot);
            Assert.AreEqual("11111111111111111111111111111111", resultNotification.Value.Owner);
            Assert.AreEqual(33594UL, resultNotification.Value.Lamports);
            Assert.AreEqual(635UL, resultNotification.Value.RentEpoch);
            Assert.IsFalse(resultNotification.Value.Executable);
        }


        [TestMethod]
        public void UnsubscribeTest()
        {
            var subConfirmContent = File.ReadAllBytes("Resources/Streaming/SubscribeConfirm.json");
            var unsubContents = File.ReadAllBytes("Resources/Streaming/Account/AccountSubUnsubscription.json");
            var unsubscribed = false;

            SetupAction(out Action<SubscriptionState, ResponseValue<AccountInfo>> action,
                _ => { },
                _ => { },
                subConfirmContent,
                unsubContents);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            const string pubKey = "CM78CPUeXjn8o3yroDHxUtKsZZgoy4GPkPPXfouKNH12";

            sut.ConnectAsync().Wait();
            var sub = sut.SubscribeAccountInfo(pubKey, action);
            sub.SubscriptionChanged += (_, e) =>
            {
                if (e.Status == SubscriptionStatus.Unsubscribed)
                {
                    unsubscribed = true;
                    _notificationEvent.Set();
                }
            };

            _subConfirmEvent.Set();

            sub.Unsubscribe();

            Assert.IsTrue(_notificationEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");
            Assert.IsTrue(unsubscribed);
        }

        [TestMethod]
        public void SubscribeLogsMentionTest()
        {
            var expected = File.ReadAllText("Resources/Streaming/Logs/LogsSubscribeMention.json");
            var subConfirmContent = File.ReadAllBytes("Resources/Streaming/SubscribeConfirm.json");
            ResponseValue<LogInfo> resultNotification = null;
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<LogInfo>> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                new byte[0]);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            const string pubKey = "11111111111111111111111111111111";

            sut.ConnectAsync().Wait();
            _ = sut.SubscribeLogInfo(pubKey, action);

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

        }

        [TestMethod]
        public void SubscribeLogsMentionConfirmed()
        {
            var expected = File.ReadAllText("Resources/Streaming/Logs/LogsSubscribeMentionConfirmed.json");
            var subConfirmContent = File.ReadAllBytes("Resources/Streaming/SubscribeConfirm.json");
            ResponseValue<LogInfo> resultNotification = null;
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<LogInfo>> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                new byte[0]);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            const string pubKey = "11111111111111111111111111111111";

            sut.ConnectAsync().Wait();
            _ = sut.SubscribeLogInfo(pubKey, action, Types.Commitment.Confirmed);

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

        }

        [TestMethod]
        public void SubscribeLogsAllTest()
        {
            var expected = File.ReadAllText("Resources/Streaming/Logs/LogsSubscribeAll.json");
            var subConfirmContent = File.ReadAllBytes("Resources/Streaming/SubscribeConfirm.json");
            var notificationContents = File.ReadAllBytes("Resources/Streaming/Logs/LogsSubscribeNotification.json");
            ResponseValue<LogInfo> resultNotification = null;
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<LogInfo>> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                notificationContents);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            sut.ConnectAsync().Wait();
            _ = sut.SubscribeLogInfo(Types.LogsSubscriptionType.All, action);
            _subConfirmEvent.Set();

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(_notificationEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");
            Assert.AreEqual(5208469UL, resultNotification.Context.Slot);
            Assert.AreEqual("5h6xBEauJ3PK6SWCZ1PGjBvj8vDdWG3KpwATGy1ARAXFSDwt8GFXM7W5Ncn16wmqokgpiKRLuS83KUxyZyv2sUYv", resultNotification.Value.Signature);
            Assert.IsNull(resultNotification.Value.Error);
            Assert.AreEqual("BPF program 83astBRguLMdt2h5U1Tpdq5tjFoJ6noeGwaY3mDLVcri success", resultNotification.Value.Logs[0]);
        }

        [TestMethod]
        public void SubscribeLogsAllProcessed()
        {
            var expected = File.ReadAllText("Resources/Streaming/Logs/LogsSubscribeAllProcessed.json");
            var subConfirmContent = File.ReadAllBytes("Resources/Streaming/SubscribeConfirm.json");
            var notificationContents = File.ReadAllBytes("Resources/Streaming/Logs/LogsSubscribeNotification.json");
            ResponseValue<LogInfo> resultNotification = null;
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<LogInfo>> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                notificationContents);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            sut.ConnectAsync().Wait();
            _ = sut.SubscribeLogInfo(Types.LogsSubscriptionType.All, action, Types.Commitment.Processed);
            _subConfirmEvent.Set();

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(_notificationEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");
            Assert.AreEqual(5208469UL, resultNotification.Context.Slot);
            Assert.AreEqual("5h6xBEauJ3PK6SWCZ1PGjBvj8vDdWG3KpwATGy1ARAXFSDwt8GFXM7W5Ncn16wmqokgpiKRLuS83KUxyZyv2sUYv", resultNotification.Value.Signature);
            Assert.IsNull(resultNotification.Value.Error);
            Assert.AreEqual("BPF program 83astBRguLMdt2h5U1Tpdq5tjFoJ6noeGwaY3mDLVcri success", resultNotification.Value.Logs[0]);
        }

        [TestMethod]
        public void SubscribeLogsWithErrors()
        {
            var expected = File.ReadAllText("Resources/Streaming/Logs/LogsSubscribeAllProcessed.json");
            var subConfirmContent = File.ReadAllBytes("Resources/Streaming/SubscribeConfirm.json");
            var notificationContents = File.ReadAllBytes("Resources/Streaming/Logs/LogsSubscribeNotificationWithError.json");
            ResponseValue<LogInfo> resultNotification = null;
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<LogInfo>> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                notificationContents);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            sut.ConnectAsync().Wait();
            _ = sut.SubscribeLogInfo(Types.LogsSubscriptionType.All, action, Types.Commitment.Processed);
            _subConfirmEvent.Set();

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(_notificationEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");
            Assert.AreEqual(TransactionErrorType.InstructionError, resultNotification.Value.Error.Type);
            Assert.AreEqual(InstructionErrorType.Custom, resultNotification.Value.Error.InstructionError.Type);
            Assert.AreEqual(41u, resultNotification.Value.Error.InstructionError.CustomError);

            Assert.AreEqual("bGNVGCa1WFchzJStauKFVk7anzuFvA7hkMcx8Zi2o4euJaivzpwz8346yJ4Xn8H7XzMp44coTxdcDRd9d4yzj4m", resultNotification.Value.Signature);
        }

        [TestMethod]
        public void SubscribeProgramTest()
        {
            var expected = File.ReadAllText("Resources/Streaming/Program/ProgramSubscribe.json");
            var subConfirmContent = File.ReadAllBytes("Resources/Streaming/SubscribeConfirm.json");
            var notificationContents = File.ReadAllBytes("Resources/Streaming/Program/ProgramSubscribeNotification.json");
            ResponseValue<AccountKeyPair> resultNotification = null;
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<AccountKeyPair>> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                notificationContents);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            sut.ConnectAsync().Wait();
            _ = sut.SubscribeProgram("11111111111111111111111111111111", action);
            _subConfirmEvent.Set();

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(_notificationEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");
            Assert.AreEqual(80854485UL, resultNotification.Context.Slot);
            Assert.AreEqual("9FXD1NXrK6xFU8i4gLAgjj2iMEWTqJhSuQN8tQuDfm2e", resultNotification.Value.PublicKey);
            Assert.AreEqual("11111111111111111111111111111111", resultNotification.Value.Account.Owner);
            Assert.IsFalse(resultNotification.Value.Account.Executable);
            Assert.AreEqual(187UL, resultNotification.Value.Account.RentEpoch);
            Assert.AreEqual(458553192193UL, resultNotification.Value.Account.Lamports);
        }

        [TestMethod]
        public void SubscribeProgramFilters()
        {
            var expected = File.ReadAllText("Resources/Streaming/Program/ProgramSubscribeFilters.json");
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<AccountKeyPair>> action,
                (x) => {},
                (x) => result = x,
                Array.Empty<byte>(),
                Array.Empty<byte>());

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            sut.ConnectAsync().Wait();
            _ = sut.SubscribeProgram("9xQeWvG816bUx9EPjHmaT23yvVM2ZWbrrpZb9PusVFin", (s, e) => {}, 
                    dataSize:3228, memCmpList: new List<MemCmp>() { new MemCmp() {Offset = 45, Bytes = "CuieVDEDtLo7FypA9SbLM9saXFdb1dsshEkyErMqkRQq"}});
            _subConfirmEvent.Set();


            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);
        }


        [TestMethod]
        public void SubscribeProgramMemcmpFilters()
        {
            var expected = File.ReadAllText("Resources/Streaming/Program/ProgramSubscribeMemcmpFilter.json");
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<AccountKeyPair>> action,
                (x) => {},
                (x) => result = x,
                Array.Empty<byte>(),
                Array.Empty<byte>());

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            sut.ConnectAsync().Wait();
            _ = sut.SubscribeProgram("9xQeWvG816bUx9EPjHmaT23yvVM2ZWbrrpZb9PusVFin", (s, e) => {}, 
                memCmpList: new List<MemCmp>() { new MemCmp() {Offset = 45, Bytes = "CuieVDEDtLo7FypA9SbLM9saXFdb1dsshEkyErMqkRQq"}});
            _subConfirmEvent.Set();


            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);
        }


        [TestMethod]
        public void SubscribeProgramDataFilter()
        {
            var expected = File.ReadAllText("Resources/Streaming/Program/ProgramSubscribeDataSizeFilter.json");
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<AccountKeyPair>> action,
                (x) => {},
                (x) => result = x,
                Array.Empty<byte>(),
                Array.Empty<byte>());

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            sut.ConnectAsync().Wait();
            _ = sut.SubscribeProgram("9xQeWvG816bUx9EPjHmaT23yvVM2ZWbrrpZb9PusVFin", (s, e) => {}, 
                dataSize:3228);
            _subConfirmEvent.Set();


            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);
        }

        [TestMethod]
        public void SubscribeProgramConfirmed()
        {
            var expected = File.ReadAllText("Resources/Streaming/Program/ProgramSubscribeConfirmed.json");
            var subConfirmContent = File.ReadAllBytes("Resources/Streaming/SubscribeConfirm.json");
            var notificationContents = File.ReadAllBytes("Resources/Streaming/Program/ProgramSubscribeNotification.json");
            ResponseValue<AccountKeyPair> resultNotification = null;
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<AccountKeyPair>> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                notificationContents);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            sut.ConnectAsync().Wait();
            _ = sut.SubscribeProgram("11111111111111111111111111111111", action, Types.Commitment.Confirmed);
            _subConfirmEvent.Set();

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(_notificationEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");
            Assert.AreEqual(80854485UL, resultNotification.Context.Slot);
            Assert.AreEqual("9FXD1NXrK6xFU8i4gLAgjj2iMEWTqJhSuQN8tQuDfm2e", resultNotification.Value.PublicKey);
            Assert.AreEqual("11111111111111111111111111111111", resultNotification.Value.Account.Owner);
            Assert.IsFalse(resultNotification.Value.Account.Executable);
            Assert.AreEqual(187UL, resultNotification.Value.Account.RentEpoch);
            Assert.AreEqual(458553192193UL, resultNotification.Value.Account.Lamports);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void SubscribeSlotTest(bool subscriptionFirst)
        {
            var expected = File.ReadAllText("Resources/Streaming/SlotSubscribe.json");
            var subConfirmContent = File.ReadAllBytes("Resources/Streaming/SubscribeConfirm.json");
            var notificationContents = File.ReadAllBytes("Resources/Streaming/SlotSubscribeNotification.json");
            if (subscriptionFirst)
                notificationContents = Encoding.UTF8.GetBytes("{\"params\":{\"subscription\":23784,\"result\":{\"slot\":76,\"parent\":75,\"root\":44}},\"method\":\"slotNotification\",\"jsonrpc\":\"2.0\"}");
            SlotInfo resultNotification = null;
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, SlotInfo> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                notificationContents);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            sut.ConnectAsync().Wait();
            _ = sut.SubscribeSlotInfo(action);
            _subConfirmEvent.Set();

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(_notificationEvent.WaitOne(TimeSpan.FromSeconds(5)));
            Assert.AreEqual(75, resultNotification.Parent);
            Assert.AreEqual(44, resultNotification.Root);
            Assert.AreEqual(76, resultNotification.Slot);
        }


        [TestMethod]
        [DataRow(null, false)]
        [DataRow("11111111111111111111111111111111", false)]
        [DataRow(null, true)]
        public void SubscribeBlockTest(string mention, bool hasError)
        {
            byte[] confirmation = File.ReadAllBytes("Resources/Streaming/SubscribeConfirm.json");
            string value = hasError
                ? "{\"slot\":76,\"block\":null,\"err\":{\"blockStoreError\":\"unavailable\"}}"
                : "{\"slot\":76,\"block\":{\"parentSlot\":75,\"blockhash\":\"hash\",\"previousBlockhash\":\"parent\",\"transactions\":[],\"rewards\":[],\"blockTime\":123,\"blockHeight\":70},\"err\":null}";
            byte[] notification = Encoding.UTF8.GetBytes("{\"params\":{\"subscription\":23784,\"result\":{\"context\":{\"slot\":77},\"value\":" + value + "}},\"method\":\"blockNotification\",\"jsonrpc\":\"2.0\"}");
            ResponseValue<BlockUpdate> update = null;
            ReadOnlyMemory<byte> sent = default;
            SetupAction(out Action<SubscriptionState, ResponseValue<BlockUpdate>> action,
                x => update = x, x => sent = x, confirmation, notification);
            using var client = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);
            client.ConnectAsync().GetAwaiter().GetResult();
            var subscription = client.SubscribeBlock(action, mention, Commitment.Confirmed, showRewards: true);
            using (JsonDocument request = JsonDocument.Parse(sent))
            {
                Assert.AreEqual("blockSubscribe", request.RootElement.GetProperty("method").GetString());
                var parameters = request.RootElement.GetProperty("params");
                if (mention == null) Assert.AreEqual("all", parameters[0].GetString());
                else Assert.AreEqual(mention, parameters[0].GetProperty("mentionsAccountOrProgram").GetString());
                Assert.AreEqual("confirmed", parameters[1].GetProperty("commitment").GetString());
                Assert.AreEqual("json", parameters[1].GetProperty("encoding").GetString());
                Assert.AreEqual("full", parameters[1].GetProperty("transactionDetails").GetString());
                Assert.IsTrue(parameters[1].GetProperty("showRewards").GetBoolean());
                Assert.AreEqual(1, parameters[1].GetProperty("maxSupportedTransactionVersion").GetInt32());
            }
            _subConfirmEvent.Set();
            Assert.IsTrue(_notificationEvent.WaitOne(TimeSpan.FromSeconds(10)));
            Assert.AreEqual(77UL, update.Context.Slot);
            Assert.AreEqual(76UL, update.Value.Slot);
            if (hasError)
            {
                Assert.IsNull(update.Value.Block);
                Assert.AreEqual("unavailable", update.Value.Err.Value.GetProperty("blockStoreError").GetString());
            }
            else
            {
                Assert.AreEqual("hash", update.Value.Block.Blockhash);
                Assert.IsNull(update.Value.Err);
            }
            client.Unsubscribe(subscription);
            using JsonDocument unsubscribe = JsonDocument.Parse(sent);
            Assert.AreEqual("blockUnsubscribe", unsubscribe.RootElement.GetProperty("method").GetString());
            Assert.AreEqual(23784, unsubscribe.RootElement.GetProperty("params")[0].GetInt32());
        }

        [TestMethod]
        public async Task BlockSubscriptionRejectsProcessedCommitment()
        {
            using var client = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);
            await Assert.ThrowsExactlyAsync<ArgumentException>(() => client.SubscribeBlockAsync((_, _) => { }, commitment: Commitment.Processed));
        }

        [TestMethod]
        [DataRow(BinaryEncoding.Base58, "base58")]
        [DataRow(BinaryEncoding.Base64, "base64")]
        [DataRow(BinaryEncoding.Base64Zstd, "base64+zstd")]
        [DataRow(BinaryEncoding.JsonParsed, "jsonParsed")]
        public void AccountAndProgramSubscriptionsRespectEncoding(BinaryEncoding encoding, string expected)
        {
            ReadOnlyMemory<byte> sent = default;
            _socketMock.Setup(x => x.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()))
                .Callback<ReadOnlyMemory<byte>, WebSocketMessageType, bool, CancellationToken>((message, _, _, _) => sent = message);
            using var client = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);
            client.SubscribeAccountInfo("11111111111111111111111111111111", (_, _) => { }, encoding: encoding);
            using (JsonDocument request = JsonDocument.Parse(sent))
                Assert.AreEqual(expected, request.RootElement.GetProperty("params")[1].GetProperty("encoding").GetString());
            client.SubscribeProgram("11111111111111111111111111111111", (_, _) => { }, encoding: encoding);
            using (JsonDocument request = JsonDocument.Parse(sent))
                Assert.AreEqual(expected, request.RootElement.GetProperty("params")[1].GetProperty("encoding").GetString());
        }

        [TestMethod]
        public void SubscribeRootTest()
        {
            var expected = File.ReadAllText("Resources/Streaming/RootSubscribe.json");
            var subConfirmContent = File.ReadAllBytes("Resources/Streaming/SubscribeConfirm.json");
            var notificationContents = File.ReadAllBytes("Resources/Streaming/RootSubscribeNotification.json");
            int resultNotification = 0;
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, int> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                notificationContents);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            sut.ConnectAsync().Wait();
            var sub = sut.SubscribeRoot(action);
            _subConfirmEvent.Set();

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(_notificationEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");
            Assert.AreEqual(42, resultNotification);
        }

        [TestMethod]
        public void SubscribeSignatureTest()
        {
            var expected = File.ReadAllText("Resources/Streaming/Signature/SignatureSubscribe.json");
            var subConfirmContent = File.ReadAllBytes("Resources/Streaming/SubscribeConfirm.json");
            var notificationContents = File.ReadAllBytes("Resources/Streaming/Signature/SignatureSubscribeNotification.json");
            ResponseValue<ErrorResult> resultNotification = null;
            AutoResetEvent subscriptionEvent = new AutoResetEvent(false);
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<ErrorResult>> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                notificationContents);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            SubscriptionEvent evt = null;


            sut.ConnectAsync().Wait();
            var sub = sut.SubscribeSignature("4orRpuqStpJDvcpBy3vDSV4TDTGNbefmqYUnG2yVnKwjnLFqCwY4h5cBTAKakKek4inuxHF71LuscBS1vwSLtWcx", action);
            sub.SubscriptionChanged += (s, e) =>
            {
                evt = e;
                if (e.Status == SubscriptionStatus.Unsubscribed)
                    subscriptionEvent.Set();
            };
            _subConfirmEvent.Set();

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(_notificationEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");
            Assert.IsNull(resultNotification.Value.Error);

            Assert.IsTrue(subscriptionEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");
            Assert.AreEqual(SubscriptionStatus.Unsubscribed, evt.Status);
            Assert.AreEqual(SubscriptionStatus.Unsubscribed, sub.State);
        }

        [TestMethod]
        public void SubscribeSignature_ErrorNotificationTest()
        {
            var expected = File.ReadAllText("Resources/Streaming/Signature/SignatureSubscribe.json");
            var subConfirmContent = File.ReadAllBytes("Resources/Streaming/SubscribeConfirm.json");
            var notificationContents = File.ReadAllBytes("Resources/Streaming/Signature/SignatureSubscribeErrorNotification.json");
            ResponseValue<ErrorResult> resultNotification = null;
            AutoResetEvent subscriptionEvent = new AutoResetEvent(false);
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<ErrorResult>> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                notificationContents);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            SubscriptionEvent evt = null;


            sut.ConnectAsync().Wait();
            var sub = sut.SubscribeSignature("4orRpuqStpJDvcpBy3vDSV4TDTGNbefmqYUnG2yVnKwjnLFqCwY4h5cBTAKakKek4inuxHF71LuscBS1vwSLtWcx", action);
            sub.SubscriptionChanged += (s, e) =>
            {
                evt = e;
                if (e.Status == SubscriptionStatus.Unsubscribed)
                    subscriptionEvent.Set();
            };
            _subConfirmEvent.Set();

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(_notificationEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");

            Assert.IsTrue(subscriptionEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");

            Assert.AreEqual(TransactionErrorType.InstructionError, resultNotification.Value.Error.Type);
            Assert.AreEqual(InstructionErrorType.Custom, resultNotification.Value.Error.InstructionError.Type);
            Assert.AreEqual(0u, resultNotification.Value.Error.InstructionError.CustomError);

            Assert.AreEqual(SubscriptionStatus.Unsubscribed, evt.Status);
            Assert.AreEqual(SubscriptionStatus.Unsubscribed, sub.State);
        }

        [TestMethod]
        public void SubscribeSignatureProcessed()
        {
            var expected = File.ReadAllText("Resources/Streaming/Signature/SignatureSubscribeProcessed.json");
            var subConfirmContent = File.ReadAllBytes("Resources/Streaming/SubscribeConfirm.json");
            var notificationContents = File.ReadAllBytes("Resources/Streaming/Signature/SignatureSubscribeNotification.json");
            ResponseValue<ErrorResult> resultNotification = null;
            AutoResetEvent subscriptionEvent = new AutoResetEvent(false);
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<ErrorResult>> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                notificationContents);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            SubscriptionEvent evt = null;


            sut.ConnectAsync().Wait();
            var sub = sut.SubscribeSignature("4orRpuqStpJDvcpBy3vDSV4TDTGNbefmqYUnG2yVnKwjnLFqCwY4h5cBTAKakKek4inuxHF71LuscBS1vwSLtWcx", action, Types.Commitment.Processed);
            sub.SubscriptionChanged += (s, e) =>
            {
                evt = e;
                if (e.Status == SubscriptionStatus.Unsubscribed)
                    subscriptionEvent.Set();
            };
            _subConfirmEvent.Set();

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(_notificationEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");
            Assert.IsNull(resultNotification.Value.Error);

            Assert.IsTrue(subscriptionEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");
            Assert.AreEqual(SubscriptionStatus.Unsubscribed, evt.Status);
            Assert.AreEqual(SubscriptionStatus.Unsubscribed, sub.State);
        }


        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void SubscribeBadAccountTest(bool idFirst)
        {
            var expected = File.ReadAllText("Resources/Streaming/Account/BadAccountSubscribe.json");
            var subConfirmContent = File.ReadAllBytes("Resources/Streaming/Account/BadAccountSubscribeResult.json");
            if (idFirst)
                subConfirmContent = Encoding.UTF8.GetBytes("{\"id\":0,\"error\":{\"message\":\"Invalid Request: Invalid pubkey provided\",\"code\":-32602},\"jsonrpc\":\"2.0\"}");
            var result = new ReadOnlyMemory<byte>();
            AutoResetEvent subscriptionEvent = new AutoResetEvent(false);
            SetupAction(out Action<SubscriptionState, ResponseValue<AccountInfo>> action,
                _ => { },
                (x) => result = x,
                subConfirmContent,
                new byte[0]);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            const string pubKey = "invalidkey1";

            sut.ConnectAsync().Wait();
            var sub = sut.SubscribeAccountInfo(pubKey, action);
            SubscriptionEvent subEvent = null;
            sub.SubscriptionChanged += (sub, evt) =>
            {
                subEvent = evt;
                subscriptionEvent.Set();
            };

            _subConfirmEvent.Set();

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(subscriptionEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");
            Assert.AreEqual("-32602", subEvent.Code);
            Assert.AreEqual(SubscriptionStatus.ErrorSubscribing, subEvent.Status);
            Assert.AreEqual("Invalid Request: Invalid pubkey provided", subEvent.Error);

            Assert.AreEqual(SubscriptionStatus.ErrorSubscribing, sub.State);
        }



        [TestMethod]
        public void SubscribeAccountBigPayloadTest()
        {
            var expected = File.ReadAllText("Resources/Streaming/Account/BigAccountSubscribe.json");
            var subConfirmContent = File.ReadAllBytes("Resources/Streaming/SubscribeConfirm.json");
            var notifContent = File.ReadAllBytes("Resources/Streaming/Account/BigAccountNotificationPayload.json");
            var expectedDataContent = File.ReadAllText("Resources/Streaming/Account/BigAccountNotificationPayloadData.txt");
            var result = new ReadOnlyMemory<byte>();

            AutoResetEvent signal = new AutoResetEvent(false);
            int currentMessageIdx = 0;
            //confirm + bigpayload divided in two read steps + empty payload
            var payloads = new Memory<byte>[]
            {
                new Memory<byte>(subConfirmContent),
                new Memory<byte>(notifContent, 0, 32768),
                new Memory<byte>(notifContent, 32768, notifContent.Length - 32768),
                Memory<byte>.Empty

            };

            AutoResetEvent subscriptionEvent = new AutoResetEvent(false);
            ResponseValue<AccountInfo> notificationValue = null;

            var actionMock = new Mock<Action<SubscriptionState, ResponseValue<AccountInfo>>>();
            actionMock.Setup(_ => _(It.IsAny<SubscriptionState>(), It.IsAny<ResponseValue<AccountInfo>>())).Callback<SubscriptionState, ResponseValue<AccountInfo>>((_, notifValue) =>
            {
                notificationValue = notifValue;
                _notificationEvent.Set();
            });

            _socketMock.Setup(_ => _.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Callback(() => _socketMock.SetupGet(s => s.State).Returns(WebSocketState.Open));

            _socketMock.Setup(_ => _.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()))
                .Callback<ReadOnlyMemory<byte>, WebSocketMessageType, bool, CancellationToken>((mem, _, _, _) => result = mem);

            _ = _socketMock.Setup(_ => _.ReceiveAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>())).
                Callback<Memory<byte>, CancellationToken>((mem, _) =>
                {
                    if (currentMessageIdx == 0)
                        if (!signal.WaitOne(TimeSpan.FromSeconds(10))) throw new TimeoutException("Streaming test signal was not released.");
                    payloads[currentMessageIdx++].CopyTo(mem);
                }).Returns(() => new ValueTask<ValueWebSocketReceiveResult>(
                    new ValueWebSocketReceiveResult(
                        payloads[currentMessageIdx - 1].Length,
                        payloads[currentMessageIdx - 1].Length == 0 ? WebSocketMessageType.Close : WebSocketMessageType.Text,
                        currentMessageIdx == 2 ? false : true)));

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", null, _socketMock.Object);

            const string pubKey = "TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA";

            sut.ConnectAsync().Wait();
            var sub = sut.SubscribeAccountInfo(pubKey, actionMock.Object);
            SubscriptionEvent subEvent = null;
            sub.SubscriptionChanged += (sub, evt) =>
            {
                subEvent = evt;
                subscriptionEvent.Set();
            };

            signal.Set();

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));

            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(subscriptionEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");
            Assert.IsTrue(string.IsNullOrEmpty(subEvent.Code));
            Assert.AreEqual(SubscriptionStatus.Subscribed, subEvent.Status);
            Assert.IsTrue(string.IsNullOrEmpty(subEvent.Error));
            Assert.AreEqual(SubscriptionStatus.Subscribed, sub.State);

            Assert.IsTrue(_notificationEvent.WaitOne(TimeSpan.FromSeconds(10)), "Timed out waiting for a streaming event.");

            Assert.AreEqual(expectedDataContent, notificationValue.Value.Data[0]);
            Assert.AreEqual("base64", notificationValue.Value.Data[1]);
        }
    }
}
