// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

using System.Text.Json.Serialization;

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
        public long BlockHeight { get; set; }

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
    /// Represents the transaction, metadata and its containing slot.
    /// </summary>
    public class TransactionMetaSlotInfo : TransactionMetaInfo
    {
        /// <summary>
        /// The slot this transaction was processed in.
        /// </summary>
        public ulong Slot { get; set; }

        /// <summary>
        /// Estimated block production time.
        /// </summary>
        public long? BlockTime { get; set; }
    }


    /// <summary>
    /// Represents the tuple transaction and metadata.
    /// </summary>
    public class TransactionMetaInfo
    {
        /// <summary>
        /// The transaction information.
        /// </summary>
        public TransactionInfo Transaction { get; set; }

        /// <summary>
        /// The metadata information.
        /// </summary>
        public TransactionMeta Meta { get; set; }
    }

    /// <summary>
    /// Represents the reward information related to a given account.
    /// </summary>
    public class RewardInfo
    {
        /// <summary>
        /// The account pubkey as base58 encoded string.
        /// </summary>
        public string Pubkey { get; set; }
        /// <summary>
        /// Number of reward lamports credited or debited by the account.
        /// </summary>
        public long Lamports { get; set; }

        /// <summary>
        /// Account balance in lamports after the reward was applied.
        /// </summary>
        public ulong PostBalance { get; set; }

        /// <summary>
        /// Type of the reward.
        /// </summary>
        public RewardType RewardType { get; set; }
    }

    /// <summary>
    /// The type of the reward.
    /// </summary>
    public enum RewardType
    {
        /// <summary>
        /// Default value in case the returned value is undefined.
        /// </summary>
        Unknown,

        /// <summary>
        /// Fee reward.
        /// </summary>
        Fee,
        
        /// <summary>
        /// Rent reward.
        /// </summary>
        Rent,
        
        /// <summary>
        /// Voting reward.
        /// </summary>
        Voting,

        /// <summary>
        /// Staking reward.
        /// </summary>
        Staking
    }

    /// <summary>
    /// Represents a transaction.
    /// </summary>
    public class TransactionInfo
    {
        /// <summary>
        /// The signatures of this transaction.
        /// </summary>
        public string[] Signatures { get; set; }

        /// <summary>
        /// The message contents of the transaction.
        /// </summary>
        public TransactionContentInfo Message { get; set; }
    }

    /// <summary>
    /// Represents the contents of the trasaction.
    /// </summary>
    public class TransactionContentInfo
    {
        /// <summary>
        /// List of base-58 encoded public keys used by the transaction, including by the instructions and for signatures.
        /// </summary>
        public string[] AccountKeys { get; set; }

        /// <summary>
        /// Details the account types and signatures required by the transaction.
        /// </summary>
        public TransactionHeaderInfo Header { get; set; }

        /// <summary>
        ///  A base-58 encoded hash of a recent block in the ledger used to prevent transaction duplication and to give transactions lifetimes.
        /// </summary>
        public string RecentBlockhash { get; set; }

        /// <summary>
        /// List of program instructions that will be executed in sequence and committed in one atomic transaction if all succeed.
        /// </summary>
        public InstructionInfo[] Instructions { get; set; }
    }

    /// <summary>
    /// Details the number and type of accounts and signatures in a given transaction.
    /// </summary>
    public class TransactionHeaderInfo
    {
        /// <summary>
        /// The total number of signatures required to make the transaction valid. 
        /// </summary>
        public int NumRequiredSignatures { get; set; }

        /// <summary>
        /// The last NumReadonlySignedAccounts of the signed keys are read-only accounts.
        /// </summary>
        public int NumReadonlySignedAccounts { get; set; }

        /// <summary>
        /// The last NumReadonlyUnsignedAccounts of the unsigned keys are read-only accounts.
        /// </summary>
        public int NumReadonlyUnsignedAccounts { get; set; }
    }

    /// <summary>
    /// Represents the transaction metadata.
    /// </summary>
    public class TransactionMeta
    {
        /// <summary>
        /// Possible transaction error.
        /// </summary>
        [JsonPropertyName("err")]
        public TransactionError Error { get; set; }

        /// <summary>
        /// Fee this transaction was charged.
        /// </summary>
        public ulong Fee { get; set; }

        /// <summary>
        /// Collection of account balances from before the transaction was processed.
        /// </summary>
        public ulong[] PreBalances { get; set; }

        /// <summary>
        /// Collection of account balances after the transaction was processed.
        /// </summary>
        public ulong[] PostBalances { get; set; }

        /// <summary>
        /// List of inner instructions or omitted if inner instruction recording was not yet enabled during this transaction.
        /// </summary>
        public InnerInstruction[] InnerInstructions { get; set; }

        /// <summary>
        /// List of token balances from before the transaction was processed or omitted if token balance recording was not yet enabled during this transaction.
        /// </summary>
        public TokenBalanceInfo[] PreTokenBalances { get; set; }

        /// <summary>
        /// List of token balances from after the transaction was processed or omitted if token balance recording was not yet enabled during this transaction.
        /// </summary>
        public TokenBalanceInfo[] PostTokenBalances { get; set; }

        /// <summary>
        /// Array of string log messages or omitted if log message recording was not yet enabled during this transaction.
        /// </summary>
        public string[] LogMessages { get; set; }
    }

    /// <summary>
    /// Represents the structure of a token balance metadata for a transaction.
    /// </summary>
    public class TokenBalanceInfo
    {
        /// <summary>
        /// Index of the account in which the token balance is provided for.
        /// </summary>
        public int AccountIndex { get; set; }

        /// <summary>
        /// Pubkey of the token's mint.
        /// </summary>
        public string Mint { get; set; }

        /// <summary>
        /// Token balance details.
        /// </summary>
        public TokenBalance UiTokenAmount { get; set; }
    }

    /// <summary>
    /// Represents an inner instruction. Inner instruction are cross-program instructions that are invoked during transaction processing.
    /// </summary>
    public class InnerInstruction
    {
        /// <summary>
        /// Index of the transaction instruction from which the inner instruction(s) originated
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// List of program instructions that will be executed in sequence and committed in one atomic transaction if all succeed.
        /// </summary>
        public InstructionInfo[] Instructions { get; set; }
    }

    /// <summary>
    /// Represents the data of given instruction.
    /// </summary>
    public class InstructionInfo
    {
        /// <summary>
        /// Index into the <i>Message.AccountKeys</i> array indicating the program account that executes this instruction.
        /// </summary>
        public int ProgramIdIndex { get; set; }

        /// <summary>
        /// List of ordered indices into the <i>Message.AccountKeys</i> array indicating which accounts to pass to the program.
        /// </summary>
        public int[] Accounts { get; set; }

        /// <summary>
        /// The program input data encoded in a base-58 string.
        /// </summary>
        public string Data { get; set; }
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
    /// Represents block hash info.
    /// </summary>
    public class BlockHash
    {
        /// <summary>
        /// A base-58 encoded public key representing the block hash.
        /// </summary>
        public string Blockhash { get; set; }

        /// <summary>
        /// The fee calculator data.
        /// </summary>
        public FeeCalculator FeeCalculator { get; set; }
    }
}