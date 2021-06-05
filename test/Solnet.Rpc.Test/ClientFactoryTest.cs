using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class ClientFactoryTest
    {
        [TestMethod]
        public void BuilRpcClient()
        {
            var c = ClientFactory.GetClient(Cluster.DevNet);

            Assert.IsInstanceOfType(c, typeof(SolanaRpcClient));
        }

        [TestMethod]
        public void BuilStreamingRpcClient()
        {
            var c = ClientFactory.GetStreamingClient(Cluster.DevNet);

            Assert.IsInstanceOfType(c, typeof(SolanaStreamingRpcClient));
        }

        [TestMethod]
        public void BuilRpcClientFromString()
        {
            string url = "https://testnet.solana.com";
            var c = ClientFactory.GetClient(url);

            Assert.IsInstanceOfType(c, typeof(SolanaRpcClient));
            Assert.AreEqual(url, c.NodeAddress.OriginalString);
        }

        [TestMethod]
        public void BuilStreamingRpcClientFromString()
        {
            string url = "wss://api.testnet.solana.com";
            var c = ClientFactory.GetStreamingClient(url);

            Assert.IsInstanceOfType(c, typeof(SolanaStreamingRpcClient));
            Assert.AreEqual(url, c.NodeAddress.OriginalString);
        }
    }
}