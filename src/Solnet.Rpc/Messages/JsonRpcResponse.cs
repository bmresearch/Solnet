namespace Solnet.Rpc.Messages
{
    public class JsonRpcResponse<T> : JsonRpcBase
    {
        public T Result { get; set; }

    }

    public class ContextObj
    {
        public int Slot { get; set; }
    }

    public class ResponseValue<T>
    {
        public ContextObj Context { get; set; }

        public T Value { get; set; }
    }
}
