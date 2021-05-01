using Nethereum.JsonRpc.Client;

namespace Solnet.Rpc
{
    public class RpcClientWrapper
    {
        public RpcClientWrapper(IClient client)
        {
            Client = client;
        }

        public IClient Client { get; protected set; }
    }
}