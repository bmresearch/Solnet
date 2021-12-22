using System;
using System.Diagnostics;

namespace Solnet.Programs.Models.NameService
{
    /// <summary>
    /// Represents a token naming record.
    /// </summary>
    [DebuggerDisplay("Type: {Type}, Symbol: {Value.Ticker}, Mint: {Value.Mint}")]
    public class TokenNameRecord : RecordBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="header">The record header.</param>
        public TokenNameRecord(RecordHeader header) : base(header, RecordType.TokenRecord)
        {
        }

        /// <summary>
        /// The token metadata.
        /// </summary>
        public TokenData Value { get; set; }

        /// <inheritdoc />
        public override object GetValue() => Value;

        /// <summary>
        /// Deserialization method for a token name record account.
        /// </summary>
        /// <param name="input">The raw data.</param>
        /// <returns>The deserialized token name record.</returns>
        public static TokenNameRecord Deserialize(byte[] input)
        {
            var data = new ReadOnlySpan<byte>(input);

            var header = RecordHeader.Deserialize(input);

            var res = new TokenNameRecord(header);

            res.Value = TokenData.Deserialize(input);

            return res;
        }
    }
}
