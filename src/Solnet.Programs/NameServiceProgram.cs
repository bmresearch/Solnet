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
        /// The program's name.
        /// </summary>
        private const string ProgramName = "Name Service Program";

        /// <summary>
        /// The offset at which the value which defines the instruction method begins.
        /// </summary>
        private const int MethodOffset = 0;

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
                new List<byte[]> { hashedName.ToArray(), nameClassKey, parentNameKeyBytes }, ProgramIdKey.KeyBytes, out byte[] nameAccountPublicKey, out _);
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
                AccountMeta.Writable(nameKey, false),
                AccountMeta.ReadOnly(nameOwner, false),
                nameClass != null
                    ? AccountMeta.ReadOnly(nameClass, true)
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
            List<AccountMeta> keys = new() { AccountMeta.Writable(nameKey, false) };

            if (nameOwner != null)
                keys.Add(AccountMeta.ReadOnly(nameOwner, true));

            if (nameClass != null)
                keys.Add(AccountMeta.ReadOnly(nameClass, true));

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
                AccountMeta.Writable(nameKey, false),
                AccountMeta.ReadOnly(nameOwner, true),
            };

            if (nameClass != null)
                keys.Add(AccountMeta.ReadOnly(nameClass, true));

            return new TransactionInstruction
            {
                Keys = keys,
                ProgramId = ProgramIdKey.KeyBytes,
                Data = EncodeTransferNameRegistryData(newOwner)
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
                AccountMeta.Writable(nameKey, false),
                AccountMeta.ReadOnly(nameOwner, true),
                AccountMeta.Writable(refundPublicKey, false)
            };

            return new TransactionInstruction
            {
                Keys = keys,
                ProgramId = ProgramIdKey.KeyBytes,
                Data = EncodeDeleteNameRegistryData()
            };
        }

        /// <summary>
        /// Encode the instruction data to be used with the <see cref="NameServiceInstructions.Values.Create"/> instruction.
        /// </summary>
        /// <param name="hashedName">The hashed name for the record.</param>
        /// <param name="lamports">The number of lamports for rent exemption.</param>
        /// <param name="space">The space for the account.</param>
        /// <returns>The transaction instruction data.</returns>
        private static byte[] EncodeCreateNameRegistryData(ReadOnlySpan<byte> hashedName, ulong lamports, uint space)
        {
            byte[] methodBuffer = new byte[17 + hashedName.Length];

            methodBuffer.WriteU8((byte)NameServiceInstructions.Values.Create, MethodOffset);
            methodBuffer.WriteU32((uint)hashedName.Length, 1);
            methodBuffer.WriteSpan(hashedName, 5);
            methodBuffer.WriteU64(lamports, 5 + hashedName.Length);
            methodBuffer.WriteU32(space, 13 + hashedName.Length);

            return methodBuffer;
        }

        /// <summary>
        /// Encode the instruction data to be used with the <see cref="NameServiceInstructions.Values.Update"/> instruction.
        /// </summary>
        /// <param name="offset">The offset at which to update the data.</param>
        /// <param name="data">The data to insert.</param>
        /// <returns>The transaction instruction data.</returns>
        private static byte[] EncodeUpdateNameRegistryData(uint offset, ReadOnlySpan<byte> data)
        {
            byte[] methodBuffer = new byte[data.Length + 5];

            methodBuffer.WriteU8((byte)NameServiceInstructions.Values.Update, MethodOffset);
            methodBuffer.WriteU32(offset, 1);
            methodBuffer.WriteSpan(data, 5);

            return methodBuffer;
        }

        /// <summary>
        /// Encode the instruction data to be used with the <see cref="NameServiceInstructions.Values.Transfer"/> instruction.
        /// </summary>
        /// <param name="newOwner">The public key of the account to transfer ownership to.</param>
        /// <returns>The transaction instruction data.</returns>
        private static byte[] EncodeTransferNameRegistryData(PublicKey newOwner)
        {
            byte[] methodBuffer = new byte[33];

            methodBuffer.WriteU8((byte)NameServiceInstructions.Values.Transfer, MethodOffset);
            methodBuffer.WritePubKey(newOwner, 1);

            return methodBuffer;
        }

        /// <summary>
        /// Encode the instruction data to be used with the <see cref="NameServiceInstructions.Values.Delete"/> instruction.
        /// </summary>
        /// <returns>The transaction instruction data.</returns>
        private static byte[] EncodeDeleteNameRegistryData()
        {
            byte[] methodBuffer = new byte[1];

            methodBuffer.WriteU8((byte)NameServiceInstructions.Values.Delete, MethodOffset);

            return methodBuffer;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="NameServiceInstructions.Values.Create"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        private static void DecodeCreateNameRegistry(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Payer", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Name Account", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Name Owner", keys[keyIndices[3]]);

            if (keyIndices.Length >= 5)
                decodedInstruction.Values.Add("Name Class", keys[keyIndices[4]]);

            if (keyIndices.Length >= 6)
                decodedInstruction.Values.Add("Parent Name", keys[keyIndices[5]]);

            if (keyIndices.Length >= 7)
                decodedInstruction.Values.Add("Parent Name Owner", keys[keyIndices[6]]);

            uint nameLength = data.GetU32(1);
            decodedInstruction.Values.Add("Hashed Name Length", nameLength);
            decodedInstruction.Values.Add("Hashed Name", data.GetSpan(5, (int)nameLength).ToArray());
            decodedInstruction.Values.Add("Lamports", data.GetU64(5 + (int)nameLength));
            decodedInstruction.Values.Add("Space", data.GetU32(13 + (int)nameLength));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="NameServiceInstructions.Values.Update"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        private static void DecodeUpdateNameRegistry(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Name Account", keys[keyIndices[0]]);

            if (keyIndices.Length == 2)
                decodedInstruction.Values.Add("Name Owner", keys[keyIndices[1]]);

            if (keyIndices.Length == 3)
                decodedInstruction.Values.Add("Name Class", keys[keyIndices[2]]);

            decodedInstruction.Values.Add("Offset", data.GetU32(1));
            decodedInstruction.Values.Add("Data", data[5..].ToArray());
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="NameServiceInstructions.Values.Transfer"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        private static void DecodeTransferNameRegistry(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Name Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Name Owner", keys[keyIndices[1]]);

            if (keyIndices.Length == 3)
                decodedInstruction.Values.Add("Name Class", keys[keyIndices[2]]);

            decodedInstruction.Values.Add("New Owner", data.GetPubKey(1));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="NameServiceInstructions.Values.Delete"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        private static void DecodeDeleteNameRegistry(DecodedInstruction decodedInstruction, IList<PublicKey> keys,
            byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Name Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Name Owner", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Refund Account", keys[keyIndices[2]]);
        }

        /// <summary>
        /// Decodes an instruction created by the System Program.
        /// </summary>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        /// <returns>A decoded instruction.</returns>
        public static DecodedInstruction Decode(ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            byte instruction = data.GetU8(MethodOffset);
            NameServiceInstructions.Values instructionValue =
                (NameServiceInstructions.Values)Enum.Parse(typeof(NameServiceInstructions.Values), instruction.ToString());

            DecodedInstruction decodedInstruction = new()
            {
                PublicKey = ProgramIdKey,
                InstructionName = NameServiceInstructions.Names[instructionValue],
                ProgramName = ProgramName,
                Values = new Dictionary<string, object>(),
                InnerInstructions = new List<DecodedInstruction>()
            };

            switch (instructionValue)
            {
                case NameServiceInstructions.Values.Create:
                    DecodeCreateNameRegistry(decodedInstruction, data, keys, keyIndices);
                    break;
                case NameServiceInstructions.Values.Update:
                    DecodeUpdateNameRegistry(decodedInstruction, data, keys, keyIndices);
                    break;
                case NameServiceInstructions.Values.Transfer:
                    DecodeTransferNameRegistry(decodedInstruction, data, keys, keyIndices);
                    break;
                case NameServiceInstructions.Values.Delete:
                    DecodeDeleteNameRegistry(decodedInstruction, keys, keyIndices);
                    break;
            }

            return decodedInstruction;
        }
    }
}