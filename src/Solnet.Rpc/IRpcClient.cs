using System;
using System.Threading.Tasks;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;

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
        /// Gets the account info using base64 encoding.
        /// </summary>
        /// <param name="pubKey">The account public key.</param>
        /// <returns>A task which may return a request result holding the context and account info.</returns>
        Task<RequestResult<ResponseValue<AccountInfo>>> GetAccountInfoAsync(string pubKey);

        /// <inheritdoc cref="SolanaRpcClient.GetAccountInfoAsync"/>
        RequestResult<ResponseValue<AccountInfo>> GetAccountInfo(string pubKey);

        /// <summary>
        /// Gets the balance for a certain public key.
        /// </summary>
        /// <param name="pubKey">The public key.</param>
        /// <returns>A task which may return a request result holding the context and address balance.</returns>
        Task<RequestResult<ResponseValue<ulong>>> GetBalanceAsync(string pubKey);

        /// <inheritdoc cref="SolanaRpcClient.GetBalanceAsync"/>
        RequestResult<ResponseValue<ulong>> GetBalance(string pubKey);

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
        Task<RequestResult<ClusterNode[]>> GetClusterNodesAsync();

        /// <inheritdoc cref="SolanaRpcClient.GetClusterNodesAsync"/>
        RequestResult<ClusterNode[]> GetClusterNodes();

        /// <summary>
        /// Gets identity and transaction information about a confirmed block, identified by slot.
        /// </summary>
        /// <returns>A task which may return a request result and information about the nodes participating in the cluster.</returns>
        Task<RequestResult<ClusterNode[]>> GetConfirmedBlockAsync(ulong slot);

        /// <inheritdoc cref="SolanaRpcClient.GetConfirmedBlockAsync"/>
        RequestResult<ClusterNode[]> GetConfirmedBlock(ulong slot);

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
        /// <returns>A task which may return a request result and a list of objects representing the inflation reward.</returns>
        Task<RequestResult<InflationReward[]>> GetInflationRewardAsync(string[] addresses, ulong epoch = 0);

        /// <inheritdoc cref="SolanaRpcClient.GetInflationRewardAsync"/>
        RequestResult<InflationReward[]> GetInflationReward(string[] addresses, ulong epoch = 0);

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
        /// <returns>A task which may return a request result and information about largest accounts.</returns>
        Task<RequestResult<ResponseValue<TokenBalance>>> GetTokenAccountBalanceAsync(string splTokenAccountPublicKey);

        /// <inheritdoc cref="SolanaRpcClient.GetTokenAccountBalanceAsync"/>
        RequestResult<ResponseValue<TokenBalance>> GetTokenAccountBalance(string splTokenAccountPublicKey);

        /// <summary>
        /// Gets all SPL Token accounts by approved delegate.
        /// </summary>
        /// <param name="ownerPubKey">Public key of account owner query, as base-58 encoded string.</param>
        /// <param name="tokenMintPubKey">Public key of the specific token Mint to limit accounts to, as base-58 encoded string.</param>
        /// <param name="tokenProgramId">Public key of the Token program ID that owns the accounts, as base-58 encoded string.</param>
        /// <returns>A task which may return a request result and information about largest accounts.</returns>
        Task<RequestResult<ResponseValue<TokenAccount[]>>> GetTokenAccountsByDelegateAsync(
            string ownerPubKey, string tokenMintPubKey = "", string tokenProgramId = "");

        /// <inheritdoc cref="SolanaRpcClient.GetTokenAccountsByDelegateAsync"/>
        RequestResult<ResponseValue<TokenAccount[]>> GetTokenAccountsByDelegate(
            string ownerPubKey, string tokenMintPubKey = "", string tokenProgramId = "");

        /// <summary>
        /// Gets all SPL Token accounts by token owner.
        /// </summary>
        /// <param name="ownerPubKey">Public key of account owner query, as base-58 encoded string.</param>
        /// <param name="tokenMintPubKey">Public key of the specific token Mint to limit accounts to, as base-58 encoded string.</param>
        /// <param name="tokenProgramId">Public key of the Token program ID that owns the accounts, as base-58 encoded string.</param>
        /// <returns>A task which may return a request result and information about largest accounts.</returns>
        Task<RequestResult<ResponseValue<TokenAccount[]>>> GetTokenAccountsByOwnerAsync(
            string ownerPubKey, string tokenMintPubKey = "", string tokenProgramId = "");

        /// <inheritdoc cref="SolanaRpcClient.GetTokenAccountsByOwnerAsync"/>
        RequestResult<ResponseValue<TokenAccount[]>> GetTokenAccountsByOwner(
            string ownerPubKey, string tokenMintPubKey = "", string tokenProgramId = "");

        /// <summary>
        /// Gets the 20 largest accounts of a particular SPL Token.
        /// </summary>
        /// <param name="tokenMintPubKey">Public key of Token Mint to query, as base-58 encoded string.</param>
        /// <returns>A task which may return a request result and information about largest accounts.</returns>
        Task<RequestResult<ResponseValue<LargeAccount[]>>> GetTokenLargestAccountsAsync(string tokenMintPubKey);

        /// <inheritdoc cref="SolanaRpcClient.GetTokenLargestAccountsAsync"/>
        RequestResult<ResponseValue<LargeAccount[]>> GetTokenLargestAccounts(string tokenMintPubKey);

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