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
    public class SolanaRpcBatchWithAsyncs
    {

        public SolanaRpcBatchWithAsyncs()
        {
            _composer = new SolanaRpcBatchComposer();
        }

        /// <summary>
        /// A batch composer instance
        /// </summary>
        private SolanaRpcBatchComposer _composer;

        /// <summary>
        /// How many requests are in this batch
        /// </summary>
        public SolanaRpcBatchComposer Composer => _composer;


        #region RPC Methods

        // this is a sample set of methods with different response types
        // TODO - add more methods 

        public async Task<ResponseValue<ulong>> GetBalanceAsync(string pubKey, Commitment commitment = Commitment.Finalized)
        {
            var parameters = Parameters.Create(
                    pubKey, ConfigObject.Create(HandleCommitment(commitment)));
            return await _composer.AddRequest<ResponseValue<ulong>>("getBalance", parameters);
        }

        public async Task<ResponseValue<List<TokenAccount>>> GetTokenAccountsByOwnerAsync(string ownerPubKey,
                                            string tokenMintPubKey = null,
                                            string tokenProgramId = null,
                                            Commitment commitment = Commitment.Finalized)
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

            return await _composer.AddRequest<ResponseValue<List<TokenAccount>>>("getTokenAccountsByOwner", parameters);
        }

        public async Task<List<SignatureStatusInfo>> GetConfirmedSignaturesForAddress2Async(string accountPubKey, ulong limit = 1000,
                                                     string before = null, string until = null,
                                                     Commitment commitment = Commitment.Finalized)
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

            return await _composer.AddRequest<List<SignatureStatusInfo>>("getConfirmedSignaturesForAddress2", parameters);
        }

        public async Task<List<AccountKeyPair>> GetProgramAccountsAsync(string pubKey, Commitment commitment = Commitment.Finalized,
                                       int? dataSize = null, IList<MemCmp> memCmpList = null,
                                       Action<List<AccountKeyPair>> callback = null)
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

            return await _composer.AddRequest<List<AccountKeyPair>>("getProgramAccounts", parameters);

        }

        public async Task<TransactionMetaSlotInfo> GetTransactionAsync(string signature,
                                        Commitment commitment = Commitment.Finalized)
        {
            var parameters = Parameters.Create(
                    signature,
                    ConfigObject.Create(
                        KeyValue.Create("encoding", "json"),
                        HandleCommitment(commitment)));

            return await _composer.AddRequest<TransactionMetaSlotInfo>("getTransaction", parameters);

        }

        #endregion

        private KeyValue HandleCommitment(Commitment parameter, Commitment defaultValue = Commitment.Finalized)
            => parameter != defaultValue ? KeyValue.Create("commitment", parameter) : null;


    }

}
