namespace Solnet.Rpc.Messages
{
    public class JsonRpcStreamResponse<T>
    {
        public T Result { get; set; }

        public int Subscription { get; set; }
    }
}