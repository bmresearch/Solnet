using Solnet.Rpc.Models;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;

namespace Solnet.Programs
{
    /// <summary>
    /// Implements the System Program methods.
    /// </summary>
    public static class SystemProgram
    {
        /// <summary>
        /// The public key of the System Program.
        /// </summary>
        public static readonly PublicKey ProgramIdKey = new ("11111111111111111111111111111111");
        
        /// <summary>
        /// The public key of the Recent Block Hashes System Variable. 
        /// </summary>
        public static readonly PublicKey
            SysVarRecentBlockHashesKey = new("SysvarRecentB1ockHashes11111111111111111111");
        
        /// <summary>
        /// The public key of the Rent System Variable.
        /// </summary>
        public static readonly PublicKey SysVarRentKey = new ("SysvarRent111111111111111111111111111111111");

        /// <summary>
        /// Account layout size.
        /// </summary>
        public const int AccountDataSize = 165;

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program to create a new account.
        /// </summary>
        /// <param name="fromAccount">The account from which the lamports will be transferred.</param>
        /// <param name="newAccountPublicKey">The public key of the account to which the lamports will be transferred.</param>
        /// <param name="lamports">The amount of lamports to transfer.</param>
        /// <param name="space">Number of bytes of memory to allocate for the account.</param>
        /// <param name="programId">The program id of the account.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction CreateAccount(
            Account fromAccount, Account newAccountPublicKey, ulong lamports, ulong space, PublicKey programId)
        {
            List<AccountMeta> keys = new ()
            {
                new AccountMeta(fromAccount, true),
                new AccountMeta(newAccountPublicKey, true)
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
        /// <param name="account">The account to assign a new owner.</param>
        /// <param name="programId">The program id of the account to assign as owner.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Assign(Account account, PublicKey programId)
        {
            List<AccountMeta> keys = new() {new AccountMeta(account, true)};
            
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
        /// <param name="fromPublicKey">The account to transfer from.</param>
        /// <param name="toPublicKey">The account to transfer to.</param>
        /// <param name="lamports">The amount of lamports.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Transfer(Account fromPublicKey, PublicKey toPublicKey, ulong lamports)
        {
            List<AccountMeta> keys = new ()
            {
                new AccountMeta(fromPublicKey, true),
                new AccountMeta(toPublicKey, true)
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
        /// <param name="fromPublicKey">The account to transfer from.</param>
        /// <param name="toPublicKey">The account to transfer to.</param>
        /// <param name="baseAccount">The base account.</param>
        /// <param name="seed">The seed to use to derive the account address.</param>
        /// <param name="lamports">The amount of lamports.</param>
        /// <param name="space">The number of bytes of space to allocate for the account.</param>
        /// <param name="owner">The public key of the owner to use to derive the account address.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction CreateAccountWithSeed(
            Account fromPublicKey, PublicKey toPublicKey, Account baseAccount,
            string seed, ulong lamports, ulong space, PublicKey owner)
        {
            List<AccountMeta> keys = new ()
            {
                new AccountMeta(fromPublicKey, true),
                new AccountMeta(toPublicKey, true),
                new AccountMeta(baseAccount, false)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = SystemProgramData.EncodeCreateAccountWithSeedData(
                    baseAccount.PublicKey, owner, lamports, space, seed)
            };
        }

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program
        /// to consume a stored nonce, replacing it with a successor.
        /// </summary>
        /// <param name="nonceAccountPublicKey">The public key of the nonce account.</param>
        /// <param name="authorized">The account authorized to perform nonce operations on the nonce account.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction AdvanceNonceAccount(PublicKey nonceAccountPublicKey, Account authorized)
        {
            List<AccountMeta> keys = new ()
            {
                new AccountMeta(nonceAccountPublicKey, true),
                new AccountMeta(SysVarRecentBlockHashesKey, false),
                new AccountMeta(authorized, false)
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
        /// <param name="toPublicKey">The account to transfer to.</param>
        /// <param name="authorized">The account authorized to perform nonce operations on the nonce account.</param>
        /// <param name="lamports">The amount of lamports to transfer.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction WithdrawNonceAccount(
            PublicKey nonceAccountPublicKey, PublicKey toPublicKey, Account authorized, ulong lamports)
        {
            List<AccountMeta> keys = new ()
            {
                new AccountMeta(nonceAccountPublicKey, true),
                new AccountMeta(toPublicKey, true),
                new AccountMeta(SysVarRecentBlockHashesKey, false),
                new AccountMeta(SysVarRentKey, false),
                new AccountMeta(authorized, false)
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
        /// <param name="authorized">The account authorized to perform nonce operations on the nonce account.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction InitializeNonceAccount(PublicKey nonceAccountPublicKey, Account authorized)
        {
            List<AccountMeta> keys = new ()
            {
                new AccountMeta(nonceAccountPublicKey, true),
                new AccountMeta(SysVarRecentBlockHashesKey, false),
                new AccountMeta(SysVarRentKey, false),
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = SystemProgramData.EncodeInitializeNonceAccountData(authorized.PublicKey)
            };
        }

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program to change
        /// the entity authorized to execute nonce instructions on the account.
        /// </summary>
        /// <param name="nonceAccountPublicKey">The public key of the nonce account.</param>
        /// <param name="authorized">The account authorized to perform nonce operations on the nonce account.</param>
        /// <param name="newAuthority">The new authority for the nonce operations.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction AuthorizeNonceAccount(PublicKey nonceAccountPublicKey, Account authorized, PublicKey newAuthority)
        {
            List<AccountMeta> keys = new ()
            {
                new AccountMeta(nonceAccountPublicKey, true),
                new AccountMeta(authorized, false),
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
        /// <param name="account">The account to allocate to.</param>
        /// <param name="space">The number of bytes of space to allocate.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Allocate(Account account, ulong space)
        {
            List<AccountMeta> keys = new ()
            {
                new AccountMeta(account, true)
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
        /// <param name="account">The account to allocate to.</param>
        /// <param name="baseAccount">The base account.</param>
        /// <param name="seed">The seed to use to derive the account address.</param>
        /// <param name="space">The number of bytes of space to allocate.</param>
        /// <param name="owner">The public key of the owner to use to derive the account address.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction AllocateWithSeed(
            PublicKey account, Account baseAccount, string seed, ulong space, PublicKey owner)
        {
            List<AccountMeta> keys = new ()
            {
                new AccountMeta(account, true),
                new AccountMeta(baseAccount, false)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = SystemProgramData.EncodeAllocateWithSeedData(baseAccount.PublicKey, owner, space, seed)
            };
        }

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program to
        /// assign an account to a program based on a seed.
        /// </summary>
        /// <param name="account">The account to assign to.</param>
        /// <param name="baseAccount">The base account.</param>
        /// <param name="seed">The seed to use to derive the account address.</param>
        /// <param name="owner">The public key of the owner to use to derive the account address.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction AssignWithSeed(
            PublicKey account, Account baseAccount, string seed, PublicKey owner)
        {
            List<AccountMeta> keys = new ()
            {
                new AccountMeta(account, true),
                new AccountMeta(baseAccount, false)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = SystemProgramData.EncodeAssignWithSeedData(baseAccount.PublicKey, seed, owner)
            };
        }

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program to
        /// transfer lamports from a derived address.
        /// </summary>
        /// <param name="fromPublicKey">The account to transfer from.</param>
        /// <param name="fromBaseAccount">The base account.</param>
        /// <param name="seed">The seed to use to derive the funding account address.</param>
        /// <param name="fromOwner">The public key of the owner to use to derive the funding account address.</param>
        /// <param name="toPublicKey">The account to transfer to.</param>
        /// <param name="lamports">The amount of lamports to transfer.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction TransferWithSeed(
            PublicKey fromPublicKey, Account fromBaseAccount, string seed, PublicKey fromOwner,
            PublicKey toPublicKey, ulong lamports)
        {
            List<AccountMeta> keys = new ()
            {
                new AccountMeta(fromPublicKey, true),
                new AccountMeta(fromBaseAccount, false),
                new AccountMeta(toPublicKey, false)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = SystemProgramData.EncodeTransferWithSeedData(fromOwner, seed, lamports)
            };
        }
    }
}