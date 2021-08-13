using Solnet.Rpc;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Extensions
{
    internal class TokenWalletRpcProxy : ITokenWalletRpcProxy
    {

        private IRpcClient _client;

        internal TokenWalletRpcProxy(IRpcClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<RequestResult<ResponseValue<ulong>>> GetBalanceAsync(string pubKey, Commitment commitment = Commitment.Finalized)
        {
            return await _client.GetBalanceAsync(pubKey, commitment);
        }

        public async Task<RequestResult<ResponseValue<BlockHash>>> GetRecentBlockHashAsync(Commitment commitment = Commitment.Finalized)
        {
            return await _client.GetRecentBlockHashAsync(commitment);
        }

        public async Task<RequestResult<ResponseValue<List<TokenAccount>>>> GetTokenAccountsByOwnerAsync(string ownerPubKey, string tokenMintPubKey = null, string tokenProgramId = null, Commitment commitment = Commitment.Finalized)
        {
            return await _client.GetTokenAccountsByOwnerAsync(ownerPubKey, tokenMintPubKey, tokenProgramId, commitment);
        }

        public async Task<RequestResult<string>> SendTransactionAsync(byte[] transaction, bool skipPreflight = false, Commitment commitment = Commitment.Finalized)
        {
            return await _client.SendTransactionAsync(transaction, skipPreflight, commitment);
        }
    }
}
