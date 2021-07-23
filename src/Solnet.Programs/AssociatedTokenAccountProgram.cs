using Solnet.Rpc.Models;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;

namespace Solnet.Programs
{
    /// <summary>
    /// Implements the Associated Token Account Program methods.
    /// <remarks>
    /// For more information see: https://spl.solana.com/associated-token-account
    /// </remarks>
    /// </summary>
    public static class AssociatedTokenAccountProgram
    {
        /// <summary>
        /// The address of the Shared Memory Program.
        /// </summary>
        public static readonly PublicKey ProgramIdKey = new("ATokenGPvbdGVxr1b2hvZbsiqW5xWH25efTNsLJA8knL");

        /// <summary>
        /// Initialize a new transaction which interacts with the Associated Token Account Program to create
        /// a new associated token account.
        /// </summary>
        /// <param name="payer">The account used to fund the associated token account.</param>
        /// <param name="owner">The public key of the owner account for the new associated token account.</param>
        /// <param name="mint">The public key of the mint for the new associated token account.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction CreateAssociatedTokenAccount(Account payer, PublicKey owner, PublicKey mint)
        {
            PublicKey associatedTokenAddress = DeriveAssociatedTokenAccount(owner, mint);
            
            List<AccountMeta> keys = new()
            {
                new AccountMeta(payer, true),
                new AccountMeta(associatedTokenAddress, true),
                new AccountMeta(owner, false),
                new AccountMeta(mint, false),
                new AccountMeta(SystemProgram.ProgramIdKey, false),
                new AccountMeta(TokenProgram.ProgramIdKey, false),
                new AccountMeta(SystemProgram.SysVarRentKey, false)
            };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = System.Array.Empty<byte>()
            };
        }

        /// <summary>
        /// Derive the public key of the associated token account for the
        /// </summary>
        /// <param name="owner">The public key of the owner account for the new associated token account.</param>
        /// <param name="mint">The public key of the mint for the new associated token account.</param>
        /// <returns>The public key of the associated token account.</returns>
        public static PublicKey DeriveAssociatedTokenAccount(PublicKey owner, PublicKey mint)
        {
            (byte[] associatedTokenAddress, int nonce) = AddressExtensions.FindProgramAddress(
                new List<byte[]> { owner.KeyBytes, TokenProgram.ProgramIdKey.KeyBytes, mint.KeyBytes },
                ProgramIdKey.KeyBytes);
            return new PublicKey(associatedTokenAddress);
        }
    }
}