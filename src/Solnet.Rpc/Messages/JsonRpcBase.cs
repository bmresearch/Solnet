namespace Solnet.Rpc.Messages
{
    public class JsonRpcBase
    {
        public string Jsonrpc { get; protected set; }

        public int Id { get; set; }

    }
}
