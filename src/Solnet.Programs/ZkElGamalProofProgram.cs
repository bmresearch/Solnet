using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;

namespace Solnet.Programs
{
    /// <summary>Instruction builders for Solana's native ZK ElGamal proof verifier. Proof generation and encryption are supplied by the caller.</summary>
    public static class ZkElGamalProofProgram
    {
        /// <summary>The ZK ElGamal proof program id.</summary>
        public static readonly PublicKey ProgramIdKey = new("ZkE1Gama1Proof11111111111111111111111111111");

        /// <summary>Supported verification instruction discriminators.</summary>
        public enum ProofType : byte
        {
            /// <summary>Zero-ciphertext proof.</summary>
            ZeroCiphertext = 1,
            /// <summary>Equality between two encrypted ciphertexts.</summary>
            CiphertextCiphertextEquality = 2,
            /// <summary>Equality between a ciphertext and a Pedersen commitment.</summary>
            CiphertextCommitmentEquality = 3,
            /// <summary>Validity of an ElGamal public key.</summary>
            PubkeyValidity = 4,
            /// <summary>Percentage with an upper cap.</summary>
            PercentageWithCap = 5,
            /// <summary>Batched 64-bit range proof.</summary>
            BatchedRangeProofU64 = 6,
            /// <summary>Batched 128-bit range proof.</summary>
            BatchedRangeProofU128 = 7,
            /// <summary>Batched 256-bit range proof.</summary>
            BatchedRangeProofU256 = 8,
            /// <summary>Validity of a grouped ciphertext with two handles.</summary>
            GroupedCiphertext2HandlesValidity = 9,
            /// <summary>Batched validity of grouped ciphertexts with two handles.</summary>
            BatchedGroupedCiphertext2HandlesValidity = 10,
            /// <summary>Validity of a grouped ciphertext with three handles.</summary>
            GroupedCiphertext3HandlesValidity = 11,
            /// <summary>Batched validity of grouped ciphertexts with three handles.</summary>
            BatchedGroupedCiphertext3HandlesValidity = 12
        }

        /// <summary>Verifies caller-generated proof data (serialized context plus proof, without the instruction discriminator). Optionally persists the verified context into an allocated context account. The on-chain program validates the proof's structure and cryptography.</summary>
        public static TransactionInstruction VerifyProof(ProofType proofType, byte[] proofData,
            PublicKey contextStateAccount = null, PublicKey contextStateAuthority = null)
        {
            ValidateProofType(proofType);
            ArgumentNullException.ThrowIfNull(proofData);
            if (proofData.Length < 32) throw new ArgumentException("Supply the complete serialized proof context and proof.", nameof(proofData));
            var data = new byte[checked(proofData.Length + 1)];
            data[0] = (byte)proofType;
            proofData.CopyTo(data, 1);
            return new TransactionInstruction { ProgramId = ProgramIdKey.KeyBytes, Keys = ContextAccounts(contextStateAccount, contextStateAuthority), Data = data };
        }

        /// <summary>Verifies a proof stored in a separate account at a byte offset. This supports proofs that cannot fit inside a single transaction.</summary>
        public static TransactionInstruction VerifyProofFromAccount(ProofType proofType, PublicKey proofAccount,
            uint proofDataOffset, PublicKey contextStateAccount = null, PublicKey contextStateAuthority = null)
        {
            ValidateProofType(proofType);
            ArgumentNullException.ThrowIfNull(proofAccount);
            var keys = new List<AccountMeta> { AccountMeta.ReadOnly(proofAccount, false) };
            keys.AddRange(ContextAccounts(contextStateAccount, contextStateAuthority));
            var data = new byte[5];
            data[0] = (byte)proofType;
            BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(1), proofDataOffset);
            return new TransactionInstruction { ProgramId = ProgramIdKey.KeyBytes, Keys = keys, Data = data };
        }

        /// <summary>Closes a verified context state account and refunds its lamports to the destination. The context authority must sign.</summary>
        public static TransactionInstruction CloseContextState(PublicKey contextStateAccount, PublicKey destination, PublicKey contextStateAuthority)
        {
            ArgumentNullException.ThrowIfNull(contextStateAccount);
            ArgumentNullException.ThrowIfNull(destination);
            ArgumentNullException.ThrowIfNull(contextStateAuthority);
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = new List<AccountMeta> { AccountMeta.Writable(contextStateAccount, false), AccountMeta.Writable(destination, false), AccountMeta.ReadOnly(contextStateAuthority, true) },
                Data = new byte[] { 0 }
            };
        }

        /// <summary>Names proof instructions and retains their resolved account list and raw data. Decoding does not verify a proof.</summary>
        public static DecodedInstruction Decode(ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            if (data.IsEmpty) throw new ArgumentException("Instruction data is empty.", nameof(data));
            ArgumentNullException.ThrowIfNull(keys);
            ArgumentNullException.ThrowIfNull(keyIndices);
            var accounts = new List<PublicKey>();
            foreach (byte index in keyIndices)
            {
                if (index >= keys.Count) throw new ArgumentException("Invalid account index.", nameof(keyIndices));
                accounts.Add(keys[index]);
            }
            bool knownProof = data[0] >= 1 && data[0] <= 12;
            return new DecodedInstruction
            {
                PublicKey = ProgramIdKey,
                ProgramName = "ZK ElGamal Proof Program",
                InstructionName = data[0] == 0 ? "CloseContextState" : knownProof ? "Verify" + (ProofType)data[0] : "Unknown Instruction",
                Values = new Dictionary<string, object> { ["Accounts"] = accounts, ["Instruction Data"] = data[1..].ToArray() },
                InnerInstructions = new List<DecodedInstruction>()
            };
        }

        private static List<AccountMeta> ContextAccounts(PublicKey account, PublicKey authority)
        {
            if ((account == null) != (authority == null)) throw new ArgumentException("Provide both the context state account and its authority, or neither.");
            return account == null ? new List<AccountMeta>()
                : new List<AccountMeta> { AccountMeta.Writable(account, false), AccountMeta.ReadOnly(authority, false) };
        }

        private static void ValidateProofType(ProofType proofType)
        {
            if (proofType < ProofType.ZeroCiphertext || proofType > ProofType.BatchedGroupedCiphertext3HandlesValidity)
                throw new ArgumentOutOfRangeException(nameof(proofType));
        }
    }
}
