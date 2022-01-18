using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solnet.Rpc
{
    /// <summary>
    /// Specifies the methods to interact with the JSON RPC API.
    /// </summary>
    public interface IRpcClient
    {
        /// <summary>
        /// The address this client connects to.
        /// </summary>
        Uri NodeAddress { get; }

        /// <summary>
        /// Gets the token mint info. This method only works if the target account is a SPL token mint.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// </summary>
        /// <param name="pubKey">The token mint public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>A task which may return a request result holding the context and account info.</returns>
        Task<RequestResult<ResponseValue<TokenMintInfo>>> GetTokenMintInfoAsync(string pubKey,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the token mint info. This method only works if the target account is a SPL token mint.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// </summary>
        /// <param name="pubKey">The token mint public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<TokenMintInfo>> GetTokenMintInfo(string pubKey, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the token account info.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// </summary>
        /// <param name="pubKey">The token account public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>A task which may return a request result holding the context and account info.</returns>
        Task<RequestResult<ResponseValue<TokenAccountInfo>>> GetTokenAccountInfoAsync(string pubKey,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the token account info.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// </summary>
        /// <param name="pubKey">The token account public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<TokenAccountInfo>> GetTokenAccountInfo(string pubKey, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the account info.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// </summary>
        /// <param name="pubKey">The account public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="encoding">The encoding of the account data.</param>
        /// <returns>A task which may return a request result holding the context and account info.</returns>
        Task<RequestResult<ResponseValue<AccountInfo>>> GetAccountInfoAsync(string pubKey,
            Commitment commitment = Commitment.Finalized, BinaryEncoding encoding = BinaryEncoding.Base64);

        /// <summary>
        /// Gets the account info.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// </summary>
        /// <param name="pubKey">The account public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="encoding">The encoding of the account data.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<AccountInfo>> GetAccountInfo(string pubKey, Commitment commitment = Commitment.Finalized,
            BinaryEncoding encoding = BinaryEncoding.Base64);

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
        /// Gets the balance <b>synchronously</b> for a certain public key.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// </summary>
        /// <param name="pubKey">The public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<ulong>> GetBalance(string pubKey, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Returns identity and transaction information about a block in the ledger.
        /// <remarks>
        /// <para>
        /// The <c>commitment</c> parameter is optional, <see cref="Commitment.Processed"/> is not supported,
        /// the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </para>
        /// <para>
        /// The <c>transactionDetails</c> parameter is optional, the default value <see cref="TransactionDetailsFilterType.Full"/> is not sent.
        /// </para>
        /// <para>
        /// The <c>blockRewards</c> parameter is optional, the default value, <c>false</c>, is not sent.
        /// </para>
        /// </remarks>
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="transactionDetails">The level of transaction detail to return, see <see cref="TransactionDetailsFilterType"/>.</param>
        /// <param name="blockRewards">Whether to populate the <c>rewards</c> array, the default includes rewards.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<BlockInfo>> GetBlockAsync(ulong slot, Commitment commitment = Commitment.Finalized,
            TransactionDetailsFilterType transactionDetails = TransactionDetailsFilterType.Full, bool blockRewards = false);

        /// <summary>
        /// Returns identity and transaction information about a confirmed block in the ledger.
        /// <remarks>
        /// <para>
        /// The <c>commitment</c> parameter is optional, <see cref="Commitment.Processed"/> is not supported,
        /// the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </para>
        /// <para>
        /// The <c>transactionDetails</c> parameter is optional, the default value <see cref="TransactionDetailsFilterType.Full"/> is not sent.
        /// </para>
        /// <para>
        /// The <c>blockRewards</c> parameter is optional, the default value, <c>false</c>, is not sent.
        /// </para>
        /// </remarks>
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="transactionDetails">The level of transaction detail to return, see <see cref="TransactionDetailsFilterType"/>.</param>
        /// <param name="blockRewards">Whether to populate the <c>rewards</c> array, the default includes rewards.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        [Obsolete("Please use GetBlockAsync whenever possible instead. This method is expected to be removed in solana-core v1.8.")]
        Task<RequestResult<BlockInfo>> GetConfirmedBlockAsync(ulong slot, Commitment commitment = Commitment.Finalized,
            TransactionDetailsFilterType transactionDetails = TransactionDetailsFilterType.Full, bool blockRewards = false);

        /// <summary>
        /// Returns identity and transaction information about a block in the ledger.
        /// <remarks>
        /// <para>
        /// The <c>commitment</c> parameter is optional, <see cref="Commitment.Processed"/> is not supported,
        /// the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </para>
        /// <para>
        /// The <c>transactionDetails</c> parameter is optional, the default value <see cref="TransactionDetailsFilterType.Full"/> is not sent.
        /// </para>
        /// <para>
        /// The <c>blockRewards</c> parameter is optional, the default value, <c>false</c>, is not sent.
        /// </para>
        /// </remarks>
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="transactionDetails">The level of transaction detail to return, see <see cref="TransactionDetailsFilterType"/>.</param>
        /// <param name="blockRewards">Whether to populate the <c>rewards</c> array, the default includes rewards.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<BlockInfo> GetBlock(ulong slot, Commitment commitment = Commitment.Finalized,
            TransactionDetailsFilterType transactionDetails = TransactionDetailsFilterType.Full, bool blockRewards = false);

        /// <summary>
        /// Returns identity and transaction information about a confirmed block in the ledger.
        /// <remarks>
        /// <para>
        /// The <c>commitment</c> parameter is optional, <see cref="Commitment.Processed"/> is not supported,
        /// the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </para>
        /// <para>
        /// The <c>transactionDetails</c> parameter is optional, the default value <see cref="TransactionDetailsFilterType.Full"/> is not sent.
        /// </para>
        /// <para>
        /// The <c>blockRewards</c> parameter is optional, the default value, <c>false</c>, is not sent.
        /// </para>
        /// </remarks>
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="transactionDetails">The level of transaction detail to return, see <see cref="TransactionDetailsFilterType"/>.</param>
        /// <param name="blockRewards">Whether to populate the <c>rewards</c> array, the default includes rewards.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        [Obsolete("Please use GetBlock whenever possible instead. This method is expected to be removed in solana-core v1.8.")]
        RequestResult<BlockInfo> GetConfirmedBlock(ulong slot, Commitment commitment = Commitment.Finalized,
            TransactionDetailsFilterType transactionDetails = TransactionDetailsFilterType.Full, bool blockRewards = false);

        /// <summary>
        /// Gets the block commitment of a certain block, identified by slot.
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<BlockCommitment>> GetBlockCommitmentAsync(ulong slot);

        /// <summary>
        /// Gets the block commitment of a certain block, identified by slot.
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<BlockCommitment> GetBlockCommitment(ulong slot);

        /// <summary>
        /// Gets the current block height of the node.
        /// </summary>
        /// <param name="commitment">The commitment state to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ulong>> GetBlockHeightAsync(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the current block height of the node.
        /// </summary>
        /// <param name="commitment">The commitment state to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ulong> GetBlockHeight(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Returns recent block production information from the current or previous epoch.
        /// <remarks>
        /// All the arguments are optional, but the lastSlot must be paired with a firstSlot argument.
        /// </remarks>
        /// </summary>
        /// <param name="identity">Filter production details only for this given validator.</param>
        /// <param name="firstSlot">The first slot to return production information (inclusive).</param>
        /// <param name="lastSlot">The last slot to return production information (inclusive and optional).</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<BlockProductionInfo>>> GetBlockProductionAsync(string identity = null,
            ulong? firstSlot = null, ulong? lastSlot = null, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Returns recent block production information from the current or previous epoch.
        /// </summary>
        /// <remarks>
        /// All the arguments are optional, but the lastSlot must be paired with a firstSlot argument.
        /// </remarks>
        /// <param name="identity">Filter production details only for this given validator.</param>
        /// <param name="firstSlot">The first slot to return production information (inclusive).</param>
        /// <param name="lastSlot">The last slot to return production information (inclusive and optional).</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<BlockProductionInfo>> GetBlockProduction(string identity = null,
            ulong? firstSlot = null, ulong? lastSlot = null, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Returns a list of blocks between two slots.
        /// </summary>
        /// <param name="startSlot">The start slot (inclusive).</param>
        /// <param name="endSlot">The start slot (inclusive and optional).</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<List<ulong>>> GetBlocksAsync(ulong startSlot, ulong endSlot = 0,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Returns a list of confirmed blocks between two slots.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <c>commitment</c> parameter is optional, <see cref="Commitment.Processed"/> is not supported,
        /// the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </para>
        /// </remarks>
        /// <param name="startSlot">The start slot (inclusive).</param>
        /// <param name="endSlot">The start slot (inclusive and optional).</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        [Obsolete("Please use GetBlocksAsync whenever possible instead. This method is expected to be removed in solana-core v1.8.")]
        Task<RequestResult<List<ulong>>> GetConfirmedBlocksAsync(ulong startSlot, ulong endSlot = 0, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Returns a list of blocks between two slots.
        /// </summary>
        /// <param name="startSlot">The start slot (inclusive).</param>
        /// <param name="endSlot">The start slot (inclusive and optional).</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<List<ulong>> GetBlocks(ulong startSlot, ulong endSlot = 0,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Returns a list of confirmed blocks between two slots.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <c>commitment</c> parameter is optional, <see cref="Commitment.Processed"/> is not supported,
        /// the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </para>
        /// </remarks>
        /// <param name="startSlot">The start slot (inclusive).</param>
        /// <param name="endSlot">The start slot (inclusive and optional).</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        [Obsolete("Please use GetBlocks whenever possible instead. This method is expected to be removed in solana-core v1.8.")]
        RequestResult<List<ulong>> GetConfirmedBlocks(ulong startSlot, ulong endSlot = 0, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Returns a list of confirmed blocks starting at the given slot.
        /// </summary>
        /// <param name="startSlot">The start slot (inclusive).</param>
        /// <param name="limit">The max number of blocks to return.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<List<ulong>>> GetBlocksWithLimitAsync(ulong startSlot,
            ulong limit, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Returns a list of confirmed blocks starting at the given slot.
        /// </summary>
        /// <param name="startSlot">The start slot (inclusive).</param>
        /// <param name="limit">The max number of blocks to return.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        [Obsolete("Please use GetBlocksWithLimitAsync whenever possible instead. This method is expected to be removed in solana-core v1.8.")]
        Task<RequestResult<List<ulong>>> GetConfirmedBlocksWithLimitAsync(ulong startSlot,
            ulong limit, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Returns a list of confirmed blocks starting at the given slot.
        /// </summary>
        /// <param name="startSlot">The start slot (inclusive).</param>
        /// <param name="limit">The max number of blocks to return.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<List<ulong>> GetBlocksWithLimit(ulong startSlot, ulong limit,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Returns a list of confirmed blocks starting at the given slot.
        /// </summary>
        /// <param name="startSlot">The start slot (inclusive).</param>
        /// <param name="limit">The max number of blocks to return.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        [Obsolete("Please use GetBlocksWithLimit whenever possible instead. This method is expected to be removed in solana-core v1.8.")]
        RequestResult<List<ulong>> GetConfirmedBlocksWithLimit(ulong startSlot,
            ulong limit, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the estimated production time for a certain block, identified by slot.
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ulong>> GetBlockTimeAsync(ulong slot);

        /// <summary>
        /// Gets the estimated production time for a certain block, identified by slot.
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ulong> GetBlockTime(ulong slot);

        /// <summary>
        /// Gets the cluster nodes.
        /// </summary>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<List<ClusterNode>>> GetClusterNodesAsync();

        /// <summary>
        /// Gets the cluster nodes.
        /// </summary>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<List<ClusterNode>> GetClusterNodes();

        /// <summary>
        /// Gets information about the current epoch.
        /// </summary>
        /// <param name="commitment">The commitment state to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<EpochInfo>> GetEpochInfoAsync(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets information about the current epoch.
        /// </summary>
        /// <param name="commitment">The commitment state to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<EpochInfo> GetEpochInfo(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets epoch schedule information from this cluster's genesis config.
        /// </summary>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<EpochScheduleInfo>> GetEpochScheduleAsync();

        /// <summary>
        /// Gets epoch schedule information from this cluster's genesis config.
        /// </summary>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<EpochScheduleInfo> GetEpochSchedule();

        /// <summary>
        /// Gets the fee calculator associated with the query blockhash, or null if the blockhash has expired.
        /// </summary>
        /// <param name="blockhash">The blockhash to query, as base-58 encoded string.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<FeeCalculatorInfo>>> GetFeeCalculatorForBlockhashAsync(
            string blockhash, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the fee calculator associated with the query blockhash, or null if the blockhash has expired.
        /// </summary>
        /// <param name="blockhash">The blockhash to query, as base-58 encoded string.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<FeeCalculatorInfo>> GetFeeCalculatorForBlockhash(string blockhash,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the fee rate governor information from the root bank.
        /// </summary>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<FeeRateGovernorInfo>>> GetFeeRateGovernorAsync();

        /// <summary>
        /// Gets the fee rate governor information from the root bank.
        /// </summary>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<FeeRateGovernorInfo>> GetFeeRateGovernor();

        /// <summary>
        /// Gets a recent block hash from the ledger, a fee schedule that can be used to compute the
        /// cost of submitting a transaction using it, and the last slot in which the blockhash will be valid.
        /// </summary>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<FeesInfo>>> GetFeesAsync(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets a recent block hash from the ledger, a fee schedule that can be used to compute the
        /// cost of submitting a transaction using it, and the last slot in which the blockhash will be valid.
        /// </summary>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<FeesInfo>> GetFees(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Returns the slot of the lowest confirmed block that has not been purged from the ledger.
        /// </summary>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ulong>> GetFirstAvailableBlockAsync();

        /// <summary>
        /// Returns the slot of the lowest confirmed block that has not been purged from the ledger.
        /// </summary>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ulong> GetFirstAvailableBlock();

        /// <summary>
        /// Gets the genesis hash of the ledger.
        /// </summary>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<string>> GetGenesisHashAsync();

        /// <summary>
        /// Gets the genesis hash of the ledger.
        /// </summary>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<string> GetGenesisHash();

        /// <summary>
        /// Returns the current health of the node. 
        /// This method should return the string 'ok' if the node is healthy, or the error code along with any information provided otherwise. 
        /// </summary>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<string>> GetHealthAsync();

        /// <summary>
        /// Returns the current health of the node. 
        /// This method should return the string 'ok' if the node is healthy, or the error code along with any information provided otherwise. 
        /// </summary>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<string> GetHealth();

        /// <summary>
        /// Gets the identity pubkey for the current node.
        /// </summary>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<NodeIdentity>> GetIdentityAsync();

        /// <summary>
        /// Gets the identity pubkey for the current node.
        /// </summary>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<NodeIdentity> GetIdentity();

        /// <summary>
        /// Gets the current inflation governor.
        /// </summary>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<InflationGovernor>> GetInflationGovernorAsync(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the current inflation governor.
        /// </summary>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<InflationGovernor> GetInflationGovernor(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the specific inflation values for the current epoch.
        /// </summary>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<InflationRate>> GetInflationRateAsync();

        /// <summary>
        /// Gets the specific inflation values for the current epoch.
        /// </summary>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<InflationRate> GetInflationRate();

        /// <summary>
        /// Gets the inflation reward for a list of addresses for an epoch.
        /// </summary>
        /// <param name="addresses">The list of addresses to query, as base-58 encoded strings.</param>
        /// <param name="epoch">The epoch.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<List<InflationReward>>> GetInflationRewardAsync(IList<string> addresses,
            ulong epoch = 0, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the inflation reward for a list of addresses for an epoch.
        /// </summary>
        /// <param name="addresses">The list of addresses to query, as base-58 encoded strings.</param>
        /// <param name="epoch">The epoch.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<List<InflationReward>> GetInflationReward(IList<string> addresses, ulong epoch = 0,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the 20 largest accounts, by lamport balance.
        /// </summary>
        /// <remarks>Results may be cached up to two hours.</remarks>
        /// <param name="filter">Filter results by account type.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<List<LargeAccount>>>> GetLargestAccountsAsync(AccountFilterType? filter = null,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the 20 largest accounts, by lamport balance.
        /// </summary>
        /// <remarks>Results may be cached up to two hours.</remarks>
        /// <param name="filter">Filter results by account type.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<List<LargeAccount>>> GetLargestAccounts(AccountFilterType? filter = null,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Returns the leader schedule for an epoch.
        /// </summary>
        /// <param name="slot">Fetch the leader schedule for the epoch that corresponds to the provided slot. 
        /// If unspecified, the leader schedule for the current epoch is fetched.</param>
        /// <param name="identity">Filter results for this validator only (base 58 encoded string and optional).</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<Dictionary<string, List<ulong>>>> GetLeaderScheduleAsync(ulong slot = 0,
            string identity = null, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Returns the leader schedule for an epoch.
        /// </summary>
        /// <param name="slot">Fetch the leader schedule for the epoch that corresponds to the provided slot. 
        /// If unspecified, the leader schedule for the current epoch is fetched.</param>
        /// <param name="identity">Filter results for this validator only (base 58 encoded string and optional).</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<Dictionary<string, List<ulong>>> GetLeaderSchedule(ulong slot = 0,
            string identity = null, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the maximum slot seen from retransmit stage.
        /// </summary>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ulong>> GetMaxRetransmitSlotAsync();

        /// <summary>
        /// Gets the maximum slot seen from retransmit stage.
        /// </summary>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ulong> GetMaxRetransmitSlot();

        /// <summary>
        /// Gets the maximum slot seen from after shred insert.
        /// </summary>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ulong>> GetMaxShredInsertSlotAsync();

        /// <summary>
        /// Gets the maximum slot seen from after shred insert.
        /// </summary>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ulong> GetMaxShredInsertSlot();

        /// <summary>
        /// Gets the minimum balance required to make account rent exempt.
        /// </summary>
        /// <param name="accountDataSize">The account data size.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ulong>> GetMinimumBalanceForRentExemptionAsync(long accountDataSize,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the minimum balance required to make account rent exempt.
        /// </summary>
        /// <param name="accountDataSize">The account data size.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ulong> GetMinimumBalanceForRentExemption(long accountDataSize,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the lowest slot that the node has information about in its ledger.
        /// <remarks>
        /// This value may decrease over time if a node is configured to purging data.
        /// </remarks>
        /// </summary>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ulong>> GetMinimumLedgerSlotAsync();

        /// <summary>
        /// Gets the lowest slot that the node has information about in its ledger.
        /// <remarks>
        /// This value may decrease over time if a node is configured to purging data.
        /// </remarks>
        /// </summary>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ulong> GetMinimumLedgerSlot();

        /// <summary>
        /// Gets the account info for multiple accounts.
        /// </summary>
        /// <param name="accounts">The list of the accounts public keys.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>A task which may return a request result holding the context and account info.</returns>
        Task<RequestResult<ResponseValue<List<AccountInfo>>>> GetMultipleAccountsAsync(IList<string> accounts,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the account info for multiple accounts.
        /// </summary>
        /// <param name="accounts">The list of the accounts public keys.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<List<AccountInfo>>> GetMultipleAccounts(IList<string> accounts,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Returns all accounts owned by the provided program Pubkey.
        /// <remarks>Accounts must meet all filter criteria to be included in the results.</remarks>
        /// </summary>
        /// <param name="pubKey">The program public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="dataSize">The data size of the account to compare against the program account data.</param>
        /// <param name="memCmpList">The list of comparisons to match against the program account data.</param>
        /// <returns>A task which may return a request result holding the context and account info.</returns>
        Task<RequestResult<List<AccountKeyPair>>> GetProgramAccountsAsync(string pubKey, Commitment commitment = Commitment.Finalized,
            int? dataSize = null, IList<MemCmp> memCmpList = null);

        /// <summary>
        /// Returns all accounts owned by the provided program Pubkey.
        /// <remarks>Accounts must meet all filter criteria to be included in the results.</remarks>
        /// </summary>
        /// <param name="pubKey">The program public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="dataSize">The data size of the account to compare against the program account data.</param>
        /// <param name="memCmpList">The list of comparisons to match against the program account data.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<List<AccountKeyPair>> GetProgramAccounts(string pubKey, Commitment commitment = Commitment.Finalized,
            int? dataSize = null, IList<MemCmp> memCmpList = null);

        /// <summary>
        /// Gets a recent block hash.
        /// </summary>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<BlockHash>>> GetRecentBlockHashAsync(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets a recent block hash.
        /// </summary>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<BlockHash>> GetRecentBlockHash(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets a list of recent performance samples.
        /// <remarks>
        /// Unless <c>searchTransactionHistory</c> is included, this method only searches the recent status cache of signatures.
        /// </remarks>
        /// </summary>
        /// <param name="limit">Maximum transaction signatures to return, between 1-720. Default is 720.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<List<PerformanceSample>>> GetRecentPerformanceSamplesAsync(ulong limit = 720);

        /// <summary>
        /// Gets a list of recent performance samples.
        /// <remarks>
        /// Unless <c>searchTransactionHistory</c> is included, this method only searches the recent status cache of signatures.
        /// </remarks>
        /// </summary>
        /// <param name="limit">Maximum transaction signatures to return, between 1-720. Default is 720.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<List<PerformanceSample>> GetRecentPerformanceSamples(ulong limit = 720);

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
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<List<SignatureStatusInfo>>> GetSignaturesForAddressAsync(string accountPubKey, ulong limit = 1000,
            string before = null, string until = null, Commitment commitment = Commitment.Finalized);

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
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        [Obsolete("Please use GetSignaturesForAddressAsync whenever possible instead. This method is expected to be removed in solana-core v1.8.")]
        Task<RequestResult<List<SignatureStatusInfo>>> GetConfirmedSignaturesForAddress2Async(string accountPubKey, ulong limit = 1000,
            string before = null, string until = null, Commitment commitment = Commitment.Finalized);

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
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<List<SignatureStatusInfo>> GetSignaturesForAddress(string accountPubKey, ulong limit = 1000,
            string before = null, string until = null, Commitment commitment = Commitment.Finalized);

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
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        [Obsolete("Please use GetSignaturesForAddress whenever possible instead. This method is expected to be removed in solana-core v1.8.")]
        RequestResult<List<SignatureStatusInfo>> GetConfirmedSignaturesForAddress2(string accountPubKey, ulong limit = 1000,
            string before = null, string until = null, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the status of a list of signatures.
        /// <remarks>
        /// Unless <c>searchTransactionHistory</c> is included, this method only searches the recent status cache of signatures.
        /// </remarks>
        /// </summary>
        /// <param name="transactionHashes">The list of transactions to search status info for.</param>
        /// <param name="searchTransactionHistory">If the node should search for signatures in it's ledger cache.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<List<SignatureStatusInfo>>>> GetSignatureStatusesAsync(List<string> transactionHashes,
            bool searchTransactionHistory = false);

        /// <summary>
        /// Gets the status of a list of signatures.
        /// <remarks>
        /// Unless <c>searchTransactionHistory</c> is included, this method only searches the recent status cache of signatures.
        /// </remarks>
        /// </summary>
        /// <param name="transactionHashes">The list of transactions to search status info for.</param>
        /// <param name="searchTransactionHistory">If the node should search for signatures in it's ledger cache.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<List<SignatureStatusInfo>>> GetSignatureStatuses(List<string> transactionHashes,
            bool searchTransactionHistory = false);

        /// <summary>
        /// Gets the current slot the node is processing
        /// </summary>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ulong>> GetSlotAsync(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the current slot the node is processing
        /// </summary>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ulong> GetSlot(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the current slot leader.
        /// </summary>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<string>> GetSlotLeaderAsync(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the current slot leader.
        /// </summary>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<string> GetSlotLeader(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the slot leaders for a given slot range.
        /// </summary>
        /// <param name="start">The start slot.</param>
        /// <param name="limit">The result limit.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<List<string>>> GetSlotLeadersAsync(ulong start, ulong limit,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the slot leaders for a given slot range.
        /// </summary>
        /// <param name="start">The start slot.</param>
        /// <param name="limit">The result limit.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<List<string>> GetSlotLeaders(ulong start, ulong limit, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the highest slot that the node has a snapshot for.
        /// </summary>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ulong>> GetSnapshotSlotAsync();

        /// <summary>
        /// Gets the highest slot that the node has a snapshot for.
        /// </summary>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ulong> GetSnapshotSlot();

        /// <summary>
        /// Gets the epoch activation information for a stake account.
        /// </summary>
        /// <param name="publicKey">Public key of account to query, as base-58 encoded string</param>
        /// <param name="epoch">Epoch for which to calculate activation details.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<StakeActivationInfo>> GetStakeActivationAsync(string publicKey, ulong epoch = 0,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the epoch activation information for a stake account.
        /// </summary>
        /// <param name="publicKey">Public key of account to query, as base-58 encoded string</param>
        /// <param name="epoch">Epoch for which to calculate activation details.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<StakeActivationInfo> GetStakeActivation(string publicKey, ulong epoch = 0,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets information about the current supply.
        /// </summary>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<Supply>>> GetSupplyAsync(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets information about the current supply.
        /// </summary>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<Supply>> GetSupply(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the token balance of an SPL Token account.
        /// </summary>
        /// <param name="splTokenAccountPublicKey">Public key of Token account to query, as base-58 encoded string.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<TokenBalance>>> GetTokenAccountBalanceAsync(string splTokenAccountPublicKey,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the token balance of an SPL Token account.
        /// </summary>
        /// <param name="splTokenAccountPublicKey">Public key of Token account to query, as base-58 encoded string.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<TokenBalance>> GetTokenAccountBalance(string splTokenAccountPublicKey,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets all SPL Token accounts by approved delegate.
        /// </summary>
        /// <param name="ownerPubKey">Public key of account owner query, as base-58 encoded string.</param>
        /// <param name="tokenMintPubKey">Public key of the specific token Mint to limit accounts to, as base-58 encoded string.</param>
        /// <param name="tokenProgramId">Public key of the Token program ID that owns the accounts, as base-58 encoded string.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<List<TokenAccount>>>> GetTokenAccountsByDelegateAsync(
            string ownerPubKey, string tokenMintPubKey = null, string tokenProgramId = null, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets all SPL Token accounts by approved delegate.
        /// </summary>
        /// <param name="ownerPubKey">Public key of account owner query, as base-58 encoded string.</param>
        /// <param name="tokenMintPubKey">Public key of the specific token Mint to limit accounts to, as base-58 encoded string.</param>
        /// <param name="tokenProgramId">Public key of the Token program ID that owns the accounts, as base-58 encoded string.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<List<TokenAccount>>> GetTokenAccountsByDelegate(
            string ownerPubKey, string tokenMintPubKey = null, string tokenProgramId = null, Commitment commitment = Commitment.Finalized);

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
        /// Gets all SPL Token accounts by token owner.
        /// </summary>
        /// <param name="ownerPubKey">Public key of account owner query, as base-58 encoded string.</param>
        /// <param name="tokenMintPubKey">Public key of the specific token Mint to limit accounts to, as base-58 encoded string.</param>
        /// <param name="tokenProgramId">Public key of the Token program ID that owns the accounts, as base-58 encoded string.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<List<TokenAccount>>> GetTokenAccountsByOwner(
            string ownerPubKey, string tokenMintPubKey = null, string tokenProgramId = null, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the 20 largest token accounts of a particular SPL Token.
        /// </summary>
        /// <param name="tokenMintPubKey">Public key of Token Mint to query, as base-58 encoded string.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<List<LargeTokenAccount>>>> GetTokenLargestAccountsAsync(string tokenMintPubKey,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the 20 largest token accounts of a particular SPL Token.
        /// </summary>
        /// <param name="tokenMintPubKey">Public key of Token Mint to query, as base-58 encoded string.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<List<LargeTokenAccount>>> GetTokenLargestAccounts(string tokenMintPubKey,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Get the token supply of an SPL Token type.
        /// </summary>
        /// <param name="tokenMintPubKey">Public key of Token Mint to query, as base-58 encoded string.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<TokenBalance>>> GetTokenSupplyAsync(string tokenMintPubKey,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Get the token supply of an SPL Token type.
        /// </summary>
        /// <param name="tokenMintPubKey">Public key of Token Mint to query, as base-58 encoded string.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<TokenBalance>> GetTokenSupply(string tokenMintPubKey, Commitment commitment = Commitment.Finalized);

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
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<TransactionMetaSlotInfo>> GetTransactionAsync(string signature,
            Commitment commitment = Commitment.Finalized);

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
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        [Obsolete("Please use GetTransactionAsync whenever possible instead. This method is expected to be removed in solana-core v1.8.")]
        Task<RequestResult<TransactionMetaSlotInfo>> GetConfirmedTransactionAsync(string signature, Commitment commitment = Commitment.Finalized);

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
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<TransactionMetaSlotInfo> GetTransaction(string signature, Commitment commitment = Commitment.Finalized);

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
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        [Obsolete("Please use GetTransaction whenever possible instead. This method is expected to be removed in solana-core v1.8.")]
        RequestResult<TransactionMetaSlotInfo> GetConfirmedTransaction(string signature, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the total transaction count of the ledger.
        /// </summary>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ulong>> GetTransactionCountAsync(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the total transaction count of the ledger.
        /// </summary>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ulong> GetTransactionCount(Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the current node's software version info.
        /// </summary>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<NodeVersion>> GetVersionAsync();

        /// <summary>
        /// Gets the current node's software version info.
        /// </summary>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<NodeVersion> GetVersion();

        /// <summary>
        /// Gets the account info and associated stake for all voting accounts in the current bank.
        /// </summary>
        /// <param name="votePubKey">Filter by validator vote address, base-58 encoded string.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<VoteAccounts>> GetVoteAccountsAsync(string votePubKey = null, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the account info and associated stake for all voting accounts in the current bank.
        /// </summary>
        /// <param name="votePubKey">Filter by validator vote address, base-58 encoded string.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<VoteAccounts> GetVoteAccounts(string votePubKey = null, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Requests an airdrop to the passed <c>pubKey</c> of the passed <c>lamports</c> amount.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default <see cref="Commitment.Finalized"/> is used.
        /// </remarks>
        /// </summary>
        /// <param name="pubKey">The public key of to receive the airdrop.</param>
        /// <param name="lamports">The amount of lamports to request.</param>
        /// <param name="commitment">The block commitment used to retrieve block hashes and verify success.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<string>> RequestAirdropAsync(string pubKey, ulong lamports,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Requests an airdrop to the passed <c>pubKey</c> of the passed <c>lamports</c> amount.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default <see cref="Commitment.Finalized"/> is used.
        /// </remarks>
        /// </summary>
        /// <param name="pubKey">The public key of to receive the airdrop.</param>
        /// <param name="lamports">The amount of lamports to request.</param>
        /// <param name="commitment">The block commitment used to retrieve block hashes and verify success.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<string> RequestAirdrop(string pubKey, ulong lamports,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Sends a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as base-64 encoded string.</param>
        /// <param name="skipPreflight">If true skip the prflight transaction checks (default false).</param>
        /// <param name="preFlightCommitment">The block commitment used for preflight.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<string>> SendTransactionAsync(string transaction, bool skipPreflight = false,
            Commitment preFlightCommitment = Commitment.Finalized);

        /// <summary>
        /// Sends a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as base-64 encoded string.</param>
        /// <param name="skipPreflight">If true skip the prflight transaction checks (default false).</param>
        /// <param name="preFlightCommitment">The block commitment used for preflight.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<string> SendTransaction(string transaction, bool skipPreflight = false,
            Commitment preFlightCommitment = Commitment.Finalized);

        /// <summary>
        /// Sends a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as byte array.</param>
        /// <param name="skipPreflight">If true skip the prflight transaction checks (default false).</param>
        /// <param name="commitment">The block commitment used to retrieve block hashes and verify success.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<string>> SendTransactionAsync(byte[] transaction, bool skipPreflight = false,
             Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Sends a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as byte array.</param>
        /// <param name="skipPreflight">If true skip the prflight transaction checks (default false).</param>
        /// <param name="commitment">The block commitment used to retrieve block hashes and verify success.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<string> SendTransaction(byte[] transaction, bool skipPreflight = false,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Simulate sending a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as a base-64 encoded string.</param>
        /// <param name="sigVerify">If the transaction signatures should be verified 
        /// (default false, conflicts with <c>replaceRecentBlockHash</c>.</param>
        /// <param name="commitment">The block commitment used to retrieve block hashes and verify success.</param>
        /// <param name="replaceRecentBlockhash">If the transaction recent blockhash should be replaced with the most recent blockhash 
        /// (default false, conflicts with <c>sigVerify</c></param>
        /// <param name="accountsToReturn">List of accounts to return, as base-58 encoded strings.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<SimulationLogs>>> SimulateTransactionAsync(string transaction, bool sigVerify = false,
            Commitment commitment = Commitment.Finalized, bool replaceRecentBlockhash = false, IList<string> accountsToReturn = null);

        /// <summary>
        /// Simulate sending a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction base-64 encoded string.</param>
        /// <param name="sigVerify">If the transaction signatures should be verified 
        /// (default false, conflicts with <c>replaceRecentBlockHash</c>.</param>
        /// <param name="commitment">The block commitment used to retrieve block hashes and verify success.</param>
        /// <param name="replaceRecentBlockhash">If the transaction recent blockhash should be replaced with the most recent blockhash 
        /// (default false, conflicts with <c>sigVerify</c></param>
        /// <param name="accountsToReturn">List of accounts to return, as base-58 encoded strings.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<SimulationLogs>> SimulateTransaction(string transaction, bool sigVerify = false,
            Commitment commitment = Commitment.Finalized, bool replaceRecentBlockhash = false, IList<string> accountsToReturn = null);

        /// <summary>
        /// Simulate sending a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as a byte array.</param>
        /// <param name="sigVerify">If the transaction signatures should be verified 
        /// (default false, conflicts with <c>replaceRecentBlockHash</c>.</param>
        /// <param name="commitment">The block commitment used to retrieve block hashes and verify success.</param>
        /// <param name="replaceRecentBlockhash">If the transaction recent blockhash should be replaced with the most recent blockhash 
        /// (default false, conflicts with <c>sigVerify</c></param>
        /// <param name="accountsToReturn">List of accounts to return, as base-58 encoded strings.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        Task<RequestResult<ResponseValue<SimulationLogs>>> SimulateTransactionAsync(byte[] transaction, bool sigVerify = false,
            Commitment commitment = Commitment.Finalized, bool replaceRecentBlockhash = false, IList<string> accountsToReturn = null);

        /// <summary>
        /// Simulate sending a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as a byte array.</param>
        /// <param name="sigVerify">If the transaction signatures should be verified 
        /// (default false, conflicts with <c>replaceRecentBlockHash</c>.</param>
        /// <param name="commitment">The block commitment used to retrieve block hashes and verify success.</param>
        /// <param name="replaceRecentBlockhash">If the transaction recent blockhash should be replaced with the most recent blockhash 
        /// (default false, conflicts with <c>sigVerify</c></param>
        /// <param name="accountsToReturn">List of accounts to return, as base-58 encoded strings.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<SimulationLogs>> SimulateTransaction(byte[] transaction, bool sigVerify = false,
            Commitment commitment = Commitment.Finalized, bool replaceRecentBlockhash = false, IList<string> accountsToReturn = null);
    }
}