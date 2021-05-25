using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Solnet.Rpc.Http
{
    public abstract class JsonRpcClient
    {
        private readonly JsonSerializerOptions _serializerOptions;

        private readonly HttpClient _httpClient;

        //https://api.devnet.solana.com
        //https://testnet.solana.com

        protected JsonRpcClient(string url)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(url) };
            _serializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
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
