using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solnet.Programs.Utilities;

namespace Solnet.Programs.Models.NameService
{
    /// <summary>
    /// Represents a Token metadata record.
    /// </summary>
    public class TokenData
    {
        /// <summary>
        /// The name of the token.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The ticker of the token.
        /// </summary>
        public string Ticker { get; set; }

        /// <summary>
        /// The mint account address of the token.
        /// </summary>
        public PublicKey Mint { get; set; }

        /// <summary>
        /// The decimals used by the token. This dictates the minimum transferrable value.
        /// </summary>
        public byte Decimals { get; set; }

        /// <summary>
        /// The website of the token or project.
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// The uri for the token logo.
        /// </summary>
        public string LogoUri { get; set; }

        /// <summary>
        /// Deserialization method for a token metadata record account.
        /// </summary>
        /// <param name="input">The raw data.</param>
        /// <returns>The deserialized token metadata record.</returns>
        public static TokenData Deserialize(byte[] input)
        {
            var data = new ReadOnlySpan<byte>(input, 96, input.Length - 96);
            int offset = 0;

            offset += data.GetBorshString(0, out var name);
            offset += data.GetBorshString(offset, out var ticker);

            var mint = data.GetPubKey(offset);
            offset += 32;

            var decimals = data.GetU8(offset);
            offset++;
            string website = null, logo = null;

            if (data.GetBool(offset++))
                offset += data.GetBorshString(offset, out website);

            if (data.GetBool(offset++))
                data.GetBorshString(offset, out logo);

            return new TokenData() { Name = name, Ticker = ticker, Decimals = decimals, LogoUri = logo, Mint = mint, Website = website };
        }
    }
}
