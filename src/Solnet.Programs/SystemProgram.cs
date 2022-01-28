using Solnet.Programs.Utilities;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;

namespace Solnet.Programs
{
    /// <summary>
    /// Implements the System Program methods.
    /// <remarks>
    /// For more information see:
    /// https://docs.solana.com/developing/runtime-facilities/programs#system-program
    /// https://docs.rs/solana-sdk/1.7.0/solana_sdk/system_instruction/enum.SystemInstruction.html
    /// </remarks>
    /// </summary>
    public static class SystemProgram
    {
        /// <summary>
        /// The public key of the System Program.
        /// </summary>
        public static readonly PublicKey ProgramIdKey = new("11111111111111111111111111111111");

        /// <summary>
        /// The program's name.
        /// </summary>
        private const string ProgramName = "System Program";

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program to create a new account.
        /// </summary>
        /// <param name="fromAccount">The public key of the account from which the lamports will be transferred.</param>
        /// <param name="newAccountPublicKey">The public key of the account to which the lamports will be transferred.</param>
        /// <param name="lamports">The amount of lamports to transfer.</param>
        /// <param name="space">Number of bytes of memory to allocate for the account.</param>
        /// <param name="programId">The program id of the account.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction CreateAccount(
            PublicKey fromAccount, PublicKey newAccountPublicKey, ulong lamports, ulong space, PublicKey programId)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(fromAccount, true),
                AccountMeta.Writable(newAccountPublicKey, true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = SystemProgramData.EncodeCreateAccountData(programId, lamports, space)
            };
        }

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program to assign a new account owner.
        /// </summary>
        /// <param name="account">The public key of the account to assign a new owner.</param>
        /// <param name="programId">The program id of the account to assign as owner.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Assign(PublicKey account, PublicKey programId)
        {
            List<AccountMeta> keys = new() { AccountMeta.Writable(account, true) };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = SystemProgramData.EncodeAssignData(programId)
            };
        }

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program to transfer lamports.
        /// </summary>
        /// <param name="fromPublicKey">The public key of the account to transfer from.</param>
        /// <param name="toPublicKey">The public key of the account to transfer to.</param>
        /// <param name="lamports">The amount of lamports.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Transfer(PublicKey fromPublicKey, PublicKey toPublicKey, ulong lamports)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(fromPublicKey, true),
                AccountMeta.Writable(toPublicKey, false)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = SystemProgramData.EncodeTransferData(lamports)
            };
        }

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program
        /// to create a new account at an address derived from a base public key and a seed.
        /// </summary>
        /// <param name="fromPublicKey">The public key of the account to transfer from.</param>
        /// <param name="toPublicKey">The public key of the account to transfer to.</param>
        /// <param name="baseAccount">The public key of the base account.</param>
        /// <param name="seed">The seed to use to derive the account address.</param>
        /// <param name="lamports">The amount of lamports.</param>
        /// <param name="space">The number of bytes of space to allocate for the account.</param>
        /// <param name="owner">The public key of the owner to use to derive the account address.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction CreateAccountWithSeed(
            PublicKey fromPublicKey, PublicKey toPublicKey, PublicKey baseAccount,
            string seed, ulong lamports, ulong space, PublicKey owner)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(fromPublicKey, true),
                AccountMeta.Writable(toPublicKey, false),
                //AccountMeta.ReadOnly(baseAccount, true)
            };
            if (baseAccount != fromPublicKey)
            {
                keys.Add(AccountMeta.ReadOnly(baseAccount, true));
            }
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = SystemProgramData.EncodeCreateAccountWithSeedData(
                    baseAccount, owner, lamports, space, seed)
            };
        }

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program
        /// to consume a stored nonce, replacing it with a successor.
        /// </summary>
        /// <param name="nonceAccountPublicKey">The public key of the nonce account.</param>
        /// <param name="authorized">The public key of the account authorized to perform nonce operations on the nonce account.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction AdvanceNonceAccount(PublicKey nonceAccountPublicKey, PublicKey authorized)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(nonceAccountPublicKey, false),
                AccountMeta.ReadOnly(SysVars.RecentBlockHashesKey, false),
                AccountMeta.ReadOnly(authorized, true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = SystemProgramData.EncodeAdvanceNonceAccountData()
            };
        }

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program to withdraw funds from a nonce account.
        /// </summary>
        /// <param name="nonceAccountPublicKey">The public key of the nonce account.</param>
        /// <param name="toPublicKey">The public key of the account to transfer to.</param>
        /// <param name="authorized">The public key of the account authorized to perform nonce operations on the nonce account.</param>
        /// <param name="lamports">The amount of lamports to transfer.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction WithdrawNonceAccount(
            PublicKey nonceAccountPublicKey, PublicKey toPublicKey, PublicKey authorized, ulong lamports)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(nonceAccountPublicKey, false),
                AccountMeta.Writable(toPublicKey, false),
                AccountMeta.ReadOnly(SysVars.RecentBlockHashesKey, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false),
                AccountMeta.ReadOnly(authorized, true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = SystemProgramData.EncodeWithdrawNonceAccountData(lamports)
            };
        }

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program to drive the
        /// state of an Uninitialized nonce account to Initialized, setting the nonce value.
        /// </summary>
        /// <param name="nonceAccountPublicKey">The public key of the nonce account.</param>
        /// <param name="authorized">The public key of the account authorized to perform nonce operations on the nonce account.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction InitializeNonceAccount(PublicKey nonceAccountPublicKey, PublicKey authorized)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(nonceAccountPublicKey, false),
                AccountMeta.ReadOnly(SysVars.RecentBlockHashesKey, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false),
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = SystemProgramData.EncodeInitializeNonceAccountData(authorized)
            };
        }

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program to change
        /// the entity authorized to execute nonce instructions on the account.
        /// </summary>
        /// <param name="nonceAccountPublicKey">The public key of the nonce account.</param>
        /// <param name="authorized">The public key of the account authorized to perform nonce operations on the nonce account.</param>
        /// <param name="newAuthority">The public key of the new authority for the nonce operations.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction AuthorizeNonceAccount(PublicKey nonceAccountPublicKey, PublicKey authorized, PublicKey newAuthority)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(nonceAccountPublicKey, false),
                AccountMeta.ReadOnly(authorized, true),
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = SystemProgramData.EncodeAuthorizeNonceAccountData(newAuthority)
            };
        }

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program to
        /// allocate space in a (possibly new) account without funding.
        /// </summary>
        /// <param name="account">The public key of the account to allocate space to.</param>
        /// <param name="space">The number of bytes of space to allocate.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Allocate(PublicKey account, ulong space)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(account, true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = SystemProgramData.EncodeAllocateData(space)
            };
        }

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program to
        /// allocate space for and assign an account at an address derived from a base public key and a seed.
        /// </summary>
        /// <param name="account">The public key of the account to allocate space to.</param>
        /// <param name="baseAccount">The public key of the base account.</param>
        /// <param name="seed">The seed to use to derive the account address.</param>
        /// <param name="space">The number of bytes of space to allocate.</param>
        /// <param name="owner">The public key of the owner to use to derive the account address.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction AllocateWithSeed(
            PublicKey account, PublicKey baseAccount, string seed, ulong space, PublicKey owner)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(account, false),
                AccountMeta.ReadOnly(baseAccount, true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = SystemProgramData.EncodeAllocateWithSeedData(baseAccount, owner, space, seed)
            };
        }

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program to
        /// assign an account to a program based on a seed.
        /// </summary>
        /// <param name="account">The public key of the account to assign to.</param>
        /// <param name="baseAccount">The public key of the base account.</param>
        /// <param name="seed">The seed to use to derive the account address.</param>
        /// <param name="owner">The public key of the owner to use to derive the account address.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction AssignWithSeed(
            PublicKey account, PublicKey baseAccount, string seed, PublicKey owner)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(account, false),
                AccountMeta.ReadOnly(baseAccount, true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = SystemProgramData.EncodeAssignWithSeedData(baseAccount, seed, owner)
            };
        }

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program to
        /// transfer lamports from a derived address.
        /// </summary>
        /// <param name="fromPublicKey">The public key of the account to transfer from.</param>
        /// <param name="fromBaseAccount">The public key of the base account.</param>
        /// <param name="seed">The seed to use to derive the funding account address.</param>
        /// <param name="fromOwner">The public key of the owner to use to derive the funding account address.</param>
        /// <param name="toPublicKey">The account to transfer to.</param>
        /// <param name="lamports">The amount of lamports to transfer.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction TransferWithSeed(
            PublicKey fromPublicKey, PublicKey fromBaseAccount, string seed, PublicKey fromOwner,
            PublicKey toPublicKey, ulong lamports)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(fromPublicKey, false),
                AccountMeta.ReadOnly(fromBaseAccount, true),
                AccountMeta.ReadOnly(toPublicKey, false)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = SystemProgramData.EncodeTransferWithSeedData(fromOwner, seed, lamports)
            };
        }

        /// <summary>
        /// Decodes an instruction created by the System Program.
        /// </summary>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        /// <returns>A decoded instruction.</returns>
        public static DecodedInstruction Decode(ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            uint instruction = data.GetU32(SystemProgramData.MethodOffset);
            SystemProgramInstructions.Values instructionValue =
                (SystemProgramInstructions.Values)Enum.Parse(typeof(SystemProgramInstructions.Values), instruction.ToString());

            DecodedInstruction decodedInstruction = new()
            {
                PublicKey = ProgramIdKey,
                InstructionName = SystemProgramInstructions.Names[instructionValue],
                ProgramName = ProgramName,
                Values = new Dictionary<string, object>(),
                InnerInstructions = new List<DecodedInstruction>()
            };

            switch (instructionValue)
            {
                case SystemProgramInstructions.Values.CreateAccount:
                    SystemProgramData.DecodeCreateAccountData(decodedInstruction, data, keys, keyIndices);
                    break;
                case SystemProgramInstructions.Values.Assign:
                    SystemProgramData.DecodeAssignData(decodedInstruction, data, keys, keyIndices);
                    break;
                case SystemProgramInstructions.Values.Transfer:
                    SystemProgramData.DecodeTransferData(decodedInstruction, data, keys, keyIndices);
                    break;
                case SystemProgramInstructions.Values.CreateAccountWithSeed:
                    SystemProgramData.DecodeCreateAccountWithSeedData(decodedInstruction, data, keys, keyIndices);
                    break;
                case SystemProgramInstructions.Values.AdvanceNonceAccount:
                    SystemProgramData.DecodeAdvanceNonceAccountData(decodedInstruction, keys, keyIndices);
                    break;
                case SystemProgramInstructions.Values.WithdrawNonceAccount:
                    SystemProgramData.DecodeWithdrawNonceAccountData(decodedInstruction, data, keys, keyIndices);
                    break;
                case SystemProgramInstructions.Values.InitializeNonceAccount:
                    SystemProgramData.DecodeInitializeNonceAccountData(decodedInstruction, data, keys, keyIndices);
                    break;
                case SystemProgramInstructions.Values.AuthorizeNonceAccount:
                    SystemProgramData.DecodeAuthorizeNonceAccountData(decodedInstruction, data, keys, keyIndices);
                    break;
                case SystemProgramInstructions.Values.Allocate:
                    SystemProgramData.DecodeAllocateData(decodedInstruction, data, keys, keyIndices);
                    break;
                case SystemProgramInstructions.Values.AllocateWithSeed:
                    SystemProgramData.DecodeAllocateWithSeedData(decodedInstruction, data, keys, keyIndices);
                    break;
                case SystemProgramInstructions.Values.AssignWithSeed:
                    SystemProgramData.DecodeAssignWithSeedData(decodedInstruction, data, keys, keyIndices);
                    break;
                case SystemProgramInstructions.Values.TransferWithSeed:
                    SystemProgramData.DecodeTransferWithSeedData(decodedInstruction, data, keys, keyIndices);
                    break;
            }

            return decodedInstruction;
        }
    }
}