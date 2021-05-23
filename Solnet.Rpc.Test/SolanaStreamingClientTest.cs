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

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class SolanaStreamingClientTest
    {
        [TestMethod]
        public void SubscribeAccountInfoTest()
        {
            var expected = File.ReadAllText("Resources/AccountSubscribe.json");
            ReadOnlyMemory<byte> result = new ReadOnlyMemory<byte>();
            var socket = new Mock<IWebSocket>();

            SolanaStreamingClient sut = new SolanaStreamingClient("wss://api.mainnet-beta.solana.com/", socket.Object);

            var pubKey = "CM78CPUeXjn8o3yroDHxUtKsZZgoy4GPkPPXfouKNH12";

            var action = new Mock<Action<SubscriptionState, ResponseValue<AccountInfo>>>();

            socket.Setup(_ => _.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Callback(() => socket.SetupGet(s => s.State).Returns(WebSocketState.Open));

            socket.Setup(_ => _.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()))
                .Callback<ReadOnlyMemory<byte>, WebSocketMessageType, bool, CancellationToken>((mem, _, _, _) => result = mem);

            sut.Init().Wait();
            sut.SubscribeAccountInfo(pubKey, action.Object);

            socket.Verify(s => s.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()));
            var res = Encoding.UTF8.GetString(result.Span);
            Assert.AreEqual(expected, res);
        }

    }
}
