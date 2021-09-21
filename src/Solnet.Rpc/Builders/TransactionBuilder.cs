using Solnet.Rpc.Models;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

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
        public const int SignatureLength = 64;

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
        /// Default constructor that initializes the transaction builder.
        /// </summary>
        public TransactionBuilder()
        {
            _messageBuilder = new MessageBuilder();
            _signatures = new List<string>();
        }

        /// <summary>
        /// Serializes the message into a byte array.
        /// </summary>
        private byte[] Serialize()
        {
            byte[] signaturesLength = ShortVectorEncoding.EncodeLength(_signatures.Count);
            MemoryStream buffer = new(signaturesLength.Length + _signatures.Count * SignatureLength + _serializedMessage.Length);

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
        /// <param name="signers">The list of signers.</param>
        /// <exception cref="Exception">Throws exception when the list of signers is null or empty or when the fee payer hasn't been set.</exception>
        private void Sign(IList<Account> signers)
        {
            if (signers == null || signers.Count == 0) throw new Exception("no signers for the transaction");

            if (_messageBuilder.FeePayer == null)
                throw new Exception("fee payer is required");

            _serializedMessage = _messageBuilder.Build();

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
        /// Sets the nonce information for the transaction.
        /// <remarks>Whenever this is set, it is used instead of the blockhash.</remarks>
        /// </summary>
        /// <param name="nonceInfo">The nonce information object to use.</param>
        /// <returns>The transaction builder, so instruction addition can be chained.</returns>
        public TransactionBuilder SetNonceInformation(NonceInformation nonceInfo)
        {
            _messageBuilder.NonceInformation = nonceInfo;
            return this;
        }

        /// <summary>
        /// Sets the fee payer for the transaction.
        /// </summary>
        /// <param name="account">The public key of the account that will pay the transaction fee</param>
        /// <returns>The transaction builder, so instruction addition can be chained.</returns>
        public TransactionBuilder SetFeePayer(PublicKey account)
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
        /// Compiles the transaction's message into wire format, ready to be signed.
        /// </summary>
        /// <returns>The serialized message.</returns>
        public byte[] CompileMessage()
        {
            return _messageBuilder.Build();
        }

        /// <summary>
        /// Signs the transaction's message with the passed signer and add it to the transaction, serializing it.
        /// </summary>
        /// <param name="signer">The signer.</param>
        /// <returns>The serialized transaction.</returns>
        public byte[] Build(Account signer)
        {
            return Build(new List<Account> { signer });
        }

        /// <summary>
        /// Signs the transaction's message with the passed list of signers and adds them to the transaction, serializing it.
        /// </summary>
        /// <param name="signers">The list of signers.</param>
        /// <returns>The serialized transaction.</returns>
        public byte[] Build(IList<Account> signers)
        {
            Sign(signers);

            return Serialize();
        }
    }
}