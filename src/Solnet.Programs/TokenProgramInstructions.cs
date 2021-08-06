namespace Solnet.Programs
{
    /// <summary>
    /// Represents the instruction types for the <see cref="TokenProgram"/> along with a friendly name so as not to use reflection.
    /// <remarks>
    /// For more information see:
    /// https://spl.solana.com/token
    /// https://docs.rs/spl-token/3.2.0/spl_token/
    /// </remarks>
    /// </summary>
    internal static class TokenProgramInstructions
    {
        /// <summary>
        /// Represents the user-friendly names for the instruction types for the <see cref="TokenProgram"/>.
        /// </summary>
        internal static string[] Names = new[]
        {
            "Initialize Mint",
            "Initialize Account",
            "Initialize Multisig",
            "Transfer",
            "Approve",
            "Revoke",
            "Set Authority",
            "Mint To",
            "Burn",
            "Close Account",
            "Freeze Account",
            "Thaw Account",
            "Transfer Checked",
            "Approve Checked",
            "Mint To Checked",
            "Burn Checked",
        };

        /// <summary>
        /// Represents the instruction types for the <see cref="TokenProgram"/>.
        /// </summary>
        internal enum Values : byte
        {
            /// <summary>
            /// Initialize a token mint.
            /// </summary>
            InitializeMint = 0,

            /// <summary>
            /// Initialize a token account.
            /// </summary>
            InitializeAccount = 1,

            /// <summary>
            /// Initialize a multi signature token account.
            /// </summary>
            InitializeMultiSignature = 2,

            /// <summary>
            /// Transfer token transaction.
            /// </summary>
            Transfer = 3,

            /// <summary>
            /// Approve token transaction.
            /// </summary>
            Approve = 4,

            /// <summary>
            /// Revoke token transaction.
            /// </summary>
            Revoke = 5,

            /// <summary>
            /// Set token authority transaction.
            /// </summary>
            SetAuthority = 6,

            /// <summary>
            /// MintTo token account transaction.
            /// </summary>
            MintTo = 7,

            /// <summary>
            /// Burn token transaction.
            /// </summary>
            Burn = 8,

            /// <summary>
            /// Close token account transaction.
            /// </summary>
            CloseAccount = 9,

            /// <summary>
            /// Freeze token account transaction.
            /// </summary>
            FreezeAccount = 10,

            /// <summary>
            /// Thaw token account transaction.
            /// </summary>
            ThawAccount = 11,

            /// <summary>
            /// Transfer checked token transaction.
            /// <remarks>Differs from <see cref="Transfer"/> in that the decimals value is asserted by the caller.</remarks>
            /// </summary>
            TransferChecked = 12,

            /// <summary>
            /// Approve checked token transaction.
            /// <remarks>Differs from <see cref="Approve"/> in that the decimals value is asserted by the caller.</remarks>
            /// </summary>
            ApproveChecked = 13,

            /// <summary>
            /// MintTo checked token transaction.
            /// <remarks>Differs from <see cref="MintTo"/> in that the decimals value is asserted by the caller.</remarks>
            /// </summary>
            MintToChecked = 14,

            /// <summary>
            /// Burn checked token transaction.
            /// <remarks>Differs from <see cref="Burn"/> in that the decimals value is asserted by the caller.</remarks>
            /// </summary>
            BurnChecked = 15
        }
    }
}