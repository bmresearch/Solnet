using Solnet.Programs.Models.Token2022;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solnet.Programs
{
    public static partial class Token2022Program
    {
        /// <summary>Calculates mint allocation for supported fixed-size extensions, including padding, type byte, TLV headers, and the multisig-size adjustment. Variable-size metadata must be allocated separately.</summary>
        public static int GetMintAccountDataSize(IEnumerable<Token2022ExtensionType> extensions) => GetExtensionAccountSize(extensions, true);

        /// <summary>Calculates token-account allocation for explicitly selected account extensions. Use GetRequiredAccountExtensions to include extensions mandated by the mint.</summary>
        public static int GetTokenAccountDataSize(IEnumerable<Token2022ExtensionType> extensions) => GetExtensionAccountSize(extensions, false);

        /// <summary>Returns account extensions required at initialization by the selected supported mint extensions. Confidential balances are opt-in and must be explicitly added for accounts that will use them.</summary>
        public static IReadOnlyList<Token2022ExtensionType> GetRequiredAccountExtensions(IEnumerable<Token2022ExtensionType> mintExtensions)
        {
            ArgumentNullException.ThrowIfNull(mintExtensions);
            var result = new List<Token2022ExtensionType>();
            foreach (var extension in mintExtensions.Distinct())
            {
                ValidateExtensionAllocation(extension, true);
                switch (extension)
                {
                    case Token2022ExtensionType.TransferFeeConfig: result.Add(Token2022ExtensionType.TransferFeeAmount); break;
                    case Token2022ExtensionType.NonTransferable:
                        result.Add(Token2022ExtensionType.NonTransferableAccount);
                        result.Add(Token2022ExtensionType.ImmutableOwner);
                        break;
                    case Token2022ExtensionType.TransferHook: result.Add(Token2022ExtensionType.TransferHookAccount); break;
                }
            }
            return result.Distinct().ToArray();
        }

        private static int GetExtensionAccountSize(IEnumerable<Token2022ExtensionType> extensions, bool mint)
        {
            ArgumentNullException.ThrowIfNull(extensions);
            var distinct = extensions.Distinct().ToList();
            if (distinct.Count == 0) return mint ? MintAccountDataSize : TokenAccountDataSize;
            if (mint && distinct.Contains(Token2022ExtensionType.ConfidentialTransferFeeConfig)
                && (!distinct.Contains(Token2022ExtensionType.TransferFeeConfig) || !distinct.Contains(Token2022ExtensionType.ConfidentialTransferMint)))
                throw new ArgumentException("Confidential fees require both transfer-fee and confidential-transfer mint extensions.", nameof(extensions));
            if (mint && distinct.Contains(Token2022ExtensionType.TransferFeeConfig) && distinct.Contains(Token2022ExtensionType.ConfidentialTransferMint)
                && !distinct.Contains(Token2022ExtensionType.ConfidentialTransferFeeConfig))
                throw new ArgumentException("Combining transfer fees with confidential transfers requires ConfidentialTransferFeeConfig.", nameof(extensions));
            if (mint && distinct.Contains(Token2022ExtensionType.NonTransferable) && distinct.Contains(Token2022ExtensionType.ConfidentialTransferMint))
                throw new NotSupportedException("Non-transferable confidential mints require ConfidentialMintBurn, which this allocation helper does not support.");
            int size = 166;
            foreach (var extension in distinct)
            {
                ValidateExtensionAllocation(extension, mint);
                size = checked(size + 4 + Token2022Layout.PayloadLength(extension));
            }
            return size == MultisigAccountDataSize ? size + 2 : size;
        }

        private static void ValidateExtensionAllocation(Token2022ExtensionType extension, bool mint)
        {
            if (Token2022Layout.PayloadLength(extension) < 0)
                throw new NotSupportedException($"Allocation for {extension} is not supported by this fixed-size helper.");
            if (Token2022Layout.IsMintExtension(extension) != mint)
                throw new ArgumentException($"Extension {extension} is not a {(mint ? "mint" : "token-account")} extension.", nameof(extension));
        }
    }
}
