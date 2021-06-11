using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
{

    [JsonConverter(typeof(TransactionErrorJsonConverter))]
    public class TransactionError
    {
        public TransactionErrorType Type { get; set; }

        public InstructionError InstructionError { get; set; }
    }

    public enum TransactionErrorType
    {
        AccountInUse,
        AccountLoadedTwice,
        AccountNotFound,
        ProgramAccountNotFound,
        InsufficientFundsForFee,
        InvalidAccountForFee,
        AlreadyProcessed,
        BlockhashNotFound,
        InstructionError, // (u8, InstructionError)
        CallChainTooDeep,
        MissingSignatureForFee,
        InvalidAccountIndex,
        SignatureFailure,
        InvalidProgramForExecution,
        SanitizeFailure,
        ClusterMaintenance,
        AccountBorrowOutstanding
    }
}