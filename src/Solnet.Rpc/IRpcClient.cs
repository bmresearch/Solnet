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
        /// Gets the account info using base64 encoding. This is an asynchronous operation.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// </summary>
        /// <param name="pubKey">The account public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>A task which may return a request result holding the context and account info.</returns>
        Task<RequestResult<ResponseValue<AccountInfo>>> GetAccountInfoAsync(string pubKey, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the account info using base64 encoding. This is a synchronous operation.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// </summary>
        /// <param name="pubKey">The account public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<AccountInfo>> GetAccountInfo(string pubKey, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Returns all accounts owned by the provided program Pubkey. This is an asynchronous operation.
        /// </summary>
        /// <param name="pubKey">The program public key.</param>
        /// <returns>A task which may return a request result holding the context and account info.</returns>
        Task<RequestResult<List<AccountKeyPair>>> GetProgramAccountsAsync(string pubKey);

        /// <summary>
        /// Returns all accounts owned by the provided program Pubkey. This is a synchronous operation.
        /// </summary>
        /// <param name="pubKey">The program public key.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<List<AccountKeyPair>> GetProgramAccounts(string pubKey);

        /// <summary>
        /// Gets the account info for multiple accounts. This is an asynchronous operation.
        /// </summary>
        /// <param name="accounts">The list of the accounts public keys.</param>
        /// <returns>A task which may return a request result holding the context and account info.</returns>
        Task<RequestResult<ResponseValue<List<AccountInfo>>>> GetMultipleAccountsAsync(IList<string> accounts);

        /// <summary>
        /// Gets the account info for multiple accounts. This is a synchronous operation.
        /// </summary>
        /// <param name="accounts">The list of the accounts public keys.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<List<AccountInfo>>> GetMultipleAccounts(IList<string> accounts);

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
        /// Returns identity and transaction information about a confirmed block in the ledger. This is an asynchronous operation.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<BlockInfo>> GetBlockAsync(ulong slot, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Returns identity and transaction information about a confirmed block in the ledger. This is a synchronous operation.
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<BlockInfo> GetBlock(ulong slot, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Returns recent block production information from the current or previous epoch. This is an asynchronous operation.
        /// </summary>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<BlockProductionInfo>>> GetBlockProductionAsync();

        /// <summary>
        /// Returns recent block production information from the current or previous epoch. This is a synchronous operation.
        /// </summary>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<BlockProductionInfo>> GetBlockProduction();

        /// <summary>
        /// Returns recent block production information from the current or previous epoch. This is an asynchronous operation.
        /// </summary>
        /// <param name="firstSlot">The first slot to return production information (inclusive).</param>
        /// <param name="lastSlot">The last slot to return production information (inclusive and optional).</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<BlockProductionInfo>>> GetBlockProductionAsync(ulong firstSlot, ulong lastSlot = 0);

        /// <summary>
        /// Returns recent block production information from the current or previous epoch. This is a synchronous operation.
        /// </summary>
        /// <param name="firstSlot">The first slot to return production information (inclusive).</param>
        /// <param name="lastSlot">The last slot to return production information (inclusive and optional).</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<BlockProductionInfo>> GetBlockProduction(ulong firstSlot, ulong lastSlot = 0);

        /// <summary>
        /// Returns recent block production information from the current or previous epoch. This is an asynchronous operation.
        /// </summary>
        /// <param name="identity">Filter production details only for this given validator.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<BlockProductionInfo>>> GetBlockProductionAsync(string identity);

        /// <summary>
        /// Returns recent block production information from the current or previous epoch. This is a synchronous operation.
        /// </summary>
        /// <param name="identity">Filter production details only for this given validator.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<BlockProductionInfo>> GetBlockProduction(string identity);

        /// <summary>
        /// Returns recent block production information from the current or previous epoch. This is an asynchronous operation.
        /// </summary>
        /// <param name="identity">Filter production details only for this given validator.</param>
        /// <param name="firstSlot">The first slot to return production information (inclusive).</param>
        /// <param name="lastSlot">The last slot to return production information (inclusive and optional).</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ResponseValue<BlockProductionInfo>>> GetBlockProductionAsync(string identity, ulong firstSlot, ulong lastSlot = 0);

        /// <summary>
        /// Returns recent block production information from the current or previous epoch. This is a synchronous operation.
        /// </summary>
        /// <param name="identity">Filter production details only for this given validator.</param>
        /// <param name="firstSlot">The first slot to return production information (inclusive).</param>
        /// <param name="lastSlot">The last slot to return production information (inclusive and optional).</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ResponseValue<BlockProductionInfo>> GetBlockProduction(string identity, ulong firstSlot, ulong lastSlot = 0);

        /// <summary>
        /// Returns transaction details for a confirmed transaction. This is an asynchronous operation.
        /// </summary>
        /// <param name="identity">ransaction signature as base-58 encoded string.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<TransactionMetaSlotInfo>> GetTransactionAsync(string signature);

        /// <summary>
        /// Returns transaction details for a confirmed transaction. This is a synchronous operation.
        /// </summary>
        /// <param name="identity">Transaction signature as base-58 encoded string.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<TransactionMetaSlotInfo> GetTransaction(string signature);

        /// <summary>
        /// Returns a list of confirmed blocks between two slots. This is a synchronous operation.
        /// </summary>
        /// <param name="startSlot">The start slot (inclusive).</param>
        /// <param name="endSlot">The start slot (inclusive and optional).</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<List<ulong>> GetBlocks(ulong startSlot, ulong endSlot = 0);

        /// <summary>
        /// Returns a list of confirmed blocks between two slots. This is an asynchronous operation.
        /// </summary>
        /// <param name="startSlot">The start slot (inclusive).</param>
        /// <param name="endSlot">The start slot (inclusive and optional).</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<List<ulong>>> GetBlocksAsync(ulong startSlot, ulong endSlot = 0);

        /// <summary>
        /// Returns a list of confirmed blocks starting at the given slot. This is a synchronous operation.
        /// </summary>
        /// <param name="startSlot">The start slot (inclusive).</param>
        /// <param name="limit">The max number of blocks to return.</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<List<ulong>> GetBlocksWithLimit(ulong startSlot, ulong limit);

        /// <summary>
        /// Returns a list of confirmed blocks starting at the given slot. This is an asynchronous operation.
        /// </summary>
        /// <param name="startSlot">The start slot (inclusive).</param>
        /// <param name="limit">The max number of blocks to return.</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<List<ulong>>> GetBlocksWithLimitAsync(ulong startSlot, ulong limit);

        /// <summary>
        /// Returns the slot of the lowest confirmed block that has not been purged from the ledger. This is a synchronous operation.
        /// </summary>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<ulong> GetFirstAvailableBlock();

        /// <summary>
        /// Returns the slot of the lowest confirmed block that has not been purged from the ledger. This is an asynchronous operation.
        /// </summary>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<ulong>> GetFirstAvailableBlockAsync();

        /// <summary>
        /// Returns the current health of the node. 
        /// This method should return the strink 'ok' if the node is healthy, or the error code along with any information provided otherwise. 
        /// This is a synchronous operation.
        /// </summary>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<string> GetHealth();

        /// <summary>
        /// Returns the current health of the node. 
        /// This method should return the strink 'ok' if the node is healthy, or the error code along with any information provided otherwise. 
        /// This is an asynchronous operation.
        /// </summary>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<string>> GetHealthAsync();

        /// <summary>
        /// Returns the leader schedule for an epoch. This is a synchronous operation.
        /// </summary>
        /// <param name="slot">Fetch the leader schedule for the epoch that corresponds to the provided slot. 
        /// If unspecified, the leader schedule for the current epoch is fetched.</param>
        /// <param name="identity">Filter results for this validator only (base 58 encoded string and optional).</param>
        /// <returns>Returns an object that wraps the result along with possible errors with the request.</returns>
        RequestResult<Dictionary<string, List<ulong>>> GetLeaderSchedule(ulong slot = 0, string identity = null);

        /// <summary>
        /// Returns the leader schedule for an epoch. This is an asynchronous operation.
        /// </summary>
        /// <param name="slot">Fetch the leader schedule for the epoch that corresponds to the provided slot. 
        /// If unspecified, the leader schedule for the current epoch is fetched.</param>
        /// <param name="identity">Filter results for this validator only (base 58 encoded string and optional).</param>
        /// <returns>Returns a task that holds the asynchronous operation result and state.</returns>
        Task<RequestResult<Dictionary<string, List<ulong>>>> GetLeaderScheduleAsync(ulong slot = 0, string identity = null);

        /// <summary>
        /// Gets the block commitment of a certain block, identified by slot.
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <returns>A task which may return a request result and block commitment information.</returns>
        Task<RequestResult<BlockCommitment>> GetBlockCommitmentAsync(ulong slot);

        /// <inheritdoc cref="SolanaRpcClient.GetBlockCommitmentAsync"/>
        RequestResult<BlockCommitment> GetBlockCommitment(ulong slot);

        /// <summary>
        /// Gets the estimated production time for a certain block, identified by slot.
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <returns>A task which may return a request result and block production time as Unix timestamp (seconds since Epoch).</returns>
        Task<RequestResult<ulong>> GetBlockTimeAsync(ulong slot);

        /// <inheritdoc cref="SolanaRpcClient.GetBlockTimeAsync"/>
        RequestResult<ulong> GetBlockTime(ulong slot);

        /// <summary>
        /// Gets the current block height of the node.
        /// </summary>
        /// <returns>A task which may return a request result and a block height.</returns>
        Task<RequestResult<ulong>> GetBlockHeightAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetBlockHeightAsync"/>
        RequestResult<ulong> GetBlockHeight();

        /// <summary>
        /// Gets the cluster nodes.
        /// </summary>
        /// <returns>A task which may return a request result and information about the nodes participating in the cluster.</returns>
        Task<RequestResult<List<ClusterNode>>> GetClusterNodesAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetClusterNodesAsync"/>
        RequestResult<List<ClusterNode>> GetClusterNodes();

        /// <summary>
        /// Gets information about the current epoch.
        /// </summary>
        /// <returns>A task which may return a request result and information about the current epoch.</returns>
        Task<RequestResult<EpochInfo>> GetEpochInfoAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetEpochInfoAsync"/>
        RequestResult<EpochInfo> GetEpochInfo();

        /// <summary>
        /// Gets epoch schedule information from this cluster's genesis config.
        /// </summary>
        /// <returns>A task which may return a request result and epoch schedule information from this cluster's genesis config.</returns>
        Task<RequestResult<EpochScheduleInfo>> GetEpochScheduleAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetEpochScheduleAsync"/>
        RequestResult<EpochScheduleInfo> GetEpochSchedule();

        /// <summary>
        /// Gets the fee calculator associated with the query blockhash, or null if the blockhash has expired.
        /// </summary>
        /// <param name="blockhash">The blockhash to query, as base-58 encoded string.</param>
        /// <returns>A task which may return a request result and the fee calculator for the block.</returns>
        Task<RequestResult<ResponseValue<FeeCalculatorInfo>>> GetFeeCalculatorForBlockhashAsync(string blockhash);

        /// <inheritdoc cref="SolanaRpcClient.GetFeeCalculatorForBlockhashAsync"/>
        RequestResult<ResponseValue<FeeCalculatorInfo>> GetFeeCalculatorForBlockhash(string blockhash);

        /// <summary>
        /// Gets the fee rate governor information from the root bank.
        /// </summary>
        /// <returns>A task which may return a request result and the fee rate governor.</returns>
        Task<RequestResult<ResponseValue<FeeRateGovernorInfo>>> GetFeeRateGovernorAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetFeeRateGovernorAsync"/>
        RequestResult<ResponseValue<FeeRateGovernorInfo>> GetFeeRateGovernor();

        /// <summary>
        /// Gets a recent block hash from the ledger, a fee schedule that can be used to compute the
        /// cost of submitting a transaction using it, and the last slot in which the blockhash will be valid.
        /// </summary>
        /// <returns>A task which may return a request result and information about fees.</returns>
        Task<RequestResult<ResponseValue<FeesInfo>>> GetFeesAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetFeesAsync"/>
        RequestResult<ResponseValue<FeesInfo>> GetFees();

        /// <summary>
        /// Gets a recent block hash.
        /// </summary>
        /// <returns>A task which may return a request result and recent block hash.</returns>
        Task<RequestResult<ResponseValue<BlockHash>>> GetRecentBlockHashAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetBlockTimeAsync"/>
        RequestResult<ResponseValue<BlockHash>> GetRecentBlockHash();

        /// <summary>
        /// Gets the minimum balance required to make account rent exempt.
        /// </summary>
        /// <param name="accountDataSize">The account data size.</param>
        /// <returns>A task which may return a request result and the rent exemption value.</returns>
        Task<RequestResult<ulong>> GetMinimumBalanceForRentExemptionAsync(long accountDataSize);

        /// <inheritdoc cref="SolanaRpcClient.GetMinimumBalanceForRentExemptionAsync"/>
        RequestResult<ulong> GetMinimumBalanceForRentExemption(long accountDataSize);

        /// <summary>
        /// Gets the genesis hash of the ledger.
        /// </summary>
        /// <returns>A task which may return a request result and a string representing the genesis hash.</returns>
        Task<RequestResult<string>> GetGenesisHashAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetGenesisHashAsync"/>
        RequestResult<string> GetGenesisHash();

        /// <summary>
        /// Gets the identity pubkey for the current node.
        /// </summary>
        /// <returns>A task which may return a request result and an object with the identity public key.</returns>
        Task<RequestResult<NodeIdentity>> GetIdentityAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetIdentityAsync"/>
        RequestResult<NodeIdentity> GetIdentity();

        /// <summary>
        /// Gets the current inflation governor.
        /// </summary>
        /// <returns>A task which may return a request result and an object representing the current inflation governor.</returns>
        Task<RequestResult<InflationGovernor>> GetInflationGovernorAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetInflationGovernorAsync"/>
        RequestResult<InflationGovernor> GetInflationGovernor();

        /// <summary>
        /// Gets the specific inflation values for the current epoch.
        /// </summary>
        /// <returns>A task which may return a request result and an object representing the current inflation rate.</returns>
        Task<RequestResult<InflationRate>> GetInflationRateAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetInflationRateAsync"/>
        RequestResult<InflationRate> GetInflationRate();

        /// <summary>
        /// Gets the inflation reward for a list of addresses for an epoch.
        /// </summary>
        /// <param name="addresses">The list of addresses to query, as base-58 encoded strings.</param>
        /// <param name="epoch">The epoch.</param>
        /// <returns>A task which may return a request result and a list of objects representing the inflation reward.</returns>
        Task<RequestResult<List<InflationReward>>> GetInflationRewardAsync(List<string> addresses, ulong epoch = 0);

        /// <inheritdoc cref="SolanaRpcClient.GetInflationRewardAsync"/>
        RequestResult<List<InflationReward>> GetInflationReward(List<string> addresses, ulong epoch = 0);

        /// <summary>
        /// Gets the 20 largest accounts, by lamport balance.
        /// </summary>
        /// <param name="filter">Filter results by account type.</param>
        /// <returns>A task which may return a request result and a list about the largest accounts.</returns>
        /// <remarks>Results may be cached up to two hours.</remarks>
        Task<RequestResult<ResponseValue<List<LargeAccount>>>> GetLargestAccountsAsync(string filter);

        /// <inheritdoc cref="SolanaRpcClient.GetInflationRewardAsync"/>
        RequestResult<ResponseValue<List<LargeAccount>>> GetLargestAccounts(string filter);

        /// <summary>
        /// Gets the maximum slot seen from retransmit stage.
        /// </summary>
        /// <returns>A task which may return a request result the maximum slot.</returns>
        Task<RequestResult<ulong>> GetMaxRetransmitSlotAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetMaxRetransmitSlotAsync"/>
        RequestResult<ulong> GetMaxRetransmitSlot();

        /// <summary>
        /// Gets the maximum slot seen from after shred insert.
        /// </summary>
        /// <returns>A task which may return a request result the maximum slot.</returns>
        Task<RequestResult<ulong>> GetMaxShredInsertSlotAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetMaxShredInsertSlotAsync"/>
        RequestResult<ulong> GetMaxShredInsertSlot();

        /// <summary>
        /// Gets the highest slot that the node has a snapshot for.
        /// </summary>
        /// <returns>A task which may return a request result the highest slot with a snapshot.</returns>
        Task<RequestResult<ulong>> GetSnapshotSlotAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetSnapshotSlotAsync"/>
        RequestResult<ulong> GetSnapshotSlot();

        /// <summary>
        /// Gets a list of recent performance samples.
        /// <remarks>
        /// Unless <c>searchTransactionHistory</c> is included, this method only searches the recent status cache of signatures.
        /// </remarks>
        /// </summary>
        /// <param name="limit">Maximum transaction signatures to return, between 1-720. Default is 720.</param>
        /// <returns>A task which may return a request result the signatures for the transactions.</returns>
        Task<RequestResult<List<PerformanceSample>>> GetRecentPerformanceSamplesAsync(ulong limit = 720);

        /// <inheritdoc cref="SolanaRpcClient.GetRecentPerformanceSamplesAsync"/>
        RequestResult<List<PerformanceSample>> GetRecentPerformanceSamples(ulong limit = 720);

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
        Task<RequestResult<List<SignatureStatusInfo>>> GetSignaturesForAddressAsync(string accountPubKey, ulong limit = 1000, string before = "", string until = "");

        /// <inheritdoc cref="IRpcClient.GetSignaturesForAddressAsync"/>
        RequestResult<List<SignatureStatusInfo>> GetSignaturesForAddress(string accountPubKey, ulong limit = 1000, string before = "", string until = "");

        /// <summary>
        /// Gets the status of a list of signatures.
        /// <remarks>
        /// Unless <c>searchTransactionHistory</c> is included, this method only searches the recent status cache of signatures.
        /// </remarks>
        /// </summary>
        /// <param name="transactionHashes">The list of transactions to search status info for.</param>
        /// <param name="searchTransactionHistory">If the node should search for signatures in it's ledger cache.</param>
        /// <returns>A task which may return a request result the highest slot with a snapshot.</returns>
        Task<RequestResult<ResponseValue<List<SignatureStatusInfo>>>> GetSignatureStatusesAsync(List<string> transactionHashes,
            bool searchTransactionHistory = false);

        /// <inheritdoc cref="SolanaRpcClient.GetSignatureStatusesAsync"/>
        RequestResult<ResponseValue<List<SignatureStatusInfo>>> GetSignatureStatuses(List<string> transactionHashes,
            bool searchTransactionHistory = false);

        /// <summary>
        /// Gets the current slot the node is processing
        /// </summary>
        /// <returns>A task which may return a request result the current slot.</returns>
        Task<RequestResult<ulong>> GetSlotAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetSlotAsync"/>
        RequestResult<ulong> GetSlot();

        /// <summary>
        /// Gets the current slot leader.
        /// </summary>
        /// <returns>A task which may return a request result the slot leader.</returns>
        Task<RequestResult<string>> GetSlotLeaderAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetSlotLeaderAsync"/>
        RequestResult<string> GetSlotLeader();

        /// <summary>
        /// Gets the slot leaders for a given slot range.
        /// </summary>
        /// <param name="start">The start slot.</param>
        /// <param name="limit">The result limit.</param>
        /// <returns>A task which may return a request result and a list slot leaders.</returns>
        Task<RequestResult<List<string>>> GetSlotLeadersAsync(ulong start, ulong limit);

        /// <inheritdoc cref="SolanaRpcClient.GetSlotLeadersAsync"/>
        RequestResult<List<string>> GetSlotLeaders(ulong start, ulong limit);

        /// <summary>
        /// Gets the epoch activation information for a stake account.
        /// </summary>
        /// <param name="publicKey">Public key of account to query, as base-58 encoded string</param>
        /// <param name="epoch">Epoch for which to calculate activation details.</param>
        /// <returns>A task which may return a request result and information about the stake activation.</returns>
        Task<RequestResult<StakeActivationInfo>> GetStakeActivationAsync(string publicKey, ulong epoch = 0);

        /// <inheritdoc cref="SolanaRpcClient.GetStakeActivationAsync"/>
        RequestResult<StakeActivationInfo> GetStakeActivation(string publicKey, ulong epoch = 0);

        /// <summary>
        /// Gets information about the current supply.
        /// </summary>
        /// <returns>A task which may return a request result and information about the supply.</returns>
        Task<RequestResult<ResponseValue<Supply>>> GetSupplyAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetSupplyAsync"/>
        RequestResult<ResponseValue<Supply>> GetSupply();

        /// <summary>
        /// Gets the token balance of an SPL Token account.
        /// </summary>
        /// <param name="splTokenAccountPublicKey">Public key of Token account to query, as base-58 encoded string.</param>
        /// <returns>A task which may return a request result and information about the token account balance.</returns>
        Task<RequestResult<ResponseValue<TokenBalance>>> GetTokenAccountBalanceAsync(string splTokenAccountPublicKey);

        /// <inheritdoc cref="SolanaRpcClient.GetTokenAccountBalanceAsync"/>
        RequestResult<ResponseValue<TokenBalance>> GetTokenAccountBalance(string splTokenAccountPublicKey);

        /// <summary>
        /// Gets all SPL Token accounts by approved delegate.
        /// </summary>
        /// <param name="ownerPubKey">Public key of account owner query, as base-58 encoded string.</param>
        /// <param name="tokenMintPubKey">Public key of the specific token Mint to limit accounts to, as base-58 encoded string.</param>
        /// <param name="tokenProgramId">Public key of the Token program ID that owns the accounts, as base-58 encoded string.</param>
        /// <returns>A task which may return a request result and information about token accounts by delegate</returns>
        Task<RequestResult<ResponseValue<List<TokenAccount>>>> GetTokenAccountsByDelegateAsync(
            string ownerPubKey, string tokenMintPubKey = "", string tokenProgramId = "");

        /// <inheritdoc cref="SolanaRpcClient.GetTokenAccountsByDelegateAsync"/>
        RequestResult<ResponseValue<List<TokenAccount>>> GetTokenAccountsByDelegate(
            string ownerPubKey, string tokenMintPubKey = "", string tokenProgramId = "");

        /// <summary>
        /// Gets all SPL Token accounts by token owner.
        /// </summary>
        /// <param name="ownerPubKey">Public key of account owner query, as base-58 encoded string.</param>
        /// <param name="tokenMintPubKey">Public key of the specific token Mint to limit accounts to, as base-58 encoded string.</param>
        /// <param name="tokenProgramId">Public key of the Token program ID that owns the accounts, as base-58 encoded string.</param>
        /// <returns>A task which may return a request result and information about token accounts by owner.</returns>
        Task<RequestResult<ResponseValue<List<TokenAccount>>>> GetTokenAccountsByOwnerAsync(
            string ownerPubKey, string tokenMintPubKey = "", string tokenProgramId = "");

        /// <inheritdoc cref="SolanaRpcClient.GetTokenAccountsByOwnerAsync"/>
        RequestResult<ResponseValue<List<TokenAccount>>> GetTokenAccountsByOwner(
            string ownerPubKey, string tokenMintPubKey = "", string tokenProgramId = "");

        /// <summary>
        /// Gets the 20 largest token accounts of a particular SPL Token.
        /// </summary>
        /// <param name="tokenMintPubKey">Public key of Token Mint to query, as base-58 encoded string.</param>
        /// <returns>A task which may return a request result and information about largest token accounts.</returns>
        Task<RequestResult<ResponseValue<List<LargeTokenAccount>>>> GetTokenLargestAccountsAsync(string tokenMintPubKey);

        /// <inheritdoc cref="SolanaRpcClient.GetTokenLargestAccountsAsync"/>
        RequestResult<ResponseValue<List<LargeTokenAccount>>> GetTokenLargestAccounts(string tokenMintPubKey);

        /// <summary>
        /// Get the token supply of an SPL Token type.
        /// </summary>
        /// <param name="tokenMintPubKey">Public key of Token Mint to query, as base-58 encoded string.</param>
        /// <returns>A task which may return a request result and information about the supply.</returns>
        Task<RequestResult<ResponseValue<TokenBalance>>> GetTokenSupplyAsync(string tokenMintPubKey);

        /// <inheritdoc cref="SolanaRpcClient.GetTokenSupplyAsync"/>
        RequestResult<ResponseValue<TokenBalance>> GetTokenSupply(string tokenMintPubKey);

        /// <summary>
        /// Gets the total transaction count of the ledger.
        /// </summary>
        /// <returns>A task which may return a request result and information about the transaction count.</returns>
        Task<RequestResult<ulong>> GetTransactionCountAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetTransactionCountAsync"/>
        RequestResult<ulong> GetTransactionCount();


        /// <summary>
        /// Gets the current node's software version info.
        /// </summary>
        /// <returns>A task which may return a request result and information about the current node's software.</returns>
        Task<RequestResult<NodeVersion>> GetVersionAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetVersionAsync"/>
        RequestResult<NodeVersion> GetVersion();

        /// <summary>
        /// Gets the account info and associated stake for all voting accounts in the current bank.
        /// </summary>
        /// <returns>A task which may return a request result and information about the vote accounts.</returns>
        Task<RequestResult<VoteAccounts>> GetVoteAccountsAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetVoteAccountsAsync"/>
        RequestResult<VoteAccounts> GetVoteAccounts();

        /// <summary>
        /// Gets the lowest slot that the node has information about in its ledger.
        /// <remarks>
        /// This value may decrease over time if a node is configured to purging data.
        /// </remarks>
        /// </summary>
        /// <returns>A task which may return a request result and the minimum slot available.</returns>
        Task<RequestResult<ulong>> GetMinimumLedgerSlotAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetMinimumLedgerSlotAsync"/>
        RequestResult<ulong> GetMinimumLedgerSlot();

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
        Task<RequestResult<string>> RequestAirdropAsync(string pubKey, ulong lamports,
            Commitment commitment = Commitment.Finalized);

        /// <inheritdoc cref="SolanaRpcClient.RequestAirdropAsync"/>
        RequestResult<string> RequestAirdrop(string pubKey, ulong lamports,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Sends a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as base-58 or base-64 encoded string.</param>
        /// <param name="encoding">The encoding of the transaction.</param>
        /// <returns>
        /// A task which may return a request result and the first transaction signature embedded in the transaction, as base-58 encoded string.
        /// </returns>
        Task<RequestResult<string>> SendTransactionAsync(string transaction,
            BinaryEncoding encoding = BinaryEncoding.Base64);

        /// <inheritdoc cref="SolanaRpcClient.SendTransactionAsync"/>
        RequestResult<string> SendTransaction(string transaction, BinaryEncoding encoding = BinaryEncoding.Base64);

        /// <summary>
        /// Sends a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as byte array.</param>
        /// <returns>
        /// A task which may return a request result and the first transaction signature embedded in the transaction, as base-58 encoded string.
        /// </returns>
        RequestResult<string> SendTransaction(byte[] transaction);

        /// <summary>
        /// Simulate sending a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as a byte array.</param>
        /// <param name="encoding">The encoding of the transaction.</param>
        /// <returns>
        /// A task which may return a request result and the transaction status.
        /// </returns>
        Task<RequestResult<ResponseValue<SimulationLogs>>> SimulateTransactionAsync(string transaction,
            BinaryEncoding encoding = BinaryEncoding.Base64);

        /// <inheritdoc cref="SolanaRpcClient.SimulateTransactionAsync"/>
        RequestResult<ResponseValue<SimulationLogs>> SimulateTransaction(string transaction,
            BinaryEncoding encoding = BinaryEncoding.Base64);

        /// <summary>
        /// Simulate sending a transaction.
        /// </summary>
        /// <param name="transaction">The signed transaction as a byte array.</param>
        /// <returns>
        /// A task which may return a request result and the transaction status.
        /// </returns>
        RequestResult<ResponseValue<SimulationLogs>> SimulateTransaction(byte[] transaction);
    }
}