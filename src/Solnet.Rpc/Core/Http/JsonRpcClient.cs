using Solnet.Rpc.Messages;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solnet.Rpc.Core.Http
{
    public abstract class JsonRpcClient
    {
        private readonly JsonSerializerOptions _serializerOptions;

        private readonly HttpClient _httpClient;

        protected JsonRpcClient(string url, HttpClient httpClient = default)
        {
            _httpClient = httpClient ?? new HttpClient { BaseAddress = new Uri(url) };
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
        }

        protected async Task<RequestResult<T>> SendRequest<T>(JsonRpcRequest req)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/", req, _serializerOptions);

            var tmp = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Result:\n" + tmp );

            RequestResult<T> result = new RequestResult<T>(response);
            if (result.WasSuccessful)
            {
                var res = await response.Content.ReadFromJsonAsync<JsonRpcResponse<T>>(_serializerOptions);
                result.Result = res.Result;
            }
            return result;
        }
    }
}
