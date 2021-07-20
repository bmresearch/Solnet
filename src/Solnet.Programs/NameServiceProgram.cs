using Solnet.Programs.Utilities;
using Solnet.Rpc.Models;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Solnet.Programs
{
    /// <summary>
    /// Implements the Token Program methods.
    /// <remarks>
    /// For more information see: https://spl.solana.com/name-service
    /// </remarks>
    /// </summary>
    public static class NameServiceProgram
    {
        /// <summary>
        /// The hash prefix used to calculate the SHA256 of the name record to be created.
        /// </summary>
        private static readonly string HashPrefix = "SPL Name Service";
        
        /// <summary>
        /// The public key of the Name Service Program.
        /// </summary>
        public static readonly PublicKey ProgramIdKey = new("namesLPneVptA9Z5rqUDD9tMTWEJwofgaYwp8cawRkX");
        
        /// <summary>
        /// The space to be used when creating the name record.
        /// </summary>
        public const int NameAccountSize = 1000;

        /// <summary>
        /// Initialize a new transaction instruction to create a name record.
        /// </summary>
        /// <param name="name">The name to use.</param>
        /// <param name="payer">The account of the payer.</param>
        /// <param name="nameOwner">The public key of the name owner.</param>
        /// <param name="nameClass">The public key of the account of the name class.</param>
        /// <param name="parentName">The public key of the parent name.</param>
        /// <param name="parentNameOwner">The public key of the account of the parent name owner.</param>
        /// <param name="space">The space to assign to the account.</param>
        /// <param name="lamports">The amount of lamports the account needs to be rate exempt.</param>
        /// <returns>The transaction instruction.</returns>
        /// <exception cref="Exception">Thrown when it was not possible to derive a program address for the account.</exception>
        public static TransactionInstruction CreateNameRegistry(
            PublicKey name, PublicKey payer, PublicKey nameOwner, ulong lamports, uint space, PublicKey nameClass = null, 
            PublicKey parentNameOwner = null, PublicKey parentName = null)
        {
            byte[] hashedName = ComputeHashedName(name.Key);
            PublicKey nameAccountKey = DeriveNameAccountKey(hashedName, nameClass, parentName);
            if (nameAccountKey == null) throw new Exception("could not derive an address for the name account");
            return CreateNameRegistryInstruction(
                nameAccountKey, nameOwner, payer, hashedName, lamports, space, nameClass, parentNameOwner, parentName);
        }
        
        /// <summary>
        /// Gets the hash for the given value with the attached hash prefix.
        /// </summary>
        /// <param name="name">The name to hash attach the prefix and hash.</param>
        /// <returns>The hash as bytes.</returns>
        public static byte[] ComputeHashedName(string name)
        {
            string prefixedName = HashPrefix + name;
            byte[] fullNameBytes = Encoding.UTF8.GetBytes(prefixedName);
            using SHA256Managed sha = new();
            return sha.ComputeHash(fullNameBytes, 0, fullNameBytes.Length);
        }
        
        /// <summary>
        /// Get's the program derived address for the name.
        /// </summary>
        /// <param name="hashedName">The hash of the name with the name service hash prefix.</param>
        /// <param name="nameClass">The account of the name class.</param>
        /// <param name="parentName">The public key of the parent name.</param>
        /// <returns>The program derived address for the name if it could be found, otherwise null.</returns>
        public static PublicKey DeriveNameAccountKey(ReadOnlySpan<byte> hashedName, PublicKey nameClass = null, PublicKey parentName = null)
        {
            byte[] nameClassKey = new byte[32];
            byte[] parentNameKeyBytes = new byte[32];

            if (nameClass != null) nameClassKey = nameClass.KeyBytes;
            if (parentName != null) parentNameKeyBytes = parentName.KeyBytes;

            bool success = AddressExtensions.TryFindProgramAddress(
                new List<byte[]> {hashedName.ToArray(), nameClassKey, parentNameKeyBytes}, ProgramIdKey.KeyBytes, out byte[] nameAccountPublicKey, out _);
            return success ? new PublicKey(nameAccountPublicKey) : null;
        }


        /// <summary>
        /// Initialize a new transaction instruction to create a name record.
        /// </summary>
        /// <param name="nameKey">The public key of the name record.</param>
        /// <param name="nameOwner">The public key of the name owner.</param>
        /// <param name="payer">The public key of the account of the payer.</param>
        /// <param name="hashedName">The hash of the name with the hash prefix.</param>
        /// <param name="space">The space to assign to the account.</param>
        /// <param name="lamports">The amount of lamports the account needs to be rate exempt.</param>
        /// <param name="nameClass">The public key of the account of the name class.</param>
        /// <param name="parentName">The public key of the parent name.</param>
        /// <param name="parentNameOwner">The public key of the account of the parent name owner.</param>
        /// <returns>The transaction instruction.</returns>
        private static TransactionInstruction CreateNameRegistryInstruction(
            PublicKey nameKey, PublicKey nameOwner, PublicKey payer, ReadOnlySpan<byte> hashedName, ulong lamports, uint space,
            PublicKey nameClass = null, PublicKey parentNameOwner = null, PublicKey parentName = null)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.Writable(payer, true),
                AccountMeta.Writable(nameKey, true),
                AccountMeta.ReadOnly(nameOwner, false),
                nameClass != null
                    ? AccountMeta.ReadOnly(nameClass, false)
                    : AccountMeta.ReadOnly(new PublicKey(new byte[32]), false),
                parentName != null
                    ? AccountMeta.ReadOnly(parentName, false)
                    : AccountMeta.ReadOnly(new PublicKey(new byte[32]), false)
            };
            if (parentNameOwner != null) keys.Add(AccountMeta.ReadOnly(parentNameOwner, false));
            return new TransactionInstruction
            {
                Keys = keys,
                ProgramId = ProgramIdKey.KeyBytes,
                Data = EncodeCreateNameRegistryData(hashedName, lamports, space)
            };
        }
        
        /// <summary>
        /// Initialize a new transaction instruction to update the data of a name record.
        /// <remarks>
        /// If the name class was defined upon name record creation then the name class parameter must be passed.
        /// </remarks>
        /// </summary>
        /// <param name="nameKey">The public key of the name record.</param>
        /// <param name="offset">The offset at which to update the data.</param>
        /// <param name="data">The data to insert.</param>
        /// <param name="nameOwner">The public key of the name owner.</param>
        /// <param name="nameClass">The public key of the account of the name class.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction UpdateNameRegistry(
            PublicKey nameKey, uint offset, ReadOnlySpan<byte> data, PublicKey nameOwner = null, PublicKey nameClass = null)
        {
            List<AccountMeta> keys = new() {AccountMeta.Writable(nameKey, true)};
            
            if (nameOwner != null)
                keys.Add(AccountMeta.ReadOnly(nameOwner, false));
            
            if (nameClass != null)
                keys.Add(AccountMeta.ReadOnly(nameClass, false));

            return new TransactionInstruction
            {
                Keys = keys,
                ProgramId = ProgramIdKey.KeyBytes,
                Data = EncodeUpdateNameRegistryData(offset, data)
            };
        }
        
        /// <summary>
        /// Initialize a new transaction instruction to transfer ownership of a name record. 
        /// <remarks>
        /// If the name class was defined upon name record creation then the name class parameter must be passed.
        /// </remarks>
        /// </summary>
        /// <param name="nameKey">The public key of the name record.</param>
        /// <param name="newOwner">The public key of the new name owner.</param>
        /// <param name="nameOwner">The public key of the name owner.</param>
        /// <param name="nameClass">The public key of the account of the name class.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction TransferNameRegistry(
            PublicKey nameKey, PublicKey newOwner, PublicKey nameOwner, PublicKey nameClass = null)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(nameKey, true),
                AccountMeta.ReadOnly(nameOwner, false),
            };

            if (nameClass != null)
                keys.Add(AccountMeta.ReadOnly(nameClass, false));

            return new TransactionInstruction
            {
                Keys = keys, ProgramId = ProgramIdKey.KeyBytes, Data = EncodeTransferNameRegistryData(newOwner)
            };
        }
        
        /// <summary>
        /// Initialize a new transaction instruction to delete a name record.
        /// </summary>
        /// <param name="nameKey">The public key of the name record.</param>
        /// <param name="nameOwner">The public key of the name owner.</param>
        /// <param name="refundPublicKey">The public key of the refund account.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction DeleteNameRegistry(
            PublicKey nameKey, PublicKey nameOwner, PublicKey refundPublicKey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(nameKey, true),
                AccountMeta.ReadOnly(nameOwner, false),
                AccountMeta.Writable(refundPublicKey, true)
            };

            return new TransactionInstruction
            {
                Keys = keys, ProgramId = ProgramIdKey.KeyBytes, Data = EncodeDeleteNameRegistryData()
            };
        }

        /// <summary>
        /// Encode the instruction data to be used with the <see cref="NameServiceInstructions.Create"/> instruction.
        /// </summary>
        /// <param name="hashedName">The hashed name for the record.</param>
        /// <param name="lamports">The number of lamports for rent exemption.</param>
        /// <param name="space">The space for the account.</param>
        /// <returns>The transaction instruction data.</returns>
        private static byte[] EncodeCreateNameRegistryData(ReadOnlySpan<byte> hashedName, ulong lamports, uint space)
        {
            byte[] methodBuffer = new byte[49];

            methodBuffer.WriteU8((byte)NameServiceInstructions.Create, 0);
            methodBuffer.WriteU32((uint) hashedName.Length, 1);
            methodBuffer.WriteSpan(hashedName, 5);
            methodBuffer.WriteU64(lamports, 37);
            methodBuffer.WriteU32(space, 45);

            return methodBuffer;
        }
        
        /// <summary>
        /// Encode the instruction data to be used with the <see cref="NameServiceInstructions.Update"/> instruction.
        /// </summary>
        /// <param name="offset">The offset at which to update the data.</param>
        /// <param name="data">The data to insert.</param>
        /// <returns>The transaction instruction data.</returns>
        private static byte[] EncodeUpdateNameRegistryData(uint offset, ReadOnlySpan<byte> data)
        {
            byte[] methodBuffer = new byte[data.Length + 5];
            
            methodBuffer.WriteU8((byte)NameServiceInstructions.Update, 0);
            methodBuffer.WriteU32(offset, 1);
            methodBuffer.WriteSpan(data, 5);

            return methodBuffer;
        }
        
        /// <summary>
        /// Encode the instruction data to be used with the <see cref="NameServiceInstructions.Transfer"/> instruction.
        /// </summary>
        /// <param name="newOwner">The public key of the account to transfer ownership to.</param>
        /// <returns>The transaction instruction data.</returns>
        private static byte[] EncodeTransferNameRegistryData(PublicKey newOwner)
        {
            byte[] methodBuffer = new byte[33];
            
            methodBuffer.WriteU8((byte)NameServiceInstructions.Transfer, 0);
            methodBuffer.WritePubKey(newOwner, 1);
            
            return methodBuffer;
        }        
        
        /// <summary>
        /// Encode the instruction data to be used with the <see cref="NameServiceInstructions.Delete"/> instruction.
        /// </summary>
        /// <returns>The transaction instruction data.</returns>
        private static byte[] EncodeDeleteNameRegistryData()
        {
            byte[] methodBuffer = new byte[1];
            
            methodBuffer.WriteU8((byte)NameServiceInstructions.Delete, 0);
            
            return methodBuffer;
        }
    }
}