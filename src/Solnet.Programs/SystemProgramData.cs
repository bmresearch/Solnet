// unset

using Solnet.Wallet;
using System;
using System.Text;

namespace Solnet.Programs
{
    /// <summary>
    /// Implements the system program data encodings.
    /// </summary>
    internal static class SystemProgramData
    {
        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.CreateAccount"/> method.
        /// </summary>
        /// <param name="owner">The public key of the owner program account.</param>
        /// <param name="lamports">The number of lamports to fund the account.</param>
        /// <param name="space">The space to be allocated to the account.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeCreateAccountData(PublicKey owner, ulong lamports, ulong space)
        {
            byte[] data = new byte[52];
            Utils.Uint32ToByteArrayLe((ulong) SystemProgramInstructions.CreateAccount, data, 0);
            Utils.Int64ToByteArrayLe(lamports, data, 4);
            Utils.Int64ToByteArrayLe(space, data, 12);
            Array.Copy(owner.KeyBytes, 0, data, 20, 32);
            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.Assign"/> method.
        /// </summary>
        /// <param name="programId">The program id to set as the account owner.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeAssignData(PublicKey programId)
        {
            byte[] data = new byte[36];
            Utils.Uint32ToByteArrayLe((ulong) SystemProgramInstructions.Assign, data, 0);
            Array.Copy(programId.KeyBytes, 0, data, 4, 32);
            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.Transfer"/> method.
        /// </summary>
        /// <param name="lamports">The number of lamports to fund the account.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeTransferData(ulong lamports)
        {
            byte[] data = new byte[12];
            Utils.Uint32ToByteArrayLe((ulong) SystemProgramInstructions.Transfer, data, 0);
            Utils.Int64ToByteArrayLe(lamports, data, 4);
            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.CreateAccountWithSeed"/> method.
        /// </summary>
        /// <param name="baseAccount">The public key of the base account used to derive the account address.</param>
        /// <param name="owner">The public key of the owner program account address.</param>
        /// <param name="lamports">Number of lamports to transfer to the new account.</param>
        /// <param name="space">Number of bytes of memory to allocate.</param>
        /// <param name="seed">Seed to use to derive the account address.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeCreateAccountWithSeedData(
            PublicKey baseAccount, PublicKey owner, ulong lamports, ulong space, string seed)
        {
            byte[] encodedSeed = EncodeRustString(seed);
            byte[] data = new byte[84 + encodedSeed.Length];
            Utils.Uint32ToByteArrayLe((ulong) SystemProgramInstructions.CreateAccountWithSeed, data,0);
            Array.Copy(baseAccount.KeyBytes, 0, data, 4, 32);
            Array.Copy(encodedSeed, 0, data, 36, encodedSeed.Length);
            Utils.Int64ToByteArrayLe(lamports, data, 36 + encodedSeed.Length);
            Utils.Int64ToByteArrayLe(space, data, 44 + encodedSeed.Length);
            Array.Copy(owner.KeyBytes, 0, data, 52 + encodedSeed.Length, 32);
            return data;
        }
        
        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.AdvanceNonceAccount"/> method.
        /// </summary>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeAdvanceNonceAccountData()
        {
            byte[] data = new byte[4];
            Utils.Uint32ToByteArrayLe((ulong) SystemProgramInstructions.AdvanceNonceAccount, data,0);
            return data;
        }
        
        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.WithdrawNonceAccount"/> method.
        /// </summary>
        /// <param name="lamports"></param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeWithdrawNonceAccountData(ulong lamports)
        {
            byte[] data = new byte[12];
            Utils.Uint32ToByteArrayLe((ulong) SystemProgramInstructions.WithdrawNonceAccount, data, 0);
            Utils.Int64ToByteArrayLe(lamports, data, 4);
            return data;
        }
        
        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.InitializeNonceAccount"/> method.
        /// </summary>
        /// <param name="authorized">The public key of the entity authorized to execute nonce instructions on the account.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeInitializeNonceAccountData(PublicKey authorized)
        {
            byte[] data = new byte[36];
            Utils.Uint32ToByteArrayLe((ulong) SystemProgramInstructions.InitializeNonceAccount, data, 0);
            Array.Copy(authorized.KeyBytes, 0, data, 4, 32);
            return data;
        }
        
        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.AuthorizeNonceAccount"/> method.
        /// </summary>
        /// <param name="authorized">The public key of the entity authorized to execute nonce instructions on the account.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeAuthorizeNonceAccountData(PublicKey authorized)
        {
            byte[] data = new byte[36];
            Utils.Uint32ToByteArrayLe((ulong) SystemProgramInstructions.AuthorizeNonceAccount, data, 0);
            Array.Copy(authorized.KeyBytes, 0, data, 4, 32);
            return data;
        }
        
        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.Allocate"/> method.
        /// </summary>
        /// <param name="space">Number of bytes of memory to allocate.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeAllocateData(ulong space)
        {
            byte[] data = new byte[12];
            Utils.Uint32ToByteArrayLe((ulong) SystemProgramInstructions.Allocate, data, 0);
            Utils.Int64ToByteArrayLe(space, data, 4);
            return data;
        }
        
        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.AllocateWithSeed"/> method.
        /// </summary>
        /// <param name="baseAccount">The public key of the base account.</param>
        /// <param name="space">Number of bytes of memory to allocate.</param>
        /// <param name="owner">Owner to use to derive the funding account address.</param>
        /// <param name="seed">Seed to use to derive the funding account address.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeAllocateWithSeedData(
            PublicKey baseAccount, PublicKey owner, ulong space, string seed)
        {
            byte[] encodedSeed = EncodeRustString(seed);
            byte[] data = new byte[76 + encodedSeed.Length];
            Utils.Uint32ToByteArrayLe((ulong) SystemProgramInstructions.AllocateWithSeed, data,0);
            Array.Copy(baseAccount.KeyBytes, 0, data, 4, 32);
            Array.Copy(encodedSeed, 0, data, 36, encodedSeed.Length);
            Utils.Int64ToByteArrayLe(space, data, 36 + encodedSeed.Length);
            Array.Copy(owner.KeyBytes, 0, data, 44 + encodedSeed.Length, 32);
            return data;
        }
        
        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.AssignWithSeed"/> method.
        /// </summary>
        /// <param name="baseAccount">The public key of the base account.</param>
        /// <param name="seed">Seed to use to derive the account address.</param>
        /// <param name="owner">The public key of the owner program account.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeAssignWithSeedData(
            PublicKey baseAccount, string seed, PublicKey owner)
        {
            byte[] encodedSeed = EncodeRustString(seed);
            byte[] data = new byte[68 + encodedSeed.Length];
            Utils.Uint32ToByteArrayLe((ulong) SystemProgramInstructions.AssignWithSeed, data, 0);
            Array.Copy(baseAccount.KeyBytes, 0, data, 4, 32);
            Array.Copy(encodedSeed, 0, data, 36, encodedSeed.Length);
            Array.Copy(owner.KeyBytes, 0, data, 36 + encodedSeed.Length, 32);
            return data;
        }
        
        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.TransferWithSeed"/> method.
        /// </summary>
        /// <param name="owner">Owner to use to derive the funding account address.</param>
        /// <param name="seed">Seed to use to derive the funding account address.</param>
        /// <param name="lamports">Amount of lamports to transfer.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeTransferWithSeedData(PublicKey owner, string seed, ulong lamports)
        {
            byte[] encodedSeed = EncodeRustString(seed);
            byte[] data = new byte[44 + encodedSeed.Length];
            Utils.Uint32ToByteArrayLe((ulong) SystemProgramInstructions.TransferWithSeed, data, 0);
            Utils.Int64ToByteArrayLe(lamports, data, 4);
            Array.Copy(encodedSeed, 0, data, 8, encodedSeed.Length);
            Array.Copy(owner.KeyBytes, 0, data, 8 + encodedSeed.Length, 32);
            return data;
        }

        /// <summary>
        /// Encodes a string for transaction instruction.
        /// <remarks>
        /// Example taken from here: https://github.com/michaelhly/solana-py/blob/c595b7bedb9574dbf3da7243175de3ab72810226/solana/_layouts/shared.py
        /// </remarks>
        /// </summary>
        /// <param name="data">The data to encode.</param>
        /// <returns>The encoded data.</returns>
        private static byte[] EncodeRustString(string data)
        {
            byte[] stringBytes = Encoding.ASCII.GetBytes(data);
            byte[] encoded = new byte[stringBytes.Length + 4];
            Utils.Uint32ToByteArrayLe((ulong) stringBytes.Length, encoded, 0);
            Array.Copy(stringBytes, 0, encoded, 4, stringBytes.Length);
            return encoded;
        }
    }
}