using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class ClientFactoryTest
    {
        [TestMethod]
        public void BuilRpcClient()
        {
            IRpcClient c = ClientFactory.GetClient(Cluster.DevNet);

            Assert.IsInstanceOfType(c, typeof(SolanaRpcClient));
        }

        [TestMethod]
        public void BuilStreamingRpcClient()
        {
            IStreamingRpcClient c = ClientFactory.GetStreamingClient(Cluster.DevNet);

            Assert.IsInstanceOfType(c, typeof(SolanaStreamingRpcClient));
        }

        [TestMethod]
        public void BuilRpcClientFromString()
        {
            string url = "https://testnet.solana.com";
            IRpcClient c = ClientFactory.GetClient(url);

            Assert.IsInstanceOfType(c, typeof(SolanaRpcClient));
            Assert.AreEqual(url, c.NodeAddress.OriginalString);
        }

        [TestMethod]
        public void BuilStreamingRpcClientFromString()
        {
            string url = "wss://api.testnet.solana.com";
            IStreamingRpcClient c = ClientFactory.GetStreamingClient(url);

            Assert.IsInstanceOfType(c, typeof(SolanaStreamingRpcClient));
            Assert.AreEqual(url, c.NodeAddress.OriginalString);
        }
    }
}