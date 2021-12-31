using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Diagnostics;

namespace Solnet.Programs.Models.NameService
{
    /// <summary>
    /// Represents the record header.
    /// </summary>
    [DebuggerDisplay("Owner: {Owner}, TLD: {ParentName}, Class: {Class}")]
    public class RecordHeader
    {
        /// <summary>
        /// The Top Level Domain of a given record.
        /// </summary>
        public PublicKey ParentName { get; set; }

        /// <summary>
        /// The owner of a given record. 
        /// For name records, its the account that owns the name.
        /// For reverse name records, its a mess.
        /// </summary>
        public PublicKey Owner { get; set; }

        /// <summary>
        /// The class of the record.
        /// </summary>
        public PublicKey Class { get; set; }

        /// <summary>
        /// Deserializes a record header from a given account data.
        /// </summary>
        /// <param name="input">The raw account data.</param>
        /// <returns>The deserialized RecordHeader from the data.</returns>
        public static RecordHeader Deserialize(byte[] input)
        {
            if (input.Length < 96)
                throw new IndexOutOfRangeException($"Record headers are 96 bytes. Found {input.Length} bytes in the current buffer.");

            var data = new ReadOnlySpan<byte>(input);
            var res = new RecordHeader();

            res.ParentName = data.GetPubKey(0);
            res.Owner = data.GetPubKey(32);
            res.Class = data.GetPubKey(64);

            return res;
        }
    }

    /// <summary>
    /// Type of record.
    /// </summary>
    public enum RecordType
    {
        /// <summary>
        /// A naming record. The account holds binary storage.
        /// </summary>
        NameRecord,
        /// <summary>
        /// A reverse naming record. The account holds the name string.
        /// </summary>
        ReverseRecord,
        /// <summary>
        /// A twitter naming record. The account holds binary storage.
        /// </summary>
        TwitterRecord,
        /// <summary>
        /// A reverse twitter record. The account holds the twitter handle.
        /// </summary>
        ReverseTwitterRecord,
        /// <summary>
        /// A token naming record. The account holds token metadata.
        /// </summary>
        TokenRecord,
        /// <summary>
        /// A reverse token record. The account holds the mint address.
        /// </summary>
        ReverseTokenRecord
    }
}
