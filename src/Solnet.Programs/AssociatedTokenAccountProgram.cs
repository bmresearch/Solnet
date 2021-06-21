using Solnet.Rpc.Models;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
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
            (byte[] associatedTokenAddress, _) = AddressExtensions.FindProgramAddress(
                new List<byte[]> { owner.KeyBytes, TokenProgram.ProgramIdKey.KeyBytes, mint.KeyBytes },
                ProgramIdKey.KeyBytes);
            
            List<AccountMeta> keys = new()
            {
                new AccountMeta(payer, true),
                new AccountMeta(new PublicKey(associatedTokenAddress), false),
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
    }
}