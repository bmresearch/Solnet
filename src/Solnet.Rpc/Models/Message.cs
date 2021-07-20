using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// The message header.
    /// </summary>
    public class MessageHeader
    {
        /// <summary>
        /// The message header length.
        /// </summary>
        internal const int HeaderLength = 3;

        /// <summary>
        /// The number of required signatures.
        /// </summary>
        public byte RequiredSignatures { get; set; }

        /// <summary>
        /// The number of read-only signed accounts.
        /// </summary>
        public byte ReadOnlySignedAccounts { get; set; }

        /// <summary>
        /// The number of read-only non-signed accounts.
        /// </summary>
        public byte ReadOnlyUnsignedAccounts { get; set; }

        /// <summary>
        /// Convert the message header to byte array format.
        /// </summary>
        /// <returns>The byte array.</returns>
        internal byte[] ToBytes()
        {
            return new[] { RequiredSignatures, ReadOnlySignedAccounts, ReadOnlyUnsignedAccounts };
        }
    }

    /// <summary>
    /// Represents the Message of a Solana <see cref="Transaction"/>.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// The header of the <see cref="Message"/>.
        /// </summary>
        public MessageHeader Header { get; init; }
        
        /// <summary>
        /// The list of account <see cref="PublicKey"/>s present in the transaction.
        /// </summary>
        public IList<PublicKey> AccountKeys { get; set; }
        
        /// <summary>
        /// The list of <see cref="TransactionInstruction"/>s present in the transaction.
        /// </summary>
        public IList<TransactionInstruction> Instructions { get; set; }
        
        /// <summary>
        /// The recent block hash for the transaction.
        /// </summary>
        public string RecentBlockhash { get; set; }

        /// <summary>
        /// Serialize the message into the wire format.
        /// </summary>
        /// <returns>A byte array corresponding to the serialized message.</returns>
        public byte[] Serialize()
        {

            return new byte[] { };
        }

        /// <summary>
        /// Deserialize a compiled message into a Message object.
        /// </summary>
        /// <param name="data">The data to deserialize into the Message object.</param>
        /// <returns>The Message object instance.</returns>
        public static Message Deserialize(ReadOnlySpan<byte> data)
        {
            return new ()
            {
                Header = new MessageHeader()
                {
                    
                },
                AccountKeys = new List<PublicKey>(),
                Instructions = new List<TransactionInstruction>(),
            };
        }
    }
}