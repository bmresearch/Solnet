using Solnet.Rpc.Models;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Solnet.Rpc.Builders
{
    /// <summary>
    /// Implements a builder for transactions.
    /// </summary>
    public class TransactionBuilder
    {
        /// <summary>
        /// The length of a signature.
        /// </summary>
        private const int SignatureLength = 64;

        /// <summary>
        /// The builder of the message contained within the transaction.
        /// </summary>
        private readonly MessageBuilder _messageBuilder;

        /// <summary>
        /// The signatures present in the message.
        /// </summary>
        private readonly List<string> _signatures;

        /// <summary>
        /// The message after being serialized.
        /// </summary>
        private byte[] _serializedMessage;

        /// <summary>
        /// Initialize the transaction builder.
        /// </summary>
        public TransactionBuilder()
        {
            _messageBuilder = new MessageBuilder();
            _signatures = new List<string>();
        }

        /// <summary>
        /// Gets the signers for the current transaction.
        /// </summary>
        /// <returns>An enumerable with the signers.</returns>
        private IEnumerable<Account> GetSigners()
        {
            List<Account> signers = new ();
            
            foreach (TransactionInstruction instruction in _messageBuilder.Instructions)
            {
                signers.AddRange(from accountMeta in instruction.Keys where accountMeta.Signer && !signers.Contains(accountMeta.Account) select accountMeta.Account);
            }

            return signers;
        }
        
        /// <summary>
        /// Serializes the message into a byte array.
        /// </summary>
        private byte[] Serialize()
        {
            byte[] signaturesLength = ShortVectorEncoding.EncodeLength(_signatures.Count);
            MemoryStream buffer = new (signaturesLength.Length + _signatures.Count * SignatureLength + _serializedMessage.Length);

            buffer.Write(signaturesLength);
            foreach (string signature in _signatures)
            {
                buffer.Write(Encoders.Base58.DecodeData(signature));
            }
            buffer.Write(_serializedMessage);

            return buffer.ToArray();
        }

        /// <summary>
        /// Sign the transaction message with each of the signer's keys.
        /// </summary>
        /// <exception cref="Exception">Throws exception when the list of signers is null or empty.</exception>
        private void Sign()
        {
            if (_messageBuilder.FeePayer == null)
                throw new Exception("fee payer is required");
            
            _serializedMessage = _messageBuilder.Build();
            var signers = GetSigners();
            
            foreach (Account signer in signers)
            {
                byte[] signatureBytes = signer.Sign(_serializedMessage);
                _signatures.Add(Encoders.Base58.EncodeData(signatureBytes));
            }
        }

        /// <summary>
        /// Sets the recent block hash for the transaction.
        /// </summary>
        /// <param name="recentBlockHash">The recent block hash as a base58 encoded string.</param>
        /// <returns>The transaction builder, so instruction addition can be chained.</returns>
        public TransactionBuilder SetRecentBlockHash(string recentBlockHash)
        {
            _messageBuilder.RecentBlockHash = recentBlockHash;
            return this;
        }
        
        /// <summary>
        /// Sets the fee payer for the transaction.
        /// </summary>
        /// <param name="account">The account that will pay the transaction fee</param>
        /// <returns>The transaction builder, so instruction addition can be chained.</returns>
        public TransactionBuilder SetFeePayer(Account account)
        {
            _messageBuilder.FeePayer = account;
            return this;
        }

        /// <summary>
        /// Adds a new instruction to the transaction.
        /// </summary>
        /// <param name="instruction">The instruction to add.</param>
        /// <returns>The transaction builder, so instruction addition can be chained.</returns>
        public TransactionBuilder AddInstruction(TransactionInstruction instruction)
        {
            _messageBuilder.AddInstruction(instruction);
            return this;
        }
        
        /// <summary>
        /// Signs the transaction's message with the passed list of signers and adds them to the transaction, serializing it.
        /// </summary>
        /// <returns>The serialized transaction.</returns>
        public byte[] Build()
        {
            Sign();

            return Serialize();
        }
    }
}