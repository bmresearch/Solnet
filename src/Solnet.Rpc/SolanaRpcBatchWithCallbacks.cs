using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Rpc
{
    /// <summary>
    /// This object is used to create a batch of RPC requests that can be executed as a single call to the RPC endpoint.
    /// Use of batches can have give a significant performance improvement instead of making multiple requests.
    /// The execution of batches can be controlled manually via the Flush method, or can be invoked automatically using auto-execute mode.
    /// Auto-execute mode is useful when iterating through large worksets.
    /// </summary>
    public class SolanaRpcBatchWithCallbacks
    {

        /// <summary>
        /// Constructs a new SolanaRpcBatchWithCallbacks instance
        /// </summary>
        /// <param name="rpcClient">An RPC client</param>
        public SolanaRpcBatchWithCallbacks(IRpcClient rpcClient)
        {
            _composer = new SolanaRpcBatchComposer(rpcClient);
        }

        /// <summary>
        /// A batch composer instance
        /// </summary>
        private SolanaRpcBatchComposer _composer;

        /// <summary>
        /// How many requests are in this batch
        /// </summary>
        public SolanaRpcBatchComposer Composer => _composer;

        /// <summary>
        /// Sets the auto execute mode and trigger threshold
        /// </summary>
        /// <param name="mode">The auto execute mode to use.</param>
        /// <param name="batchSizeTrigger">The number of requests that will trigger a batch execution.</param>
        public void AutoExecute(BatchAutoExecuteMode mode, int batchSizeTrigger)
        {
            _composer.AutoExecute(mode, batchSizeTrigger);
        }

        /// <summary>
        /// Used to execute any requests remaining in the batch.
        /// </summary>
        public void Flush()
        {
            if (_composer.Count > 0)
                _composer.Flush();
        }

        #region RPC Methods

        /// <summary>
        /// Gets the balance for a certain public key.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// </summary>
        /// <param name="pubKey">The public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="callback">The callback to handle the result.</param>
        public void GetBalance(string pubKey, Commitment commitment = Commitment.Finalized,
                               Action<ResponseValue<ulong>, Exception> callback = null)
        {
            var parameters = Parameters.Create(pubKey, ConfigObject.Create(HandleCommitment(commitment)));
            _composer.AddRequest("getBalance", parameters, callback);
        }

        /// <summary>
        /// Gets all SPL Token accounts by token owner.
        /// </summary>
        /// <param name="ownerPubKey">Public key of account owner query, as base-58 encoded string.</param>
        /// <param name="tokenMintPubKey">Public key of the specific token Mint to limit accounts to, as base-58 encoded string.</param>
        /// <param name="tokenProgramId">Public key of the Token program ID that owns the accounts, as base-58 encoded string.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="callback">The callback to handle the result.</param>
        public void GetTokenAccountsByOwner(string ownerPubKey, string tokenMintPubKey = null,
                                            string tokenProgramId = null, Commitment commitment = Commitment.Finalized,
                                            Action<ResponseValue<List<TokenAccount>>, Exception> callback = null)
        {
            if (string.IsNullOrWhiteSpace(tokenMintPubKey) && string.IsNullOrWhiteSpace(tokenProgramId))
                throw new ArgumentException("either tokenProgramId or tokenMintPubKey must be set");

            var parameters = Parameters.Create(
                    ownerPubKey,
                    ConfigObject.Create(
                        KeyValue.Create("mint", tokenMintPubKey),
                        KeyValue.Create("programId", tokenProgramId)),
                    ConfigObject.Create(
                        HandleCommitment(commitment),
                        KeyValue.Create("encoding", "jsonParsed")));

            _composer.AddRequest("getTokenAccountsByOwner", parameters, callback);

        }


        /// <summary>
        /// Gets signatures with the given commitment for transactions involving the address.
        /// <remarks>
        /// Unless <c>searchTransactionHistory</c> is included, this method only searches the recent status cache of signatures.
        /// </remarks>
        /// </summary>
        /// <param name="accountPubKey">The account address as base-58 encoded string.</param>
        /// <param name="limit">Maximum transaction signatures to return, between 1-1000. Default is 1000.</param>
        /// <param name="before">Start searching backwards from this transaction signature.</param>
        /// <param name="until">Search until this transaction signature, if found before limit is reached.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="callback">The callback to handle the result.</param>
        public void GetSignaturesForAddress(string accountPubKey, ulong limit = 1000,
            string before = null, string until = null, Commitment commitment = Commitment.Finalized,
            Action<List<SignatureStatusInfo>, Exception> callback = null)
        {
            if (commitment == Commitment.Processed)
                throw new ArgumentException("Commitment.Processed is not supported for this method.");

            var parameters = Parameters.Create(
                    accountPubKey,
                    ConfigObject.Create(
                        KeyValue.Create("limit", limit != 1000 ? limit : null),
                        KeyValue.Create("before", before),
                        KeyValue.Create("until", until),
                        HandleCommitment(commitment)));

            _composer.AddRequest("getSignaturesForAddress", parameters, callback);
        }

        /// <summary>
        /// Returns all accounts owned by the provided program Pubkey.
        /// <remarks>Accounts must meet all filter criteria to be included in the results.</remarks>
        /// </summary>
        /// <param name="pubKey">The program public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="dataSize">The data size of the account to compare against the program account data.</param>
        /// <param name="memCmpList">The list of comparisons to match against the program account data.</param>
        /// <param name="callback">The callback to handle the result.</param>
        public void GetProgramAccounts(string pubKey, Commitment commitment = Commitment.Finalized,
                                       int? dataSize = null, IList<MemCmp> memCmpList = null,
                                       Action<List<AccountKeyPair>, Exception> callback = null)
        {
            List<object> filters = Parameters.Create(ConfigObject.Create(KeyValue.Create("dataSize", dataSize)));
            if (memCmpList != null)
            {
                filters ??= new List<object>();
                filters.AddRange(memCmpList.Select(filter => ConfigObject.Create(KeyValue.Create("memcmp",
                    ConfigObject.Create(KeyValue.Create("offset", filter.Offset),
                        KeyValue.Create("bytes", filter.Bytes))))));
            }

            var parameters = Parameters.Create(
                    pubKey,
                    ConfigObject.Create(
                        KeyValue.Create("encoding", "base64"),
                        KeyValue.Create("filters", filters),
                        HandleCommitment(commitment)));

            _composer.AddRequest("getProgramAccounts", parameters, callback);

        }


        /// <summary>
        /// Returns transaction details for a confirmed transaction.
        /// <remarks>
        /// <para>
        /// The <c>commitment</c> parameter is optional, <see cref="Commitment.Processed"/> is not supported,
        /// the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </para>
        /// </remarks>
        /// </summary>
        /// <param name="signature">Transaction signature as base-58 encoded string.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="callback">The callback to handle the result.</param>
        public void GetTransaction(string signature,
                                   Commitment commitment = Commitment.Finalized,
                                   Action<TransactionMetaSlotInfo, Exception> callback = null)
        {
            var parameters = Parameters.Create(
                    signature,
                    ConfigObject.Create(
                        KeyValue.Create("encoding", "json"),
                        HandleCommitment(commitment)));

            _composer.AddRequest("getTransaction", parameters, callback);

        }

        /// <summary>
        /// Gets the account info.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// </summary>
        /// <param name="pubKey">The account public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="encoding">The encoding of the account data.</param>
        /// <param name="callback">The callback to handle the result.</param>
        public void GetAccountInfo(string pubKey, Commitment commitment = Commitment.Finalized,
            BinaryEncoding encoding = BinaryEncoding.Base64, Action<ResponseValue<AccountInfo>, Exception> callback = null)
        {
            var parameters = Parameters.Create(
                    pubKey,
                    ConfigObject.Create(
                        KeyValue.Create("encoding", encoding),
                        HandleCommitment(commitment)));
            _composer.AddRequest("getAccountInfo", parameters, callback);
        }

        /// <summary>
        /// Gets the token mint info. This method only works if the target account is a SPL token mint.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// </summary>
        /// <param name="pubKey">The token mint public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="callback">The callback to handle the result.</param>
        public void GetTokenMintInfo(string pubKey, Commitment commitment = Commitment.Finalized, Action<ResponseValue<TokenMintInfo>, Exception> callback = null)
        {
            var parameters = Parameters.Create(
                    pubKey,
                    ConfigObject.Create(
                        KeyValue.Create("encoding", "jsonParsed"),
                        HandleCommitment(commitment)));
            _composer.AddRequest("getAccountInfo", parameters, callback);
        }

        /// <summary>
        /// Gets the 20 largest token accounts of a particular SPL Token.
        /// </summary>
        /// <param name="tokenMintPubKey">Public key of Token Mint to query, as base-58 encoded string.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="callback">The callback to handle the result.</param>
        public void GetTokenLargestAccounts(string tokenMintPubKey, Commitment commitment = Commitment.Finalized,
            Action<ResponseValue<List<LargeTokenAccount>>, Exception> callback = null)
        {
            var parameters = Parameters.Create(tokenMintPubKey, ConfigObject.Create(HandleCommitment(commitment)));

            _composer.AddRequest("getTokenLargestAccounts", parameters, callback);
        }

        /// <summary>
        /// Sends a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as byte array.</param>
        /// <param name="skipPreflight">If true skip the prflight transaction checks (default false).</param>
        /// <param name="preflightCommitment">The block commitment used to retrieve block hashes and verify success.</param>
        /// <param name="callback">The callback to handle the result.</param>
        public void SendTransaction(byte[] transaction,
                            bool skipPreflight = false,
                            Commitment preflightCommitment = Commitment.Finalized,
                            Action<string, Exception> callback = null) =>
            SendTransaction(Convert.ToBase64String(transaction), skipPreflight, preflightCommitment, callback);


        /// <summary>
        /// Sends a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as base-64 encoded string.</param>
        /// <param name="skipPreflight">If true skip the prflight transaction checks (default false).</param>
        /// <param name="preflightCommitment">The block commitment used for preflight.</param>
        /// <param name="callback">The callback to handle the result.</param>
        public void SendTransaction(string transaction,
                                    bool skipPreflight = false,
                                    Commitment preflightCommitment = Commitment.Finalized,
                                    Action<string, Exception> callback = null)
        {
            var parameters = Parameters.Create(
                  transaction,
                  ConfigObject.Create(
                        KeyValue.Create("skipPreflight", skipPreflight ? skipPreflight : null),
                        KeyValue.Create("preflightCommitment",
                            preflightCommitment == Commitment.Finalized ? null : preflightCommitment),
                        KeyValue.Create("encoding", BinaryEncoding.Base64)));

            _composer.AddRequest("sendTransaction", parameters, callback);
        }


        /// <summary>
        /// Gets the account info for multiple accounts.
        /// </summary>
        /// <param name="accounts">The list of the accounts public keys.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="callback">The callback to handle the result.</param>
        public void GetMultipleAccounts(IList<string> accounts,
                                Commitment commitment = Commitment.Finalized,
                                Action<ResponseValue<List<AccountInfo>>, Exception> callback = null)
        {
            var parameters = Parameters.Create(
                    accounts,
                    ConfigObject.Create(
                        KeyValue.Create("encoding", "base64"),
                        HandleCommitment(commitment)));

            _composer.AddRequest("getMultipleAccounts", parameters, callback);

        }


        /// <summary>
        /// Gets the token account info.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// </summary>
        /// <param name="pubKey">The token account public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="callback">The callback to handle the result.</param>
        public void GetTokenAccountInfo(string pubKey,
                                        Commitment commitment = Commitment.Finalized,
                                        Action<ResponseValue<TokenAccountInfo>, Exception> callback = null)
        {
            var parameters = Parameters.Create(
                    pubKey,
                    ConfigObject.Create(
                        KeyValue.Create("encoding", "jsonParsed"),
                        HandleCommitment(commitment)));

            _composer.AddRequest("getAccountInfo", parameters, callback);

        }
        #endregion

        private KeyValue HandleCommitment(Commitment parameter, Commitment defaultValue = Commitment.Finalized)
            => parameter != defaultValue ? KeyValue.Create("commitment", parameter) : null;

    }
}
