using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Diagnostics;
using System.Text;

namespace Solnet.Programs.Models.NameService
{
    /// <summary>
    /// Represents a reverse twitter record.
    /// </summary>
    [DebuggerDisplay("Type: {Type}, TwitterHandle: {TwitterHandle}")]
    public class ReverseTwitterRecord : RecordBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="header">The record header.</param>
        public ReverseTwitterRecord(RecordHeader header) : base(header, RecordType.ReverseTwitterRecord)
        {
        }

        /// <summary>
        /// The twitter registry address.
        /// </summary>
        public PublicKey TwitterRegistryKey { get; set; }

        /// <summary>
        /// The twitter handle.
        /// </summary>
        public string TwitterHandle { get; set; }

        /// <inheritdoc />
        public override object GetValue() => TwitterHandle;

        /// <summary>
        /// Deserialization method for a reverse twitter name record account.
        /// </summary>
        /// <param name="input">The raw data.</param>
        /// <returns>The deserialized reverse twitter record.</returns>
        public static ReverseTwitterRecord Deserialize(byte[] input)
        {
            var data = new ReadOnlySpan<byte>(input, 96, input.Length - 96);
            var header = RecordHeader.Deserialize(input);

            var ret = new ReverseTwitterRecord(header);

            ret.TwitterRegistryKey = data.GetPubKey(0);
            _ = data.GetBorshString(32, out var str);
            ret.TwitterHandle = str;

            return ret;
        }
    }
}
