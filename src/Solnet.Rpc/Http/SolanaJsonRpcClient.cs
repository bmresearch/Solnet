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
    public class SolanaJsonRpcClient
    {
        private int _id;

        private JsonSerializerOptions _serializerOptions;

        private HttpClient _httpClient;

        private int GetNextId()
        {
            lock (this)
            {
                return _id++;
            }
        }
        //https://api.devnet.solana.com
        //https://testnet.solana.com

        public SolanaJsonRpcClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.mainnet-beta.solana.com");
            _serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        }

        private async Task<RequestResult<T>> SendRequest<T>(JsonRpcRequest req)
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

        #region RequestBuilder
        private JsonRpcRequest BuildRequest<T>(string method, IList<object> parameters)
            => new JsonRpcRequest(GetNextId(), method, parameters);

        private async Task<RequestResult<T>> SendRequestAsync<T>(string method)
        {
            var req = BuildRequest<T>(method, null);
            return await SendRequest<T>(req);
        }

        private async Task<RequestResult<T>> SendRequestAsync<T>(string method, IList<object> parameters)
        {
            var req = BuildRequest<T>(method, parameters.ToList());
            return await SendRequest<T>(req);
        }

        private async Task<RequestResult<T>> SendRequestAsync<T>(string method, IList<object> parameters, Dictionary<string, object> configurationObject)
        {
            if (configurationObject == null)
            {
                configurationObject = new Dictionary<string, object>()
                {
                    { "encoding" , "jsonParsed" }
                };
            }

            var newList = parameters.ToList();
            newList.Add(configurationObject);

            var req = BuildRequest<T>(method, newList);
            return await SendRequest<T>(req);
        }

        #endregion

        public async Task<RequestResult<ResponseValue<AccountInfo>>> GetAccountInfoAsync(string pubKey)
        {
            return await SendRequestAsync<ResponseValue<AccountInfo>>("getAccountInfo", new List<object>() { pubKey }, null);
        }

        public RequestResult<ResponseValue<AccountInfo>> GetAccountInfo(string pubkey) => GetAccountInfoAsync(pubkey).Result;



        public async Task<RequestResult<string>> GetGenesisHashAsync()
        {
            return await SendRequestAsync<string>("getGenesisHash");
        }

        public RequestResult<string> GetGenesisHash() => GetGenesisHashAsync().Result;


        public async Task<RequestResult<ResponseValue<ulong>>> GetBalanceAsync(string pubKey)
        {
            return await SendRequestAsync<ResponseValue<ulong>>("getBalance", new List<object>() { pubKey });
        }

        public RequestResult<ResponseValue<ulong>> GetBalance(string pubkey) => GetBalanceAsync(pubkey).Result;

        public async Task<RequestResult<BlockCommitment>> GetBlockCommitmentAsync(ulong block)
        {
            return await SendRequestAsync<BlockCommitment>("getBlockCommitment", new List<object>() { block });
        }

        public RequestResult<BlockCommitment> GetBlockCommitment(ulong block) => GetBlockCommitmentAsync(block).Result;


        public async Task<RequestResult<ulong>> GetBlockTimeAsync(ulong block)
        {
            return await SendRequestAsync<ulong>("getBlockTime", new List<object>() { block });
        }

        public RequestResult<ulong> GetBlockTime(ulong block) => GetBlockTimeAsync(block).Result;


    }
}
