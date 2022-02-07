using Solnet.Programs.Utilities;
using System;
using System.Diagnostics;
using System.Text;

namespace Solnet.Programs.Models.NameService
{
    /// <summary>
    /// Represents a reverse naming record.
    /// </summary>
    [DebuggerDisplay("Type: {Type}, Name: {Name}")]
    public class ReverseNameRecord : RecordBase
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="header">The record header.</param>
        /// <param name="name">The name of this reverse record.</param>
        public ReverseNameRecord(RecordHeader header, string name) : base(header, RecordType.ReverseRecord)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the record.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc />
        public override object GetValue() => Name;

        /// <summary>
        /// The record this <c>Name</c> points to.
        /// </summary>
        public NameRecord Value { get; internal set; }

        /// <summary>
        /// Deserialization method for a reverse name record account.
        /// </summary>
        /// <param name="input">The raw data.</param>
        /// <returns>The deserialized reverse record.</returns>
        public static ReverseNameRecord Deserialize(byte[] input)
        {
            var data = new ReadOnlySpan<byte>(input, 96, input.Length - 96);

            var header = RecordHeader.Deserialize(input);
            _ = data.GetBorshString(0, out var str);

            var res = new ReverseNameRecord(header, str);

            return res;
        }
    }
}
