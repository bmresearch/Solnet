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
    /// This interface contains the subset of methods from RPC client used by TokenWallet.
    /// </summary>
    public interface ITokenWalletRpcProxy
    {

        /// <summary>
        /// Gets the balance <b>asynchronously</b> for a certain public key.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// </summary>
        /// <param name="pubKey">The public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>A task which may return a request result holding the context and address balance.</returns>
        Task<RequestResult<ResponseValue<ulong>>> GetBalanceAsync(string pubKey, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets all SPL Token accounts by token owner.
        /// </summary>
        /// <param name="ownerPubKey">Public key of account owner query, as base-58 encoded string.</param>
        /// <param name="tokenMintPubKey">Public key of the specific token Mint to limit accounts to, as base-58 encoded string.</param>
        /// <param name="tokenProgramId">Public key of the Token program ID that owns the accounts, as base-58 encoded string.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<List<TokenAccount>>>> GetTokenAccountsByOwnerAsync(
            string ownerPubKey, string tokenMintPubKey = null, string tokenProgramId = null, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets a recent block hash.
        /// </summary>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<BlockHash>>> GetRecentBlockHashAsync(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Sends a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as byte array.</param>
        /// <param name="skipPreflight">If true skip the prflight transaction checks (default false).</param>
        /// <param name="commitment">The block commitment used to retrieve block hashes and verify success.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<string>> SendTransactionAsync(byte[] transaction, bool skipPreflight = false,
             Commitment commitment = Commitment.Finalized);

    }
}