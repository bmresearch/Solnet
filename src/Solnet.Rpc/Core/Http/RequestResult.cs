using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace Solnet.Rpc.Core.Http
{
    /// <summary>
    /// Represents the result of a given request.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public class RequestResult<T>
    {
        /// <summary>
        /// Returns <c>true</c> if the request was successfully handled and parsed.
        /// </summary>
        public bool WasSuccessful => WasHttpRequestSuccessful && WasRequestSuccessfullyHandled;

        /// <summary>
        /// Returns <c>true</c> if the HTTP request was successful (e.g. Code 200).
        /// </summary>
        public bool WasHttpRequestSuccessful { get; }

        /// <summary>
        /// Returns <c>true</c> if the request was successfully handled by the server and no error parameters are found in the result.
        /// </summary>
        public bool WasRequestSuccessfullyHandled { get; set; }

        /// <summary>
        /// Returns a string with the reason for the error if <see cref="WasSuccessful"/> is <c>false</c>.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Returns the actual result of the request if <see cref="WasSuccessful"/> is <c>true</c>.
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// Returns the <see cref="HttpStatusCode"/> of the request.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; }

        /// <summary>
        /// Returns the error code if one was found in the error object when the server is unable to handle the request.
        /// </summary>
        public int ServerErrorCode { get; set; }
        
        /// <summary>
        /// Initialize the request result.
        /// <param name="resultMsg">An http request result.</param>
        /// <param name="result">The type of the request result.</param>
        /// </summary>
        public RequestResult(HttpResponseMessage resultMsg, T result = default(T))
        {
            HttpStatusCode = resultMsg.StatusCode;
            WasHttpRequestSuccessful = resultMsg.IsSuccessStatusCode;
            Reason = resultMsg.ReasonPhrase;
            Result = result;
        }

        internal RequestResult(HttpStatusCode code, string reason)
        {
            HttpStatusCode = code;
            Reason = reason;
            WasHttpRequestSuccessful = false;
        }
    }
}