using System.Collections.Generic;

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
        internal static readonly Dictionary<Values, string> Names = new()
        {
            { Values.InitializeMint, "Initialize Mint" },
            { Values.InitializeAccount, "Initialize Account" },
            { Values.InitializeMultiSignature, "Initialize Multisig" },
            { Values.Transfer, "Transfer" },
            { Values.Approve, "Approve" },
            { Values.Revoke, "Revoke" },
            { Values.SetAuthority, "Set Authority" },
            { Values.MintTo, "Mint To" },
            { Values.Burn, "Burn" },
            { Values.CloseAccount, "Close Account" },
            { Values.FreezeAccount, "Freeze Account" },
            { Values.ThawAccount, "Thaw Account" },
            { Values.TransferChecked, "Transfer Checked" },
            { Values.ApproveChecked, "Approve Checked" },
            { Values.MintToChecked, "Mint To Checked" },
            { Values.BurnChecked, "Burn Checked" },
            { Values.SyncNative, "Sync Native" },
            { Values.InitializeAccount2, "Initialize Account 2" },
            { Values.InitializeAccount3, "Initialize Account 3" },
            { Values.InitializeMultiSignature2, "Initialize Multisig 2" },
            { Values.InitializeMint2, "Initialize Mint 2" },
            { Values.GetAccountDataSize, "Get Account Data Size" },
            { Values.InitializeImmutableOwner, "Initialize Immutable Owner" },
            { Values.AmountToUiAmount, "Amount To Ui Amount" },
            { Values.UiAmountToAmount, "Ui Amount To Amount" },
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
            BurnChecked = 15,

            /// <summary>
            /// Like InitializeAccount, but the owner pubkey is passed via instruction data
            /// rather than the accounts list. This variant may be preferable when using
            /// Cross Program Invocation from an instruction that does not need the owner's
            /// `AccountInfo` otherwise.
            /// </summary>
            InitializeAccount2 = 16,

            /// <summary>
            /// SyncNative token transaction.
            /// Given a wrapped / native token account (a token account containing SOL)
            /// updates its amount field based on the account's underlying `lamports`.
            /// This is useful if a non-wrapped SOL account uses `system_instruction::transfer`
            /// to move lamports to a wrapped token account, and needs to have its token
            /// `amount` field updated.
            /// </summary>
            SyncNative = 17,

            /// <summary>
            /// Like InitializeAccount2, but does not require the Rent sysvar to be provided.
            /// </summary>
            InitializeAccount3 = 18,

            /// <summary>
            /// Like InitializeMultisig, but does not require the Rent sysvar to be provided.
            /// </summary>
            InitializeMultiSignature2 = 19,

            /// <summary>
            /// Like InitializeMint, but does not require the Rent sysvar to be provided.
            /// </summary>
            InitializeMint2 = 20,

            /// <summary>
            /// Gets the required size of an account for the given mint as a little-endian `u64`.
            /// </summary>
            GetAccountDataSize = 21,

            /// <summary>
            /// Initialize the Immutable Owner extension for the given token account.
            /// </summary>
            InitializeImmutableOwner = 22,

            /// <summary>
            /// Convert an Amount of tokens to a UiAmount `string`, using the given mint.
            /// In this version of the program, the mint can only specify the number of decimals.
            /// </summary>
            AmountToUiAmount = 23,

            /// <summary>
            /// Convert a UiAmount of tokens to a little-endian `u64` raw Amount, using the given mint.
            /// In this version of the program, the mint can only specify the number of decimals.
            /// </summary>
            UiAmountToAmount = 24

        }
    }
}