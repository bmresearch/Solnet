namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents an Instruction error.
    /// </summary>
    public class InstructionError
    {
        /// <summary>
        /// The index of the instruction that caused the error.
        /// </summary>
        public int InstructionIndex { get; set; }

        /// <summary>
        /// The type of the instruction error.
        /// </summary>
        public InstructionErrorType Type { get; set; }

        /// <summary>
        /// Possible custom error id from a program.
        /// </summary>
        public long? CustomError { get; set; }

        /// <summary>
        /// Possible string from borsh error.
        /// </summary>
        public string BorshIoError { get; set; }
    }

    /// <summary>
    /// Possible Types of instruction errors.
    /// </summary>
    public enum InstructionErrorType
    {
        /// <summary>
        /// The program instruction returned an error. (Deprecated)
        /// </summary>
        GenericError,
        /// <summary>
        /// The arguments provided to a program were invalid
        /// </summary>
        InvalidArgument,
        /// <summary>
        /// 
        /// </summary>An instruction's data contents were invalid
        InvalidInstructionData,
        /// <summary>
        /// An account's data contents was invalid
        /// </summary>
        InvalidAccountData,
        /// <summary>
        /// An account's data was too small
        /// </summary>
        AccountDataTooSmall,
        /// <summary>
        /// An account's balance was too small to complete the instruction
        /// </summary>
        InsufficientFunds,
        /// <summary>
        /// The account did not have the expected program id
        /// </summary>
        IncorrectProgramId,
        /// <summary>
        /// A signature was required but not found
        /// </summary>
        MissingRequiredSignature,
        /// <summary>
        ///  An initialize instruction was sent to an account that has already been initialized.
        /// </summary>
        AccountAlreadyInitialized,
        /// <summary>
        /// An attempt to operate on an account that hasn't been initialized.
        /// </summary>
        UninitializedAccount,
        /// <summary>
        /// Program's instruction lamport balance does not equal the balance after the instruction
        /// </summary>
        UnbalancedInstruction,
        /// <summary>
        /// Program modified an account's program id
        /// </summary>
        ModifiedProgramId,
        /// <summary>
        /// Program spent the lamports of an account that doesn't belong to it
        /// </summary>
        ExternalAccountLamportSpend,
        /// <summary>
        ///  Program modified the data of an account that doesn't belong to it
        /// </summary>
        ExternalAccountDataModified,
        /// <summary>
        /// Read-only account's lamports modified
        /// </summary>
        ReadonlyLamportChange,
        /// <summary>
        /// Read-only account's data was modified
        /// </summary>
        ReadonlyDataModified,
        /// <summary>
        /// An account was referenced more than once in a single instruction 
        /// (Deprecated, instructions can now contain duplicate accounts)
        /// </summary>
        DuplicateAccountIndex,
        /// <summary>
        /// Executable bit on account changed, but shouldn't have
        /// </summary>
        ExecutableModified,
        /// <summary>
        /// Rent_epoch account changed, but shouldn't have
        /// </summary>
        RentEpochModified,
        /// <summary>
        ///  The instruction expected additional account keys
        /// </summary>
        NotEnoughAccountKeys,
        /// <summary>
        /// A non-system program changed the size of the account data
        /// </summary>
        AccountDataSizeChanged,
        /// <summary>
        /// The instruction expected an executable account
        /// </summary>
        AccountNotExecutable,
        /// <summary>
        /// Failed to borrow a reference to account data, already borrowed
        /// </summary>
        AccountBorrowFailed,
        /// <summary>
        /// Account data has an outstanding reference after a program's execution
        /// </summary>
        AccountBorrowOutstanding,
        /// <summary>
        /// The same account was multiply passed to an on-chain program's entrypoint, but the program
        /// modified them differently.  A program can only modify one instance of the account because
        /// the runtime cannot determine which changes to pick or how to merge them if both are modified
        /// </summary>
        DuplicateAccountOutOfSync,
        /// <summary>
        /// Allows on-chain programs to implement program-specific error types and see them returned
        /// by the Solana runtime. A program-specific error may be any type that is represented as
        /// or serialized to a u32 integer.
        /// </summary>
        Custom, //(u32)
        /// <summary>
        /// The return value from the program was invalid.  Valid errors are either a defined builtin
        /// error value or a user-defined error in the lower 32 bits.
        /// </summary>
        InvalidError,
        /// <summary>
        /// Executable account's data was modified
        /// </summary>
        ExecutableDataModified,
        /// <summary>
        /// Executable account's lamports modified
        /// </summary>
        ExecutableLamportChange,
        /// <summary>
        /// Executable accounts must be rent exempt
        /// </summary>
        ExecutableAccountNotRentExempt,
        /// <summary>
        /// Unsupported program id
        /// </summary>
        UnsupportedProgramId,
        /// <summary>
        /// Cross-program invocation call depth too deep
        /// </summary>
        CallDepth,
        /// <summary>
        /// An account required by the instruction is missing
        /// </summary>
        MissingAccount,
        /// <summary>
        /// Cross-program invocation reentrancy not allowed for this instruction
        /// </summary>
        ReentrancyNotAllowed,
        /// <summary>
        /// Length of the seed is too long for address generation
        /// </summary>
        MaxSeedLengthExceeded,
        /// <summary>
        /// Provided seeds do not result in a valid address
        /// </summary>
        InvalidSeeds,
        /// <summary>
        /// Failed to reallocate account data of this length
        /// </summary>
        InvalidRealloc,
        /// <summary>
        /// Computational budget exceeded
        /// </summary>
        ComputationalBudgetExceeded,
        /// <summary>
        /// Cross-program invocation with unauthorized signer or writable account
        /// </summary>
        PrivilegeEscalation,
        /// <summary>
        /// Failed to create program execution environment
        /// </summary>
        ProgramEnvironmentSetupFailure,
        /// <summary>
        /// Program failed to complete
        /// </summary>
        ProgramFailedToComplete,
        /// <summary>
        /// Program failed to compile
        /// </summary>
        ProgramFailedToCompile,
        /// <summary>
        /// Account is immutable
        /// </summary>
        Immutable,
        /// <summary>
        /// Incorrect authority provided
        /// </summary>
        IncorrectAuthority,
        /// <summary>
        /// Failed to serialize or deserialize account data
        /// </summary>
        BorshIoError, //(String)
        /// <summary>
        /// An account does not have enough lamports to be rent-exempt
        /// </summary>
        AccountNotRentExempt,
        /// <summary>
        /// Invalid account owner
        /// </summary>
        InvalidAccountOwner,
        /// <summary>
        /// Program arithmetic overflowed
        /// </summary>
        ArithmeticOverflow,
        /// <summary>
        /// Unsupported sysvar
        /// </summary>
        UnsupportedSysvar
    }
}