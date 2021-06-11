using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Solnet.Rpc.Messages
{
    public class JsonRpcRequest : JsonRpcBase
    {
        public string Method { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
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