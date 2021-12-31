using Solnet.Wallet;
using System.Diagnostics;

namespace Solnet.Programs.Models.NameService
{
    /// <summary>
    /// Base class containing record registration details.
    /// </summary>
    [DebuggerDisplay("Type: {Type}, Address: {AccountAddress}")]
    public abstract class RecordBase
    {
        /// <summary>
        /// The record header.
        /// </summary>
        public RecordHeader Header { get; }

        /// <summary>
        /// The type of the record.
        /// </summary>
        public RecordType Type { get; }

        /// <summary>
        /// The address of the native account containing this record.
        /// </summary>
        public PublicKey AccountAddress { get; internal set; }

        /// <summary>
        /// The name lookup value (when applicable).
        /// </summary>
        public string LookupValue { get; internal set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="header">The header of this record.</param>
        /// <param name="type">The type of this record (directly related to header TLD and Class).</param>
        public RecordBase(RecordHeader header, RecordType type)
        {
            Header = header;
            Type = type;
        }

        /// <summary>
        /// Gets the value held by this record.
        /// </summary>
        /// <returns>The value.</returns>
        public abstract object GetValue();
    }
}
