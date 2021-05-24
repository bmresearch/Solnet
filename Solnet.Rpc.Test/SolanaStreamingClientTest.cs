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
        [TestMethod]
        public void SubscribeAccountInfoTest()
        {
            var notifEvent = new ManualResetEvent(false);
            var subConfirmEvent = new ManualResetEvent(false);
            var expected = File.ReadAllText("Resources/AccountSubscribe.json");
            var subConfirmContent = File.ReadAllBytes("Resources/AccountSubscribeConfirm.json");
            var notificationContents = File.ReadAllBytes("Resources/AccountSubscribeNotification.json");
            bool confirmed = false;

            ValueTask<ValueWebSocketReceiveResult> valueWebSocketReceiveResult = new ValueTask<ValueWebSocketReceiveResult>(
                new ValueWebSocketReceiveResult(subConfirmContent.Length, WebSocketMessageType.Text, true));

            ReadOnlyMemory<byte> result = new ReadOnlyMemory<byte>();
            var socket = new Mock<IWebSocket>();
            ResponseValue<AccountInfo> resultNotif = null;

            SolanaStreamingClient sut = new SolanaStreamingClient("wss://api.mainnet-beta.solana.com/", socket.Object);

            var pubKey = "CM78CPUeXjn8o3yroDHxUtKsZZgoy4GPkPPXfouKNH12";
            var action = new Mock<Action<SubscriptionState, ResponseValue<AccountInfo>>>();

            //TODO: refactor these setups so that they can be reused by every subscription test, 
            // just need to use generics with type being notified

            socket.Setup(_ => _.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Callback(() => socket.SetupGet(s => s.State).Returns(WebSocketState.Open));

            socket.Setup(_ => _.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()))
                .Callback<ReadOnlyMemory<byte>, WebSocketMessageType, bool, CancellationToken>((mem, _, _, _) => result = mem);

            socket.Setup(_ => _.ReceiveAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>())).
                Callback<Memory<byte>, CancellationToken>((mem, _) =>
                {
                    if (!confirmed)
                    {
                        subConfirmEvent.WaitOne();
                        subConfirmContent.CopyTo(mem);
                        confirmed = true;
                    }
                    else
                    {
                        notificationContents.CopyTo(mem);
                        valueWebSocketReceiveResult = new ValueTask<ValueWebSocketReceiveResult>(
                new ValueWebSocketReceiveResult(notificationContents.Length, WebSocketMessageType.Text, true));
                    }

                }).Returns(valueWebSocketReceiveResult);

            action.Setup(_ => _(It.IsAny<SubscriptionState>(), It.IsAny<ResponseValue<AccountInfo>>())).Callback<SubscriptionState, ResponseValue<AccountInfo>>((_, notifValue) =>
            {
                resultNotif = notifValue;
                notifEvent.Set();
            });

            sut.Init().Wait();
            var sub = sut.SubscribeAccountInfo(pubKey, action.Object);
            subConfirmEvent.Set();

            socket.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);

            Assert.IsTrue(notifEvent.WaitOne(5000));
            Assert.AreEqual(5199307, resultNotif.Context.Slot);
            Assert.AreEqual("11111111111111111111111111111111", resultNotif.Value.Owner);
            Assert.AreEqual(33594UL, resultNotif.Value.Lamports);
            Assert.AreEqual(635UL, resultNotif.Value.RentEpoch);
            Assert.AreEqual(false, resultNotif.Value.Executable);
            Assert.AreEqual("11116bv5nS2h3y12kD1yUKeMZvGcKLSjQgX6BeV7u1FrjeJcKfsHPXHRDEHrBesJhZyqnnq9qJeUuF7WHxiuLuL5twc38w2TXNLxnDbjmuR", resultNotif.Value.Data[0]);
        }

    }
}
