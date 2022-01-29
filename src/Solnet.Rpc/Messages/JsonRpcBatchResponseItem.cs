using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solnet.Rpc.Messages
{
    /// <summary>
    /// An object that represents a response item from an API batch request.
    /// The response type hint is supplied, 
    /// </summary>
    public class JsonRpcBatchResponseItem : JsonRpcBase
    {
        /// <summary>
        /// The anticipated runtime type of this result.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Type ResultType { get; set; }

        /// <summary>
        /// The RPC result of a given request as object.
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// The RPC result of a given request cast as T
        /// </summary>
        public T ResultAs<T>() 
        { 
            return (T) Result;
        } 

    }
}
