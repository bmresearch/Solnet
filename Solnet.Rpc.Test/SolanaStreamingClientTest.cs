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
        private Mock<IWebSocket> socketMock;
        private ManualResetEvent notifEvent;
        private ManualResetEvent subConfirmEvent;
        private bool isSubConfirmed;
        private ValueTask<ValueWebSocketReceiveResult> valueTaskConfirmation;
        private ValueTask<ValueWebSocketReceiveResult> valueTaskNotification;

        private void SetupAction<T>(out Action<SubscriptionState, T> action, Action<T> resultCaptureCallback, Action<ReadOnlyMemory<byte>> sentPayloadCaptureCallback, byte[] subConfirmContent, byte[] notificationContents)
        {

            var actionMock = new Mock<Action<SubscriptionState, T>>();
            actionMock.Setup(_ => _(It.IsAny<SubscriptionState>(), It.IsAny<T>())).Callback<SubscriptionState, T>((_, notifValue) =>
            {
                resultCaptureCallback(notifValue);
                notifEvent.Set();
            });
            action = actionMock.Object;

            valueTaskConfirmation = new ValueTask<ValueWebSocketReceiveResult>(
                                            new ValueWebSocketReceiveResult(subConfirmContent.Length, WebSocketMessageType.Text, true));
            valueTaskNotification = new ValueTask<ValueWebSocketReceiveResult>(
                                                        new ValueWebSocketReceiveResult(notificationContents.Length, WebSocketMessageType.Text, true));

            socketMock.Setup(_ => _.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Callback(() => socketMock.SetupGet(s => s.State).Returns(WebSocketState.Open));

            socketMock.Setup(_ => _.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()))
                .Callback<ReadOnlyMemory<byte>, WebSocketMessageType, bool, CancellationToken>((mem, _, _, _) => sentPayloadCaptureCallback(mem));

            socketMock.Setup(_ => _.ReceiveAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>())).
                Callback<Memory<byte>, CancellationToken>((mem, _) =>
                {
                    if (!isSubConfirmed)
                    {
                        subConfirmEvent.WaitOne();
                        subConfirmContent.CopyTo(mem);
                        isSubConfirmed = true;
                    }
                    else
                    {
                        notificationContents.CopyTo(mem);
                    }

                }).Returns(() => isSubConfirmed ? valueTaskNotification : valueTaskConfirmation);
        }

        [TestInitialize]
        public void SetupTest()
        {
            socketMock = new Mock<IWebSocket>();
            notifEvent = new ManualResetEvent(false);
            subConfirmEvent = new ManualResetEvent(false);
            isSubConfirmed = false;
        }

        [TestMethod]
        public void SubscribeAccountInfoTest()
        {
            var expected = File.ReadAllText("Resources/AccountSubscribe.json");
            var subConfirmContent = File.ReadAllBytes("Resources/AccountSubscribeConfirm.json");
            var notificationContents = File.ReadAllBytes("Resources/AccountSubscribeNotification.json");
            ResponseValue<AccountInfo> resultNotif = null;
            Action<SubscriptionState, ResponseValue<AccountInfo>> action;
            ReadOnlyMemory<byte> result = new ReadOnlyMemory<byte>();

            SetupAction(out action, (x) => resultNotif = x, (x) => result = x, subConfirmContent, notificationContents);

            SolanaStreamingClient sut = new SolanaStreamingClient("wss://api.mainnet-beta.solana.com/", socketMock.Object);

            var pubKey = "CM78CPUeXjn8o3yroDHxUtKsZZgoy4GPkPPXfouKNH12";

            sut.Init().Wait();
            var sub = sut.SubscribeAccountInfo(pubKey, action);
            subConfirmEvent.Set();

            socketMock.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(notifEvent.WaitOne(500));
            Assert.AreEqual(5199307, resultNotif.Context.Slot);
            Assert.AreEqual("11111111111111111111111111111111", resultNotif.Value.Owner);
            Assert.AreEqual(33594UL, resultNotif.Value.Lamports);
            Assert.AreEqual(635UL, resultNotif.Value.RentEpoch);
            Assert.AreEqual(false, resultNotif.Value.Executable);
            Assert.AreEqual("11116bv5nS2h3y12kD1yUKeMZvGcKLSjQgX6BeV7u1FrjeJcKfsHPXHRDEHrBesJhZyqnnq9qJeUuF7WHxiuLuL5twc38w2TXNLxnDbjmuR", resultNotif.Value.Data[0]);
        }


        [TestMethod]
        public void UnsubscribeTest()
        {
            var expected = File.ReadAllText("Resources/AccountSubscribe.json");
            var subConfirmContent = File.ReadAllBytes("Resources/AccountSubscribeConfirm.json");
            var unsubContents = File.ReadAllBytes("Resources/AccountSubUnsubscription.json");
            bool unsubscribed = false;
            ResponseValue<AccountInfo> resultNotif = null;
            Action<SubscriptionState, ResponseValue<AccountInfo>> action;
            ReadOnlyMemory<byte> result = new ReadOnlyMemory<byte>();

            SetupAction(out action, (x) => resultNotif = x, (x) => result = x, subConfirmContent, unsubContents);

            SolanaStreamingClient sut = new SolanaStreamingClient("wss://api.mainnet-beta.solana.com/", socketMock.Object);

            var pubKey = "CM78CPUeXjn8o3yroDHxUtKsZZgoy4GPkPPXfouKNH12";

            sut.Init().Wait();
            var sub = sut.SubscribeAccountInfo(pubKey, action);
            sub.SubscriptionChanged += (_, e) =>
            {
                if (e.Status == SubscriptionStatus.Unsubscribed)
                {
                    notifEvent.Set();
                    unsubscribed = true;
                }
            };

            subConfirmEvent.Set();

            sub.Unsubscribe();

            Assert.IsTrue(notifEvent.WaitOne(500));
            Assert.IsTrue(unsubscribed);
        }
    }
}
