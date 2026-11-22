using Solnet.Programs.Models.Token2022;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solnet.Programs
{
    public static partial class Token2022Program
    {
        /// <summary>Initializes confidential fee collection before mint initialization. Requires the transfer-fee and confidential-transfer mint extensions.</summary>
        public static TransactionInstruction InitializeConfidentialTransferFeeConfig(PublicKey mint, PublicKey authority,
            byte[] withdrawWithheldAuthorityElGamalPublicKey)
        {
            ValidateEncryptedBytes(withdrawWithheldAuthorityElGamalPublicKey, 32, nameof(withdrawWithheldAuthorityElGamalPublicKey));
            return ExtensionInstruction(37, 0, new() { ExtensionWritable(mint) }, writer =>
            {
                WriteNullableKey(writer, authority);
                writer.Write(withdrawWithheldAuthorityElGamalPublicKey);
            });
        }

        /// <summary>Withdraws the mint's encrypted withheld fees using a CiphertextCiphertextEquality proof.</summary>
        public static TransactionInstruction WithdrawConfidentialWithheldTokensFromMint(PublicKey mint, PublicKey destination,
            PublicKey withdrawAuthority, byte[] newDecryptableAvailableBalance, ConfidentialProofLocation equalityProof,
            IEnumerable<PublicKey> signers = null)
        {
            ValidateEncryptedBytes(newDecryptableAvailableBalance, DecryptableBalanceSize, nameof(newDecryptableAvailableBalance));
            var keys = ConfidentialProofAccounts(new() { ExtensionWritable(mint), ExtensionWritable(destination) }, equalityProof);
            return ExtensionInstruction(37, 1, ExtensionAuthority(keys, withdrawAuthority, signers), writer =>
            {
                WriteProofOffsets(writer, equalityProof);
                writer.Write(newDecryptableAvailableBalance);
            });
        }

        /// <summary>Withdraws encrypted withheld fees from up to 255 token accounts using a CiphertextCiphertextEquality proof.</summary>
        public static TransactionInstruction WithdrawConfidentialWithheldTokensFromAccounts(PublicKey mint, PublicKey destination,
            PublicKey withdrawAuthority, IEnumerable<PublicKey> sources, byte[] newDecryptableAvailableBalance,
            ConfidentialProofLocation equalityProof, IEnumerable<PublicKey> signers = null)
        {
            ArgumentNullException.ThrowIfNull(sources);
            var sourceList = sources.ToList();
            if (sourceList.Count > byte.MaxValue) throw new ArgumentOutOfRangeException(nameof(sources));
            ValidateEncryptedBytes(newDecryptableAvailableBalance, DecryptableBalanceSize, nameof(newDecryptableAvailableBalance));
            var keys = ConfidentialProofAccounts(new() { ExtensionWritable(mint), ExtensionWritable(destination) }, equalityProof);
            ExtensionAuthority(keys, withdrawAuthority, signers);
            keys.AddRange(sourceList.Select(ExtensionWritable));
            return ExtensionInstruction(37, 2, keys, writer =>
            {
                writer.Write((byte)sourceList.Count);
                WriteProofOffsets(writer, equalityProof);
                writer.Write(newDecryptableAvailableBalance);
            });
        }

        /// <summary>Harvests encrypted withheld fees from token accounts into the mint when harvesting is enabled.</summary>
        public static TransactionInstruction HarvestConfidentialWithheldTokensToMint(PublicKey mint, IEnumerable<PublicKey> sources)
        {
            ArgumentNullException.ThrowIfNull(sources);
            var keys = new List<AccountMeta> { ExtensionWritable(mint) };
            keys.AddRange(sources.Select(ExtensionWritable));
            return ExtensionInstruction(37, 3, keys);
        }

        /// <summary>Enables permissionless harvesting of encrypted fees into the mint.</summary>
        public static TransactionInstruction EnableConfidentialFeeHarvestToMint(PublicKey mint, PublicKey authority, IEnumerable<PublicKey> signers = null)
            => ExtensionInstruction(37, 4, ExtensionAuthority(new() { ExtensionWritable(mint) }, authority, signers));

        /// <summary>Disables permissionless harvesting of encrypted fees into the mint.</summary>
        public static TransactionInstruction DisableConfidentialFeeHarvestToMint(PublicKey mint, PublicKey authority, IEnumerable<PublicKey> signers = null)
            => ExtensionInstruction(37, 5, ExtensionAuthority(new() { ExtensionWritable(mint) }, authority, signers));
    }
}
