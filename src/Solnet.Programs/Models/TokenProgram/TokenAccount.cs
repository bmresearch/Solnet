using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Models.TokenProgram
{
    /// <summary>
    /// Represents a <see cref="Programs.TokenProgram" /> token account.
    /// </summary>
    public class TokenAccount
    {
        /// <summary>
        /// Represents the state of a token account.
        /// </summary>
        public enum AccountState : byte
        {
            /// <summary>
            /// Account is uninitialized.
            /// </summary>
            Uninitialized,
            /// <summary>
            /// Account is initialized. The owner and/or delegate may operate the account.
            /// </summary>
            Initialized,
            /// <summary>
            /// The account is frozen. The owner and delegate can't operate the account.
            /// </summary>
            Frozen
        }
        /// <summary>
        /// The layout of the <see cref="TokenAccount"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the structure.
            /// </summary>
            public const int Length = 165;

            /// <summary>
            /// The offset at which the token mint pubkey begins.
            /// </summary>
            public const int MintOffset = 0;

            /// <summary>
            /// The offset at which the owner pubkey begins.
            /// </summary>
            public const int OwnerOffset = 32;

            /// <summary>
            /// The offset at which the amount value begins.
            /// </summary>
            public const int AmountOffset = 64;

            /// <summary>
            /// The offset at which the delegate pubkey COption value begins.
            /// </summary>
            public const int DelegateOptionOffset = 72;

            /// <summary>
            /// The offset at which the delegate pubkey value begins.
            /// </summary>
            public const int DelegateOffset = 76;

            /// <summary>
            /// The offset at which the state value begins.
            /// </summary>
            public const int StateOffset = 108;

            /// <summary>
            /// The offset at which the IsNative COption begins.
            /// </summary>
            public const int IsNativeOptionOffset = 109;

            /// <summary>
            /// The offset at which the IsNative  begins.
            /// </summary>
            public const int IsNativeOffset = 113;

            /// <summary>
            /// The offset at which the delegaterrd amount value begins.
            /// </summary>
            public const int DelegatedAmountOffset = 121;

            /// <summary>
            /// The offset at which the close authority pubkey COption begins.
            /// </summary>
            public const int CloseAuthorityOptionOffset = 129;

            /// <summary>
            /// The offset at which the close authority pubkey begins.
            /// </summary>
            public const int CloseAuthorityOffset = 133;
        }

        /// <summary>
        /// The token mint.
        /// </summary>
        public PublicKey Mint { get; set; }

        /// <summary>
        /// The owner of the token account.
        /// </summary>
        public PublicKey Owner { get; set; }

        /// <summary>
        /// The amount of tokens this account holds.
        /// </summary>
        public ulong Amount { get; set; }

        /// <summary>
        /// Delegate address. If Delegate has value then DelegatedAmount represents the amount authorized by the delegate.
        /// </summary>
        public PublicKey Delegate { get; set; }

        /// <summary>
        /// Represents the state of this account.
        /// </summary>
        public AccountState State { get; set; }

        /// <summary>
        /// If IsNative has value, this is a native token and the value logs the rent-exempt reserve.
        /// </summary>
        public ulong? IsNative { get; set; }

        /// <summary>
        /// The amount delegated.
        /// </summary>
        public ulong DelegatedAmount { get; set; }

        /// <summary>
        /// Optional authority to close the account.
        /// </summary>
        public PublicKey CloseAuthority { get; set; }

        /// <summary>
        /// Deserialize the given data into the <see cref="TokenAccount"/> structure.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The <see cref="TokenAccount"/> structure.</returns>
        public static TokenAccount Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"{nameof(data)} has wrong size. Expected {Layout.Length} bytes, actual {data.Length} bytes.");

            var res = new TokenAccount
            {
                Mint = data.GetPubKey(Layout.MintOffset),
                Owner = data.GetPubKey(Layout.OwnerOffset),
                Amount = data.GetU64(Layout.AmountOffset),

                State = (AccountState)data.GetU8(Layout.StateOffset),
                DelegatedAmount = data.GetU64(Layout.DelegatedAmountOffset),
            };

            if (data.GetU32(Layout.DelegateOptionOffset) == 1)
                res.Delegate = data.GetPubKey(Layout.DelegateOffset);

            if (data.GetU32(Layout.IsNativeOptionOffset) == 1)
                res.IsNative = data.GetU64(Layout.IsNativeOffset);

            if (data.GetU32(Layout.CloseAuthorityOptionOffset) == 1)
                res.CloseAuthority = data.GetPubKey(Layout.CloseAuthorityOffset);

            return res;
        }
    }
}
