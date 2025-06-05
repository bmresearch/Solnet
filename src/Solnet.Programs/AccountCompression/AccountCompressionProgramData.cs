using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Solnet.Programs.Models.Stake.State;

namespace Solnet.Programs.AccountCompression
{
    /// <summary>
    /// Represents the instruction data for the <see cref="AccountCompressionProgram"/>.
    /// </summary>
    internal static class AccountCompressionProgramData
    {
        /// <summary>
        /// The offset for the instruction discriminator in the instruction data.
        /// </summary>
        internal const int MethodOffset = 0;
        /// <summary>
        /// Encode the Append instruction data
        /// </summary>
        /// <param name="leaf">32-byte leaf data</param>
        /// <returns>Encoded byte array for the instruction data</returns>
        public static byte[] EncodeAppendData(byte[] leaf)
        {
            if (leaf == null || leaf.Length != 32)
                throw new ArgumentException("Leaf must be 32 bytes");

            byte[] data = new byte[4 + 32]; // 4 bytes discriminator + 32 bytes leaf
            int offset = 0;

            // Write the 4-byte discriminator
            BitConverter.GetBytes((uint)AccountCompressionProgramInstructions.Values.Append).CopyTo(data, MethodOffset);
            offset += 4;

            // Write the 32-byte leaf data
            Buffer.BlockCopy(leaf, 0, data, offset, 32);

            return data;
        }
        /// <summary>
        /// Encodes the CloseEmptyTree instruction data.
        /// </summary>
        /// <returns></returns>
        internal static byte[] EncodeCloseEmptyTreeData()
        {
            // Only the discriminator (first 4 bytes) is needed
            var data = new byte[4];

            // You should replace this with the actual discriminator used by your Anchor program
            // For example: discriminator = Hash("global:close_empty_tree").Take(8) or a known constant
            uint discriminator = (uint)AccountCompressionProgramInstructions.Values.CloseEmptyTree;

            data.WriteU32(discriminator, 0);
            return data;
        }
        /// <summary>
        /// Encodes the InitEmptyMerkleTree instruction data.
        /// </summary>
        /// <param name="maxDepth"></param>
        /// <param name="maxBufferSize"></param>
        /// <returns></returns>
        internal static byte[] EncodeInitEmptyMerkleTreeData(byte maxDepth, byte maxBufferSize)
        {
            // Total size = 4 (discriminator) + 1 (maxDepth) + 1 (maxBufferSize) = 6 bytes
            byte[] data = new byte[6];
            int offset = 0;

            // 1. Instruction Discriminator (4 bytes)
            BitConverter.GetBytes((uint)AccountCompressionProgramInstructions.Values.InitEmptyMerkleTree)
                .CopyTo(data, MethodOffset);
            offset += 4;

            // 2. Max Depth (1 byte)
            data[offset++] = maxDepth;

            // 3. Max Buffer Size (1 byte)
            data[offset] = maxBufferSize;

            return data;
        }
        /// <summary>
        /// Encodes the ReplaceLeaf instruction data.
        /// </summary>
        /// <param name="newLeaf"></param>
        /// <param name="previousLeaf"></param>
        /// <param name="root"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        internal static byte[] EncodeReplaceLeafData(
            byte[] newLeaf,
            byte[] previousLeaf,
            byte[] root,
            uint index
        )
        {
            // Validate inputs
            if (newLeaf == null || newLeaf.Length != 32) throw new ArgumentException("newLeaf must be 32 bytes");
            if (previousLeaf == null || previousLeaf.Length != 32) throw new ArgumentException("previousLeaf must be 32 bytes");
            if (root == null || root.Length != 32) throw new ArgumentException("root must be 32 bytes");

            // Total size: 4 (discriminator) + 32 (root) + 32 (previousLeaf) + 32 (newLeaf) + 4 (index) = 104 bytes
            byte[] data = new byte[104];
            int offset = 0;

            // 1. Instruction Discriminator (4 bytes)
            BitConverter.GetBytes((uint)AccountCompressionProgramInstructions.Values.ReplaceLeaf)
                .CopyTo(data, MethodOffset);
            offset += 4;

            // 2. Root [32 bytes]
            Buffer.BlockCopy(root, 0, data, offset, 32);
            offset += 32;

            // 3. Previous Leaf [32 bytes]
            Buffer.BlockCopy(previousLeaf, 0, data, offset, 32);
            offset += 32;

            // 4. New Leaf [32 bytes]
            Buffer.BlockCopy(newLeaf, 0, data, offset, 32);
            offset += 32;

            // 5. Index [4 bytes]
            BitConverter.GetBytes(index).CopyTo(data, offset);

            return data;
        }

        /// <summary>
        /// Encodes the InsertOrAppend instruction data.
        /// </summary>
        /// <param name="Leaf"></param>
        /// <param name="root"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        internal static byte[] EncodeInsertOrAppendData(
            byte[] Leaf,
            byte[] root,
            uint index
        )
        {
            // Validate inputs
            if (root == null || root.Length != 32) throw new ArgumentException("root must be 32 bytes");

            // Total size: 4 (discriminator) + 32 (root) + 32 (Leaf) + 4 (index) = 104 bytes
            byte[] data = new byte[72];
            int offset = 0;

            // 1. Instruction Discriminator (4 bytes)
            BitConverter.GetBytes((uint)AccountCompressionProgramInstructions.Values.InsertOrAppend)
                .CopyTo(data, MethodOffset);
            offset += 4;

            // 2. Root [32 bytes]
            Buffer.BlockCopy(root, 0, data, offset, 32);
            offset += 32;

            // 3. Leaf [32 bytes]
            Buffer.BlockCopy(Leaf, 0, data, offset, 32);
            offset += 32;

            // 4. Index [4 bytes]
            BitConverter.GetBytes(index).CopyTo(data, offset);

            return data;
        }

        /// <summary>
        /// Encodes the TransferAuthority instruction data.
        /// </summary>
        /// <param name="newAuthority"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static byte[] EncodeTransferAuthorityData(PublicKey newAuthority)
        {
            if (newAuthority is null) throw new ArgumentNullException(nameof(newAuthority));

            byte[] data = new byte[36];

            // 1. Add instruction discriminator
            data.WriteU32((uint)AccountCompressionProgramInstructions.Values.TransferAuthority,MethodOffset);

            // 2. Add new authority public key (32 bytes)
            data.WritePubKey(newAuthority, 4);

            return data;
        }
        /// <summary>
        /// Encodes the VerifyLeaf instruction data.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="leaf"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] EncodeVerifyLeafData(byte[] root, byte[] leaf, uint index)
        {
            if (root == null || root.Length != 32) throw new ArgumentException("Root must be 32 bytes", nameof(root));
            if (leaf == null || leaf.Length != 32) throw new ArgumentException("Leaf must be 32 bytes", nameof(leaf));

            // Total size: 4 (discriminator) + 32 (root) + 32 (leaf) + 4 (index) = 72 bytes
            byte[] data = new byte[72];
            int offset = 0;

            // 1. Instruction Discriminator (4 bytes)
            BitConverter.GetBytes((uint)AccountCompressionProgramInstructions.Values.VerifyLeaf).CopyTo(data, MethodOffset);
            offset += 4;

            // 2. Root (32 bytes)
            Buffer.BlockCopy(root, 0, data, offset, 32);
            offset += 32;

            // 3. Leaf (32 bytes)
            Buffer.BlockCopy(leaf, 0, data, offset, 32);
            offset += 32;

            // 4. Index (4 bytes)
            BitConverter.GetBytes(index).CopyTo(data, offset);

            return data;
        }
        /// <summary>
        /// Decodes the AppendLeaf instruction data.
        /// </summary>
        /// <param name="decodedInstruction"></param>
        /// <param name="data"></param>
        /// <param name="keys"></param>
        /// <param name="keyIndices"></param>
        internal static void DecodeAppendLeafData(
            DecodedInstruction decodedInstruction,
            ReadOnlySpan<byte> data,
            IList<PublicKey> keys,
            byte[] keyIndices)
        {
            // Decode accounts
            decodedInstruction.Values.Add("authority", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("merkle_tree", keys[keyIndices[1]]);

            // Instruction data offsets:
            // 0..4   = discriminator
            // 4..36  = leaf (32 bytes)
            var leafBytes = data.GetBytes(4, 32);
            decodedInstruction.Values.Add("leaf", leafBytes);
        }
        /// <summary>
        /// Decodes the CloseEmptyTree instruction data.
        /// </summary>
        /// <param name="decodedInstruction"></param>
        /// <param name="data"></param>
        /// <param name="keys"></param>
        /// <param name="keyIndices"></param>
        /// <exception cref="ArgumentException"></exception>
        internal static void DecodeCloseEmptyTreeData(
            DecodedInstruction decodedInstruction,
            ReadOnlySpan<byte> data,
            IList<PublicKey> keys,
            byte[] keyIndices)
        {
            if (data.Length < 4) throw new ArgumentException("Instruction data too short.");

            uint discriminator = BitConverter.ToUInt32(data.Slice(0, 4));

            decodedInstruction.Values ??= new Dictionary<string, object>();
            decodedInstruction.Values.Add("instruction", discriminator);

            // Match accounts: [merkle_tree, authority, recipient]
            if (keyIndices.Length < 3) throw new ArgumentException("Insufficient account keys for CloseEmptyTree");

            decodedInstruction.Values.Add("merkle_tree", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("authority", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("recipient", keys[keyIndices[2]]);
        }
        /// <summary>
        /// Decodes the InitEmptyMerkleTree instruction data.
        /// </summary>
        /// <param name="decodedInstruction"></param>
        /// <param name="data"></param>
        /// <param name="keys"></param>
        /// <param name="keyIndices"></param>
        internal static void DecodeInitEmptyMerkleTreeData(
            DecodedInstruction decodedInstruction,
            ReadOnlySpan<byte> data,
            IList<PublicKey> keys,
            byte[] keyIndices)
        {
            // Accounts used
            decodedInstruction.Values.Add("merkle_tree", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("authority", keys[keyIndices[1]]);

            // Data: [4 bytes discriminator][1 byte maxDepth][1 byte maxBufferSize]
            decodedInstruction.Values.Add("max_depth", data[4]);
            decodedInstruction.Values.Add("max_buffer_size", data[5]);
        }
        /// <summary>
        /// Decodes the ReplaceLeaf instruction data.
        /// </summary>
        /// <param name="decodedInstruction"></param>
        /// <param name="data"></param>
        /// <param name="keys"></param>
        /// <param name="keyIndices"></param>
        internal static void DecodeReplaceLeafData(
            DecodedInstruction decodedInstruction,
            ReadOnlySpan<byte> data,
            IList<PublicKey> keys,
            byte[] keyIndices)
        {
            // Decode accounts

            decodedInstruction.Values.Add("merkle_tree", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("authority", keys[keyIndices[1]]);

            // 0..4    = discriminator
            // 4..36   = root
            // 36..68  = previousLeaf
            // 68..100 = newLeaf
            // 100..104 = index (uint32)

            decodedInstruction.Values.Add("root", data.GetBytes(4, 32));
            decodedInstruction.Values.Add("previous_leaf", data.GetBytes(36, 32));
            decodedInstruction.Values.Add("new_leaf", data.GetBytes(68, 32));
            decodedInstruction.Values.Add("index", data.GetU32(100));
        }
        /// <summary>
        /// Decodes the InsertOrAppend instruction data.
        /// </summary>
        /// <param name="decodedInstruction"></param>
        /// <param name="data"></param>
        /// <param name="keys"></param>
        /// <param name="keyIndices"></param>
        internal static void DecodeInsertOrAppendData(
            DecodedInstruction decodedInstruction,
            ReadOnlySpan<byte> data,
            IList<PublicKey> keys,
            byte[] keyIndices)
        {
            // Decode accounts

            decodedInstruction.Values.Add("merkle_tree", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("authority", keys[keyIndices[1]]);

            // 0..4    = discriminator
            // 4..36   = root
            // 36..68  = previousLeaf
            // 68..100 = newLeaf
            // 100..104 = index (uint32)

            decodedInstruction.Values.Add("root", data.GetBytes(4,32));
            decodedInstruction.Values.Add("leaf", data.GetBytes(36,32));
            decodedInstruction.Values.Add("index", data.GetU32(68));
        }
        /// <summary>
        /// Decodes the TransferAuthority instruction data.
        /// </summary>
        /// <param name="decodedInstruction"></param>
        /// <param name="data"></param>
        /// <param name="keys"></param>
        /// <param name="keyIndices"></param>
        internal static void DecodeTransferAuthorityInstruction(
            DecodedInstruction decodedInstruction,
            ReadOnlySpan<byte> data,
            IList<PublicKey> keys,
            byte[] keyIndices)
        {
            // Decode accounts

            decodedInstruction.Values.Add("merkle_tree", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("authority", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("new_authority", keys[keyIndices[2]]);

            // Instruction data offsets:
            // 0..4   = discriminator
            // 4..36  = newAuthority public key
            var newAuthority = data.GetPubKey(4);
            decodedInstruction.Values.Add("new_authority", newAuthority);
        }
        /// <summary>
        /// Decodes the VerifyLeaf instruction data.
        /// </summary>
        /// <param name="decodedInstruction"></param>
        /// <param name="data"></param>
        /// <param name="keys"></param>
        /// <param name="keyIndices"></param>
        /// <exception cref="ArgumentException"></exception>
        internal static void DecodeVerifyLeafData(
            DecodedInstruction decodedInstruction,
            ReadOnlySpan<byte> data,
            IList<PublicKey> keys,
            byte[] keyIndices)
        {
            // This instruction typically has only one key: merkleTree
            decodedInstruction.Values.Add("merkle_tree", keys[keyIndices[0]]);

            // Decode data
            if (data.Length < 72) throw new ArgumentException("Invalid data length for VerifyLeaf");

            var root = data.GetBytes(4,32);
            var leaf = data.GetBytes(36,32);
            var index = data.GetU32(68);

            decodedInstruction.Values.Add("root", root);
            decodedInstruction.Values.Add("leaf", leaf);
            decodedInstruction.Values.Add("index", index);
        }
    }


}
