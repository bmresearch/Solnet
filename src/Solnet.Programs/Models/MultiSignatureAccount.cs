using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Programs.Models
{
    /// <summary>
    /// Represents a <see cref="TokenProgram"/> Multi Signature Account in Solana.
    /// </summary>
    public class MultiSignatureAccount
    {
        /// <summary>
        /// The maximum number of signers.
        /// </summary>
        public const int MaxSigners = 11;

        /// <summary>
        /// The layout of the <see cref="MultiSignatureAccount"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the structure.
            /// </summary>
            public const int Length = 355;

            /// <summary>
            /// The offset at which the number of signers required value begins.
            /// </summary>
            public const int MOffset = 0;

            /// <summary>
            /// The offset at which the number of valid signers value begins.
            /// </summary>
            public const int NOffset = 1;

            /// <summary>
            /// The offset at which the is initialized value begins.
            /// </summary>
            public const int IsInitializedOffset = 2;

            /// <summary>
            /// The offset at which the array with signers' public keys begins.
            /// </summary>
            public const int SignersOffset = 3;
        }

        /// <summary>
        /// Number of signers required
        /// </summary>
        public byte M;

        /// <summary>
        /// Number of valid signers
        /// </summary>
        public byte N;

        /// <summary>
        /// Whether the account has been initialized
        /// </summary>
        public bool IsInitialized;

        /// <summary>
        /// Signer public keys
        /// </summary>
        public List<PublicKey> Signers;

        /// <summary>
        /// Deserialize the given data into the <see cref="MultiSignatureAccount"/> structure.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The <see cref="MultiSignatureAccount"/> structure.</returns>
        public static MultiSignatureAccount Deserialize(ReadOnlySpan<byte> data)
        {
            List<PublicKey> signers = new();

            for(int i= 0; i < MaxSigners; i++)
            {
                var signer = data.GetPubKey(Layout.SignersOffset + i * PublicKey.PublicKeyLength);
                if (signer != SystemProgram.ProgramIdKey)
                    signers.Add(signer);
            }

            return new MultiSignatureAccount
            {
                M = data.GetU8(Layout.MOffset),
                N = data.GetU8(Layout.NOffset),
                IsInitialized = data.GetBool(Layout.IsInitializedOffset),
                Signers = signers
            };
        }
    }
}
