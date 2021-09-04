// unset

using Solnet.Programs.Utilities;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;

namespace Solnet.Programs.Models
{
    /// <summary>
    /// Represents a <see cref="SystemProgram"/> Nonce Account in Solana.
    /// </summary>
    public class NonceAccount
    {
        /// <summary>
        /// The size of the data for a nonce account.
        /// </summary>
        public const int AccountDataSize = 80;

        #region Layout

        /// <summary>
        /// Represents the layout of the <see cref="NonceAccount"/> data structure.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The offset at which the version value begins.
            /// </summary>
            internal const int VersionOffset = 0;

            /// <summary>
            /// The offset at which the state value begins.
            /// </summary>
            internal const int StateOffset = 4;

            /// <summary>
            /// The offset at which the authorized public key value begins.
            /// </summary>
            internal const int AuthorizedKeyOffset = 8;

            /// <summary>
            /// The offset at which the current nonce public key value begins.
            /// </summary>
            internal const int NonceKeyOffset = 40;

            /// <summary>
            /// The offset at which the fee calculator value begins.
            /// </summary>
            internal const int FeeCalculatorOffset = 72;
        }

        #endregion

        /// <summary>
        /// The value used to specify version.
        /// </summary>
        public uint Version;

        /// <summary>
        /// The state of the nonce account.
        /// </summary>
        public uint State;

        /// <summary>
        /// The public key of the account authorized to interact with the nonce account.
        /// </summary>
        public PublicKey Authorized;

        /// <summary>
        /// The nonce.
        /// </summary>
        public PublicKey Nonce;

        /// <summary>
        /// 
        /// </summary>
        public FeeCalculator FeeCalculator;

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="NonceAccount"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The Nonce Account structure.</returns>
        public static NonceAccount Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != AccountDataSize)
                return null;

            return new()
            {
                Version = data.GetU32(Layout.VersionOffset),
                State = data.GetU32(Layout.StateOffset),
                Authorized = data.GetPubKey(Layout.AuthorizedKeyOffset),
                Nonce = data.GetPubKey(Layout.NonceKeyOffset),
                FeeCalculator = new FeeCalculator
                {
                    LamportsPerSignature = data.GetU64(Layout.FeeCalculatorOffset)
                }
            };
        }
    }
}