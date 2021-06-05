using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Rpc.Models
{
    public class InstructionError
    {
        public int InstructionIndex { get; set; }

        public InstructionErrorType Type { get; set; }

        public int? CustomError { get; set; }

        public string BorshIoError { get; set; }
    }


    public enum InstructionErrorType
    {
        GenericError,
        InvalidArgument,
        InvalidInstructionData,
        InvalidAccountData,
        AccountDataTooSmall,
        InsufficientFunds,
        IncorrectProgramId,
        MissingRequiredSignature,
        AccountAlreadyInitialized,
        UninitializedAccount,
        UnbalancedInstruction,
        ModifiedProgramId,
        ExternalAccountLamportSpend,
        ExternalAccountDataModified,
        ReadonlyLamportChange,
        ReadonlyDataModified,
        DuplicateAccountIndex,
        ExecutableModified,
        RentEpochModified,
        NotEnoughAccountKeys,
        AccountDataSizeChanged,
        AccountNotExecutable,
        AccountBorrowFailed,
        AccountBorrowOutstanding,
        DuplicateAccountOutOfSync,
        Custom, //(u32)
        InvalidError,
        ExecutableDataModified,
        ExecutableLamportChange,
        ExecutableAccountNotRentExempt,
        UnsupportedProgramId,
        CallDepth,
        MissingAccount,
        ReentrancyNotAllowed,
        MaxSeedLengthExceeded,
        InvalidSeeds,
        InvalidRealloc,
        ComputationalBudgetExceeded,
        PrivilegeEscalation,
        ProgramEnvironmentSetupFailure,
        ProgramFailedToComplete,
        ProgramFailedToCompile,
        Immutable,
        IncorrectAuthority,
        BorshIoError, //(String)
        AccountNotRentExempt,
        InvalidAccountOwner,
        ArithmeticOverflow,
        UnsupportedSysvar
    }
}
