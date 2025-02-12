namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents the block info.
    /// </summary>
    public class BlockInfo
    {
        /// <summary>
        /// Estimated block production time.
        /// </summary>
        public long BlockTime { get; set; }

        /// <summary>
        /// A base-58 encoded public key representing the block hash.
        /// </summary>
        public string Blockhash { get; set; }

        /// <summary>
        /// A base-58 encoded public key representing the block hash of this block's parent.
        /// <remarks>
        /// If the parent block is no longer available due to ledger cleanup, this field will return
        /// '11111111111111111111111111111111'
        /// </remarks>
        /// </summary>
        public string PreviousBlockhash { get; set; }

        /// <summary>
        /// The slot index of this block's parent.
        /// </summary>
        public ulong ParentSlot { get; set; }

        /// <summary>
        /// The number of blocks beneath this block.
        /// </summary>
        public long? BlockHeight { get; set; }

        /// <summary>
        /// Max transaction version allowed
        /// </summary>
        public int maxSupportedTransactionVersion { get; set; }

        /// <summary>
        /// The rewards for this given block.
        /// </summary>
        public RewardInfo[] Rewards { get; set; }

        /// <summary>
        /// Collection of transactions and their metadata within this block.
        /// </summary>
        public TransactionMetaInfo[] Transactions { get; set; }
    }

    /// <summary>
    /// Represents the block commitment info.
    /// </summary>
    public class BlockCommitment
    {
        /// <summary>
        /// A list of values representing the amount of cluster stake in lamports that has
        /// voted onn the block at each depth from 0 to (max lockout history + 1).
        /// </summary>
        public ulong[] Commitment { get; set; }

        /// <summary>
        /// Total active stake, in lamports, of the current epoch.
        /// </summary>
        public ulong TotalStake { get; set; }
    }

    /// <summary>
    /// Represents the fee calculator info.
    /// </summary>
    public class FeeCalculator
    {
        /// <summary>
        /// The amount, in lamports, to be paid per signature.
        /// </summary>
        public ulong LamportsPerSignature { get; set; }
    }

    /// <summary>
    /// Represents the fee calculator info.
    /// </summary>
    public class FeeCalculatorInfo
    {
        /// <summary>
        /// The fee calculator info.
        /// </summary>
        public FeeCalculator FeeCalculator { get; set; }
    }

    /// <summary>
    /// Represents block hash info.
    /// </summary>
    public class BlockHash
    {
        /// <summary>
        /// A base-58 encoded string representing the block hash.
        /// </summary>
        public string Blockhash { get; set; }

        /// <summary>
        /// The fee calculator data.
        /// </summary>
        public FeeCalculator FeeCalculator { get; set; }
    }

    /// <summary>
    /// Represents the latest block hash info.
    /// </summary>
    public class LatestBlockHash
    {
        /// <summary>
        /// A base-58 encoded string representing the block hash.
        /// </summary>
        public string Blockhash { get; set; }

        /// <summary>
        /// The last block height at which the blockhash will be valid.
        /// </summary>
        public ulong LastValidBlockHeight { get; set; }
    }
}