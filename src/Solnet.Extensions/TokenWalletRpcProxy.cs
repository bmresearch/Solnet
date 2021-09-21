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
    /// <summary>
    /// An internal RPC proxy that has everything required by TokenWallet.
    /// </summary>
    internal class TokenWalletRpcProxy : ITokenWalletRpcProxy
    {

        /// <summary>
        /// The RPC client to use.
        /// </summary>
        private IRpcClient _client;

        /// <summary>
        /// Constructs an instance of the TokenWalletRpcProxy.
        /// </summary>
        /// <param name="client"></param>
        internal TokenWalletRpcProxy(IRpcClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// Gets the balance <b>asynchronously</b> for a certain public key.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// </summary>
        /// <param name="pubKey">The public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>A task which may return a request result holding the context and address balance.</returns>
        public async Task<RequestResult<ResponseValue<ulong>>> GetBalanceAsync(string pubKey, Commitment commitment = Commitment.Finalized)
        {
            return await _client.GetBalanceAsync(pubKey, commitment);
        }

        /// <summary>
        /// Gets a recent block hash.
        /// </summary>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        public async Task<RequestResult<ResponseValue<BlockHash>>> GetRecentBlockHashAsync(Commitment commitment = Commitment.Finalized)
        {
            return await _client.GetRecentBlockHashAsync(commitment);
        }

        /// <summary>
        /// Gets all SPL Token accounts by token owner.
        /// </summary>
        /// <param name="ownerPubKey">Public key of account owner query, as base-58 encoded string.</param>
        /// <param name="tokenMintPubKey">Public key of the specific token Mint to limit accounts to, as base-58 encoded string.</param>
        /// <param name="tokenProgramId">Public key of the Token program ID that owns the accounts, as base-58 encoded string.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        public async Task<RequestResult<ResponseValue<List<TokenAccount>>>> GetTokenAccountsByOwnerAsync(string ownerPubKey, string tokenMintPubKey = null, string tokenProgramId = null, Commitment commitment = Commitment.Finalized)
        {
            return await _client.GetTokenAccountsByOwnerAsync(ownerPubKey, tokenMintPubKey, tokenProgramId, commitment);
        }

        /// <summary>
        /// Sends a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as byte array.</param>
        /// <param name="skipPreflight">If true skip the prflight transaction checks (default false).</param>
        /// <param name="commitment">The block commitment used to retrieve block hashes and verify success.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        public async Task<RequestResult<string>> SendTransactionAsync(byte[] transaction, bool skipPreflight = false, Commitment commitment = Commitment.Finalized)
        {
            return await _client.SendTransactionAsync(transaction, skipPreflight, commitment);
        }
    }
}