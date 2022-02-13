using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Linq;
using System.Text;

namespace Solnet.Programs.TokenLending.Models
{
    /// <summary>
    /// The state of a lending market.
    /// </summary>
    public class LendingMarket
    {
        /// <summary>
        /// The layout of the <see cref="LendingMarket"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the structure.
            /// </summary>
            public const int Length = 258;

            /// <summary>
            /// The offset of the Version property.
            /// </summary>
            public const int VersionOffset = 0;

            /// <summary>
            /// The offset of the BumpSeed property.
            /// </summary>
            public const int BumpSeedOffset = 1;

            /// <summary>
            /// The offset of the Owner property.
            /// </summary>
            public const int OwnerOffset = 2;

            /// <summary>
            /// The offset of the QuoteCurrency property.
            /// </summary>
            public const int QuoteCurrencyOffset = 34;

            /// <summary>
            /// The offset of the TokenProgramId property.
            /// </summary>
            public const int TokenProgramIdOffset = 66;

            /// <summary>
            /// The offset of the OracleProgramId property.
            /// </summary>
            public const int OracleProgramIdOffset = 98;
        }

        /// <summary>
        /// The version of the lending market.
        /// </summary>
        public byte Version;

        /// <summary>
        /// The bump seed for the derived authority address.
        /// </summary>
        public byte BumpSeed;

        /// <summary>
        /// Owner authority which can add new reserves.
        /// </summary>
        public PublicKey Owner;

        /// <summary>
        /// Currency market prices are quoted in
        /// e.g. "USD" null padded (`*b"USD\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0"`) or a SPL token mint pubkey
        /// </summary>
        public byte[] QuoteBytes;

        /// <summary>
        /// Token program id.
        /// </summary>
        public PublicKey TokenProgramId;

        /// <summary>
        /// Oracle (Pyth) program id.
        /// </summary>
        public PublicKey OracleProgramId;

        /// <summary>
        /// The quote currency mint, in case it is specified as a SPL token mint public key.
        /// </summary>
        public PublicKey QuoteCurrencyMint;

        /// <summary>
        /// The quote currency, in case it is specified as a ticker.
        /// </summary>
        public string QuoteCurrency;

        /// <summary>
        /// Initialize a new <see cref="LendingMarket"/> with the given data.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        public LendingMarket(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"{nameof(data)} has wrong size. Expected {Layout.Length} bytes, actual {data.Length} bytes.");

            byte[] quote = data.GetSpan(Layout.QuoteCurrencyOffset, PublicKey.PublicKeyLength).ToArray();

            Version = data.GetU8(Layout.VersionOffset);
            BumpSeed = data.GetU8(Layout.BumpSeedOffset);
            Owner = data.GetPubKey(Layout.OwnerOffset);
            QuoteBytes = quote;
            TokenProgramId = data.GetPubKey(Layout.TokenProgramIdOffset);
            OracleProgramId = data.GetPubKey(Layout.OracleProgramIdOffset);
            QuoteCurrencyMint = quote.All(x => x != 0) ? new PublicKey(quote) : null;
            QuoteCurrency = quote.All(x => x != 0) ? null : Encoding.UTF8.GetString(quote).Trim('\0');
        }

        /// <summary>
        /// Deserialize a byte array into the <see cref="LendingMarket"/> structure.
        /// </summary>
        /// <param name="data">The byte array to deserialize.</param>
        /// <returns>The <see cref="LendingMarket"/> structure.</returns>
        public static LendingMarket Deserialize(byte[] data) => new (data.AsSpan());
    }
}
