using Microsoft.Extensions.Logging;
using Solnet.Rpc.Core;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Solnet.Rpc
{
    /// <summary>
    /// Implements functionality to interact with the Solana JSON RPC API.
    public class SolanaRpcClient : JsonRpcClient, IRpcClient
    {
        /// <summary>
        /// Message Id generator.
        /// </summary>
        private readonly IdGenerator _idGenerator = new IdGenerator();

        /// <summary>
        /// Initialize the Rpc Client with the passed url.
        /// </summary>
        /// <param name="url">The url of the node exposing the JSON RPC API.</param>
        /// <param name="logger">The logger to use.</param>
        /// <param name="httpClient">An http client.</param>
        internal SolanaRpcClient(string url, ILogger logger, HttpClient httpClient = default) : base(url, logger,
            httpClient)
        {
        }

        #region RequestBuilder

        /// <summary>
        /// Build the request for the passed RPC method and parameters.
        /// </summary>
        /// <param name="method">The request's RPC method.</param>
        /// <param name="parameters">A list of parameters to include in the request.</param>
        /// <typeparam name="T">The type of the request result.</typeparam>
        /// <returns>A task which may return a request result.</returns>
        private JsonRpcRequest BuildRequest<T>(string method, IList<object> parameters)
            => new JsonRpcRequest(_idGenerator.GetNextId(), method, parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method">The request's RPC method.</param>
        /// <typeparam name="T">The type of the request result.</typeparam>
        /// <returns>A task which may return a request result.</returns>
        private async Task<RequestResult<T>> SendRequestAsync<T>(string method)
        {
            var req = BuildRequest<T>(method, null);
            return await SendRequest<T>(req);
        }

        /// <summary>
        /// Send a request asynchronously.
        /// </summary>
        /// <param name="method">The request's RPC method.</param>
        /// <param name="parameters">A list of parameters to include in the request.</param>
        /// <typeparam name="T">The type of the request result.</typeparam>
        /// <returns>A task which may return a request result.</returns>
        private async Task<RequestResult<T>> SendRequestAsync<T>(string method, IList<object> parameters)
        {
            var req = BuildRequest<T>(method, parameters);
            return await SendRequest<T>(req);
        }

        /// <summary>
        /// Send the request asynchronously.
        /// </summary>
        /// <param name="method">The request's RPC method.</param>
        /// <param name="parameters">A list of parameters to include in the request.</param>
        /// <param name="configurationObject">Optional parameters to include in the request.</param>
        /// <typeparam name="T">The type of the request result.</typeparam>
        /// <returns>A task which may return a request result.</returns>
        private async Task<RequestResult<T>> SendRequestAsync<T>(
            string method, IList<object> parameters, Dictionary<string, object> configurationObject
        )
        {
            var newList = parameters.ToList();

            if (configurationObject == null)
            {
                configurationObject = new Dictionary<string, object>
                {
                    {"encoding", "jsonParsed"}
                };
            }

            foreach (var key in configurationObject.Keys)
            {
                var ok = configurationObject.TryGetValue(key, out var value);
                if (!ok) continue;

                newList.Add(new Dictionary<string, object>
                {
                    {key, value}
                });
            }

            var req = BuildRequest<T>(method, newList);
            return await SendRequest<T>(req);
        }

        #endregion

        #region Accounts
        /// <inheritdoc cref="IRpcClient.GetAccountInfoAsync(string,Commitment)"/>
        public async Task<RequestResult<ResponseValue<AccountInfo>>> GetAccountInfoAsync(string pubKey, Commitment commitment = Commitment.Finalized)
        {
            var configParams = new Dictionary<string, object> { { "encoding", "base64" } };

            // given that finalized is the default, lets not add it to speed things up
            if (commitment != Commitment.Finalized)
            {
                configParams.Add("commitment", commitment);
            }

            return await SendRequestAsync<ResponseValue<AccountInfo>>("getAccountInfo", new List<object> { pubKey, configParams });
        }

        /// <inheritdoc cref="IRpcClient.GetAccountInfo(string,Commitment)"/>
        public RequestResult<ResponseValue<AccountInfo>> GetAccountInfo(string pubKey, Commitment commitment = Commitment.Finalized)
            => GetAccountInfoAsync(pubKey, commitment).Result;


        /// <inheritdoc cref="IRpcClient.GetProgramAccountsAsync(string)"/>
        public async Task<RequestResult<List<AccountKeyPair>>> GetProgramAccountsAsync(string pubKey)
        {
            return await SendRequestAsync<List<AccountKeyPair>>("getProgramAccounts",
                new List<object> { pubKey },
                new Dictionary<string, object>
                {
                    {
                        "encoding", "base64"
                    }
                });
        }

        /// <inheritdoc cref="IRpcClient.GetProgramAccounts(string)"/>
        public RequestResult<List<AccountKeyPair>> GetProgramAccounts(string pubKey)
            => GetProgramAccountsAsync(pubKey).Result;


        /// <inheritdoc cref="IRpcClient.GetMultipleAccountsAsync(IList{string})"/>
        public async Task<RequestResult<ResponseValue<List<AccountInfo>>>> GetMultipleAccountsAsync(IList<string> accounts)
        {
            return await SendRequestAsync<ResponseValue<List<AccountInfo>>>("getMultipleAccounts", new List<object> { accounts,
                new Dictionary<string, string> { { "encoding", "base64" } } });
        }

        /// <inheritdoc cref="IRpcClient.GetMultipleAccounts(IList{string})"/>
        public RequestResult<ResponseValue<List<AccountInfo>>> GetMultipleAccounts(IList<string> accounts)
            => GetMultipleAccountsAsync(accounts).Result;

        #endregion

        /// <inheritdoc cref="IRpcClient.GetBalanceAsync"/>
        public async Task<RequestResult<ResponseValue<ulong>>> GetBalanceAsync(string pubKey, Commitment commitment = Commitment.Finalized)
        {
            var paramsList = new List<object> { pubKey };

            if (commitment != Commitment.Finalized)
            {
                paramsList.Add(new Dictionary<string, Commitment> { { "commitment", commitment } });
            }

            return await SendRequestAsync<ResponseValue<ulong>>("getBalance", paramsList);
        }

        /// <inheritdoc cref="IRpcClient.GetBalance"/>
        public RequestResult<ResponseValue<ulong>> GetBalance(string pubKey, Commitment commitment = Commitment.Finalized)
            => GetBalanceAsync(pubKey, commitment).Result;

        #region Blocks
        /// <inheritdoc cref="IRpcClient.GetBlockAsync(ulong,Commitment)"/>
        public async Task<RequestResult<BlockInfo>> GetBlockAsync(ulong slot, Commitment commitment = Commitment.Finalized)
        {
            // transaction details default is aready full
            var configParams = new Dictionary<string, object> { { "encoding", "json" } };

            if (commitment == Commitment.Processed)
            {
                throw new ArgumentException("Commitment.Processed is not supported for this method.");
            }
            else if (commitment != Commitment.Finalized)
            {
                configParams.Add("commitment", commitment);
            }

            return await SendRequestAsync<BlockInfo>("getBlock", new List<object> { slot, configParams });
        }

        /// <inheritdoc cref="IRpcClient.GetBlock(ulong,Commitment)"/>
        public RequestResult<BlockInfo> GetBlock(ulong slot, Commitment commitment = Commitment.Finalized)
            => GetBlockAsync(slot, commitment).Result;


        /// <inheritdoc cref="IRpcClient.GetBlocksAsync(ulong, ulong, Commitment)"/>
        public async Task<RequestResult<List<ulong>>> GetBlocksAsync(ulong startSlot, ulong endSlot = 0, Commitment commitment = Commitment.Finalized)
        {
            var parameters = new List<object> { startSlot };
            if (endSlot > 0) parameters.Add(endSlot);

            if (commitment == Commitment.Processed)
            {
                throw new ArgumentException("Commitment.Processed is not supported for this method.");
            }
            else if (commitment != Commitment.Finalized)
            {
                parameters.Add(new Dictionary<string, Commitment> { { "commitment", commitment } });
            }

            return await SendRequestAsync<List<ulong>>("getBlocks", parameters);
        }

        /// <inheritdoc cref="IRpcClient.GetBlocks(ulong, ulong, Commitment)"/>
        public RequestResult<List<ulong>> GetBlocks(ulong startSlot, ulong endSlot = 0, Commitment commitment = Commitment.Finalized)
            => GetBlocksAsync(startSlot, endSlot, commitment).Result;


        /// <inheritdoc cref="IRpcClient.GetBlocksWithLimit(ulong, ulong, Commitment)"/>
        public RequestResult<List<ulong>> GetBlocksWithLimit(ulong startSlot, ulong limit, Commitment commitment = Commitment.Finalized)
            => GetBlocksWithLimitAsync(startSlot, limit, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetBlocksWithLimitAsync(ulong, ulong, Commitment)"/>
        public async Task<RequestResult<List<ulong>>> GetBlocksWithLimitAsync(ulong startSlot, ulong limit, Commitment commitment = Commitment.Finalized)
        {
            var parameters = new List<object> { startSlot, limit };

            if (commitment == Commitment.Processed)
            {
                throw new ArgumentException("Commitment.Processed is not supported for this method.");
            }
            else if (commitment != Commitment.Finalized)
            {
                parameters.Add(new Dictionary<string, Commitment> { { "commitment", commitment } });
            }

            return await SendRequestAsync<List<ulong>>("getBlocksWithLimit", parameters);
        }


        /// <inheritdoc cref="IRpcClient.GetFirstAvailableBlock()"/>
        public RequestResult<ulong> GetFirstAvailableBlock()
            => GetFirstAvailableBlockAsync().Result;

        /// <inheritdoc cref="IRpcClient.GetFirstAvailableBlock()"/>
        public async Task<RequestResult<ulong>> GetFirstAvailableBlockAsync()
        {
            return await SendRequestAsync<ulong>("getFirstAvailableBlock");
        }
        #endregion

        #region Block Production
        /// <inheritdoc cref="IRpcClient.GetBlockProductionAsync(string, ulong?, ulong?, Commitment)"/>
        public async Task<RequestResult<ResponseValue<BlockProductionInfo>>> GetBlockProductionAsync(string identity = null, ulong? firstSlot = null, ulong? lastSlot = null, Commitment commitment = Commitment.Finalized)
        {
            var parameters = new Dictionary<string, object>();

            if (commitment != Commitment.Finalized)
            {
                parameters.Add("commitment", commitment);
            }

            if (!string.IsNullOrEmpty(identity))
            {
                parameters.Add("identity", identity);
            }

            if (firstSlot.HasValue)
            {
                var range = new Dictionary<string, object> { { "firstSlot", firstSlot.Value } };

                if (lastSlot.HasValue)
                {
                    range.Add("lastSlot", lastSlot.Value);
                }
                parameters.Add("range", range);
            }
            else if (lastSlot.HasValue)
            {
                throw new ArgumentException("Range parameters are optional, but the lastSlot argument must be paired with a firstSlot.");
            }

            List<object> args = parameters.Count > 0 ? new List<object> { parameters } : null;

            return await SendRequestAsync<ResponseValue<BlockProductionInfo>>("getBlockProduction", args);
        }

        /// <inheritdoc cref="IRpcClient.GetBlockProduction(string, ulong?, ulong?, Commitment)"/>
        public RequestResult<ResponseValue<BlockProductionInfo>> GetBlockProduction(string identity = null, ulong? firstSlot = null, ulong? lastSlot = null, Commitment commitment = Commitment.Finalized)
            => GetBlockProductionAsync(identity, firstSlot, lastSlot, commitment).Result;
        #endregion

        /// <inheritdoc cref="IRpcClient.GetHealth()"/>
        public RequestResult<string> GetHealth()
            => GetHealthAsync().Result;

        /// <inheritdoc cref="IRpcClient.GetHealthAsync()"/>
        public async Task<RequestResult<string>> GetHealthAsync()
        {
            return await SendRequestAsync<string>("getHealth");
        }


        /// <inheritdoc cref="IRpcClient.GetLeaderSchedule(ulong, string)"/>
        public RequestResult<Dictionary<string, List<ulong>>> GetLeaderSchedule(ulong slot = 0, string identity = null)
            => GetLeaderScheduleAsync(slot, identity).Result;

        /// <inheritdoc cref="IRpcClient.GetLeaderScheduleAsync(ulong, string)"/>
        public async Task<RequestResult<Dictionary<string, List<ulong>>>> GetLeaderScheduleAsync(ulong slot = 0, string identity = null)
        {
            List<object> parameters = new List<object>();

            if (slot > 0) parameters.Add(slot);
            if (identity != null)
            {
                if (slot == 0) parameters.Add(null);
                parameters.Add(new Dictionary<string, string> { { "identity", identity } });
            }

            return await SendRequestAsync<Dictionary<string, List<ulong>>>("getLeaderSchedule", parameters.Count > 0 ? parameters : null);
        }


        /// <inheritdoc cref="IRpcClient.GetTransactionAsync(string)"/>
        public async Task<RequestResult<TransactionMetaSlotInfo>> GetTransactionAsync(string signature)
        {
            return await SendRequestAsync<TransactionMetaSlotInfo>("getTransaction", new List<object> { signature, "json" });
        }

        /// <inheritdoc cref="IRpcClient.GetTransaction(string)"/>
        public RequestResult<TransactionMetaSlotInfo> GetTransaction(string signature)
            => GetTransactionAsync(signature).Result;


        /// <inheritdoc cref="IRpcClient.GetBlockHeightAsync(Commitment)"/>
        public async Task<RequestResult<ulong>> GetBlockHeightAsync(Commitment commitment = Commitment.Finalized)
        {
            List<object> parameters = null;

            if (commitment != Commitment.Finalized)
            {
                parameters = new List<object> { new Dictionary<string, Commitment> { { "commitment", commitment } } };
            }

            return await SendRequestAsync<ulong>("getBlockHeight", parameters);
        }

        /// <inheritdoc cref="IRpcClient.GetBlockHeight(Commitment)"/>
        public RequestResult<ulong> GetBlockHeight(Commitment commitment = Commitment.Finalized)
            => GetBlockHeightAsync(commitment).Result;

        /// <summary>
        /// Gets the block commitment of a certain block, identified by slot.
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <returns>A task which may return a request result and block commitment information.</returns>
        public async Task<RequestResult<BlockCommitment>> GetBlockCommitmentAsync(ulong slot)
        {
            return await SendRequestAsync<BlockCommitment>("getBlockCommitment", new List<object> { slot });
        }

        /// <inheritdoc cref="GetBlockCommitmentAsync"/>
        public RequestResult<BlockCommitment> GetBlockCommitment(ulong slot)
            => GetBlockCommitmentAsync(slot).Result;

        /// <summary>
        /// Gets the estimated production time for a certain block, identified by slot.
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <returns>A task which may return a request result and block production time as Unix timestamp (seconds since Epoch).</returns>
        public async Task<RequestResult<ulong>> GetBlockTimeAsync(ulong slot)
        {
            return await SendRequestAsync<ulong>("getBlockTime", new List<object> { slot });
        }

        /// <inheritdoc cref="GetBlockTimeAsync"/>
        public RequestResult<ulong> GetBlockTime(ulong slot)
            => GetBlockTimeAsync(slot).Result;

        /// <summary>
        /// Gets the cluster nodes.
        /// </summary>
        /// <returns>A task which may return a request result and information about the nodes participating in the cluster.</returns>
        public async Task<RequestResult<List<ClusterNode>>> GetClusterNodesAsync()
        {
            return await SendRequestAsync<List<ClusterNode>>("getClusterNodes");
        }

        /// <inheritdoc cref="GetClusterNodesAsync"/>
        public RequestResult<List<ClusterNode>> GetClusterNodes()
            => GetClusterNodesAsync().Result;

        /// <inheritdoc cref="IRpcClient.GetEpochInfoAsync(Commitment)"/>
        public async Task<RequestResult<EpochInfo>> GetEpochInfoAsync(Commitment commitment = Commitment.Finalized)
        {
            List<object> parameters = null;

            if (commitment != Commitment.Finalized)
            {
                parameters = new List<Object> { new Dictionary<string, Commitment> { { "commitment", commitment } } };
            }

            return await SendRequestAsync<EpochInfo>("getEpochInfo", parameters);
        }

        /// <inheritdoc cref="IRpcClient.GetEpochInfo(Commitment)"/>
        public RequestResult<EpochInfo> GetEpochInfo(Commitment commitment = Commitment.Finalized) => GetEpochInfoAsync(commitment).Result;

        /// <summary>
        /// Gets epoch schedule information from this cluster's genesis config.
        /// </summary>
        /// <returns>A task which may return a request result and epoch schedule information from this cluster's genesis config.</returns>
        public async Task<RequestResult<EpochScheduleInfo>> GetEpochScheduleAsync()
        {
            return await SendRequestAsync<EpochScheduleInfo>("getEpochSchedule");
        }

        /// <inheritdoc cref="GetEpochScheduleAsync"/>
        public RequestResult<EpochScheduleInfo> GetEpochSchedule() => GetEpochScheduleAsync().Result;

        /// <summary>
        /// Gets the fee calculator associated with the query blockhash, or null if the blockhash has expired.
        /// </summary>
        /// <param name="blockhash">The blockhash to query, as base-58 encoded string.</param>
        /// <returns>A task which may return a request result and the fee calculator for the block.</returns>
        public async Task<RequestResult<ResponseValue<FeeCalculatorInfo>>> GetFeeCalculatorForBlockhashAsync(
            string blockhash)
        {
            return await SendRequestAsync<ResponseValue<FeeCalculatorInfo>>("getFeeCalculatorForBlockhash",
                new List<object> { blockhash });
        }

        /// <inheritdoc cref="GetFeeCalculatorForBlockhashAsync"/>
        public RequestResult<ResponseValue<FeeCalculatorInfo>> GetFeeCalculatorForBlockhash(string blockhash) =>
            GetFeeCalculatorForBlockhashAsync(blockhash).Result;

        /// <summary>
        /// Gets the fee rate governor information from the root bank.
        /// </summary>
        /// <returns>A task which may return a request result and the fee rate governor.</returns>
        public async Task<RequestResult<ResponseValue<FeeRateGovernorInfo>>> GetFeeRateGovernorAsync()
        {
            return await SendRequestAsync<ResponseValue<FeeRateGovernorInfo>>("getFeeRateGovernor");
        }

        /// <inheritdoc cref="GetFeeRateGovernorAsync"/>
        public RequestResult<ResponseValue<FeeRateGovernorInfo>> GetFeeRateGovernor()
            => GetFeeRateGovernorAsync().Result;

        /// <summary>
        /// Gets a recent block hash from the ledger, a fee schedule that can be used to compute the
        /// cost of submitting a transaction using it, and the last slot in which the blockhash will be valid.
        /// </summary>
        /// <returns>A task which may return a request result and information about fees.</returns>
        public async Task<RequestResult<ResponseValue<FeesInfo>>> GetFeesAsync()
        {
            return await SendRequestAsync<ResponseValue<FeesInfo>>("getFees");
        }

        /// <inheritdoc cref="GetFeesAsync"/>
        public RequestResult<ResponseValue<FeesInfo>> GetFees() => GetFeesAsync().Result;

        /// <summary>
        /// Gets a recent block hash.
        /// </summary>
        /// <returns>A task which may return a request result and recent block hash.</returns>
        public async Task<RequestResult<ResponseValue<BlockHash>>> GetRecentBlockHashAsync()
        {
            return await SendRequestAsync<ResponseValue<BlockHash>>("getRecentBlockhash");
        }

        /// <inheritdoc cref="GetBlockTimeAsync"/>
        public RequestResult<ResponseValue<BlockHash>> GetRecentBlockHash()
            => GetRecentBlockHashAsync().Result;

        /// <summary>
        /// Gets the maximum slot seen from retransmit stage.
        /// </summary>
        /// <returns>A task which may return a request result the maximum slot.</returns>
        public async Task<RequestResult<ulong>> GetMaxRetransmitSlotAsync()
        {
            return await SendRequestAsync<ulong>("getMaxRetransmitSlot");
        }

        /// <inheritdoc cref="GetMaxRetransmitSlotAsync"/>
        public RequestResult<ulong> GetMaxRetransmitSlot()
            => GetMaxRetransmitSlotAsync().Result;

        /// <summary>
        /// Gets the maximum slot seen from after shred insert.
        /// </summary>
        /// <returns>A task which may return a request result the maximum slot.</returns>
        public async Task<RequestResult<ulong>> GetMaxShredInsertSlotAsync()
        {
            return await SendRequestAsync<ulong>("getMaxShredInsertSlot");
        }

        /// <inheritdoc cref="GetMaxShredInsertSlotAsync"/>
        public RequestResult<ulong> GetMaxShredInsertSlot()
            => GetMaxShredInsertSlotAsync().Result;

        /// <summary>
        /// Gets the minimum balance required to make account rent exempt.
        /// </summary>
        /// <param name="accountDataSize">The account data size.</param>
        /// <returns>A task which may return a request result and the rent exemption value.</returns>
        public async Task<RequestResult<ulong>> GetMinimumBalanceForRentExemptionAsync(long accountDataSize)
        {
            return await SendRequestAsync<ulong>("getMinimumBalanceForRentExemption",
                new List<object> { accountDataSize });
        }

        /// <inheritdoc cref="GetMinimumBalanceForRentExemptionAsync"/>
        public RequestResult<ulong> GetMinimumBalanceForRentExemption(long accountDataSize)
            => GetMinimumBalanceForRentExemptionAsync(accountDataSize).Result;

        /// <summary>
        /// Gets the genesis hash of the ledger.
        /// </summary>
        /// <returns>A task which may return a request result and a string representing the genesis hash.</returns>
        public async Task<RequestResult<string>> GetGenesisHashAsync()
        {
            return await SendRequestAsync<string>("getGenesisHash");
        }

        /// <inheritdoc cref="GetGenesisHashAsync"/>
        public RequestResult<string> GetGenesisHash()
            => GetGenesisHashAsync().Result;

        /// <summary>
        /// Gets the identity pubkey for the current node.
        /// </summary>
        /// <returns>A task which may return a request result and a string representing the genesis hash.</returns>
        public async Task<RequestResult<NodeIdentity>> GetIdentityAsync()
        {
            return await SendRequestAsync<NodeIdentity>("getIdentity");
        }

        /// <inheritdoc cref="GetGenesisHashAsync"/>
        public RequestResult<NodeIdentity> GetIdentity()
            => GetIdentityAsync().Result;

        /// <summary>
        /// Gets the current inflation governor.
        /// </summary>
        /// <returns>A task which may return a request result and an object representing the current inflation governor.</returns>
        public async Task<RequestResult<InflationGovernor>> GetInflationGovernorAsync()
        {
            return await SendRequestAsync<InflationGovernor>("getInflationGovernor");
        }

        /// <inheritdoc cref="GetInflationGovernorAsync"/>
        public RequestResult<InflationGovernor> GetInflationGovernor()
            => GetInflationGovernorAsync().Result;

        /// <summary>
        /// Gets the specific inflation values for the current epoch.
        /// </summary>
        /// <returns>A task which may return a request result and an object representing the current inflation rate.</returns>
        public async Task<RequestResult<InflationRate>> GetInflationRateAsync()
        {
            return await SendRequestAsync<InflationRate>("getInflationRate");
        }

        /// <inheritdoc cref="GetInflationRateAsync"/>
        public RequestResult<InflationRate> GetInflationRate()
            => GetInflationRateAsync().Result;

        /// <summary>
        /// Gets the inflation reward for a list of addresses for an epoch.
        /// </summary>
        /// <param name="addresses">The list of addresses to query for, as base-58 encoded strings.</param>
        /// <param name="epoch">The epoch.</param>
        /// <returns>A task which may return a request result and a list of objects representing the inflation reward.</returns>
        public async Task<RequestResult<List<InflationReward>>> GetInflationRewardAsync(List<string> addresses, ulong epoch = 0)
        {
            if (epoch != 0)
                return await SendRequestAsync<List<InflationReward>>("getInflationReward",
                    new List<object> { addresses, epoch });
            return await SendRequestAsync<List<InflationReward>>("getInflationReward", new List<object> { addresses });
        }

        /// <inheritdoc cref="GetInflationRewardAsync"/>
        public RequestResult<List<InflationReward>> GetInflationReward(List<string> addresses, ulong epoch = 0)
            => GetInflationRewardAsync(addresses, epoch).Result;

        /// <summary>
        /// Gets the 20 largest accounts, by lamport balance.
        /// </summary>
        /// <param name="filter">Filter results by account type. Available types: circulating/nonCirculating </param>
        /// <returns>A task which may return a request result the current slot.</returns>
        /// <remarks>Results may be cached up to two hours.</remarks>
        public async Task<RequestResult<ResponseValue<List<LargeAccount>>>> GetLargestAccountsAsync(string filter)
        {
            return await SendRequestAsync<ResponseValue<List<LargeAccount>>>("getLargestAccounts",
                new List<object> { new Dictionary<string, string> { { "filter", filter } } });
        }

        /// <inheritdoc cref="GetSlotAsync"/>
        public RequestResult<ResponseValue<List<LargeAccount>>> GetLargestAccounts(string filter) =>
            GetLargestAccountsAsync(filter).Result;

        /// <summary>
        /// Gets the highest slot that the node has a snapshot for.
        /// </summary>
        /// <returns>A task which may return a request result the highest slot with a snapshot.</returns>
        public async Task<RequestResult<ulong>> GetSnapshotSlotAsync()
        {
            return await SendRequestAsync<ulong>("getSnapshotSlot");
        }

        /// <inheritdoc cref="GetSnapshotSlotAsync"/>
        public RequestResult<ulong> GetSnapshotSlot() => GetSnapshotSlotAsync().Result;

        /// <summary>
        /// Gets a list of recent performance samples.
        /// <remarks>
        /// Unless <c>searchTransactionHistory</c> is included, this method only searches the recent status cache of signatures.
        /// </remarks>
        /// </summary>
        /// <param name="limit">Maximum transaction signatures to return, between 1-720. Default is 720.</param>
        /// <returns>A task which may return a request result the signatures for the transactions.</returns>
        public async Task<RequestResult<List<PerformanceSample>>> GetRecentPerformanceSamplesAsync(ulong limit = 720)
        {
            return await SendRequestAsync<List<PerformanceSample>>("getRecentPerformanceSamples",
                new List<object> { limit });
        }

        /// <inheritdoc cref="GetRecentPerformanceSamplesAsync"/>
        public RequestResult<List<PerformanceSample>> GetRecentPerformanceSamples(ulong limit = 720)
            => GetRecentPerformanceSamplesAsync(limit).Result;

        /// <summary>
        /// Gets confirmed signatures for transactions involving the address.
        /// <remarks>
        /// Unless <c>searchTransactionHistory</c> is included, this method only searches the recent status cache of signatures.
        /// </remarks>
        /// </summary>
        /// <param name="accountPubKey">The account address as base-58 encoded string.</param>
        /// <param name="limit">Maximum transaction signatures to return, between 1-1000. Default is 1000.</param>
        /// <param name="before">Start searching backwards from this transaction signature.</param>
        /// <param name="until">Search until this transaction signature, if found before limit is reached.</param>
        /// <returns>A task which may return a request result the signatures for the transactions.</returns>
        public async Task<RequestResult<List<SignatureStatusInfo>>> GetSignaturesForAddressAsync(
            string accountPubKey, ulong limit = 1000, string before = "", string until = "")
        {
            var dictionary = new Dictionary<string, object> { { "limit", limit } };

            if (!string.IsNullOrWhiteSpace(before))
                dictionary.Add("before", before);

            if (!string.IsNullOrWhiteSpace(until))
                dictionary.Add("until", until);

            return await SendRequestAsync<List<SignatureStatusInfo>>("getSignaturesForAddress",
                new List<object> { accountPubKey, dictionary });
        }

        /// <inheritdoc cref="GetSignaturesForAddressAsync"/>
        public RequestResult<List<SignatureStatusInfo>> GetSignaturesForAddress(
            string accountPubKey, ulong limit = 1000, string before = "", string until = "")
            => GetSignaturesForAddressAsync(accountPubKey, limit, before, until).Result;

        /// <summary>
        /// Gets the status of a list of signatures.
        /// <remarks>
        /// Unless <c>searchTransactionHistory</c> is included, this method only searches the recent status cache of signatures.
        /// </remarks>
        /// </summary>
        /// <param name="transactionHashes">The list of transactions to search status info for.</param>
        /// <param name="searchTransactionHistory">If the node should search for signatures in it's ledger cache.</param>
        /// <returns>A task which may return a request result the highest slot with a snapshot.</returns>
        public async Task<RequestResult<ResponseValue<List<SignatureStatusInfo>>>> GetSignatureStatusesAsync(List<string> transactionHashes, bool searchTransactionHistory = false)
        {
            if (searchTransactionHistory)
                return await SendRequestAsync<ResponseValue<List<SignatureStatusInfo>>>("getSignatureStatuses",
                    new List<object> { transactionHashes, new Dictionary<string, bool> { { "searchTransactionHistory", true } } });
            return await SendRequestAsync<ResponseValue<List<SignatureStatusInfo>>>("getSignatureStatuses",
                new List<object> { transactionHashes });
        }

        /// <inheritdoc cref="GetSignatureStatusesAsync"/>
        public RequestResult<ResponseValue<List<SignatureStatusInfo>>> GetSignatureStatuses(List<string> transactionHashes,
            bool searchTransactionHistory = false)
            => GetSignatureStatusesAsync(transactionHashes, searchTransactionHistory).Result;

        /// <summary>
        /// Gets the current slot the node is processing
        /// </summary>
        /// <returns>A task which may return a request result the current slot.</returns>
        public async Task<RequestResult<ulong>> GetSlotAsync()
        {
            return await SendRequestAsync<ulong>("getSlot");
        }

        /// <inheritdoc cref="GetSlotAsync"/>
        public RequestResult<ulong> GetSlot() => GetSlotAsync().Result;

        /// <summary>
        /// Gets the current slot leader.
        /// </summary>
        /// <returns>A task which may return a request result the slot leader.</returns>
        public async Task<RequestResult<string>> GetSlotLeaderAsync()
        {
            return await SendRequestAsync<string>("getSlotLeader");
        }

        /// <inheritdoc cref="GetSlotLeaderAsync"/>
        public RequestResult<string> GetSlotLeader() => GetSlotLeaderAsync().Result;

        /// <summary>
        /// Gets the slot leaders for a given slot range.
        /// </summary>
        /// <param name="start">The start slot.</param>
        /// <param name="limit">The result limit.</param>
        /// <returns>A task which may return a request result and a list slot leaders.</returns>
        public async Task<RequestResult<List<string>>> GetSlotLeadersAsync(ulong start, ulong limit)
        {
            return await SendRequestAsync<List<string>>("getSlotLeaders", new List<object> { start, limit });
        }

        /// <inheritdoc cref="GetSlotLeadersAsync"/>
        public RequestResult<List<string>> GetSlotLeaders(ulong start, ulong limit)
            => GetSlotLeadersAsync(start, limit).Result;

        #region Token Supply and Balances

        /// <summary>
        /// Gets the epoch activation information for a stake account.
        /// </summary>
        /// <param name="publicKey">Public key of account to query, as base-58 encoded string</param>
        /// <param name="epoch">Epoch for which to calculate activation details.</param>
        /// <returns>A task which may return a request result and information about the stake activation.</returns>
        public async Task<RequestResult<StakeActivationInfo>> GetStakeActivationAsync(string publicKey, ulong epoch = 0)
        {
            if (epoch != 0)
                return await SendRequestAsync<StakeActivationInfo>("getStakeActivation",
                new List<object> { publicKey, new Dictionary<string, ulong> { { "epoch", epoch } } });
            return await SendRequestAsync<StakeActivationInfo>("getStakeActivation",
                new List<object> { publicKey });
        }

        /// <inheritdoc cref="GetStakeActivationAsync"/>
        public RequestResult<StakeActivationInfo> GetStakeActivation(string publicKey, ulong epoch = 0) =>
            GetStakeActivationAsync(publicKey, epoch).Result;

        /// <summary>
        /// Gets information about the current supply.
        /// </summary>
        /// <returns>A task which may return a request result and information about the supply.</returns>
        public async Task<RequestResult<ResponseValue<Supply>>> GetSupplyAsync()
        {
            return await SendRequestAsync<ResponseValue<Supply>>("getSupply");
        }

        /// <inheritdoc cref="GetSupplyAsync"/>
        public RequestResult<ResponseValue<Supply>> GetSupply()
            => GetSupplyAsync().Result;

        /// <summary>
        /// Gets the token balance of an SPL Token account.
        /// </summary>
        /// <param name="splTokenAccountPublicKey">Public key of Token account to query, as base-58 encoded string.</param>
        /// <returns>A task which may return a request result and information about token account balance.</returns>
        public async Task<RequestResult<ResponseValue<TokenBalance>>> GetTokenAccountBalanceAsync(
            string splTokenAccountPublicKey)
        {
            return await SendRequestAsync<ResponseValue<TokenBalance>>("getTokenAccountBalance",
                new List<object> { splTokenAccountPublicKey });
        }

        /// <inheritdoc cref="GetTokenAccountBalanceAsync"/>
        public RequestResult<ResponseValue<TokenBalance>> GetTokenAccountBalance(string splTokenAccountPublicKey)
            => GetTokenAccountBalanceAsync(splTokenAccountPublicKey).Result;

        /// <summary>
        /// Gets all SPL Token accounts by approved delegate.
        /// </summary>
        /// <param name="ownerPubKey">Public key of account owner query, as base-58 encoded string.</param>
        /// <param name="tokenMintPubKey">Public key of the specific token Mint to limit accounts to, as base-58 encoded string.</param>
        /// <param name="tokenProgramId">Public key of the Token program ID that owns the accounts, as base-58 encoded string.</param>
        /// <returns>A task which may return a request result and information about token accounts by delegate.</returns>
        public async Task<RequestResult<ResponseValue<List<TokenAccount>>>> GetTokenAccountsByDelegateAsync(
            string ownerPubKey, string tokenMintPubKey = "", string tokenProgramId = "")
        {
            if (string.IsNullOrWhiteSpace(tokenMintPubKey) && string.IsNullOrWhiteSpace(tokenProgramId))
                throw new ArgumentException("either tokenProgramId or tokenMintPubKey must be set");
            var options = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(tokenMintPubKey)) options.Add("mint", tokenMintPubKey);
            if (!string.IsNullOrWhiteSpace(tokenProgramId)) options.Add("programId", tokenProgramId);
            return await SendRequestAsync<ResponseValue<List<TokenAccount>>>(
                "getTokenAccountsByDelegate",
                new List<object> { ownerPubKey, options, new Dictionary<string, string> { { "encoding", "jsonParsed" } } });
        }

        /// <inheritdoc cref="GetTokenAccountsByDelegateAsync"/>
        public RequestResult<ResponseValue<List<TokenAccount>>> GetTokenAccountsByDelegate(
            string ownerPubKey, string tokenMintPubKey = "", string tokenProgramId = "")
            => GetTokenAccountsByDelegateAsync(ownerPubKey, tokenMintPubKey, tokenProgramId).Result;

        /// <summary>
        /// Gets all SPL Token accounts by token owner.
        /// </summary>
        /// <param name="ownerPubKey">Public key of account owner query, as base-58 encoded string.</param>
        /// <param name="tokenMintPubKey">Public key of the specific token Mint to limit accounts to, as base-58 encoded string.</param>
        /// <param name="tokenProgramId">Public key of the Token program ID that owns the accounts, as base-58 encoded string.</param>
        /// <returns>A task which may return a request result and information about token accounts by owner.</returns>
        public async Task<RequestResult<ResponseValue<List<TokenAccount>>>> GetTokenAccountsByOwnerAsync(
            string ownerPubKey, string tokenMintPubKey = "", string tokenProgramId = "")
        {
            if (string.IsNullOrWhiteSpace(tokenMintPubKey) && string.IsNullOrWhiteSpace(tokenProgramId))
                throw new ArgumentException("either tokenProgramId or tokenMintPubKey must be set");
            var options = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(tokenMintPubKey)) options.Add("mint", tokenMintPubKey);
            if (!string.IsNullOrWhiteSpace(tokenProgramId)) options.Add("programId", tokenProgramId);
            return await SendRequestAsync<ResponseValue<List<TokenAccount>>>(
                "getTokenAccountsByOwner",
                new List<object> { ownerPubKey, options, new Dictionary<string, string> { { "encoding", "jsonParsed" } } });
        }

        /// <inheritdoc cref="GetTokenAccountsByOwnerAsync"/>
        public RequestResult<ResponseValue<List<TokenAccount>>> GetTokenAccountsByOwner(
            string ownerPubKey, string tokenMintPubKey = "", string tokenProgramId = "")
            => GetTokenAccountsByOwnerAsync(ownerPubKey, tokenMintPubKey, tokenProgramId).Result;

        /// <summary>
        /// Gets the 20 largest accounts of a particular SPL Token.
        /// </summary>
        /// <param name="tokenMintPubKey">Public key of Token Mint to query, as base-58 encoded string.</param>
        /// <returns>A task which may return a request result and information about largest accounts.</returns>
        public async Task<RequestResult<ResponseValue<List<LargeTokenAccount>>>> GetTokenLargestAccountsAsync(
            string tokenMintPubKey)
        {
            return await SendRequestAsync<ResponseValue<List<LargeTokenAccount>>>("getTokenLargestAccounts",
                new List<object> { tokenMintPubKey });
        }

        /// <inheritdoc cref="GetTokenLargestAccountsAsync"/>
        public RequestResult<ResponseValue<List<LargeTokenAccount>>> GetTokenLargestAccounts(string tokenMintPubKey)
            => GetTokenLargestAccountsAsync(tokenMintPubKey).Result;

        /// <summary>
        /// Get the token supply of an SPL Token type.
        /// </summary>
        /// <param name="tokenMintPubKey">Public key of Token Mint to query, as base-58 encoded string.</param>
        /// <returns>A task which may return a request result and information about the supply.</returns>
        public async Task<RequestResult<ResponseValue<TokenBalance>>> GetTokenSupplyAsync(string tokenMintPubKey)
        {
            return await SendRequestAsync<ResponseValue<TokenBalance>>("getTokenSupply",
                new List<object> { tokenMintPubKey });
        }

        /// <inheritdoc cref="GetTokenSupplyAsync"/>
        public RequestResult<ResponseValue<TokenBalance>> GetTokenSupply(string tokenMintPubKey)
            => GetTokenSupplyAsync(tokenMintPubKey).Result;

        #endregion

        /// <summary>
        /// Gets the total transaction count of the ledger.
        /// </summary>
        /// <returns>A task which may return a request result and information about the transaction count.</returns>
        public async Task<RequestResult<ulong>> GetTransactionCountAsync()
        {
            return await SendRequestAsync<ulong>("getTransactionCount");
        }

        /// <inheritdoc cref="GetTransactionCountAsync"/>
        public RequestResult<ulong> GetTransactionCount()
            => GetTransactionCountAsync().Result;

        /// <summary>
        /// Gets the current node's software version.
        /// </summary>
        /// <returns>A task which may return a request result and information about the node's software version.</returns>
        public async Task<RequestResult<NodeVersion>> GetVersionAsync()
        {
            return await SendRequestAsync<NodeVersion>("getVersion");
        }

        /// <inheritdoc cref="GetVersionAsync"/>
        public RequestResult<NodeVersion> GetVersion()
            => GetVersionAsync().Result;

        /// <summary>
        /// Gets the account info and associated stake for all voting accounts in the current bank.
        /// </summary>
        /// <returns>A task which may return a request result and information about the vote accounts.</returns>
        public async Task<RequestResult<VoteAccounts>> GetVoteAccountsAsync()
        {
            return await SendRequestAsync<VoteAccounts>("getVoteAccounts");
        }

        /// <inheritdoc cref="GetVoteAccountsAsync"/>
        public RequestResult<VoteAccounts> GetVoteAccounts()
            => GetVoteAccountsAsync().Result;

        /// <summary>
        /// Gets the lowest slot that the node has information about in its ledger.
        /// <remarks>
        /// This value may decrease over time if a node is configured to purging data.
        /// </remarks>
        /// </summary>
        /// <returns>A task which may return a request result and the minimum slot available.</returns>
        public async Task<RequestResult<ulong>> GetMinimumLedgerSlotAsync()
        {
            return await SendRequestAsync<ulong>("minimumLedgerSlot");
        }

        /// <inheritdoc cref="GetMinimumLedgerSlotAsync"/>
        public RequestResult<ulong> GetMinimumLedgerSlot()
            => GetMinimumLedgerSlotAsync().Result;

        /// <summary>
        /// Requests an airdrop to the passed <c>pubKey</c> of the passed <c>lamports</c> amount.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default <see cref="Commitment.Finalized"/> is used.
        /// </remarks>
        /// </summary>
        /// <param name="pubKey">The public key of to receive the airdrop.</param>
        /// <param name="lamports">The amount of lamports to request.</param>
        /// <param name="commitment">The block commitment used to retrieve block hashes and verify success.</param>
        /// <returns>A task which may return a request result and the transaction signature of the airdrop, as base-58 encoded string..</returns>
        public async Task<RequestResult<string>> RequestAirdropAsync(string pubKey, ulong lamports,
            Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<string>("requestAirdrop", new List<object> { pubKey, lamports, commitment });
        }

        /// <inheritdoc cref="RequestAirdropAsync"/>
        public RequestResult<string> RequestAirdrop(string pubKey, ulong lamports,
            Commitment commitment = Commitment.Finalized)
            => RequestAirdropAsync(pubKey, lamports, commitment).Result;

        #region Transactions

        /// <summary>
        /// Sends a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as base-58 or base-64 encoded string.</param>
        /// <param name="encoding">The encoding of the transaction.</param>
        /// <returns>
        /// A task which may return a request result and the first transaction signature embedded in the transaction, as base-58 encoded string.
        /// </returns>
        public async Task<RequestResult<string>> SendTransactionAsync(string transaction,
            BinaryEncoding encoding = BinaryEncoding.Base64)
        {
            return await SendRequestAsync<string>("sendTransaction",
                new List<object>
                {
                    transaction
                },
                new Dictionary<string, object>
                {
                    {
                        "encoding", encoding
                    }
                });
        }

        /// <inheritdoc cref="SendTransactionAsync(string, BinaryEncoding)"/>
        public RequestResult<string> SendTransaction(string transaction,
            BinaryEncoding encoding = BinaryEncoding.Base64)
            => SendTransactionAsync(transaction, encoding).Result;

        /// <summary>
        /// Sends a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as byte array.</param>
        /// <returns>
        /// A task which may return a request result and the first transaction signature embedded in the transaction, as base-58 encoded string.
        /// </returns>
        public RequestResult<string> SendTransaction(byte[] transaction)
            => SendTransactionAsync(Convert.ToBase64String(transaction)).Result;

        /// <summary>
        /// Simulate sending a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as a byte array.</param>
        /// <param name="encoding">The encoding of the transaction.</param>
        /// <returns>
        /// A task which may return a request result and the transaction status.
        /// </returns>
        public async Task<RequestResult<ResponseValue<SimulationLogs>>> SimulateTransactionAsync(string transaction,
            BinaryEncoding encoding = BinaryEncoding.Base64)
        {
            return await SendRequestAsync<ResponseValue<SimulationLogs>>("simulateTransaction",
                new List<object>
                {
                    transaction
                }, new Dictionary<string, object>
                {
                    {
                        "encoding", encoding
                    }
                });
        }

        /// <inheritdoc cref="SimulateTransactionAsync"/>
        public RequestResult<ResponseValue<SimulationLogs>> SimulateTransaction(string transaction,
            BinaryEncoding encoding = BinaryEncoding.Base64)
            => SimulateTransactionAsync(transaction, encoding).Result;

        /// <summary>
        /// Simulate sending a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as a byte array.</param>
        /// <returns>
        /// A task which may return a request result and the transaction status.
        /// </returns>
        public RequestResult<ResponseValue<SimulationLogs>> SimulateTransaction(byte[] transaction)
            => SimulateTransactionAsync(Convert.ToBase64String(transaction)).Result;

        #endregion
    }
}