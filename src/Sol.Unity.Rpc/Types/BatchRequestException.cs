using Sol.Unity.Rpc.Core.Http;
using Sol.Unity.Rpc.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sol.Unity.Rpc.Types
{
    /// <summary>
    /// Encapsulates the batch request failure that is relayed to all callbacks
    /// </summary>
    public class BatchRequestException : ApplicationException
    {
        /// <summary>
        /// The RPC result that failed
        /// </summary>
        public RequestResult<JsonRpcBatchResponse> RpcResult;

        /// <summary>
        /// Contructs a BatchRequestException based on the JsonRpcBatchResponse result.
        /// </summary>
        /// <param name="result"></param>
        public BatchRequestException(RequestResult<JsonRpcBatchResponse> result) : base($"Batch request failure - {result.Reason}")
        {
            RpcResult = result;
        }

    }
}
