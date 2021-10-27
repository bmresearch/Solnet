using Solnet.Rpc.Builders;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// A pair corresponding of a public key and it's verifiable signature. 
    /// </summary>
    public class SignaturePubKeyPair
    {
        /// <summary>
        /// The public key to verify the signature against.
        /// </summary>
        public PublicKey PublicKey { get; set; }

        /// <summary>
        /// The signature created by the corresponding <see cref="PrivateKey"/> of this pair's <see cref="PublicKey"/>.
        /// </summary>
        public byte[] Signature { get; set; }
    }

    /// <summary>
    /// Nonce information to be used to build an offline transaction.
    /// </summary>
    public class NonceInformation
    {
        /// <summary>
        /// The current blockhash stored in the nonce account.
        /// </summary>
        public string Nonce { get; set; }

        /// <summary>
        /// An AdvanceNonceAccount instruction.
        /// </summary>
        public TransactionInstruction Instruction { get; set; }
    }

    /// <summary>
    /// Represents a Transaction in Solana.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// The transaction's fee payer.
        /// </summary>
        public PublicKey FeePayer { get; set; }

        /// <summary>
        /// The list of <see cref="TransactionInstruction"/>s present in the transaction.
        /// </summary>
        public List<TransactionInstruction> Instructions { get; set; }

        /// <summary>
        /// The recent block hash for the transaction.
        /// </summary>
        public string RecentBlockHash { get; set; }

        /// <summary>
        /// The nonce information of the transaction.
        /// <remarks>
        /// When this is set, the <see cref="NonceInformation"/>'s Nonce is used as the <c>RecentBlockhash</c>.
        /// </remarks>
        /// </summary>
        public NonceInformation NonceInformation { get; set; }

        /// <summary>
        /// The signatures for the transaction.
        /// <remarks>
        /// These are typically created by invoking the <c>Build(IList{Account} signers)</c> method of the <see cref="TransactionBuilder"/>,
        /// but can be created by deserializing a Transaction and adding signatures manually.
        /// </remarks>
        /// </summary>
        public List<SignaturePubKeyPair> Signatures { get; set; }

        /// <summary>
        /// Compile the transaction data.
        /// </summary>
        public byte[] CompileMessage()
        {
            MessageBuilder messageBuilder = new() { FeePayer = FeePayer };

            if (RecentBlockHash != null) messageBuilder.RecentBlockHash = RecentBlockHash;
            if (NonceInformation != null) messageBuilder.NonceInformation = NonceInformation;

            foreach (TransactionInstruction instruction in Instructions)
            {
                messageBuilder.AddInstruction(instruction);
            }

            return messageBuilder.Build();
        }

        /// <summary>
        /// Verifies the signatures a given serialized message.
        /// </summary>
        /// <returns>true if they are valid, false otherwise.</returns>
        private bool VerifySignatures(byte[] serializedMessage) =>
            Signatures.All(pair => pair.PublicKey.Verify(serializedMessage, pair.Signature));

        /// <summary>
        /// Verifies the signatures of a complete and signed transaction.
        /// </summary>
        /// <returns>true if they are valid, false otherwise.</returns>
        public bool VerifySignatures() => VerifySignatures(CompileMessage());

        /// <summary>
        /// Sign the transaction with the specified signers. Multiple signatures may be applied to a transaction.
        /// The first signature is considered primary and is used to identify and confirm transaction.
        /// <remarks>
        /// <para>
        /// If the transaction <c>FeePayer</c> is not set, the first signer will be used as the transaction fee payer account.
        /// </para>
        /// <para>
        /// Transaction fields SHOULD NOT be modified after the first call to <c>Sign</c> or an externally created signature
        /// has been added to the transaction object, doing so will invalidate the signature and cause the transaction to be
        /// rejected by the cluster.
        /// </para>
        /// <para>
        /// The transaction must have been assigned a valid <c>RecentBlockHash</c> or <c>NonceInformation</c> before invoking this method.
        /// </para>
        /// </remarks>
        /// </summary>
        /// <param name="signers">The signer accounts.</param>
        public bool Sign(IList<Account> signers)
        {
            Signatures ??= new List<SignaturePubKeyPair>();
            IEnumerable<Account> uniqueSigners = DeduplicateSigners(signers);
            byte[] serializedMessage = CompileMessage();

            foreach (Account account in uniqueSigners)
            {
                byte[] signatureBytes = account.Sign(serializedMessage);
                Signatures.Add(new SignaturePubKeyPair { PublicKey = account.PublicKey, Signature = signatureBytes });
            }

            return VerifySignatures();
        }

        /// <summary>
        /// Sign the transaction with the specified signer. Multiple signatures may be applied to a transaction.
        /// The first signature is considered primary and is used to identify and confirm transaction.
        /// <remarks>
        /// <para>
        /// If the transaction <c>FeePayer</c> is not set, the first signer will be used as the transaction fee payer account.
        /// </para>
        /// <para>
        /// Transaction fields SHOULD NOT be modified after the first call to <c>Sign</c> or an externally created signature
        /// has been added to the transaction object, doing so will invalidate the signature and cause the transaction to be
        /// rejected by the cluster.
        /// </para>
        /// <para>
        /// The transaction must have been assigned a valid <c>RecentBlockHash</c> or <c>NonceInformation</c> before invoking this method.
        /// </para>
        /// </remarks>
        /// </summary>
        /// <param name="signer">The signer account.</param>
        public bool Sign(Account signer) => Sign(new List<Account> { signer });

        /// <summary>
        /// Partially sign a transaction with the specified accounts.
        /// All accounts must correspond to either the fee payer or a signer account in the transaction instructions.
        /// </summary>
        /// <param name="signers">The signer accounts.</param>
        public void PartialSign(IList<Account> signers)
        {
            Signatures ??= new List<SignaturePubKeyPair>();
            IEnumerable<Account> uniqueSigners = DeduplicateSigners(signers);
            byte[] serializedMessage = CompileMessage();

            foreach (Account account in uniqueSigners)
            {
                byte[] signatureBytes = account.Sign(serializedMessage);
                Signatures.Add(new SignaturePubKeyPair { PublicKey = account.PublicKey, Signature = signatureBytes });
            }
        }

        /// <summary>
        /// Deduplicate the list of given signers.
        /// </summary>
        /// <param name="signers">The signer accounts.</param>
        /// <returns>The signer accounts with removed duplicates</returns>
        private static IEnumerable<Account> DeduplicateSigners(IEnumerable<Account> signers)
        {
            List<Account> uniqueSigners = new();
            HashSet<Account> seen = new();

            foreach (Account account in signers)
            {
                if (seen.Contains(account)) continue;

                seen.Add(account);
                uniqueSigners.Add(account);
            }

            return uniqueSigners;
        }

        /// <summary>
        /// Partially sign a transaction with the specified account.
        /// The account must correspond to either the fee payer or a signer account in the transaction instructions.
        /// </summary>
        /// <param name="signer">The signer account.</param>
        public void PartialSign(Account signer) => PartialSign(new List<Account> { signer });

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

        /// <summary>
        /// Adds an externally created signature to the transaction.
        /// The public key must correspond to either the fee payer or a signer account in the transaction instructions.
        /// </summary>
        /// <param name="publicKey">The public key of the account that signed the transaction.</param>
        /// <param name="signature">The transaction signature.</param>
        public void AddSignature(PublicKey publicKey, byte[] signature)
        {
            Signatures ??= new List<SignaturePubKeyPair>();
            Signatures.Add(new SignaturePubKeyPair { PublicKey = publicKey, Signature = signature });
        }

        /// <summary>
        /// Adds one or more instructions to the transaction.
        /// </summary>
        /// <param name="instructions">The instructions to add.</param>
        /// <returns>The transaction instance.</returns>
        public Transaction Add(IEnumerable<TransactionInstruction> instructions)
        {
            Instructions ??= new List<TransactionInstruction>();
            Instructions.AddRange(instructions);
            return this;
        }

        /// <summary>
        /// Adds an instruction to the transaction.
        /// </summary>
        /// <param name="instruction">The instruction to add.</param>
        /// <returns>The transaction instance.</returns>
        public Transaction Add(TransactionInstruction instruction) =>
            Add(new List<TransactionInstruction> { instruction });

        /// <summary>
        /// Serializes the transaction into wire format.
        /// </summary>
        /// <returns>The transaction encoded in wire format.</returns>
        public byte[] Serialize()
        {
            byte[] signaturesLength = ShortVectorEncoding.EncodeLength(Signatures.Count);
            byte[] serializedMessage = CompileMessage();
            MemoryStream buffer = new(signaturesLength.Length + Signatures.Count * TransactionBuilder.SignatureLength +
                                      serializedMessage.Length);

            buffer.Write(signaturesLength);
            foreach (SignaturePubKeyPair signaturePair in Signatures)
            {
                buffer.Write(signaturePair.Signature);
            }

            buffer.Write(serializedMessage);
            return buffer.ToArray();
        }

        /// <summary>
        /// Populate the Transaction from the given message and signatures.
        /// </summary>
        /// <param name="message">The <see cref="Message"/> object.</param>
        /// <param name="signatures">The list of signatures.</param>
        /// <returns>The Transaction object.</returns>
        public static Transaction Populate(Message message, IList<byte[]> signatures = null)
        {
            Transaction tx = new()
            {
                RecentBlockHash = message.RecentBlockhash,
                Signatures = new List<SignaturePubKeyPair>(),
                Instructions = new List<TransactionInstruction>()
            };

            if (message.Header.RequiredSignatures > 0)
            {
                tx.FeePayer = message.AccountKeys[0];
            }

            if (signatures != null)
            {
                for (int i = 0; i < signatures.Count; i++)
                {
                    tx.Signatures.Add(new SignaturePubKeyPair
                    {
                        PublicKey = message.AccountKeys[i],
                        Signature = signatures[i]
                    });
                }
            }

            for (int i = 0; i < message.Instructions.Count; i++)
            {
                CompiledInstruction compiledInstruction = message.Instructions[i];
                (int accountLength, _) = ShortVectorEncoding.DecodeLength(compiledInstruction.KeyIndicesCount);

                List<AccountMeta> accounts = new(accountLength);
                for (int j = 0; j < accountLength; j++)
                {
                    int k = compiledInstruction.KeyIndices[j];
                    accounts.Add(new AccountMeta(message.AccountKeys[k], message.IsAccountWritable(k),
                        tx.Signatures.Any(pair => pair.PublicKey.Key == message.AccountKeys[k].Key)));
                }

                TransactionInstruction instruction = new()
                {
                    Keys = accounts,
                    ProgramId = message.AccountKeys[compiledInstruction.ProgramIdIndex],
                    Data = compiledInstruction.Data
                };
                if (i == 0 && accounts.Any(a => a.PublicKey == "SysvarRecentB1ockHashes11111111111111111111"))
                {
                    tx.NonceInformation = new NonceInformation { Instruction = instruction, Nonce = tx.RecentBlockHash };
                    continue;
                }
                tx.Instructions.Add(instruction);
            }

            return tx;
        }

        /// <summary>
        /// Populate the Transaction from the given compiled message and signatures.
        /// </summary>
        /// <param name="message">The compiled message, as base-64 encoded string.</param>
        /// <param name="signatures">The list of signatures.</param>
        /// <returns>The Transaction object.</returns>
        public static Transaction Populate(string message, IList<byte[]> signatures = null)
            => Populate(Message.Deserialize(message), signatures);

        /// <summary>
        /// Deserialize a wire format transaction into a Transaction object.
        /// </summary>
        /// <param name="data">The data to deserialize into the Transaction object.</param>
        /// <returns>The Transaction object.</returns>
        public static Transaction Deserialize(ReadOnlySpan<byte> data)
        {
            // Read number of signatures
            (int signaturesLength, int encodedLength) =
                ShortVectorEncoding.DecodeLength(data[..ShortVectorEncoding.SpanLength]);
            List<byte[]> signatures = new(signaturesLength);

            for (int i = 0; i < signaturesLength; i++)
            {
                ReadOnlySpan<byte> signature =
                    data.Slice(encodedLength + (i * TransactionBuilder.SignatureLength),
                        TransactionBuilder.SignatureLength);
                signatures.Add(signature.ToArray());
            }

            return Populate(
                Message.Deserialize(data[
                    (encodedLength + (signaturesLength * TransactionBuilder.SignatureLength))..]),
                signatures);
        }

        /// <summary>
        /// Deserialize a transaction encoded as base-64 into a Transaction object.
        /// </summary>
        /// <param name="data">The data to deserialize into the Transaction object.</param>
        /// <returns>The Transaction object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the given string is null.</exception>
        public static Transaction Deserialize(string data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            byte[] decodedBytes;

            try
            {
                decodedBytes = Convert.FromBase64String(data);
            }
            catch (Exception ex)
            {
                throw new Exception("could not decode transaction data from base64", ex);
            }

            return Deserialize(decodedBytes);
        }
    }
}