using Solnet.Wallet;
using System;
using Solnet.Programs.Utilities;

namespace Solnet.Programs.Models.TokenProgram
{
    /// <summary>
    /// Represents a <see cref="Programs.TokenProgram" /> token mint account.
    /// </summary>
    public class TokenMint
    {
        /// <summary>
        /// The layout of the <see cref="TokenMint"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the structure.
            /// </summary>
            public const int Length = 82;

            /// <summary>
            /// The offset at which the mint authority COption begins.
            /// </summary>
            public const int MintAuthorityOptionOffset = 0;

            /// <summary>
            /// The offset at which the mint authority pubkey value begins.
            /// </summary>
            public const int MintAuthorityOffset = 4;

            /// <summary>
            /// The offset at which the supply value begins.
            /// </summary>
            public const int SupplyOffset = 36;

            /// <summary>
            /// The offset at which the decimals value begins.
            /// </summary>
            public const int DecimalsOffset = 44;

            /// <summary>
            /// The offset at which the is initialized value begins.
            /// </summary>
            public const int IsInitializedOffset = 45;

            /// <summary>
            /// The offset at which the freeze authority COption begins.
            /// </summary>
            public const int FreezeAuthorityOptionOffset = 46;

            /// <summary>
            /// The offset at which the freeze authority pubkey value begins.
            /// </summary>
            public const int FreezeAuthorityOffset = 50;
        }

        /// <summary>
        /// Optional authority to mint new tokens. If no mint authority is present, no new tokens can be issued.
        /// </summary>
        public PublicKey MintAuthority { get; set; }

        /// <summary>
        /// Total supply of tokens.
        /// </summary>
        public ulong Supply { get; set; }

        /// <summary>
        /// Number of base 10 digits to the right of the decimal polace.
        /// </summary>
        public byte Decimals { get; set; }

        /// <summary>
        /// Whether or not the account has been initialized.
        /// </summary>
        public bool IsInitialized { get; set; }

        /// <summary>
        /// Optional authority to freeze token accounts.
        /// </summary>
        public PublicKey FreezeAuthority { get; set; }


        /// <summary>
        /// Deserialize the given data into the <see cref="TokenMint"/> structure.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The <see cref="TokenMint"/> structure.</returns>
        public static TokenMint Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"{nameof(data)} has wrong size. Expected {Layout.Length} bytes, actual {data.Length} bytes.");

            var res = new TokenMint();

            if (data.GetU32(Layout.MintAuthorityOptionOffset) == 1)
                res.MintAuthority = data.GetPubKey(Layout.MintAuthorityOffset);

            res.Supply = data.GetU64(Layout.SupplyOffset);
            res.Decimals = data.GetU8(Layout.DecimalsOffset);
            res.IsInitialized= data.GetBool(Layout.IsInitializedOffset);

            if (data.GetU32(Layout.FreezeAuthorityOptionOffset) == 1)
                res.FreezeAuthority = data.GetPubKey(Layout.FreezeAuthorityOptionOffset);

            return res;
        }
    }
}
