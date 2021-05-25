using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Solnet.Rpc.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;

namespace Solnet.Rpc
{
    public class SolanaRpcClient : JsonRpcClient
    {
        private int _id;
        
        private int GetNextId()
        {
            lock (this)
            {
                return _id++;
            }
        }
        
        public SolanaRpcClient(string url) : base(url)
        {
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
                configurationObject = new Dictionary<string, object>
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