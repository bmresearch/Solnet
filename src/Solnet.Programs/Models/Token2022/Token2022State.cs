using Solnet.Programs.Models.TokenProgram;
using Solnet.Wallet;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Solnet.Programs.Models.Token2022
{
    /// <summary>An epoch-specific transfer fee. Amounts and caps are raw token units; basis points use a denominator of 10,000.</summary>
    public sealed record TransferFee(ulong Epoch, ulong MaximumFee, ushort BasisPoints)
    {
        /// <summary>Calculates the fee with integer ceiling and the configured cap, without floating-point arithmetic.</summary>
        public ulong CalculateFee(ulong amount)
        {
            if (BasisPoints > 10000) throw new InvalidOperationException("Invalid transfer fee basis points.");
            UInt128 fee = ((UInt128)amount * BasisPoints + 9999) / 10000;
            return fee > MaximumFee ? MaximumFee : (ulong)fee;
        }
    }

    /// <summary>Decoded transfer-fee mint configuration.</summary>
    public sealed record TransferFeeConfiguration(PublicKey ConfigAuthority, PublicKey WithdrawAuthority,
        ulong WithheldAmount, TransferFee OlderFee, TransferFee NewerFee)
    {
        /// <summary>Selects the active fee for the cluster epoch.</summary>
        public TransferFee GetFeeForEpoch(ulong epoch) => epoch >= NewerFee.Epoch ? NewerFee : OlderFee;
    }

    /// <summary>Decoded transfer-hook authority and program. Null denotes a disabled option.</summary>
    public sealed record TransferHookConfiguration(PublicKey Authority, PublicKey ProgramId);

    /// <summary>Decoded metadata-pointer authority and target address.</summary>
    public sealed record MetadataPointerConfiguration(PublicKey Authority, PublicKey MetadataAddress);

    /// <summary>Decoded confidential mint policy and optional 32-byte auditor ElGamal public key.</summary>
    public sealed record ConfidentialTransferMintConfiguration(PublicKey Authority, bool AutoApproveNewAccounts,
        ReadOnlyMemory<byte>? AuditorElGamalPublicKey);

    /// <summary>Decoded confidential fee configuration. Encrypted withheld amounts require the withdraw authority's ElGamal secret key to decrypt.</summary>
    public sealed record ConfidentialTransferFeeConfiguration(PublicKey Authority, ReadOnlyMemory<byte> WithdrawAuthorityElGamalPublicKey,
        bool HarvestToMintEnabled, ReadOnlyMemory<byte> EncryptedWithheldAmount);

    /// <summary>Decoded confidential account balances and counters. Encryption and proof generation remain the caller's responsibility.</summary>
    public sealed class ConfidentialTransferAccountState
    {
        /// <summary>Whether the mint's confidential authority has approved this account.</summary>
        public bool Approved { get; }
        /// <summary>The account's ElGamal public key, separate from its wallet public key.</summary>
        public ReadOnlyMemory<byte> ElGamalPublicKey { get; }
        /// <summary>The encrypted low pending-balance component.</summary>
        public ReadOnlyMemory<byte> PendingBalanceLow { get; }
        /// <summary>The encrypted high pending-balance component.</summary>
        public ReadOnlyMemory<byte> PendingBalanceHigh { get; }
        /// <summary>The encrypted available balance.</summary>
        public ReadOnlyMemory<byte> AvailableBalance { get; }
        /// <summary>The authenticated-encryption available balance maintained by the owner.</summary>
        public ReadOnlyMemory<byte> DecryptableAvailableBalance { get; }
        /// <summary>Whether confidential incoming credits are enabled.</summary>
        public bool AllowConfidentialCredits { get; }
        /// <summary>Whether public incoming credits are enabled.</summary>
        public bool AllowNonConfidentialCredits { get; }
        /// <summary>Current number of pending credits.</summary>
        public ulong PendingBalanceCreditCounter { get; }
        /// <summary>Maximum allowed pending credits.</summary>
        public ulong MaximumPendingBalanceCreditCounter { get; }
        /// <summary>Expected credit count supplied to the last apply-pending instruction.</summary>
        public ulong ExpectedPendingBalanceCreditCounter { get; }
        /// <summary>Actual credit count observed by the last apply-pending instruction.</summary>
        public ulong ActualPendingBalanceCreditCounter { get; }

        internal ConfidentialTransferAccountState(ReadOnlySpan<byte> data)
        {
            Approved = Token2022Layout.ReadBool(data[0]);
            ElGamalPublicKey = data.Slice(1, 32).ToArray();
            PendingBalanceLow = data.Slice(33, 64).ToArray();
            PendingBalanceHigh = data.Slice(97, 64).ToArray();
            AvailableBalance = data.Slice(161, 64).ToArray();
            DecryptableAvailableBalance = data.Slice(225, 36).ToArray();
            AllowConfidentialCredits = Token2022Layout.ReadBool(data[261]);
            AllowNonConfidentialCredits = Token2022Layout.ReadBool(data[262]);
            PendingBalanceCreditCounter = Token2022Layout.ReadU64(data, 263);
            MaximumPendingBalanceCreditCounter = Token2022Layout.ReadU64(data, 271);
            ExpectedPendingBalanceCreditCounter = Token2022Layout.ReadU64(data, 279);
            ActualPendingBalanceCreditCounter = Token2022Layout.ReadU64(data, 287);
        }
    }

    /// <summary>A Token-2022 mint with its base state and validated TLV extensions. Verify the RPC account owner is Token2022Program.ProgramIdKey before decoding.</summary>
    public sealed class Token2022Mint
    {
        /// <summary>The base SPL mint fields.</summary>
        public TokenMint BaseState { get; }
        /// <summary>Raw extension payloads, including unknown future extension types.</summary>
        public IReadOnlyDictionary<Token2022ExtensionType, ReadOnlyMemory<byte>> Extensions { get; }
        /// <summary>Transfer-fee settings, or null when absent.</summary>
        public TransferFeeConfiguration TransferFees { get; }
        /// <summary>Transfer-hook settings, or null when absent.</summary>
        public TransferHookConfiguration TransferHook { get; }
        /// <summary>Metadata-pointer settings, or null when absent.</summary>
        public MetadataPointerConfiguration MetadataPointer { get; }
        /// <summary>The permanent delegate, or null when absent or disabled.</summary>
        public PublicKey PermanentDelegate { get; }
        /// <summary>Default token-account state, or null when absent.</summary>
        public TokenAccount.AccountState? DefaultAccountState { get; }
        /// <summary>Whether the mint is non-transferable.</summary>
        public bool IsNonTransferable => Extensions.ContainsKey(Token2022ExtensionType.NonTransferable);
        /// <summary>Confidential-transfer policy, or null when absent.</summary>
        public ConfidentialTransferMintConfiguration ConfidentialTransfer { get; }
        /// <summary>Confidential fee settings, or null when absent.</summary>
        public ConfidentialTransferFeeConfiguration ConfidentialTransferFees { get; }

        private Token2022Mint(ReadOnlySpan<byte> data)
        {
            Extensions = Token2022Layout.Parse(data, true);
            BaseState = TokenMint.Deserialize(data[..Token2022Program.MintAccountDataSize]);
            if (Extensions.TryGetValue(Token2022ExtensionType.TransferFeeConfig, out var fees))
            {
                var bytes = fees.Span;
                TransferFees = new(Token2022Layout.OptionalKey(bytes[..32]), Token2022Layout.OptionalKey(bytes.Slice(32, 32)),
                    Token2022Layout.ReadU64(bytes, 64), Token2022Layout.ReadFee(bytes.Slice(72, 18)), Token2022Layout.ReadFee(bytes.Slice(90, 18)));
            }
            if (Extensions.TryGetValue(Token2022ExtensionType.TransferHook, out var hook))
                TransferHook = new(Token2022Layout.OptionalKey(hook.Span[..32]), Token2022Layout.OptionalKey(hook.Span.Slice(32, 32)));
            if (Extensions.TryGetValue(Token2022ExtensionType.MetadataPointer, out var metadata))
                MetadataPointer = new(Token2022Layout.OptionalKey(metadata.Span[..32]), Token2022Layout.OptionalKey(metadata.Span.Slice(32, 32)));
            if (Extensions.TryGetValue(Token2022ExtensionType.PermanentDelegate, out var permanentDelegate))
                PermanentDelegate = Token2022Layout.OptionalKey(permanentDelegate.Span);
            if (Extensions.TryGetValue(Token2022ExtensionType.DefaultAccountState, out var state))
            {
                if (state.Span[0] > 2) throw new FormatException("Invalid default account state.");
                DefaultAccountState = (TokenAccount.AccountState)state.Span[0];
            }
            if (Extensions.TryGetValue(Token2022ExtensionType.ConfidentialTransferMint, out var confidential))
            {
                var bytes = confidential.Span;
                ConfidentialTransfer = new(Token2022Layout.OptionalKey(bytes[..32]), Token2022Layout.ReadBool(bytes[32]),
                    Token2022Layout.IsZero(bytes.Slice(33, 32)) ? null : bytes.Slice(33, 32).ToArray());
            }
            if (Extensions.TryGetValue(Token2022ExtensionType.ConfidentialTransferFeeConfig, out var confidentialFees))
            {
                var bytes = confidentialFees.Span;
                ConfidentialTransferFees = new(Token2022Layout.OptionalKey(bytes[..32]), bytes.Slice(32, 32).ToArray(),
                    Token2022Layout.ReadBool(bytes[64]), bytes.Slice(65, 64).ToArray());
            }
        }

        /// <summary>Decodes a base mint or a Token-2022 mint with extensions; malformed lengths, padding, and duplicate TLV entries are rejected.</summary>
        public static Token2022Mint Deserialize(ReadOnlySpan<byte> data) => new(data);
    }

    /// <summary>A Token-2022 token account and its TLV extensions. Verify the RPC account owner before decoding.</summary>
    public sealed class Token2022Account
    {
        /// <summary>The base SPL token account fields.</summary>
        public TokenAccount BaseState { get; }
        /// <summary>Raw extension payloads, including unknown future extension types.</summary>
        public IReadOnlyDictionary<Token2022ExtensionType, ReadOnlyMemory<byte>> Extensions { get; }
        /// <summary>Public withheld transfer fees, or null when absent.</summary>
        public ulong? WithheldTransferFees { get; }
        /// <summary>Confidential balance state, or null when absent.</summary>
        public ConfidentialTransferAccountState ConfidentialTransfer { get; }
        /// <summary>Encrypted withheld fees, or null when absent.</summary>
        public ReadOnlyMemory<byte>? ConfidentialWithheldFees { get; }
        /// <summary>Whether this account has immutable ownership.</summary>
        public bool HasImmutableOwner => Extensions.ContainsKey(Token2022ExtensionType.ImmutableOwner);
        /// <summary>Whether this account belongs to a non-transferable mint.</summary>
        public bool IsNonTransferable => Extensions.ContainsKey(Token2022ExtensionType.NonTransferableAccount);
        /// <summary>Whether Token-2022 is currently invoking the account's transfer hook, or null when the extension is absent.</summary>
        public bool? IsTransferring { get; }

        private Token2022Account(ReadOnlySpan<byte> data)
        {
            Extensions = Token2022Layout.Parse(data, false);
            BaseState = TokenAccount.Deserialize(data[..Token2022Program.TokenAccountDataSize]);
            if (Extensions.TryGetValue(Token2022ExtensionType.TransferFeeAmount, out var fees)) WithheldTransferFees = Token2022Layout.ReadU64(fees.Span, 0);
            if (Extensions.TryGetValue(Token2022ExtensionType.ConfidentialTransferAccount, out var confidential)) ConfidentialTransfer = new(confidential.Span);
            if (Extensions.TryGetValue(Token2022ExtensionType.ConfidentialTransferFeeAmount, out var confidentialFees)) ConfidentialWithheldFees = confidentialFees;
            if (Extensions.TryGetValue(Token2022ExtensionType.TransferHookAccount, out var hook)) IsTransferring = Token2022Layout.ReadBool(hook.Span[0]);
        }

        /// <summary>Decodes a base token account or a Token-2022 account with extensions.</summary>
        public static Token2022Account Deserialize(ReadOnlySpan<byte> data) => new(data);
    }

    internal static class Token2022Layout
    {
        internal static int PayloadLength(Token2022ExtensionType type) => type switch
        {
            Token2022ExtensionType.TransferFeeConfig => 108,
            Token2022ExtensionType.TransferFeeAmount => 8,
            Token2022ExtensionType.MintCloseAuthority or Token2022ExtensionType.PermanentDelegate => 32,
            Token2022ExtensionType.ConfidentialTransferMint => 65,
            Token2022ExtensionType.ConfidentialTransferAccount => 295,
            Token2022ExtensionType.DefaultAccountState or Token2022ExtensionType.MemoTransfer or Token2022ExtensionType.CpiGuard or Token2022ExtensionType.TransferHookAccount => 1,
            Token2022ExtensionType.ImmutableOwner or Token2022ExtensionType.NonTransferable or Token2022ExtensionType.NonTransferableAccount => 0,
            Token2022ExtensionType.InterestBearingConfig => 52,
            Token2022ExtensionType.TransferHook or Token2022ExtensionType.MetadataPointer => 64,
            Token2022ExtensionType.ConfidentialTransferFeeConfig => 129,
            Token2022ExtensionType.ConfidentialTransferFeeAmount => 64,
            _ => -1
        };

        internal static bool? IsMintExtension(Token2022ExtensionType type) => type switch
        {
            Token2022ExtensionType.TransferFeeConfig or Token2022ExtensionType.MintCloseAuthority or Token2022ExtensionType.ConfidentialTransferMint
                or Token2022ExtensionType.DefaultAccountState or Token2022ExtensionType.NonTransferable or Token2022ExtensionType.InterestBearingConfig
                or Token2022ExtensionType.PermanentDelegate or Token2022ExtensionType.TransferHook or Token2022ExtensionType.ConfidentialTransferFeeConfig
                or Token2022ExtensionType.MetadataPointer => true,
            Token2022ExtensionType.TransferFeeAmount or Token2022ExtensionType.ImmutableOwner or Token2022ExtensionType.ConfidentialTransferAccount
                or Token2022ExtensionType.MemoTransfer or Token2022ExtensionType.CpiGuard or Token2022ExtensionType.NonTransferableAccount
                or Token2022ExtensionType.TransferHookAccount or Token2022ExtensionType.ConfidentialTransferFeeAmount => false,
            _ => null
        };

        internal static IReadOnlyDictionary<Token2022ExtensionType, ReadOnlyMemory<byte>> Parse(ReadOnlySpan<byte> data, bool mint)
        {
            int baseLength = mint ? Token2022Program.MintAccountDataSize : Token2022Program.TokenAccountDataSize;
            if (data.Length < baseLength || data.Length == Token2022Program.MultisigAccountDataSize) throw new FormatException("Invalid Token-2022 account length.");
            var result = new Dictionary<Token2022ExtensionType, ReadOnlyMemory<byte>>();
            if (data.Length == baseLength) return new ReadOnlyDictionary<Token2022ExtensionType, ReadOnlyMemory<byte>>(result);
            if (data.Length < 166 || data[165] != (mint ? 1 : 2)) throw new FormatException("Invalid Token-2022 account type.");
            if (mint && !IsZero(data.Slice(82, 83))) throw new FormatException("Invalid mint padding.");
            int offset = 166;
            while (offset < data.Length)
            {
                if (IsZero(data[offset..])) break;
                if (data.Length - offset < 4) throw new FormatException("Truncated Token-2022 extension header.");
                var type = (Token2022ExtensionType)BinaryPrimitives.ReadUInt16LittleEndian(data[offset..]);
                int length = BinaryPrimitives.ReadUInt16LittleEndian(data[(offset + 2)..]);
                offset += 4;
                if (type == Token2022ExtensionType.Uninitialized || length > data.Length - offset) throw new FormatException("Invalid Token-2022 extension length or type.");
                int expected = PayloadLength(type);
                if (expected >= 0 && expected != length) throw new FormatException($"Invalid payload length for {type}.");
                bool? mintExtension = IsMintExtension(type);
                if (mintExtension.HasValue && mintExtension.Value != mint) throw new FormatException($"Extension {type} has the wrong account type.");
                if (!result.TryAdd(type, data.Slice(offset, length).ToArray())) throw new FormatException($"Duplicate extension {type}.");
                offset += length;
            }
            return new ReadOnlyDictionary<Token2022ExtensionType, ReadOnlyMemory<byte>>(result);
        }

        internal static bool IsZero(ReadOnlySpan<byte> data)
        {
            foreach (byte value in data) if (value != 0) return false;
            return true;
        }

        internal static PublicKey OptionalKey(ReadOnlySpan<byte> data) => IsZero(data) ? null : new PublicKey(data);
        internal static ulong ReadU64(ReadOnlySpan<byte> data, int offset) => BinaryPrimitives.ReadUInt64LittleEndian(data[offset..]);
        internal static bool ReadBool(byte value) => value switch { 0 => false, 1 => true, _ => throw new FormatException("Invalid extension boolean.") };
        internal static TransferFee ReadFee(ReadOnlySpan<byte> data)
        {
            ushort bps = BinaryPrimitives.ReadUInt16LittleEndian(data[16..]);
            if (bps > 10000) throw new FormatException("Invalid transfer fee basis points.");
            return new(ReadU64(data, 0), ReadU64(data, 8), bps);
        }
    }
}
