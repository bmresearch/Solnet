using System;
using System.Collections.Generic;
using NBitcoin.DataEncoders;
using Solnet.Rpc.Models;

namespace Solnet.Programs
{
    /// <summary>
    /// Helper class for the System Program.
    /// <remarks>
    /// Used to transfer lamports between accounts and create new accounts.
    /// </remarks>
    /// </summary>
    public static class SystemProgram
    {
        /// <summary>
        /// The base58 encoder instance.
        /// </summary>
        private static readonly Base58Encoder Encoder = new ();

        /// <summary>
        /// The address of the System Program.
        /// </summary>
        public static string ProgramId = "11111111111111111111111111111111";

        /// <summary>
        /// 
        /// </summary>
        private static int ProgramIndexCreateAccount = 0;
        /// <summary>
        /// 
        /// </summary>
        private static int ProgramIndexTransfer = 2;

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
        public static TransactionInstruction Transfer(string fromPublicKey, string toPublicKey, long lamports)
        {
            return Transfer(Encoder.DecodeData(fromPublicKey), Encoder.DecodeData(toPublicKey), lamports);
        }
        
        /// <summary>
        /// Initialize a transaction to transfer lamports.
        /// </summary>
        /// <param name="fromPublicKey">The account to transfer from.</param>
        /// <param name="toPublicKey">The account to transfer to.</param>
        /// <param name="lamports">The amount of lamports</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Transfer(byte[] fromPublicKey, byte[] toPublicKey, long lamports)
        {
            var keys = new List<AccountMeta>
            {
                new(fromPublicKey, true, true),
                new(toPublicKey, false, true)
            };
            var data = new byte[12];
            
            Utils.Uint32ToByteArrayLe(ProgramIndexTransfer, data, 0);
            Utils.Int64ToByteArrayLe(lamports, data, 4);

            return new TransactionInstruction
            {
                ProgramId =  Encoder.DecodeData(ProgramId),
                Keys = keys,
                Data = data
            };
        }
        
        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program to create a new account.
        /// </summary>
        /// <param name="fromPublicKey">The account from which the lamports will be transferred.</param>
        /// <param name="newAccountPublicKey">The account to which the lamports will be transferred.</param>
        /// <param name="lamports">The amount of lamports to transfer</param>
        /// <param name="space">Number of bytes of memory to allocate for the account.</param>
        /// <param name="programId"></param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction CreateAccount(
            string fromPublicKey, string newAccountPublicKey, long lamports,
            long space, string programId)
        {
            return CreateAccount(
                Encoder.DecodeData(fromPublicKey),
                Encoder.DecodeData(newAccountPublicKey),
                lamports,
                space,
                Encoder.DecodeData(programId)
                );
        }

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the System Program to create a new account.
        /// </summary>
        /// <param name="fromPublicKey">The account from which the lamports will be transferred.</param>
        /// <param name="newAccountPublicKey">The account to which the lamports will be transferred.</param>
        /// <param name="lamports">The amount of lamports to transfer.</param>
        /// <param name="space">Number of bytes of memory to allocate for the account.</param>
        /// <param name="programId">Public key of the program to assign as the owner of the created account.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction CreateAccount(
            byte[] fromPublicKey, byte[] newAccountPublicKey, long lamports,
            long space, byte[] programId)
        {
            var keys = new List<AccountMeta>
            {
                new(fromPublicKey, true, true),
                new(newAccountPublicKey, true, true)
            };
            var data = new byte[52];
            
            Utils.Uint32ToByteArrayLe(ProgramIndexCreateAccount, data, 0);
            Utils.Int64ToByteArrayLe(lamports, data, 4);
            Utils.Int64ToByteArrayLe(space, data, 12);
            Array.Copy(programId, 0, data, 20, 32);

            return new TransactionInstruction
            {
                ProgramId = Encoder.DecodeData(ProgramId),
                Keys = keys,
                Data = data
            };
        }

    }
}