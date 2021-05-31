namespace Solnet.Rpc.Messages
{
    public class JsonRpcResponse<T> : JsonRpcBase
    {
        public T Result { get; set; }
    }

    public class JsonRpcErrorResponse : JsonRpcBase
    {
        public ErrorContent Error { get; set; }
    }

    public class ErrorContent
    {
        public int Code { get; set; }
        public string Message { get; set; }
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
