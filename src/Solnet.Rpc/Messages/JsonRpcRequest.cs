using System.Collections.Generic;

namespace Solnet.Rpc.Messages
{
    public class JsonRpcRequest : JsonRpcBase
    {
        public string Method { get; }

        public IList<object> Params { get; }

        internal JsonRpcRequest(int id, string method, IList<object> parameters)
        {
            Params = parameters;
            Method = method;
            Id = id;
            Jsonrpc = "2.0";
        }
    }
}
