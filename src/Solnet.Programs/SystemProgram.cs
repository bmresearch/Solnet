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
        /// The bytes that represent the program id address.
        /// </summary>
        public static readonly PublicKey ProgramIdKey = new ("11111111111111111111111111111111");

        /// <summary>
        /// The public key of the Rent System Variable.
        /// </summary>
        public static readonly PublicKey SysVarRentKey = new ("SysvarRent111111111111111111111111111111111");

        /// <summary>
        /// Account layout size.
        /// </summary>
        public const int AccountDataSize = 165;

        /// <summary>
        /// Initialize a transaction to transfer lamports.
        /// </summary>
        /// <param name="fromPublicKey">The account to transfer from.</param>
        /// <param name="toPublicKey">The account to transfer to.</param>
        /// <param name="lamports">The amount of lamports</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Transfer(Account fromPublicKey, PublicKey toPublicKey, ulong lamports)
        {
            List<AccountMeta> keys = new ()
            {
                new AccountMeta(fromPublicKey, true),
                new AccountMeta(toPublicKey, true)
            };
            byte[] data = new byte[12];
            data[0] = (byte)SystemProgramInstructions.Transfer;
            Utils.Int64ToByteArrayLe(lamports, data, 4);

            return new TransactionInstruction
            {
              ProgramId = ProgramIdKey.KeyBytes,
              Keys = keys,
              Data = data
            };
        }

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program to create a new account.
        /// </summary>
        /// <param name="fromAccount">The account from which the lamports will be transferred.</param>
        /// <param name="newAccountPublicKey">The public key of the account to which the lamports will be transferred.</param>
        /// <param name="lamports">The amount of lamports to transfer</param>
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
            byte[] data = new byte[52];

            data[0] = (byte)SystemProgramInstructions.CreateAccount;
            Utils.Int64ToByteArrayLe(lamports, data, 4);
            Utils.Int64ToByteArrayLe(space, data, 12);
            Array.Copy(programId.KeyBytes, 0, data, 20, 32);
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = data
            };
        }
    }
}