using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solnet.Extensions.Test
{
    internal class MockTokenWalletRpc : ITokenWalletRpcProxy
    {

        /// <summary>
        /// Queue of RPC responses
        /// </summary>
        private Queue<string> _responses;

        /// <summary>
        /// The request sequence number
        /// </summary>
        private int _reqSeqId;

        /// <summary>
        /// The Json serializer options to be reused between calls.
        /// </summary>
        private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// Constructor for the MockWalletRpc
        /// </summary>
        public MockTokenWalletRpc()
        {
            _reqSeqId = 0;
            _responses = new Queue<string>();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
        }


        public void Add(string resp)
        {
            _responses.Enqueue(resp);
        }

        
        private async Task<RequestResult<ResponseValue<T>>> MockResult<T>()
        {
            // make sure we've got a queued response 
            if (_responses.Count == 0) throw new ApplicationException("Mock responses exhausted");

            // thread-safe increment response id + dequeue
            int id = 0;
            string json = null;
            lock (this)
            {
                _reqSeqId++;
                id = _reqSeqId;
                json = _responses.Dequeue();
            }

            // deserialize JSON RPC response
            var result = JsonSerializer.Deserialize<JsonRpcResponse<ResponseValue<T>>>(json, _serializerOptions);
            if (result == null) throw new ApplicationException("Mock response did not deserialize");

            // overwrite response id
            result.Id = id;

            // package as a successful response
            var reqResult = new RequestResult<ResponseValue<T>>();
            reqResult.WasHttpRequestSuccessful = true;
            reqResult.WasRequestSuccessfullyHandled = true;
            reqResult.HttpStatusCode = HttpStatusCode.OK;
            reqResult.Result = result.Result;
            return await Task.FromResult<RequestResult<ResponseValue<T>>>(reqResult);

        }

        public async Task<RequestResult<ResponseValue<ulong>>> GetBalanceAsync(string pubKey, Commitment commitment = Commitment.Finalized)
        {
            return await MockResult<ulong>();
        }

        public async Task<RequestResult<ResponseValue<BlockHash>>> GetRecentBlockHashAsync(Commitment commitment = Commitment.Finalized)
        {
            return await MockResult<BlockHash>();
        }

        public async Task<RequestResult<ResponseValue<List<TokenAccount>>>> GetTokenAccountsByOwnerAsync(string ownerPubKey, string tokenMintPubKey = null, string tokenProgramId = null, Commitment commitment = Commitment.Finalized)
        {
            return await MockResult<List<TokenAccount>>();
        }

        public async Task<RequestResult<string>> SendTransactionAsync(byte[] transaction, bool skipPreflight = false, Commitment commitment = Commitment.Finalized)
        {
            var mocked = await MockResult<string>();

            var reqResult = new RequestResult<string>();
            reqResult.HttpStatusCode = HttpStatusCode.OK;
            reqResult.Result = mocked.Result.Value;

            return await Task.FromResult<RequestResult<string>>(reqResult);

        }
    }
}
