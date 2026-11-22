using TokenAccount = Solnet.Programs.Models.TokenProgram.TokenAccount;
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
        /// <summary>Initializes transfer fees before InitializeMint/InitializeMint2. Fees are in basis points and raw token units.</summary>
        public static TransactionInstruction InitializeTransferFeeConfig(PublicKey mint, PublicKey configAuthority,
            PublicKey withdrawAuthority, ushort basisPoints, ulong maximumFee)
        {
            ValidateBasisPoints(basisPoints);
            return ExtensionInstruction(26, 0, new() { ExtensionWritable(mint) }, writer =>
            {
                WriteKeyOption(writer, configAuthority);
                WriteKeyOption(writer, withdrawAuthority);
                writer.Write(basisPoints);
                writer.Write(maximumFee);
            });
        }

        /// <summary>Transfers tokens while checking the mint decimals and expected fee. Append resolved hook accounts when required by the mint.</summary>
        public static TransactionInstruction TransferCheckedWithFee(PublicKey source, PublicKey mint, PublicKey destination,
            PublicKey authority, ulong amount, byte decimals, ulong expectedFee, IEnumerable<PublicKey> signers = null,
            IEnumerable<AccountMeta> remainingAccounts = null)
        {
            var keys = ExtensionAuthority(new() { ExtensionWritable(source), ExtensionReadOnly(mint), ExtensionWritable(destination) }, authority, signers);
            AppendRemainingAccounts(keys, remainingAccounts);
            return ExtensionInstruction(26, 1, keys, writer =>
            {
                writer.Write(amount);
                writer.Write(decimals);
                writer.Write(expectedFee);
            });
        }

        /// <summary>Withdraws fees previously harvested into the mint to a destination token account.</summary>
        public static TransactionInstruction WithdrawWithheldTokensFromMint(PublicKey mint, PublicKey destination,
            PublicKey withdrawAuthority, IEnumerable<PublicKey> signers = null)
            => ExtensionInstruction(26, 2, ExtensionAuthority(new() { ExtensionWritable(mint), ExtensionWritable(destination) }, withdrawAuthority, signers));

        /// <summary>Withdraws withheld fees directly from up to 255 token accounts.</summary>
        public static TransactionInstruction WithdrawWithheldTokensFromAccounts(PublicKey mint, PublicKey destination,
            PublicKey withdrawAuthority, IEnumerable<PublicKey> sources, IEnumerable<PublicKey> signers = null)
        {
            ArgumentNullException.ThrowIfNull(sources);
            var sourceList = sources.ToList();
            if (sourceList.Count > byte.MaxValue) throw new ArgumentOutOfRangeException(nameof(sources));
            var keys = ExtensionAuthority(new() { ExtensionReadOnly(mint), ExtensionWritable(destination) }, withdrawAuthority, signers);
            keys.AddRange(sourceList.Select(ExtensionWritable));
            return ExtensionInstruction(26, 3, keys, writer => writer.Write((byte)sourceList.Count));
        }

        /// <summary>Permissionlessly harvests withheld fees from token accounts into the mint.</summary>
        public static TransactionInstruction HarvestWithheldTokensToMint(PublicKey mint, IEnumerable<PublicKey> sources)
        {
            ArgumentNullException.ThrowIfNull(sources);
            var keys = new List<AccountMeta> { ExtensionWritable(mint) };
            keys.AddRange(sources.Select(ExtensionWritable));
            return ExtensionInstruction(26, 4, keys);
        }

        /// <summary>Schedules new transfer fees under the program's epoch activation rules.</summary>
        public static TransactionInstruction SetTransferFee(PublicKey mint, PublicKey configAuthority, ushort basisPoints,
            ulong maximumFee, IEnumerable<PublicKey> signers = null)
        {
            ValidateBasisPoints(basisPoints);
            return ExtensionInstruction(26, 5, ExtensionAuthority(new() { ExtensionWritable(mint) }, configAuthority, signers), writer =>
            {
                writer.Write(basisPoints);
                writer.Write(maximumFee);
            });
        }

        /// <summary>Initializes a transfer-hook authority and program before initializing the mint. Null keys disable the corresponding option.</summary>
        public static TransactionInstruction InitializeTransferHook(PublicKey mint, PublicKey authority, PublicKey hookProgram)
            => ExtensionInstruction(36, 0, new() { ExtensionWritable(mint) }, writer =>
            {
                WriteNullableKey(writer, authority);
                WriteNullableKey(writer, hookProgram);
            });

        /// <summary>Updates or clears a mint's transfer-hook program.</summary>
        public static TransactionInstruction UpdateTransferHook(PublicKey mint, PublicKey authority, PublicKey hookProgram,
            IEnumerable<PublicKey> signers = null)
            => ExtensionInstruction(36, 1, ExtensionAuthority(new() { ExtensionWritable(mint) }, authority, signers), writer => WriteNullableKey(writer, hookProgram));

        /// <summary>Builds a checked transfer with caller-resolved transfer-hook accounts, including the hook program and validation account in program-required order.</summary>
        public static TransactionInstruction TransferCheckedWithTransferHook(PublicKey source, PublicKey mint,
            PublicKey destination, PublicKey authority, ulong amount, byte decimals,
            IEnumerable<AccountMeta> resolvedHookAccounts, IEnumerable<PublicKey> signers = null)
        {
            ArgumentNullException.ThrowIfNull(resolvedHookAccounts);
            var keys = ExtensionAuthority(new() { ExtensionWritable(source), ExtensionReadOnly(mint), ExtensionWritable(destination) }, authority, signers);
            AppendRemainingAccounts(keys, resolvedHookAccounts);
            return ExtensionInstruction(12, null, keys, writer => { writer.Write(amount); writer.Write(decimals); });
        }

        /// <summary>Derives the transfer-hook ExtraAccountMetaList address for a mint.</summary>
        public static PublicKey DeriveTransferHookValidationAddress(PublicKey mint, PublicKey hookProgram)
        {
            ArgumentNullException.ThrowIfNull(mint);
            ArgumentNullException.ThrowIfNull(hookProgram);
            if (!PublicKey.TryFindProgramAddress(new List<byte[]> { System.Text.Encoding.UTF8.GetBytes("extra-account-metas"), mint.KeyBytes }, hookProgram, out PublicKey address, out _))
                throw new InvalidOperationException("Could not derive the transfer-hook validation address.");
            return address;
        }

        /// <summary>Initializes a metadata pointer before initializing the mint. This sets the pointer; it does not create metadata.</summary>
        public static TransactionInstruction InitializeMetadataPointer(PublicKey mint, PublicKey authority, PublicKey metadataAddress)
            => ExtensionInstruction(39, 0, new() { ExtensionWritable(mint) }, writer =>
            {
                WriteNullableKey(writer, authority);
                WriteNullableKey(writer, metadataAddress);
            });

        /// <summary>Updates or clears a mint's metadata pointer.</summary>
        public static TransactionInstruction UpdateMetadataPointer(PublicKey mint, PublicKey authority, PublicKey metadataAddress,
            IEnumerable<PublicKey> signers = null)
            => ExtensionInstruction(39, 1, ExtensionAuthority(new() { ExtensionWritable(mint) }, authority, signers), writer => WriteNullableKey(writer, metadataAddress));

        /// <summary>Sets the initial default state for new token accounts before initializing the mint. Frozen defaults require a freeze authority.</summary>
        public static TransactionInstruction InitializeDefaultAccountState(PublicKey mint, TokenAccount.AccountState state)
        {
            ValidateDefaultState(state);
            return ExtensionInstruction(28, 0, new() { ExtensionWritable(mint) }, writer => writer.Write((byte)state));
        }

        /// <summary>Updates the default state using the mint's freeze authority.</summary>
        public static TransactionInstruction UpdateDefaultAccountState(PublicKey mint, PublicKey freezeAuthority,
            TokenAccount.AccountState state, IEnumerable<PublicKey> signers = null)
        {
            ValidateDefaultState(state);
            return ExtensionInstruction(28, 1, ExtensionAuthority(new() { ExtensionWritable(mint) }, freezeAuthority, signers), writer => writer.Write((byte)state));
        }

        /// <summary>Makes a mint's tokens non-transferable. Call before initializing the mint; this extension cannot be removed.</summary>
        public static TransactionInstruction InitializeNonTransferableMint(PublicKey mint)
            => ExtensionInstruction(32, null, new() { ExtensionWritable(mint) });

        /// <summary>Sets a permanent delegate before initializing the mint. The delegate can transfer or burn public token balances for the mint.</summary>
        public static TransactionInstruction InitializePermanentDelegate(PublicKey mint, PublicKey permanentDelegate)
        {
            ArgumentNullException.ThrowIfNull(permanentDelegate);
            return ExtensionInstruction(35, null, new() { ExtensionWritable(mint) }, writer => writer.Write(permanentDelegate.KeyBytes));
        }

        private static TransactionInstruction ExtensionInstruction(byte instruction, byte? subInstruction,
            List<AccountMeta> keys, Action<BinaryWriter> writeData = null)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            writer.Write(instruction);
            if (subInstruction.HasValue) writer.Write(subInstruction.Value);
            writeData?.Invoke(writer);
            return new TransactionInstruction { ProgramId = ProgramIdKey.KeyBytes, Keys = keys, Data = stream.ToArray() };
        }

        private static AccountMeta ExtensionWritable(PublicKey key)
        {
            ArgumentNullException.ThrowIfNull(key);
            return AccountMeta.Writable(key, false);
        }

        private static AccountMeta ExtensionReadOnly(PublicKey key)
        {
            ArgumentNullException.ThrowIfNull(key);
            return AccountMeta.ReadOnly(key, false);
        }

        private static List<AccountMeta> ExtensionAuthority(List<AccountMeta> keys, PublicKey authority, IEnumerable<PublicKey> signers)
        {
            ArgumentNullException.ThrowIfNull(authority);
            var signerList = signers?.ToList() ?? new List<PublicKey>();
            if (signerList.Count > 11) throw new ArgumentOutOfRangeException(nameof(signers), "Token multisigs support at most 11 signers.");
            keys.Add(AccountMeta.ReadOnly(authority, signerList.Count == 0));
            foreach (PublicKey signer in signerList)
            {
                ArgumentNullException.ThrowIfNull(signer);
                keys.Add(AccountMeta.ReadOnly(signer, true));
            }
            return keys;
        }

        private static void AppendRemainingAccounts(List<AccountMeta> keys, IEnumerable<AccountMeta> accounts)
        {
            if (accounts == null) return;
            foreach (AccountMeta account in accounts)
            {
                ArgumentNullException.ThrowIfNull(account);
                keys.Add(account);
            }
        }

        private static void WriteKeyOption(BinaryWriter writer, PublicKey key)
        {
            writer.Write((byte)(key == null ? 0 : 1));
            if (key != null) writer.Write(key.KeyBytes);
        }

        private static void WriteNullableKey(BinaryWriter writer, PublicKey key)
        {
            if (key != null && key.KeyBytes.All(value => value == 0))
                throw new ArgumentException("Use null for an absent extension key, not the all-zero public key.", nameof(key));
            writer.Write(key?.KeyBytes ?? new byte[32]);
        }

        private static void ValidateBasisPoints(ushort basisPoints)
        {
            if (basisPoints > 10000) throw new ArgumentOutOfRangeException(nameof(basisPoints), "Basis points must be between 0 and 10000.");
        }

        private static void ValidateDefaultState(TokenAccount.AccountState state)
        {
            if (state != TokenAccount.AccountState.Initialized && state != TokenAccount.AccountState.Frozen)
                throw new ArgumentOutOfRangeException(nameof(state), "The default state must be Initialized or Frozen.");
        }
    }
}
