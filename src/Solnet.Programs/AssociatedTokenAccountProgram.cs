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
        /// The program's name.
        /// </summary>
        private const string ProgramName = "Associated Token Account Program";

        /// <summary>
        /// The instruction's name.
        /// </summary>
        private const string InstructionName = "Create Associated Token Account";

        /// <summary>
        /// Initialize a new transaction which interacts with the Associated Token Account Program to create
        /// a new associated token account.
        /// </summary>
        /// <param name="payer">The public key of the account used to fund the associated token account.</param>
        /// <param name="owner">The public key of the owner account for the new associated token account.</param>
        /// <param name="mint">The public key of the mint for the new associated token account.</param>
        /// <returns>The transaction instruction, returns null whenever an associated token address could not be derived..</returns>
        public static TransactionInstruction CreateAssociatedTokenAccount(PublicKey payer, PublicKey owner, PublicKey mint)
        {
            PublicKey associatedTokenAddress = DeriveAssociatedTokenAccount(owner, mint);

            if (associatedTokenAddress == null) return null;

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(payer, true),
                AccountMeta.Writable(associatedTokenAddress, false),
                AccountMeta.ReadOnly(owner, false),
                AccountMeta.ReadOnly(mint, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false)
            };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = Array.Empty<byte>()
            };
        }

        /// <summary>
        /// Derive the public key of the associated token account for the
        /// </summary>
        /// <param name="owner">The public key of the owner account for the new associated token account.</param>
        /// <param name="mint">The public key of the mint for the new associated token account.</param>
        /// <returns>The public key of the associated token account if it could be found, otherwise null.</returns>
        public static PublicKey DeriveAssociatedTokenAccount(PublicKey owner, PublicKey mint)
        {
            bool success = AddressExtensions.TryFindProgramAddress(
                new List<byte[]> { owner.KeyBytes, TokenProgram.ProgramIdKey.KeyBytes, mint.KeyBytes },
                ProgramIdKey.KeyBytes, out byte[] derivedAssociatedTokenAddress, out _);
            return success ? new PublicKey(derivedAssociatedTokenAddress) : null;
        }

        /// <summary>
        /// Decodes an instruction created by the Associated Token Account Program.
        /// </summary>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        /// <returns>A decoded instruction.</returns>
        public static DecodedInstruction Decode(ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            DecodedInstruction decodedInstruction = new()
            {
                PublicKey = ProgramIdKey,
                InstructionName = InstructionName,
                ProgramName = ProgramName,
                Values = new Dictionary<string, object>
                {
                    {"Payer", keys[keyIndices[0]]},
                    {"Associated Token Account Address", keys[keyIndices[1]]},
                    {"Owner", keys[keyIndices[2]]},
                    {"Mint", keys[keyIndices[3]]},
                },
                InnerInstructions = new List<DecodedInstruction>(),
            };

            return decodedInstruction;
        }
    }
}