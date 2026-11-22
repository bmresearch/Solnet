using Solnet.Programs.Models.Token2022;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Solnet.Programs
{
    public static partial class Token2022Program
    {
        /// <summary>Byte length of an authenticated-encryption balance from Solana's proof SDK.</summary>
        public const int DecryptableBalanceSize = 36;

        /// <summary>Byte length of an ElGamal ciphertext.</summary>
        public const int ElGamalCiphertextSize = 64;

        /// <summary>Initializes confidential balances before initializing the mint. The optional auditor key is a 32-byte ElGamal key, not a wallet address.</summary>
        public static TransactionInstruction InitializeConfidentialTransferMint(PublicKey mint, PublicKey authority,
            bool autoApproveNewAccounts, byte[] auditorElGamalPublicKey = null)
            => ExtensionInstruction(27, 0, new() { ExtensionWritable(mint) }, writer =>
            {
                WriteNullableKey(writer, authority);
                writer.Write(autoApproveNewAccounts);
                WriteOptionalElGamalKey(writer, auditorElGamalPublicKey);
            });

        /// <summary>Updates approval policy and the optional auditor key for future confidential transfers.</summary>
        public static TransactionInstruction UpdateConfidentialTransferMint(PublicKey mint, PublicKey authority,
            bool autoApproveNewAccounts, byte[] auditorElGamalPublicKey = null, IEnumerable<PublicKey> signers = null)
            => ExtensionInstruction(27, 1, ExtensionAuthority(new() { ExtensionWritable(mint) }, authority, signers), writer =>
            {
                writer.Write(autoApproveNewAccounts);
                WriteOptionalElGamalKey(writer, auditorElGamalPublicKey);
            });

        /// <summary>Configures a token account using a PubkeyValidity proof and an encrypted zero balance generated with the account owner's encryption keys.</summary>
        public static TransactionInstruction ConfigureConfidentialTransferAccount(PublicKey account, PublicKey mint,
            PublicKey authority, byte[] decryptableZeroBalance, ConfidentialProofLocation pubkeyValidityProof,
            ulong maximumPendingBalanceCreditCounter = 65536, IEnumerable<PublicKey> signers = null)
        {
            ValidateEncryptedBytes(decryptableZeroBalance, DecryptableBalanceSize, nameof(decryptableZeroBalance));
            var keys = ConfidentialProofAccounts(new() { ExtensionWritable(account), ExtensionReadOnly(mint) }, pubkeyValidityProof);
            return ExtensionInstruction(27, 2, ExtensionAuthority(keys, authority, signers), writer =>
            {
                writer.Write(decryptableZeroBalance);
                writer.Write(maximumPendingBalanceCreditCounter);
                WriteProofOffsets(writer, pubkeyValidityProof);
            });
        }

        /// <summary>Approves a configured confidential account using the mint's confidential-transfer authority.</summary>
        public static TransactionInstruction ApproveConfidentialTransferAccount(PublicKey account, PublicKey mint,
            PublicKey authority, IEnumerable<PublicKey> signers = null)
            => ExtensionInstruction(27, 3, ExtensionAuthority(new() { ExtensionWritable(account), ExtensionReadOnly(mint) }, authority, signers));

        /// <summary>Proves that the available confidential balance is zero so the account can be closed. Pending balances and withheld fees must also be cleared.</summary>
        public static TransactionInstruction EmptyConfidentialTransferAccount(PublicKey account, PublicKey authority,
            ConfidentialProofLocation zeroCiphertextProof, IEnumerable<PublicKey> signers = null)
        {
            var keys = ConfidentialProofAccounts(new() { ExtensionWritable(account) }, zeroCiphertextProof);
            return ExtensionInstruction(27, 4, ExtensionAuthority(keys, authority, signers), writer => WriteProofOffsets(writer, zeroCiphertextProof));
        }

        /// <summary>Moves raw token units from the public balance into the confidential pending balance. Each deposit is limited to 48 bits.</summary>
        public static TransactionInstruction DepositConfidentialTokens(PublicKey account, PublicKey mint,
            PublicKey authority, ulong amount, byte decimals, IEnumerable<PublicKey> signers = null)
        {
            if (amount > (1UL << 48) - 1) throw new ArgumentOutOfRangeException(nameof(amount), "Confidential deposits are limited to 48-bit amounts.");
            return ExtensionInstruction(27, 5, ExtensionAuthority(new() { ExtensionWritable(account), ExtensionReadOnly(mint) }, authority, signers), writer =>
            {
                writer.Write(amount);
                writer.Write(decimals);
            });
        }

        /// <summary>Withdraws confidential tokens to the public balance using CiphertextCommitmentEquality and BatchedRangeProofU64 proofs.</summary>
        public static TransactionInstruction WithdrawConfidentialTokens(PublicKey account, PublicKey mint,
            PublicKey authority, ulong amount, byte decimals, byte[] newDecryptableAvailableBalance,
            ConfidentialProofLocation equalityProof, ConfidentialProofLocation rangeProof, IEnumerable<PublicKey> signers = null)
        {
            ValidateEncryptedBytes(newDecryptableAvailableBalance, DecryptableBalanceSize, nameof(newDecryptableAvailableBalance));
            var keys = ConfidentialProofAccounts(new() { ExtensionWritable(account), ExtensionReadOnly(mint) }, equalityProof, rangeProof);
            return ExtensionInstruction(27, 6, ExtensionAuthority(keys, authority, signers), writer =>
            {
                writer.Write(amount);
                writer.Write(decimals);
                writer.Write(newDecryptableAvailableBalance);
                WriteProofOffsets(writer, equalityProof, rangeProof);
            });
        }

        /// <summary>Transfers confidential tokens using equality, batched three-handle validity, and U128 range proofs. Auditor ciphertexts may be null only when no auditor is configured.</summary>
        public static TransactionInstruction TransferConfidentialTokens(PublicKey source, PublicKey mint,
            PublicKey destination, PublicKey authority, byte[] newSourceDecryptableAvailableBalance,
            ConfidentialProofLocation equalityProof, ConfidentialProofLocation ciphertextValidityProof,
            ConfidentialProofLocation rangeProof, byte[] auditorCiphertextLow = null, byte[] auditorCiphertextHigh = null,
            IEnumerable<PublicKey> signers = null)
            => ConfidentialTransferInstruction(7, source, mint, destination, authority, newSourceDecryptableAvailableBalance,
                auditorCiphertextLow, auditorCiphertextHigh, signers, equalityProof, ciphertextValidityProof, rangeProof);

        /// <summary>Applies pending credits and updates the locally decryptable available balance. The counter and encrypted balance must be computed from fresh account state.</summary>
        public static TransactionInstruction ApplyConfidentialPendingBalance(PublicKey account, PublicKey authority,
            ulong expectedPendingBalanceCreditCounter, byte[] newDecryptableAvailableBalance, IEnumerable<PublicKey> signers = null)
        {
            ValidateEncryptedBytes(newDecryptableAvailableBalance, DecryptableBalanceSize, nameof(newDecryptableAvailableBalance));
            return ExtensionInstruction(27, 8, ExtensionAuthority(new() { ExtensionWritable(account) }, authority, signers), writer =>
            {
                writer.Write(expectedPendingBalanceCreditCounter);
                writer.Write(newDecryptableAvailableBalance);
            });
        }

        /// <summary>Allows an account to receive confidential credits.</summary>
        public static TransactionInstruction EnableConfidentialCredits(PublicKey account, PublicKey authority, IEnumerable<PublicKey> signers = null)
            => ConfidentialCreditsInstruction(9, account, authority, signers);

        /// <summary>Blocks incoming confidential credits.</summary>
        public static TransactionInstruction DisableConfidentialCredits(PublicKey account, PublicKey authority, IEnumerable<PublicKey> signers = null)
            => ConfidentialCreditsInstruction(10, account, authority, signers);

        /// <summary>Allows an account to receive ordinary public token transfers.</summary>
        public static TransactionInstruction EnableNonConfidentialCredits(PublicKey account, PublicKey authority, IEnumerable<PublicKey> signers = null)
            => ConfidentialCreditsInstruction(11, account, authority, signers);

        /// <summary>Blocks incoming ordinary public token transfers.</summary>
        public static TransactionInstruction DisableNonConfidentialCredits(PublicKey account, PublicKey authority, IEnumerable<PublicKey> signers = null)
            => ConfidentialCreditsInstruction(12, account, authority, signers);

        /// <summary>Transfers confidential tokens with fees using equality, transfer validity, percentage-with-cap, fee validity, and U256 range proofs, in that order.</summary>
        public static TransactionInstruction TransferConfidentialTokensWithFee(PublicKey source, PublicKey mint,
            PublicKey destination, PublicKey authority, byte[] newSourceDecryptableAvailableBalance,
            ConfidentialProofLocation equalityProof, ConfidentialProofLocation transferValidityProof,
            ConfidentialProofLocation feePercentageProof, ConfidentialProofLocation feeValidityProof,
            ConfidentialProofLocation rangeProof, byte[] auditorCiphertextLow = null, byte[] auditorCiphertextHigh = null,
            IEnumerable<PublicKey> signers = null)
            => ConfidentialTransferInstruction(13, source, mint, destination, authority, newSourceDecryptableAvailableBalance,
                auditorCiphertextLow, auditorCiphertextHigh, signers, equalityProof, transferValidityProof,
                feePercentageProof, feeValidityProof, rangeProof);

        /// <summary>Configures an account from an ElGamal registry entry. Supply a payer to fund any reallocation required by the program.</summary>
        public static TransactionInstruction ConfigureConfidentialTransferAccountWithRegistry(PublicKey account,
            PublicKey mint, PublicKey registryAccount, PublicKey payer = null)
        {
            var keys = new List<AccountMeta> { ExtensionWritable(account), ExtensionReadOnly(mint), ExtensionReadOnly(registryAccount) };
            if (payer != null)
            {
                keys.Add(AccountMeta.Writable(payer, true));
                keys.Add(ExtensionReadOnly(SystemProgram.ProgramIdKey));
            }
            return ExtensionInstruction(27, 14, keys);
        }

        private static TransactionInstruction ConfidentialCreditsInstruction(byte subInstruction, PublicKey account,
            PublicKey authority, IEnumerable<PublicKey> signers)
            => ExtensionInstruction(27, subInstruction, ExtensionAuthority(new() { ExtensionWritable(account) }, authority, signers));

        private static TransactionInstruction ConfidentialTransferInstruction(byte subInstruction, PublicKey source,
            PublicKey mint, PublicKey destination, PublicKey authority, byte[] newBalance, byte[] auditorLow,
            byte[] auditorHigh, IEnumerable<PublicKey> signers, params ConfidentialProofLocation[] proofs)
        {
            ValidateEncryptedBytes(newBalance, DecryptableBalanceSize, nameof(newBalance));
            if (auditorLow != null) ValidateEncryptedBytes(auditorLow, ElGamalCiphertextSize, nameof(auditorLow));
            if (auditorHigh != null) ValidateEncryptedBytes(auditorHigh, ElGamalCiphertextSize, nameof(auditorHigh));
            if ((auditorLow == null) != (auditorHigh == null)) throw new ArgumentException("Both auditor ciphertext components must be provided together.");
            var keys = ConfidentialProofAccounts(new() { ExtensionWritable(source), ExtensionReadOnly(mint), ExtensionWritable(destination) }, proofs);
            return ExtensionInstruction(27, subInstruction, ExtensionAuthority(keys, authority, signers), writer =>
            {
                writer.Write(newBalance);
                writer.Write(auditorLow ?? new byte[ElGamalCiphertextSize]);
                writer.Write(auditorHigh ?? new byte[ElGamalCiphertextSize]);
                WriteProofOffsets(writer, proofs);
            });
        }

        private static List<AccountMeta> ConfidentialProofAccounts(List<AccountMeta> keys, params ConfidentialProofLocation[] proofs)
        {
            foreach (var proof in proofs) ArgumentNullException.ThrowIfNull(proof);
            if (proofs.Any(proof => proof.InstructionOffset != 0)) keys.Add(ExtensionReadOnly(SysVars.InstructionsKey));
            foreach (var proof in proofs)
                if (proof.ContextStateAccount != null) keys.Add(ExtensionReadOnly(proof.ContextStateAccount));
            return keys;
        }

        private static void WriteProofOffsets(BinaryWriter writer, params ConfidentialProofLocation[] proofs)
        {
            foreach (var proof in proofs) writer.Write(unchecked((byte)proof.InstructionOffset));
        }

        private static void WriteOptionalElGamalKey(BinaryWriter writer, byte[] key)
        {
            if (key != null)
            {
                ValidateEncryptedBytes(key, 32, nameof(key));
                if (key.All(value => value == 0)) throw new ArgumentException("Use null for an absent auditor key.", nameof(key));
            }
            writer.Write(key ?? new byte[32]);
        }

        private static void ValidateEncryptedBytes(byte[] value, int length, string parameterName)
        {
            ArgumentNullException.ThrowIfNull(value, parameterName);
            if (value.Length != length) throw new ArgumentException($"Expected exactly {length} bytes.", parameterName);
        }
    }
}
