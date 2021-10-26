using Microsoft.Extensions.Logging;
using Solnet.Rpc.Core;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Solnet.Rpc
{
    /// <summary>
    /// Implements functionality to interact with the Solana JSON RPC API.
    /// </summary>
    [DebuggerDisplay("Cluster = {" + nameof(NodeAddress) + "}")]
    internal class SolanaRpcClient : JsonRpcClient, IRpcClient
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
            JsonRpcRequest req = BuildRequest<T>(method, null);
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
            JsonRpcRequest req = BuildRequest<T>(method, parameters);
            return await SendRequest<T>(req);
        }

        private KeyValue HandleCommitment(Commitment parameter, Commitment defaultValue = Commitment.Finalized)
            => parameter != defaultValue ? KeyValue.Create("commitment", parameter) : null;

        private KeyValue HandleTransactionDetails(TransactionDetailsFilterType parameter,
            TransactionDetailsFilterType defaultValue = TransactionDetailsFilterType.Full)
            => parameter != defaultValue ? KeyValue.Create("transactionDetails", parameter) : null;

        #endregion


        #region Accounts

        /// <inheritdoc cref="IRpcClient.GetTokenMintInfoAsync(string,Commitment)"/>
        public async Task<RequestResult<ResponseValue<TokenMintInfo>>> GetTokenMintInfoAsync(string pubKey,
            Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<ResponseValue<TokenMintInfo>>("getAccountInfo",
                Parameters.Create(
                    pubKey,
                    ConfigObject.Create(
                        KeyValue.Create("encoding", "jsonParsed"),
                        HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetTokenMintInfo(string,Commitment)"/>
        public RequestResult<ResponseValue<TokenMintInfo>> GetTokenMintInfo(string pubKey,
            Commitment commitment = Commitment.Finalized)
            => GetTokenMintInfoAsync(pubKey, commitment).Result;


        /// <inheritdoc cref="IRpcClient.GetTokenAccountInfoAsync(string,Commitment)"/>
        public async Task<RequestResult<ResponseValue<TokenAccountInfo>>> GetTokenAccountInfoAsync(string pubKey,
            Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<ResponseValue<TokenAccountInfo>>("getAccountInfo",
                Parameters.Create(
                    pubKey,
                    ConfigObject.Create(
                        KeyValue.Create("encoding", "jsonParsed"),
                        HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetTokenAccountInfo(string,Commitment)"/>
        public RequestResult<ResponseValue<TokenAccountInfo>> GetTokenAccountInfo(string pubKey,
            Commitment commitment = Commitment.Finalized)
            => GetTokenAccountInfoAsync(pubKey, commitment).Result;



        /// <inheritdoc cref="IRpcClient.GetAccountInfoAsync(string,Commitment,BinaryEncoding)"/>
        public async Task<RequestResult<ResponseValue<AccountInfo>>> GetAccountInfoAsync(string pubKey,
            Commitment commitment = Commitment.Finalized, BinaryEncoding encoding = BinaryEncoding.Base64)
        {
            return await SendRequestAsync<ResponseValue<AccountInfo>>("getAccountInfo",
                Parameters.Create(
                    pubKey,
                    ConfigObject.Create(
                        KeyValue.Create("encoding", encoding),
                        HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetAccountInfo(string,Commitment,BinaryEncoding)"/>
        public RequestResult<ResponseValue<AccountInfo>> GetAccountInfo(string pubKey,
            Commitment commitment = Commitment.Finalized, BinaryEncoding encoding = BinaryEncoding.Base64)
            => GetAccountInfoAsync(pubKey, commitment, encoding).Result;


        /// <inheritdoc cref="IRpcClient.GetProgramAccountsAsync"/>
        public async Task<RequestResult<List<AccountKeyPair>>> GetProgramAccountsAsync(string pubKey,
            Commitment commitment = Commitment.Finalized, int? dataSize = null, IList<MemCmp> memCmpList = null)
        {
            List<object> filters = Parameters.Create(ConfigObject.Create(KeyValue.Create("dataSize", dataSize)));
            if (memCmpList != null)
            {
                filters ??= new List<object>();
                filters.AddRange(memCmpList.Select(filter => ConfigObject.Create(KeyValue.Create("memcmp",
                    ConfigObject.Create(KeyValue.Create("offset", filter.Offset),
                        KeyValue.Create("bytes", filter.Bytes))))));
            }

            return await SendRequestAsync<List<AccountKeyPair>>("getProgramAccounts",
                Parameters.Create(
                    pubKey,
                    ConfigObject.Create(
                        KeyValue.Create("encoding", "base64"),
                        KeyValue.Create("filters", filters),
                        HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetProgramAccounts"/>
        public RequestResult<List<AccountKeyPair>> GetProgramAccounts(string pubKey,
            Commitment commitment = Commitment.Finalized,
            int? dataSize = null, IList<MemCmp> memCmpList = null)
            => GetProgramAccountsAsync(pubKey, commitment, dataSize, memCmpList).Result;


        /// <inheritdoc cref="IRpcClient.GetMultipleAccountsAsync"/>
        public async Task<RequestResult<ResponseValue<List<AccountInfo>>>> GetMultipleAccountsAsync(
            IList<string> accounts,
            Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<ResponseValue<List<AccountInfo>>>("getMultipleAccounts",
                Parameters.Create(
                    accounts,
                    ConfigObject.Create(
                        KeyValue.Create("encoding", "base64"),
                        HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetMultipleAccounts"/>
        public RequestResult<ResponseValue<List<AccountInfo>>> GetMultipleAccounts(IList<string> accounts,
            Commitment commitment = Commitment.Finalized)
            => GetMultipleAccountsAsync(accounts, commitment).Result;

        #endregion

        /// <inheritdoc cref="IRpcClient.GetBalanceAsync"/>
        public async Task<RequestResult<ResponseValue<ulong>>> GetBalanceAsync(string pubKey,
            Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<ResponseValue<ulong>>("getBalance",
                Parameters.Create(pubKey, ConfigObject.Create(HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetBalance"/>
        public RequestResult<ResponseValue<ulong>> GetBalance(string pubKey,
            Commitment commitment = Commitment.Finalized)
            => GetBalanceAsync(pubKey, commitment).Result;

        #region Blocks

        /// <inheritdoc cref="IRpcClient.GetBlockAsync"/>
        public async Task<RequestResult<BlockInfo>> GetBlockAsync(ulong slot,
            Commitment commitment = Commitment.Finalized,
            TransactionDetailsFilterType transactionDetails = TransactionDetailsFilterType.Full,
            bool blockRewards = false)
        {
            if (commitment == Commitment.Processed)
            {
                throw new ArgumentException("Commitment.Processed is not supported for this method.");
            }

            return await SendRequestAsync<BlockInfo>("getBlock",
                Parameters.Create(slot, ConfigObject.Create(
                    KeyValue.Create("encoding", "json"),
                    HandleTransactionDetails(transactionDetails),
                    KeyValue.Create("rewards", blockRewards ? blockRewards : null),
                    HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetBlock"/>
        public RequestResult<BlockInfo> GetBlock(ulong slot, Commitment commitment = Commitment.Finalized,
            TransactionDetailsFilterType transactionDetails = TransactionDetailsFilterType.Full,
            bool blockRewards = false)
            => GetBlockAsync(slot, commitment, transactionDetails, blockRewards).Result;


        /// <inheritdoc cref="IRpcClient.GetBlocksAsync"/>
        public async Task<RequestResult<List<ulong>>> GetBlocksAsync(ulong startSlot, ulong endSlot = 0,
            Commitment commitment = Commitment.Finalized)
        {
            if (commitment == Commitment.Processed)
            {
                throw new ArgumentException("Commitment.Processed is not supported for this method.");
            }

            return await SendRequestAsync<List<ulong>>("getBlocks",
                Parameters.Create(startSlot, endSlot > 0 ? endSlot : null,
                    ConfigObject.Create(HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetConfirmedBlockAsync"/>
        public async Task<RequestResult<BlockInfo>> GetConfirmedBlockAsync(ulong slot,
            Commitment commitment = Commitment.Finalized,
            TransactionDetailsFilterType transactionDetails = TransactionDetailsFilterType.Full,
            bool blockRewards = false)
        {
            if (commitment == Commitment.Processed)
            {
                throw new ArgumentException("Commitment.Processed is not supported for this method.");
            }

            return await SendRequestAsync<BlockInfo>("getConfirmedBlock",
                Parameters.Create(slot, ConfigObject.Create(
                    KeyValue.Create("encoding", "json"),
                    HandleTransactionDetails(transactionDetails),
                    KeyValue.Create("rewards", blockRewards ? blockRewards : null),
                    HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetConfirmedBlock"/>
        public RequestResult<BlockInfo> GetConfirmedBlock(ulong slot, Commitment commitment = Commitment.Finalized,
            TransactionDetailsFilterType transactionDetails = TransactionDetailsFilterType.Full,
            bool blockRewards = false)
            => GetConfirmedBlockAsync(slot, commitment, transactionDetails, blockRewards).Result;


        /// <inheritdoc cref="IRpcClient.GetBlocks"/>
        public RequestResult<List<ulong>> GetBlocks(ulong startSlot, ulong endSlot = 0,
            Commitment commitment = Commitment.Finalized)
            => GetBlocksAsync(startSlot, endSlot, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetConfirmedBlocksAsync"/>
        public async Task<RequestResult<List<ulong>>> GetConfirmedBlocksAsync(ulong startSlot, ulong endSlot = 0,
            Commitment commitment = Commitment.Finalized)
        {
            if (commitment == Commitment.Processed)
            {
                throw new ArgumentException("Commitment.Processed is not supported for this method.");
            }

            return await SendRequestAsync<List<ulong>>("getConfirmedBlocks",
                Parameters.Create(startSlot, endSlot > 0 ? endSlot : null,
                    ConfigObject.Create(HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetConfirmedBlocks"/>
        public RequestResult<List<ulong>> GetConfirmedBlocks(ulong startSlot, ulong endSlot = 0,
            Commitment commitment = Commitment.Finalized)
            => GetConfirmedBlocksAsync(startSlot, endSlot, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetConfirmedBlocksWithLimitAsync"/>
        public async Task<RequestResult<List<ulong>>> GetConfirmedBlocksWithLimitAsync(ulong startSlot, ulong limit,
            Commitment commitment = Commitment.Finalized)
        {
            if (commitment == Commitment.Processed)
            {
                throw new ArgumentException("Commitment.Processed is not supported for this method.");
            }

            return await SendRequestAsync<List<ulong>>("getConfirmedBlocksWithLimit",
                Parameters.Create(startSlot, limit, ConfigObject.Create(HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetBlocksWithLimit"/>
        public RequestResult<List<ulong>> GetBlocksWithLimit(ulong startSlot, ulong limit,
            Commitment commitment = Commitment.Finalized)
            => GetBlocksWithLimitAsync(startSlot, limit, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetConfirmedBlocksWithLimit"/>
        public RequestResult<List<ulong>> GetConfirmedBlocksWithLimit(ulong startSlot, ulong limit,
            Commitment commitment = Commitment.Finalized)
            => GetConfirmedBlocksWithLimitAsync(startSlot, limit, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetBlocksWithLimitAsync"/>
        public async Task<RequestResult<List<ulong>>> GetBlocksWithLimitAsync(ulong startSlot, ulong limit,
            Commitment commitment = Commitment.Finalized)
        {
            if (commitment == Commitment.Processed)
            {
                throw new ArgumentException("Commitment.Processed is not supported for this method.");
            }

            return await SendRequestAsync<List<ulong>>("getBlocksWithLimit",
                Parameters.Create(startSlot, limit, ConfigObject.Create(HandleCommitment(commitment))));
        }


        /// <inheritdoc cref="IRpcClient.GetFirstAvailableBlock"/>
        public RequestResult<ulong> GetFirstAvailableBlock()
            => GetFirstAvailableBlockAsync().Result;

        /// <inheritdoc cref="IRpcClient.GetFirstAvailableBlock"/>
        public async Task<RequestResult<ulong>> GetFirstAvailableBlockAsync()
        {
            return await SendRequestAsync<ulong>("getFirstAvailableBlock");
        }

        #endregion

        #region Block Production

        /// <inheritdoc cref="IRpcClient.GetBlockProductionAsync(string, ulong?, ulong?, Commitment)"/>
        public async Task<RequestResult<ResponseValue<BlockProductionInfo>>> GetBlockProductionAsync(
            string identity = null, ulong? firstSlot = null, ulong? lastSlot = null,
            Commitment commitment = Commitment.Finalized)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

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
                Dictionary<string, object> range = new Dictionary<string, object> { { "firstSlot", firstSlot.Value } };

                if (lastSlot.HasValue)
                {
                    range.Add("lastSlot", lastSlot.Value);
                }

                parameters.Add("range", range);
            }
            else if (lastSlot.HasValue)
            {
                throw new ArgumentException(
                    "Range parameters are optional, but the lastSlot argument must be paired with a firstSlot.");
            }

            List<object> args = parameters.Count > 0 ? new List<object> { parameters } : null;

            return await SendRequestAsync<ResponseValue<BlockProductionInfo>>("getBlockProduction", args);
        }

        /// <inheritdoc cref="IRpcClient.GetBlockProduction(string, ulong?, ulong?, Commitment)"/>
        public RequestResult<ResponseValue<BlockProductionInfo>> GetBlockProduction(string identity = null,
            ulong? firstSlot = null, ulong? lastSlot = null, Commitment commitment = Commitment.Finalized)
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


        /// <inheritdoc cref="IRpcClient.GetLeaderSchedule"/>
        public RequestResult<Dictionary<string, List<ulong>>> GetLeaderSchedule(ulong slot = 0,
            string identity = null, Commitment commitment = Commitment.Finalized)
            => GetLeaderScheduleAsync(slot, identity, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetLeaderScheduleAsync"/>
        public async Task<RequestResult<Dictionary<string, List<ulong>>>> GetLeaderScheduleAsync(ulong slot = 0,
            string identity = null, Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<Dictionary<string, List<ulong>>>("getLeaderSchedule",
                Parameters.Create(
                    slot > 0 ? slot : null,
                    ConfigObject.Create(
                        HandleCommitment(commitment),
                        KeyValue.Create("identity", identity))));
        }


        /// <inheritdoc cref="IRpcClient.GetTransactionAsync"/>
        public async Task<RequestResult<TransactionMetaSlotInfo>> GetTransactionAsync(string signature,
            Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<TransactionMetaSlotInfo>("getTransaction",
                Parameters.Create(signature,
                    ConfigObject.Create(KeyValue.Create("encoding", "json"), HandleCommitment(commitment))));
        }

        public async Task<RequestResult<TransactionMetaSlotInfo>> GetConfirmedTransactionAsync(string signature,
            Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<TransactionMetaSlotInfo>("getConfirmedTransaction",
                Parameters.Create(signature,
                    ConfigObject.Create(KeyValue.Create("encoding", "json"), HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetTransaction"/>
        public RequestResult<TransactionMetaSlotInfo> GetTransaction(string signature,
            Commitment commitment = Commitment.Finalized)
            => GetTransactionAsync(signature, commitment).Result;

        public RequestResult<TransactionMetaSlotInfo> GetConfirmedTransaction(string signature,
            Commitment commitment = Commitment.Finalized) =>
            GetConfirmedTransactionAsync(signature, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetBlockHeightAsync"/>
        public async Task<RequestResult<ulong>> GetBlockHeightAsync(Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<ulong>("getBlockHeight",
                Parameters.Create(ConfigObject.Create(HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetBlockHeight"/>
        public RequestResult<ulong> GetBlockHeight(Commitment commitment = Commitment.Finalized)
            => GetBlockHeightAsync(commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetBlockCommitmentAsync"/>
        public async Task<RequestResult<BlockCommitment>> GetBlockCommitmentAsync(ulong slot)
        {
            return await SendRequestAsync<BlockCommitment>("getBlockCommitment", Parameters.Create(slot));
        }

        /// <inheritdoc cref="IRpcClient.GetBlockCommitment"/>
        public RequestResult<BlockCommitment> GetBlockCommitment(ulong slot)
            => GetBlockCommitmentAsync(slot).Result;

        /// <inheritdoc cref="IRpcClient.GetBlockTimeAsync"/>
        public async Task<RequestResult<ulong>> GetBlockTimeAsync(ulong slot)
        {
            return await SendRequestAsync<ulong>("getBlockTime", Parameters.Create(slot));
        }

        /// <inheritdoc cref="IRpcClient.GetBlockTime"/>
        public RequestResult<ulong> GetBlockTime(ulong slot)
            => GetBlockTimeAsync(slot).Result;

        /// <inheritdoc cref="IRpcClient.GetClusterNodesAsync"/>
        public async Task<RequestResult<List<ClusterNode>>> GetClusterNodesAsync()
        {
            return await SendRequestAsync<List<ClusterNode>>("getClusterNodes");
        }

        /// <inheritdoc cref="IRpcClient.GetClusterNodes"/>
        public RequestResult<List<ClusterNode>> GetClusterNodes()
            => GetClusterNodesAsync().Result;

        /// <inheritdoc cref="IRpcClient.GetEpochInfoAsync"/>
        public async Task<RequestResult<EpochInfo>> GetEpochInfoAsync(Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<EpochInfo>("getEpochInfo",
                Parameters.Create(ConfigObject.Create(HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetEpochInfo"/>
        public RequestResult<EpochInfo> GetEpochInfo(Commitment commitment = Commitment.Finalized) =>
            GetEpochInfoAsync(commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetEpochScheduleAsync"/>
        public async Task<RequestResult<EpochScheduleInfo>> GetEpochScheduleAsync()
        {
            return await SendRequestAsync<EpochScheduleInfo>("getEpochSchedule");
        }

        /// <inheritdoc cref="IRpcClient.GetEpochSchedule"/>
        public RequestResult<EpochScheduleInfo> GetEpochSchedule() => GetEpochScheduleAsync().Result;


        /// <inheritdoc cref="IRpcClient.GetFeeCalculatorForBlockhashAsync"/>
        public async Task<RequestResult<ResponseValue<FeeCalculatorInfo>>> GetFeeCalculatorForBlockhashAsync(
            string blockhash, Commitment commitment = Commitment.Finalized)
        {
            List<object> parameters = Parameters.Create(blockhash, ConfigObject.Create(HandleCommitment(commitment)));

            return await SendRequestAsync<ResponseValue<FeeCalculatorInfo>>("getFeeCalculatorForBlockhash", parameters);
        }

        /// <inheritdoc cref="IRpcClient.GetFeeCalculatorForBlockhash"/>
        public RequestResult<ResponseValue<FeeCalculatorInfo>> GetFeeCalculatorForBlockhash(string blockhash,
            Commitment commitment = Commitment.Finalized) =>
            GetFeeCalculatorForBlockhashAsync(blockhash, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetFeeRateGovernorAsync"/>
        public async Task<RequestResult<ResponseValue<FeeRateGovernorInfo>>> GetFeeRateGovernorAsync()
        {
            return await SendRequestAsync<ResponseValue<FeeRateGovernorInfo>>("getFeeRateGovernor");
        }

        /// <inheritdoc cref="IRpcClient.GetFeeRateGovernor"/>
        public RequestResult<ResponseValue<FeeRateGovernorInfo>> GetFeeRateGovernor()
            => GetFeeRateGovernorAsync().Result;

        /// <inheritdoc cref="IRpcClient.GetFeesAsync"/>
        public async Task<RequestResult<ResponseValue<FeesInfo>>> GetFeesAsync(
            Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<ResponseValue<FeesInfo>>("getFees",
                Parameters.Create(ConfigObject.Create(HandleCommitment(commitment))));
        }


        /// <inheritdoc cref="IRpcClient.GetFees"/>
        public RequestResult<ResponseValue<FeesInfo>> GetFees(Commitment commitment = Commitment.Finalized)
            => GetFeesAsync(commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetRecentBlockHashAsync"/>
        public async Task<RequestResult<ResponseValue<BlockHash>>> GetRecentBlockHashAsync(
            Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<ResponseValue<BlockHash>>("getRecentBlockhash",
                Parameters.Create(ConfigObject.Create(HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetRecentBlockHash"/>
        public RequestResult<ResponseValue<BlockHash>> GetRecentBlockHash(Commitment commitment = Commitment.Finalized)
            => GetRecentBlockHashAsync(commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetMaxRetransmitSlotAsync"/>
        public async Task<RequestResult<ulong>> GetMaxRetransmitSlotAsync()
        {
            return await SendRequestAsync<ulong>("getMaxRetransmitSlot");
        }

        /// <inheritdoc cref="IRpcClient.GetMaxRetransmitSlot"/>
        public RequestResult<ulong> GetMaxRetransmitSlot()
            => GetMaxRetransmitSlotAsync().Result;

        /// <inheritdoc cref="IRpcClient.GetMaxShredInsertSlotAsync"/>
        public async Task<RequestResult<ulong>> GetMaxShredInsertSlotAsync()
        {
            return await SendRequestAsync<ulong>("getMaxShredInsertSlot");
        }

        /// <inheritdoc cref="IRpcClient.GetMaxShredInsertSlot"/>
        public RequestResult<ulong> GetMaxShredInsertSlot()
            => GetMaxShredInsertSlotAsync().Result;

        /// <inheritdoc cref="IRpcClient.GetMinimumBalanceForRentExemptionAsync"/>
        public async Task<RequestResult<ulong>> GetMinimumBalanceForRentExemptionAsync(long accountDataSize,
            Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<ulong>("getMinimumBalanceForRentExemption",
                Parameters.Create(accountDataSize, ConfigObject.Create(HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetMinimumBalanceForRentExemption"/>
        public RequestResult<ulong> GetMinimumBalanceForRentExemption(long accountDataSize,
            Commitment commitment = Commitment.Finalized)
            => GetMinimumBalanceForRentExemptionAsync(accountDataSize, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetGenesisHashAsync"/>
        public async Task<RequestResult<string>> GetGenesisHashAsync()
        {
            return await SendRequestAsync<string>("getGenesisHash");
        }

        /// <inheritdoc cref="IRpcClient.GetGenesisHash"/>
        public RequestResult<string> GetGenesisHash()
            => GetGenesisHashAsync().Result;

        /// <inheritdoc cref="IRpcClient.GetIdentityAsync"/>
        public async Task<RequestResult<NodeIdentity>> GetIdentityAsync()
        {
            return await SendRequestAsync<NodeIdentity>("getIdentity");
        }

        /// <inheritdoc cref="IRpcClient.GetIdentity"/>
        public RequestResult<NodeIdentity> GetIdentity()
            => GetIdentityAsync().Result;

        /// <inheritdoc cref="IRpcClient.GetInflationGovernorAsync"/>
        public async Task<RequestResult<InflationGovernor>> GetInflationGovernorAsync(
            Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<InflationGovernor>("getInflationGovernor",
                Parameters.Create(ConfigObject.Create(HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetInflationGovernor"/>
        public RequestResult<InflationGovernor> GetInflationGovernor(Commitment commitment = Commitment.Finalized)
            => GetInflationGovernorAsync(commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetInflationRateAsync"/>
        public async Task<RequestResult<InflationRate>> GetInflationRateAsync()
        {
            return await SendRequestAsync<InflationRate>("getInflationRate");
        }

        /// <inheritdoc cref="IRpcClient.GetInflationRate"/>
        public RequestResult<InflationRate> GetInflationRate()
            => GetInflationRateAsync().Result;

        /// <inheritdoc cref="IRpcClient.GetInflationRewardAsync"/>
        public async Task<RequestResult<List<InflationReward>>> GetInflationRewardAsync(IList<string> addresses,
            ulong epoch = 0, Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<List<InflationReward>>("getInflationReward",
                Parameters.Create(
                    addresses,
                    ConfigObject.Create(
                        HandleCommitment(commitment),
                        KeyValue.Create("epoch", epoch > 0 ? epoch : null))));
        }

        /// <inheritdoc cref="IRpcClient.GetInflationReward"/>
        public RequestResult<List<InflationReward>> GetInflationReward(IList<string> addresses, ulong epoch = 0,
            Commitment commitment = Commitment.Finalized)
            => GetInflationRewardAsync(addresses, epoch, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetLargestAccountsAsync"/>
        public async Task<RequestResult<ResponseValue<List<LargeAccount>>>> GetLargestAccountsAsync(
            AccountFilterType? filter = null,
            Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<ResponseValue<List<LargeAccount>>>("getLargestAccounts",
                Parameters.Create(
                    ConfigObject.Create(
                        HandleCommitment(commitment),
                        KeyValue.Create("filter", filter))));
        }

        /// <inheritdoc cref="IRpcClient.GetLargestAccounts"/>
        public RequestResult<ResponseValue<List<LargeAccount>>> GetLargestAccounts(AccountFilterType? filter = null,
            Commitment commitment = Commitment.Finalized)
            => GetLargestAccountsAsync(filter, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetSnapshotSlotAsync"/>
        public async Task<RequestResult<ulong>> GetSnapshotSlotAsync()
        {
            return await SendRequestAsync<ulong>("getSnapshotSlot");
        }

        /// <inheritdoc cref="IRpcClient.GetSnapshotSlot"/>
        public RequestResult<ulong> GetSnapshotSlot() => GetSnapshotSlotAsync().Result;

        /// <inheritdoc cref="IRpcClient.GetRecentPerformanceSamplesAsync"/>
        public async Task<RequestResult<List<PerformanceSample>>> GetRecentPerformanceSamplesAsync(ulong limit = 720)
        {
            return await SendRequestAsync<List<PerformanceSample>>("getRecentPerformanceSamples",
                new List<object> { limit });
        }

        /// <inheritdoc cref="IRpcClient.GetRecentPerformanceSamples"/>
        public RequestResult<List<PerformanceSample>> GetRecentPerformanceSamples(ulong limit = 720)
            => GetRecentPerformanceSamplesAsync(limit).Result;

        /// <inheritdoc cref="IRpcClient.GetSignaturesForAddressAsync"/>
        public async Task<RequestResult<List<SignatureStatusInfo>>> GetSignaturesForAddressAsync(string accountPubKey,
            ulong limit = 1000, string before = null, string until = null, Commitment commitment = Commitment.Finalized)
        {
            if (commitment == Commitment.Processed)
                throw new ArgumentException("Commitment.Processed is not supported for this method.");

            return await SendRequestAsync<List<SignatureStatusInfo>>("getSignaturesForAddress",
                Parameters.Create(
                    accountPubKey,
                    ConfigObject.Create(
                        KeyValue.Create("limit", limit != 1000 ? limit : null),
                        KeyValue.Create("before", before),
                        KeyValue.Create("until", until),
                        HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetConfirmedSignaturesForAddress2Async"/>
        public async Task<RequestResult<List<SignatureStatusInfo>>> GetConfirmedSignaturesForAddress2Async(
            string accountPubKey, ulong limit = 1000, string before = null, string until = null,
            Commitment commitment = Commitment.Finalized)
        {
            if (commitment == Commitment.Processed)
                throw new ArgumentException("Commitment.Processed is not supported for this method.");

            return await SendRequestAsync<List<SignatureStatusInfo>>("getConfirmedSignaturesForAddress2",
                Parameters.Create(
                    accountPubKey,
                    ConfigObject.Create(
                        KeyValue.Create("limit", limit != 1000 ? limit : null),
                        KeyValue.Create("before", before),
                        KeyValue.Create("until", until),
                        HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetSignaturesForAddress"/>
        public RequestResult<List<SignatureStatusInfo>> GetSignaturesForAddress(string accountPubKey,
            ulong limit = 1000,
            string before = null, string until = null, Commitment commitment = Commitment.Finalized)
            => GetSignaturesForAddressAsync(accountPubKey, limit, before, until, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetConfirmedSignaturesForAddress2"/>
        public RequestResult<List<SignatureStatusInfo>> GetConfirmedSignaturesForAddress2(string accountPubKey,
            ulong limit = 1000, string before = null,
            string until = null, Commitment commitment = Commitment.Finalized)
            => GetConfirmedSignaturesForAddress2Async(accountPubKey, limit, before, until, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetSignatureStatusesAsync"/>
        public async Task<RequestResult<ResponseValue<List<SignatureStatusInfo>>>> GetSignatureStatusesAsync(
            List<string> transactionHashes,
            bool searchTransactionHistory = false)
        {
            return await SendRequestAsync<ResponseValue<List<SignatureStatusInfo>>>("getSignatureStatuses",
                Parameters.Create(
                    transactionHashes,
                    ConfigObject.Create(
                        KeyValue.Create("searchTransactionHistory",
                            searchTransactionHistory ? searchTransactionHistory : null))));
        }

        /// <inheritdoc cref="IRpcClient.GetSignatureStatuses"/>
        public RequestResult<ResponseValue<List<SignatureStatusInfo>>> GetSignatureStatuses(
            List<string> transactionHashes,
            bool searchTransactionHistory = false)
            => GetSignatureStatusesAsync(transactionHashes, searchTransactionHistory).Result;

        /// <inheritdoc cref="IRpcClient.GetSlotAsync"/>
        public async Task<RequestResult<ulong>> GetSlotAsync(Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<ulong>("getSlot",
                Parameters.Create(ConfigObject.Create(HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetSlot"/>
        public RequestResult<ulong> GetSlot(Commitment commitment = Commitment.Finalized) =>
            GetSlotAsync(commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetSlotLeaderAsync"/>
        public async Task<RequestResult<string>> GetSlotLeaderAsync(Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<string>("getSlotLeader",
                Parameters.Create(ConfigObject.Create(HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetSlotLeader"/>
        public RequestResult<string> GetSlotLeader(Commitment commitment = Commitment.Finalized) =>
            GetSlotLeaderAsync(commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetSlotLeadersAsync"/>
        public async Task<RequestResult<List<string>>> GetSlotLeadersAsync(ulong start, ulong limit,
            Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<List<string>>("getSlotLeaders",
                Parameters.Create(start, limit, ConfigObject.Create(HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetSlotLeaders"/>
        public RequestResult<List<string>> GetSlotLeaders(ulong start, ulong limit,
            Commitment commitment = Commitment.Finalized)
            => GetSlotLeadersAsync(start, limit, commitment).Result;

        #region Token Supply and Balances

        /// <inheritdoc cref="IRpcClient.GetStakeActivationAsync"/>
        public async Task<RequestResult<StakeActivationInfo>> GetStakeActivationAsync(string publicKey, ulong epoch = 0,
            Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<StakeActivationInfo>("getStakeActivation",
                Parameters.Create(
                    publicKey,
                    ConfigObject.Create(
                        HandleCommitment(commitment),
                        KeyValue.Create("epoch", epoch > 0 ? epoch : null))));
        }

        /// <inheritdoc cref="IRpcClient.GetStakeActivation"/>
        public RequestResult<StakeActivationInfo> GetStakeActivation(string publicKey, ulong epoch = 0,
            Commitment commitment = Commitment.Finalized) =>
            GetStakeActivationAsync(publicKey, epoch, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetSupplyAsync"/>
        public async Task<RequestResult<ResponseValue<Supply>>> GetSupplyAsync(
            Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<ResponseValue<Supply>>("getSupply",
                Parameters.Create(ConfigObject.Create(HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetSupply"/>
        public RequestResult<ResponseValue<Supply>> GetSupply(Commitment commitment = Commitment.Finalized)
            => GetSupplyAsync(commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetTokenAccountBalanceAsync"/>
        public async Task<RequestResult<ResponseValue<TokenBalance>>> GetTokenAccountBalanceAsync(
            string splTokenAccountPublicKey, Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<ResponseValue<TokenBalance>>("getTokenAccountBalance",
                Parameters.Create(splTokenAccountPublicKey, ConfigObject.Create(HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetTokenAccountBalance"/>
        public RequestResult<ResponseValue<TokenBalance>> GetTokenAccountBalance(string splTokenAccountPublicKey,
            Commitment commitment = Commitment.Finalized)
            => GetTokenAccountBalanceAsync(splTokenAccountPublicKey, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetTokenAccountsByDelegateAsync"/>
        public async Task<RequestResult<ResponseValue<List<TokenAccount>>>> GetTokenAccountsByDelegateAsync(
            string ownerPubKey, string tokenMintPubKey = null, string tokenProgramId = null,
            Commitment commitment = Commitment.Finalized)
        {
            if (string.IsNullOrWhiteSpace(tokenMintPubKey) && string.IsNullOrWhiteSpace(tokenProgramId))
                throw new ArgumentException("either tokenProgramId or tokenMintPubKey must be set");

            return await SendRequestAsync<ResponseValue<List<TokenAccount>>>("getTokenAccountsByDelegate",
                Parameters.Create(
                    ownerPubKey,
                    ConfigObject.Create(
                        KeyValue.Create("mint", tokenMintPubKey),
                        KeyValue.Create("programId", tokenProgramId)),
                    ConfigObject.Create(
                        HandleCommitment(commitment),
                        KeyValue.Create("encoding", "jsonParsed"))));
        }

        /// <inheritdoc cref="IRpcClient.GetTokenAccountsByDelegate"/>
        public RequestResult<ResponseValue<List<TokenAccount>>> GetTokenAccountsByDelegate(string ownerPubKey,
            string tokenMintPubKey = null, string tokenProgramId = null, Commitment commitment = Commitment.Finalized)
            => GetTokenAccountsByDelegateAsync(ownerPubKey, tokenMintPubKey, tokenProgramId, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetTokenAccountsByOwnerAsync"/>
        public async Task<RequestResult<ResponseValue<List<TokenAccount>>>> GetTokenAccountsByOwnerAsync(
            string ownerPubKey, string tokenMintPubKey = null, string tokenProgramId = null,
            Commitment commitment = Commitment.Finalized)
        {
            if (string.IsNullOrWhiteSpace(tokenMintPubKey) && string.IsNullOrWhiteSpace(tokenProgramId))
                throw new ArgumentException("either tokenProgramId or tokenMintPubKey must be set");

            return await SendRequestAsync<ResponseValue<List<TokenAccount>>>("getTokenAccountsByOwner",
                Parameters.Create(
                    ownerPubKey,
                    ConfigObject.Create(
                        KeyValue.Create("mint", tokenMintPubKey),
                        KeyValue.Create("programId", tokenProgramId)),
                    ConfigObject.Create(
                        HandleCommitment(commitment),
                        KeyValue.Create("encoding", "jsonParsed"))));
        }

        /// <inheritdoc cref="IRpcClient.GetTokenAccountsByOwner"/>
        public RequestResult<ResponseValue<List<TokenAccount>>> GetTokenAccountsByOwner(
            string ownerPubKey, string tokenMintPubKey = null, string tokenProgramId = null,
            Commitment commitment = Commitment.Finalized)
            => GetTokenAccountsByOwnerAsync(ownerPubKey, tokenMintPubKey, tokenProgramId, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetTokenLargestAccountsAsync"/>
        public async Task<RequestResult<ResponseValue<List<LargeTokenAccount>>>> GetTokenLargestAccountsAsync(
            string tokenMintPubKey, Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<ResponseValue<List<LargeTokenAccount>>>("getTokenLargestAccounts",
                Parameters.Create(tokenMintPubKey, ConfigObject.Create(HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetTokenLargestAccounts"/>
        public RequestResult<ResponseValue<List<LargeTokenAccount>>> GetTokenLargestAccounts(string tokenMintPubKey,
            Commitment commitment = Commitment.Finalized)
            => GetTokenLargestAccountsAsync(tokenMintPubKey, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetTokenSupplyAsync"/>
        public async Task<RequestResult<ResponseValue<TokenBalance>>> GetTokenSupplyAsync(string tokenMintPubKey,
            Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<ResponseValue<TokenBalance>>("getTokenSupply",
                Parameters.Create(tokenMintPubKey, ConfigObject.Create(HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetTokenSupply"/>
        public RequestResult<ResponseValue<TokenBalance>> GetTokenSupply(string tokenMintPubKey,
            Commitment commitment = Commitment.Finalized)
            => GetTokenSupplyAsync(tokenMintPubKey, commitment).Result;

        #endregion

        /// <inheritdoc cref="IRpcClient.GetTransactionCountAsync"/>
        public async Task<RequestResult<ulong>> GetTransactionCountAsync(Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<ulong>("getTransactionCount",
                Parameters.Create(ConfigObject.Create(HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.GetTransactionCount"/>
        public RequestResult<ulong> GetTransactionCount(Commitment commitment = Commitment.Finalized)
            => GetTransactionCountAsync(commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetVersionAsync"/>
        public async Task<RequestResult<NodeVersion>> GetVersionAsync()
        {
            return await SendRequestAsync<NodeVersion>("getVersion");
        }

        /// <inheritdoc cref="IRpcClient.GetVersion"/>
        public RequestResult<NodeVersion> GetVersion()
            => GetVersionAsync().Result;

        /// <inheritdoc cref="IRpcClient.GetVoteAccountsAsync"/>
        public async Task<RequestResult<VoteAccounts>> GetVoteAccountsAsync(string votePubKey = null,
            Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<VoteAccounts>("getVoteAccounts",
                Parameters.Create(ConfigObject.Create(HandleCommitment(commitment),
                    KeyValue.Create("votePubkey", votePubKey))));
        }

        /// <inheritdoc cref="IRpcClient.GetVoteAccounts"/>
        public RequestResult<VoteAccounts> GetVoteAccounts(string votePubKey = null,
            Commitment commitment = Commitment.Finalized)
            => GetVoteAccountsAsync(votePubKey, commitment).Result;

        /// <inheritdoc cref="IRpcClient.GetMinimumLedgerSlotAsync"/>
        public async Task<RequestResult<ulong>> GetMinimumLedgerSlotAsync()
        {
            return await SendRequestAsync<ulong>("minimumLedgerSlot");
        }

        /// <inheritdoc cref="IRpcClient.GetMinimumLedgerSlot"/>
        public RequestResult<ulong> GetMinimumLedgerSlot()
            => GetMinimumLedgerSlotAsync().Result;

        /// <inheritdoc cref="IRpcClient.RequestAirdropAsync"/>
        public async Task<RequestResult<string>> RequestAirdropAsync(string pubKey, ulong lamports,
            Commitment commitment = Commitment.Finalized)
        {
            return await SendRequestAsync<string>("requestAirdrop",
                Parameters.Create(pubKey, lamports, ConfigObject.Create(HandleCommitment(commitment))));
        }

        /// <inheritdoc cref="IRpcClient.RequestAirdrop"/>
        public RequestResult<string> RequestAirdrop(string pubKey, ulong lamports,
            Commitment commitment = Commitment.Finalized)
            => RequestAirdropAsync(pubKey, lamports, commitment).Result;

        #region Transactions

        /// <inheritdoc cref="IRpcClient.SendTransactionAsync(byte[], bool, Commitment)"/>
        public async Task<RequestResult<string>> SendTransactionAsync(byte[] transaction, bool skipPreflight = false,
            Commitment preflightCommitment = Commitment.Finalized)
        {
            return await SendTransactionAsync(Convert.ToBase64String(transaction), skipPreflight, preflightCommitment)
                .ConfigureAwait(false);
        }


        /// <inheritdoc cref="IRpcClient.SendTransactionAsync(string, bool, Commitment)"/>
        public async Task<RequestResult<string>> SendTransactionAsync(string transaction, bool skipPreflight = false,
            Commitment preflightCommitment = Commitment.Finalized)
        {
            return await SendRequestAsync<string>("sendTransaction",
                Parameters.Create(
                    transaction,
                    ConfigObject.Create(
                        KeyValue.Create("skipPreflight", skipPreflight ? skipPreflight : null),
                        KeyValue.Create("preflightCommitment",
                            preflightCommitment == Commitment.Finalized ? null : preflightCommitment),
                        KeyValue.Create("encoding", BinaryEncoding.Base64))));
        }

        /// <inheritdoc cref="IRpcClient.SendTransaction(string, bool, Commitment)"/>
        public RequestResult<string> SendTransaction(string transaction, bool skipPreFlight = false,
            Commitment preFlightCommitment = Commitment.Finalized)
            => SendTransactionAsync(transaction, skipPreFlight, preFlightCommitment).Result;

        /// <inheritdoc cref="IRpcClient.SendTransactionAsync(byte[], bool, Commitment)"/>
        public RequestResult<string> SendTransaction(byte[] transaction, bool skipPreFlight = false,
            Commitment preFlightCommitment = Commitment.Finalized)
            => SendTransactionAsync(transaction, skipPreFlight, preFlightCommitment).Result;

        /// <inheritdoc cref="IRpcClient.SimulateTransactionAsync(string, bool, Commitment, bool, IList{string})"/>
        public async Task<RequestResult<ResponseValue<SimulationLogs>>> SimulateTransactionAsync(string transaction,
            bool sigVerify = false,
            Commitment commitment = Commitment.Finalized, bool replaceRecentBlockhash = false,
            IList<string> accountsToReturn = null)
        {
            if (sigVerify && replaceRecentBlockhash)
            {
                throw new ArgumentException(
                    $"Parameters {nameof(sigVerify)} and {nameof(replaceRecentBlockhash)} are incompatible, only one can be set to true.");
            }

            return await SendRequestAsync<ResponseValue<SimulationLogs>>("simulateTransaction",
                Parameters.Create(
                    transaction,
                    ConfigObject.Create(
                        KeyValue.Create("sigVerify", sigVerify ? sigVerify : null),
                        HandleCommitment(commitment),
                        KeyValue.Create("encoding", BinaryEncoding.Base64),
                        KeyValue.Create("replaceRecentBlockhash",
                            replaceRecentBlockhash ? replaceRecentBlockhash : null),
                        KeyValue.Create("accounts", accountsToReturn != null
                            ? ConfigObject.Create(
                                KeyValue.Create("encoding", BinaryEncoding.Base64),
                                KeyValue.Create("addresses", accountsToReturn))
                            : null))));
        }

        /// <inheritdoc cref="IRpcClient.SimulateTransactionAsync(byte[], bool, Commitment, bool, IList{string})"/>
        public async Task<RequestResult<ResponseValue<SimulationLogs>>> SimulateTransactionAsync(byte[] transaction,
            bool sigVerify = false,
            Commitment commitment = Commitment.Finalized, bool replaceRecentBlockhash = false,
            IList<string> accountsToReturn = null)
        {
            return await SimulateTransactionAsync(Convert.ToBase64String(transaction), sigVerify, commitment,
                    replaceRecentBlockhash, accountsToReturn)
                .ConfigureAwait(false);
        }

        /// <inheritdoc cref="IRpcClient.SimulateTransaction(string, bool, Commitment, bool, IList{string})"/>
        public RequestResult<ResponseValue<SimulationLogs>> SimulateTransaction(string transaction,
            bool sigVerify = false,
            Commitment commitment = Commitment.Finalized, bool replaceRecentBlockhash = false,
            IList<string> accountsToReturn = null)
            => SimulateTransactionAsync(transaction, sigVerify, commitment, replaceRecentBlockhash, accountsToReturn)
                .Result;

        /// <inheritdoc cref="IRpcClient.SimulateTransaction(byte[], bool, Commitment, bool, IList{string})"/>
        public RequestResult<ResponseValue<SimulationLogs>> SimulateTransaction(byte[] transaction,
            bool sigVerify = false,
            Commitment commitment = Commitment.Finalized, bool replaceRecentBlockhash = false,
            IList<string> accountsToReturn = null)
            => SimulateTransactionAsync(transaction, sigVerify, commitment, replaceRecentBlockhash, accountsToReturn)
                .Result;

        #endregion
    }
}