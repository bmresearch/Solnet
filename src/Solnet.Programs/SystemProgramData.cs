using Solnet.Programs.Utilities;
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
            
            data.WriteU32((uint) SystemProgramInstructions.CreateAccount, 0);
            data.WriteU64(lamports, 4);
            data.WriteU64(space, 12);
            data.WritePubKey(owner, 20);
            
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
            
            data.WriteU32((uint) SystemProgramInstructions.Assign, 0);
            data.WritePubKey(programId, 4);
            
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
            
            data.WriteU32((uint) SystemProgramInstructions.Transfer, 0);
            data.WriteU64(lamports, 4);
            
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
            
            data.WriteU32((uint) SystemProgramInstructions.CreateAccountWithSeed, 0);
            data.WritePubKey(baseAccount, 4);
            data.WriteSpan(encodedSeed, 36);
            data.WriteU64(lamports, 36 + encodedSeed.Length);
            data.WriteU64(space, 44 + encodedSeed.Length);
            data.WritePubKey(owner, 52 + encodedSeed.Length);
            
            return data;
        }
        
        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.AdvanceNonceAccount"/> method.
        /// </summary>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeAdvanceNonceAccountData()
        {
            byte[] data = new byte[4];
            
            data.WriteU32((uint) SystemProgramInstructions.AdvanceNonceAccount, 0);
            
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
            
            data.WriteU32((uint) SystemProgramInstructions.WithdrawNonceAccount, 0);
            data.WriteU64(lamports, 4);
            
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
            
            data.WriteU32((uint) SystemProgramInstructions.InitializeNonceAccount, 0);
            data.WritePubKey(authorized, 4);
            
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
            
            data.WriteU32((uint) SystemProgramInstructions.AuthorizeNonceAccount, 0);
            data.WritePubKey(authorized, 4);
            
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
            
            data.WriteU32((uint) SystemProgramInstructions.Allocate, 0);
            data.WriteU64(space, 4);
            
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
            
            data.WriteU32((uint) SystemProgramInstructions.AllocateWithSeed, 0);
            data.WritePubKey(baseAccount, 4);
            data.WriteSpan(encodedSeed, 36);
            data.WriteU64(space, 36 + encodedSeed.Length);
            data.WritePubKey(owner, 44 + encodedSeed.Length);
            
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
            
            data.WriteU32((uint) SystemProgramInstructions.AssignWithSeed, 0);
            data.WritePubKey(baseAccount, 4);
            data.WriteSpan(encodedSeed, 36);
            data.WritePubKey(owner, 36 + encodedSeed.Length);
            
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
            
            data.WriteU32((uint) SystemProgramInstructions.TransferWithSeed, 0);
            data.WriteU64(lamports, 4);
            data.WriteSpan(encodedSeed, 12);
            data.WritePubKey(owner, 12 + encodedSeed.Length);
            
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
            
            encoded.WriteU32((uint) stringBytes.Length, 0);
            encoded.WriteSpan(stringBytes, 4);
            return encoded;
        }
    }
}