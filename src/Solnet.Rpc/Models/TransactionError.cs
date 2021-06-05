using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
