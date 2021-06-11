using System.Net;
using System.Net.Http;

namespace Solnet.Rpc.Core.Http
{
    public class RequestResult<T>
    {

        public bool WasSuccessful { get => WasHttpRequestSuccessful && WasRequestSuccessfullyHandled; }

        public bool WasHttpRequestSuccessful { get; }

        public bool WasRequestSuccessfullyHandled { get; internal set; }

        public string Reason { get; internal set; }

        public T Result { get; internal set; }

        public HttpStatusCode HttpStatusCode { get; }

        public int ServerErrorCode { get; internal set; }

        internal RequestResult(HttpResponseMessage resultMsg, T result = default(T))
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