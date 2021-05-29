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
            var subConfirmContent = File.ReadAllBytes("Resources/AccountSubscribeConfirm.json");
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

            Assert.IsTrue(_notificationEvent.WaitOne(500));
            Assert.AreEqual(5199307, resultNotification.Context.Slot);
            Assert.AreEqual("11111111111111111111111111111111", resultNotification.Value.Owner);
            Assert.AreEqual(33594UL, resultNotification.Value.Lamports);
            Assert.AreEqual(635UL, resultNotification.Value.RentEpoch);
            Assert.AreEqual(false, resultNotification.Value.Executable);

            var data = resultNotification.Value.TryGetAccountData(out string accountDataString);
            Assert.IsTrue(data);
            Assert.AreEqual(
                "11116bv5nS2h3y12kD1yUKeMZvGcKLSjQgX6BeV7u1FrjeJcKfsHPXHRDEHrBesJhZyqnnq9qJeUuF7WHxiuLuL5twc38w2TXNLxnDbjmuR",
                accountDataString);
        }


        [TestMethod]
        public void UnsubscribeTest()
        {
            var subConfirmContent = File.ReadAllBytes("Resources/AccountSubscribeConfirm.json");
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
                    _notificationEvent.Set();
                    unsubscribed = true;
                }
            };

            _subConfirmEvent.Set();

            sub.Unsubscribe();

            Assert.IsTrue(_notificationEvent.WaitOne(500));
            Assert.IsTrue(unsubscribed);
        }
    }
}
