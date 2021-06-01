using Solnet.Rpc.Core.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using System;
using System.Threading;
using System.IO;
using System.Net.WebSockets;
using System.Text;
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
        private ValueTask<ValueWebSocketReceiveResult> _valueTaskConfirmation;
        private ValueTask<ValueWebSocketReceiveResult> _valueTaskNotification;

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

            _socketMock.Setup(_ => _.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Callback(() => _socketMock.SetupGet(s => s.State).Returns(WebSocketState.Open));

            _socketMock.Setup(_ => _.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()))
                .Callback<ReadOnlyMemory<byte>, WebSocketMessageType, bool, CancellationToken>((mem, _, _, _) => sentPayloadCaptureCallback(mem));

            _socketMock.Setup(_ => _.ReceiveAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>())).
                Callback<Memory<byte>, CancellationToken>((mem, _) =>
                {
                    if (!_isSubConfirmed)
                    {
                        _subConfirmEvent.WaitOne();
                        subConfirmContent.CopyTo(mem);
                        _isSubConfirmed = true;
                    }
                    else
                    {
                        notificationContents.CopyTo(mem);
                    }

                }).Returns(() => _isSubConfirmed ? _valueTaskNotification : _valueTaskConfirmation);
        }

        [TestInitialize]
        public void SetupTest()
        {
            _socketMock = new Mock<IWebSocket>();
            _notificationEvent = new ManualResetEvent(false);
            _subConfirmEvent = new ManualResetEvent(false);
            _isSubConfirmed = false;
        }

        [TestMethod]
        public void SubscribeAccountInfoTest()
        {
            var expected = File.ReadAllText("Resources/AccountSubscribe.json");
            var subConfirmContent = File.ReadAllBytes("Resources/SubscribeConfirm.json");
            var notificationContents = File.ReadAllBytes("Resources/AccountSubscribeNotification.json");
            ResponseValue<AccountInfo> resultNotification = null;
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<AccountInfo>> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                notificationContents);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", _socketMock.Object);

            const string pubKey = "CM78CPUeXjn8o3yroDHxUtKsZZgoy4GPkPPXfouKNH12";

            sut.Init().Wait();
            _ = sut.SubscribeAccountInfo(pubKey, action);
            _subConfirmEvent.Set();

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(_notificationEvent.WaitOne());
            Assert.AreEqual(5199307, resultNotification.Context.Slot);
            Assert.AreEqual("11111111111111111111111111111111", resultNotification.Value.Owner);
            Assert.AreEqual(33594UL, resultNotification.Value.Lamports);
            Assert.AreEqual(635UL, resultNotification.Value.RentEpoch);
            Assert.AreEqual(false, resultNotification.Value.Executable);
        }


        [TestMethod]
        public void UnsubscribeTest()
        {
            var subConfirmContent = File.ReadAllBytes("Resources/SubscribeConfirm.json");
            var unsubContents = File.ReadAllBytes("Resources/AccountSubUnsubscription.json");
            var unsubscribed = false;

            SetupAction(out Action<SubscriptionState, ResponseValue<AccountInfo>> action,
                _ => { },
                _ => { },
                subConfirmContent,
                unsubContents);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", _socketMock.Object);

            const string pubKey = "CM78CPUeXjn8o3yroDHxUtKsZZgoy4GPkPPXfouKNH12";

            sut.Init().Wait();
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

            Assert.IsTrue(_notificationEvent.WaitOne());
            Assert.IsTrue(unsubscribed);
        }

        [TestMethod]
        public void SubscribeLogsMentionTest()
        {
            var expected = File.ReadAllText("Resources/LogsSubscribeMention.json");
            var subConfirmContent = File.ReadAllBytes("Resources/SubscribeConfirm.json");
            ResponseValue<LogInfo> resultNotification = null;
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<LogInfo>> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                new byte[0]);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", _socketMock.Object);

            const string pubKey = "11111111111111111111111111111111";

            sut.Init().Wait();
            _ = sut.SubscribeLogInfo(pubKey, action);

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
            var expected = File.ReadAllText("Resources/LogsSubscribeAll.json");
            var subConfirmContent = File.ReadAllBytes("Resources/SubscribeConfirm.json");
            var notificationContents = File.ReadAllBytes("Resources/LogsSubscribeNotification.json");
            ResponseValue<LogInfo> resultNotification = null;
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<LogInfo>> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                notificationContents);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", _socketMock.Object);

            sut.Init().Wait();
            _ = sut.SubscribeLogInfo(Types.LogsSubscriptionType.All, action);
            _subConfirmEvent.Set();

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(_notificationEvent.WaitOne());
            Assert.AreEqual(5208469, resultNotification.Context.Slot);
            Assert.AreEqual("5h6xBEauJ3PK6SWCZ1PGjBvj8vDdWG3KpwATGy1ARAXFSDwt8GFXM7W5Ncn16wmqokgpiKRLuS83KUxyZyv2sUYv", resultNotification.Value.Signature);
            Assert.IsNull(resultNotification.Value.Error);
            Assert.AreEqual("BPF program 83astBRguLMdt2h5U1Tpdq5tjFoJ6noeGwaY3mDLVcri success", resultNotification.Value.Logs[0]);
        }


        [TestMethod]
        public void SubscribeProgramTest()
        {
            var expected = File.ReadAllText("Resources/ProgramSubscribe.json");
            var subConfirmContent = File.ReadAllBytes("Resources/SubscribeConfirm.json");
            var notificationContents = File.ReadAllBytes("Resources/ProgramSubscribeNotification.json");
            ResponseValue<ProgramInfo> resultNotification = null;
            var result = new ReadOnlyMemory<byte>();

            SetupAction(out Action<SubscriptionState, ResponseValue<ProgramInfo>> action,
                (x) => resultNotification = x,
                (x) => result = x,
                subConfirmContent,
                notificationContents);

            var sut = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/", _socketMock.Object);

            sut.Init().Wait();
            _ = sut.SubscribeProgram("11111111111111111111111111111111", action);
            _subConfirmEvent.Set();

            _socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(_notificationEvent.WaitOne());
            Assert.AreEqual(80854485, resultNotification.Context.Slot);
            Assert.AreEqual("9FXD1NXrK6xFU8i4gLAgjj2iMEWTqJhSuQN8tQuDfm2e", resultNotification.Value.PublicKey);
            Assert.AreEqual("11111111111111111111111111111111", resultNotification.Value.Account.Owner);
            Assert.AreEqual(false, resultNotification.Value.Account.Executable);
            Assert.AreEqual(187, resultNotification.Value.Account.RentEpoch);
            Assert.AreEqual(458553192193, resultNotification.Value.Account.Lamports);
        }
    }
}
