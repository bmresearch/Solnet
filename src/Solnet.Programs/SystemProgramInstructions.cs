namespace Solnet.Programs
{
    /// <summary>
    /// Implements the Token Program methods.
    /// <remarks>
    /// For more information see:
    /// https://docs.solana.com/developing/runtime-facilities/programs#system-program
    /// https://docs.rs/solana-sdk/1.7.0/solana_sdk/system_instruction/enum.SystemInstruction.html
    /// </remarks>
    /// </summary>
    internal enum SystemProgramInstructions : byte
    {
        /// <summary>
        /// Create a new account.
        /// </summary>
        CreateAccount = 0,
        
        /// <summary>
        /// Assign account to a program.
        /// </summary>
        Assign = 1,
        
        /// <summary>
        /// Transfer lamports.
        /// </summary>
        Transfer = 2,
        
        /// <summary>
        /// Create a new account at an address derived from a base public key and a seed.
        /// </summary>
        CreateAccountWithSeed = 3,
        
        /// <summary>
        /// Consumes a stored nonce, replacing it with a successor.
        /// </summary>
        AdvanceNonceAccount = 4,
        
        /// <summary>
        /// Withdraw funds from a nonce account.
        /// </summary>
        WithdrawNonceAccount = 5,
        
        /// <summary>
        /// Drive state of uninitialized nonce account to Initialized, setting the nonce value.
        /// </summary>
        InitializeNonceAccount = 6,
        
        /// <summary>
        /// Change the entity authorized to execute nonce instructions on the account.
        /// </summary>
        AuthorizeNonceAccount = 7,
        
        /// <summary>
        /// Allocate space in a (possibly new) account without funding.
        /// </summary>
        Allocate = 8,
        
        /// <summary>
        /// Allocate space for and assign an account at an address derived from a base public key and a seed.
        /// </summary>
        AllocateWithSeed = 9,
        
        /// <summary>
        /// Assign account to a program based on a seed
        /// </summary>
        AssignWithSeed = 10,
        
        /// <summary>
        /// Transfer lamports from a derived address.
        /// </summary>
        TransferWithSeed = 11
    }
}