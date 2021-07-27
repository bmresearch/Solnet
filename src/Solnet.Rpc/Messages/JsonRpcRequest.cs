using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Solnet.Rpc.Messages
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
        /// The raw RPC payload for this request
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string RawRequest { get; internal set; }

        /// <summary>
        /// The raw RPC response 
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string RawResponse { get; internal set; }

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