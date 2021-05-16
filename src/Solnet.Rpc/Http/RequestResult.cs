using System.Net;
using System.Net.Http;

namespace Solnet.Rpc.Http
{
    public class RequestResult<T>
    {

        public bool WasSuccessful { get; }

        public string Reason { get; }

        public T Result { get; internal set; }

        public HttpStatusCode StatusCode { get; }

        internal RequestResult(HttpResponseMessage resultMsg, T result = default(T))
        {
            StatusCode = resultMsg.StatusCode;
            WasSuccessful = resultMsg.IsSuccessStatusCode;
            Reason = resultMsg.ReasonPhrase;
            Result = result;
        }
    }
}
