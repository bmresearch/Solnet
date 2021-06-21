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
        /// <param name="nameClass">The account of the name class.</param>
        /// <param name="parentName">The public key of the parent name.</param>
        /// <param name="parentNameOwner">The account of the parent name owner.</param>
        /// <param name="space">The space to assign to the account.</param>
        /// <param name="lamports">The amount of lamports the account needs to be rate exempt.</param>
        /// <returns>The transaction instruction.</returns>
        /// <exception cref="Exception">Thrown when it was not possible to derive a program address for the account.</exception>
        public static TransactionInstruction CreateNameRegistry(
            PublicKey name, Account payer, PublicKey nameOwner, ulong lamports, int space, Account nameClass = null, 
            Account parentNameOwner = null, PublicKey parentName = null)
        {
            byte[] hashedName = ComputeHashedName(name.Key);
            Console.WriteLine("Hashed Name: " + Convert.ToHexString(hashedName).ToLowerInvariant());
            PublicKey nameAccountKey = DeriveNameAccountKey(hashedName, nameClass?.PublicKey, parentName);
            Console.WriteLine("Name Account Key: " + nameAccountKey.Key);
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
            Console.WriteLine(prefixedName);
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
        /// <returns>The program derived address for the name.</returns>
        public static PublicKey DeriveNameAccountKey(byte[] hashedName, PublicKey nameClass = null, PublicKey parentName = null)
        {
            byte[] nameClassKey = new byte[32];
            byte[] parentNameKeyBytes = new byte[32];

            if (nameClass != null) nameClassKey = nameClass.KeyBytes;
            if (parentName != null) parentNameKeyBytes = parentName.KeyBytes;

            try
            {
                (byte[] nameAccountPublicKey, _) = AddressExtensions.FindProgramAddress(
                    new List<byte[]> {hashedName, nameClassKey, parentNameKeyBytes}, ProgramIdKey.KeyBytes);
                return new PublicKey(nameAccountPublicKey);
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// Initialize a new transaction instruction to create a name record.
        /// </summary>
        /// <param name="nameKey">The public key of the name record.</param>
        /// <param name="nameOwner">The public key of the name owner.</param>
        /// <param name="payer">The account of the payer.</param>
        /// <param name="hashedName">The hash of the name with the hash prefix.</param>
        /// <param name="space">The space to assign to the account.</param>
        /// <param name="lamports">The amount of lamports the account needs to be rate exempt.</param>
        /// <param name="nameClass">The account of the name class.</param>
        /// <param name="parentName">The public key of the parent name.</param>
        /// <param name="parentNameOwner">The account of the parent name owner.</param>
        /// <returns>The transaction instruction.</returns>
        private static TransactionInstruction CreateNameRegistryInstruction(
            PublicKey nameKey, PublicKey nameOwner, Account payer, byte[] hashedName, ulong lamports, int space,
            Account nameClass = null, Account parentNameOwner = null, PublicKey parentName = null)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(SystemProgram.ProgramIdKey, false),
                new AccountMeta(payer, true),
                new AccountMeta(nameKey, true),
                new AccountMeta(nameOwner, false),
                nameClass != null
                    ? new AccountMeta(nameClass, false)
                    : new AccountMeta(new PublicKey(new byte[32]), false),
                nameClass != null
                    ? new AccountMeta(parentName, false)
                    : new AccountMeta(new PublicKey(new byte[32]), false)
            };
            if (parentNameOwner != null) keys.Add(new AccountMeta(parentNameOwner, false));
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
        /// <param name="nameClass">The account of the name class.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction UpdateNameRegistry(
            PublicKey nameKey, int offset, byte[] data, Account nameOwner = null, Account nameClass = null)
        {
            List<AccountMeta> keys = new() {new AccountMeta(nameKey, true)};
            
            if (nameOwner != null)
                keys.Add(new AccountMeta(nameOwner, false));
            
            if (nameClass != null)
                keys.Add(new AccountMeta(nameClass, false));

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
        /// <param name="nameClass">The account of the name class.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction TransferNameRegistry(
            PublicKey nameKey, PublicKey newOwner, Account nameOwner, Account nameClass = null)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(nameKey, true),
                new AccountMeta(nameOwner, false),
            };

            if (nameClass != null)
                keys.Add(new AccountMeta(nameClass, false));

            return new TransactionInstruction
            {
                Keys = keys, ProgramId = ProgramIdKey.KeyBytes, Data = EncodeTransferNameRegistryData(newOwner.KeyBytes)
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
            PublicKey nameKey, Account nameOwner, PublicKey refundPublicKey)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(nameKey, true),
                new AccountMeta(nameOwner, false),
                new AccountMeta(refundPublicKey, true)
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
        private static byte[] EncodeCreateNameRegistryData(byte[] hashedName, ulong lamports, int space)
        {
            byte[] methodBuffer = new byte[49];

            Utils.Uint32ToByteArrayLe(hashedName.Length, methodBuffer, 1);
            Array.Copy(hashedName, 0, methodBuffer, 5, hashedName.Length);
            Utils.Int64ToByteArrayLe(lamports, methodBuffer, 37);
            Utils.Uint32ToByteArrayLe(space, methodBuffer, 45);

            return methodBuffer;
        }
        
        /// <summary>
        /// Encode the instruction data to be used with the <see cref="NameServiceInstructions.Update"/> instruction.
        /// </summary>
        /// <param name="offset">The offset at which to update the data.</param>
        /// <param name="data">The data to insert.</param>
        /// <returns>The transaction instruction data.</returns>
        private static byte[] EncodeUpdateNameRegistryData(int offset, byte[] data)
        {
            byte[] methodBuffer = new byte[data.Length + 5];
            
            methodBuffer[0] = (byte)NameServiceInstructions.Transfer;
            Utils.Uint32ToByteArrayLe(offset, methodBuffer, 1);
            Array.Copy(data, 0, methodBuffer, 5, data.Length);

            return methodBuffer;
        }
        
        /// <summary>
        /// Encode the instruction data to be used with the <see cref="NameServiceInstructions.Transfer"/> instruction.
        /// </summary>
        /// <param name="publicKey">The public key of the account to transfer ownership to.</param>
        /// <returns>The transaction instruction data.</returns>
        private static byte[] EncodeTransferNameRegistryData(byte[] publicKey)
        {
            byte[] methodBuffer = new byte[33];
            methodBuffer[0] = (byte)NameServiceInstructions.Transfer;
            Array.Copy(publicKey, 0, methodBuffer, 2, publicKey.Length);
            return methodBuffer;
        }        
        
        /// <summary>
        /// Encode the instruction data to be used with the <see cref="NameServiceInstructions.Delete"/> instruction.
        /// </summary>
        /// <returns>The transaction instruction data.</returns>
        private static byte[] EncodeDeleteNameRegistryData()
        {
            byte[] methodBuffer = new byte[1];
            methodBuffer[0] = (byte)NameServiceInstructions.Delete;
            return methodBuffer;
        }
    }
}