using System.Collections.Generic;
using System.Net;

namespace Solnet.Rpc.Core.Http
{
    /// <summary>
    /// Interface used to tepresent the result of a given request.
    /// </summary>
    public interface IRequestResult
    {
        /// <summary>
        /// Returns <c>true</c> if the request was successfully handled and parsed.
        /// </summary>
        bool WasSuccessful { get; }

        /// <summary>
        /// Returns <c>true</c> if the HTTP request was successful (e.g. Code 200).
        /// </summary>
        bool WasHttpRequestSuccessful { get; set; }

        /// <summary>
        /// Returns <c>true</c> if the request was successfully handled by the server and no error parameters are found in the result.
        /// </summary>
        bool WasRequestSuccessfullyHandled { get; set; }

        /// <summary>
        /// Returns a string with the reason for the error if <see cref="WasSuccessful"/> is <c>false</c>.
        /// </summary>
        string Reason { get; set; }

        /// <summary>
        /// Returns the <see cref="HttpStatusCode"/> of the request.
        /// </summary>
        HttpStatusCode HttpStatusCode { get; set; }

        /// <summary>
        /// Returns the error code if one was found in the error object when the server is unable to handle the request.
        /// </summary>
        int ServerErrorCode { get; set; }

        /// <summary>
        /// The error data, if applicable.
        /// </summary>
        Dictionary<string, object> ErrorData { get; set; }

        /// <summary>
        /// Contains the JSON RPC request payload
        /// </summary>
        string RawRpcRequest { get; }

        /// <summary>
        /// Contains the JSON RPC response payload
        /// </summary>
        string RawRpcResponse { get; }

    }

}