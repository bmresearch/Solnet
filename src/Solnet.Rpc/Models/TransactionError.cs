using Solnet.Rpc.Converters;
using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents a Transaction Error.
    /// </summary>
    [JsonConverter(typeof(TransactionErrorJsonConverter))]
    public class TransactionError
    {
        /// <summary>
        /// The type of transaction error.
        /// </summary>
        public TransactionErrorType Type { get; set; }

        /// <summary>
        /// The inner instruction error, if the <c>Type</c> is <c>TransactionErrorType.InstructionError</c>.
        /// </summary>
        public InstructionError InstructionError { get; set; }
    }

    /// <summary>
    /// The possible types of Transaction errors.
    /// </summary>
    public enum TransactionErrorType
    {
        /// <summary>
        /// An account is already being processed in another transaction in a way
        /// that does not support parallelism
        /// </summary>
        AccountInUse,
        /// <summary>
        /// A `Pubkey` appears twice in the transaction's `account_keys`.  Instructions can reference
        /// `Pubkey`s more than once but the message must contain a list with no duplicate keys
        /// </summary>
        AccountLoadedTwice,
        /// <summary>
        /// Attempt to debit an account but found no record of a prior credit.
        /// </summary>
        AccountNotFound,
        /// <summary>
        /// Attempt to load a program that does not exist
        /// </summary>
        ProgramAccountNotFound,
        /// <summary>
        /// The from `Pubkey` does not have sufficient balance to pay the fee to schedule the transaction
        /// </summary>
        InsufficientFundsForFee,
        /// <summary>
        /// This account may not be used to pay transaction fees
        /// </summary>
        InvalidAccountForFee,
        /// <summary>
        /// The bank has seen this transaction before. This can occur under normal operation
        /// when a UDP packet is duplicated, as a user error from a client not updating
        /// its `recent_blockhash`, or as a double-spend attack.
        /// </summary>
        AlreadyProcessed,
        /// <summary>
        /// The bank has not seen the given `recent_blockhash` or the transaction is too old and
        /// the `recent_blockhash` has been discarded.
        /// </summary>
        BlockhashNotFound,
        /// <summary>
        /// An error occurred while processing an instruction.
        /// </summary>
        InstructionError,
        /// <summary>
        ///  Loader call chain is too deep
        /// </summary>
        CallChainTooDeep,
        /// <summary>
        /// Transaction requires a fee but has no signature present
        /// </summary>
        MissingSignatureForFee,
        /// <summary>
        /// Transaction contains an invalid account reference
        /// </summary>
        InvalidAccountIndex,
        /// <summary>
        /// Transaction did not pass signature verification
        /// </summary>
        SignatureFailure,
        /// <summary>
        /// This program may not be used for executing instructions
        /// </summary>
        InvalidProgramForExecution,
        /// <summary>
        /// Transaction failed to sanitize accounts offsets correctly
        /// implies that account locks are not taken for this TX, and should
        /// not be unlocked.
        /// </summary>
        SanitizeFailure,
        /// <summary>
        /// Transactions are currently disabled due to cluster maintenance
        /// </summary>
        ClusterMaintenance,
        /// <summary>
        /// Transaction processing left an account with an outstanding borrowed reference
        /// </summary>
        AccountBorrowOutstanding
    }
}