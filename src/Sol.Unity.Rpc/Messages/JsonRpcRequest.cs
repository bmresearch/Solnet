using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sol.Unity.Rpc.Messages
{
    /// <summary>
    /// Rpc request message.
    /// </summary>
    public class JsonRpcRequest : JsonRpcBase
    {
        /// <summary>
        /// The request method.
        /// </summary>
        public string Method { get; }

        /// <summary>
        /// The method parameters list.
        /// </summary>
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