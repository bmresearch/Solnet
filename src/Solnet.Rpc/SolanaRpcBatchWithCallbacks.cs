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
            _composer.Flush();
        }

        #region RPC Methods

        // this is a sample set of methods with different response types

        public void GetBalance(string pubKey, Commitment commitment = Commitment.Finalized,
                               Action<ResponseValue<ulong>, Exception> callback = null)
        {
            var parameters = Parameters.Create(pubKey, ConfigObject.Create(HandleCommitment(commitment)));
            _composer.AddRequest<ResponseValue<ulong>>("getBalance", parameters, callback);
        }

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

            _composer.AddRequest<ResponseValue<List<TokenAccount>>>("getTokenAccountsByOwner", parameters, callback);
   
        }

        public void GetConfirmedSignaturesForAddress2(string accountPubKey, ulong limit = 1000,
                                                      string before = null, string until = null,
                                                      Commitment commitment = Commitment.Finalized,
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

            _composer.AddRequest<List<SignatureStatusInfo>>("getConfirmedSignaturesForAddress2", parameters, callback);
        }

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

            _composer.AddRequest<List<AccountKeyPair>>("getProgramAccounts", parameters, callback);

        }

        public void GetTransaction(string signature,
                                   Commitment commitment = Commitment.Finalized,
                                   Action<TransactionMetaSlotInfo, Exception> callback = null)
        {
            var parameters = Parameters.Create(
                    signature,
                    ConfigObject.Create(
                        KeyValue.Create("encoding", "json"),
                        HandleCommitment(commitment)));

            _composer.AddRequest<TransactionMetaSlotInfo>("getTransaction", parameters, callback);

        }

        #endregion

        private KeyValue HandleCommitment(Commitment parameter, Commitment defaultValue = Commitment.Finalized)
            => parameter != defaultValue ? KeyValue.Create("commitment", parameter) : null;

    }
}
