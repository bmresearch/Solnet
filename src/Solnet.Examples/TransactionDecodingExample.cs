using Solnet.Rpc;

namespace Solnet.Examples
{
    public class TransactionDecodingExample : IExample
    {
        private static readonly IRpcClient RpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        
        public void Run()
        {
            
        }
    }
}