using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Diagnostics;

namespace Solnet.Programs.Models.NameService
{
    /// <summary>
    /// Represents a reverse token name record.
    /// </summary>
    [DebuggerDisplay("Type: {Type}, Mint: {Value}")]
    public class ReverseTokenNameRecord : RecordBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="header">The record header.</param>
        public ReverseTokenNameRecord(RecordHeader header) : base(header, RecordType.ReverseTokenRecord)
        {
        }

        /// <summary>
        /// The token mint address.
        /// </summary>
        public PublicKey Value { get; set; }

        /// <inheritdoc />
        public override object GetValue() => Value;

        /// <summary>
        /// Deserialization method for a reverse token name record account.
        /// </summary>
        /// <param name="input">The raw data.</param>
        /// <returns>The deserialized reverse token name record.</returns>
        public static ReverseTokenNameRecord Deserialize(byte[] input)
        {
            var data = new ReadOnlySpan<byte>(input);
            var header = RecordHeader.Deserialize(input);
            var res = new ReverseTokenNameRecord(header);

            res.Value = data.GetPubKey(96);

            return res;
        }
    }
}
