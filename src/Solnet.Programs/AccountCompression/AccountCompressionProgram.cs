using Solnet.Programs.Abstract;
using Solnet.Programs.AccountCompression;
using Solnet.Programs.Utilities;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using static Solnet.Programs.Models.Stake.State;


namespace Solnet.Programs
{
    public static class AccountCompressionProgram
    {
        public static PublicKey ProgramIdKey = new ("compr6CUsB5m2jS4Y3831ztGSTnDpnKJTKS95d64XVq");


        public static readonly PublicKey ConfigKey = new("ComprConfig11111111111111111111111111111111");
        /// <summary>
        /// The program's name.
        /// </summary>
        private const string ProgramName = "Account Compression Program";




        public static TransactionInstruction Append(
            PublicKey merkleTree,
            PublicKey authority,
            byte[] leaf)
        {
            List<AccountMeta> metas = new()
            {
                AccountMeta.Writable(authority, true),      // Authority (signer)                
                AccountMeta.Writable(merkleTree, false),  // Merkle tree account (writable)
            };

           
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = metas,
                Data = AccountCompressionProgramData.EncodeAppendData(leaf)
            };
        }
        public static TransactionInstruction CloseEmptyTree(
            PublicKey merkleTree,
            PublicKey authority,
            PublicKey recipient
        )
        {
            List<AccountMeta> metas = new()
            {
                AccountMeta.Writable(merkleTree, false),  // Merkle tree account (writable)
                AccountMeta.Writable(authority, true),      // Authority (signer)          
                AccountMeta.ReadOnly(recipient, false)  // recipient
            };


            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = metas,
                Data = AccountCompressionProgramData.EncodeCloseEmptyTreeData()
            };
        }
        public static TransactionInstruction InitEmptyMerkleTree(
            PublicKey merkleTree,
            PublicKey authority,
            byte maxDepth, byte maxBufferSize
        )
        {
            List<AccountMeta> metas = new()
            {
                AccountMeta.Writable(merkleTree, false),  // Merkle tree account (writable)
                AccountMeta.Writable(authority, true),      // Authority (signer)  
            };


            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = metas,
                Data = AccountCompressionProgramData.EncodeInitEmptyMerkleTreeData(maxDepth, maxBufferSize)
            };
        }

        public static TransactionInstruction ReplaceLeaf(
            PublicKey merkleTree,
            PublicKey authority,
            byte[] newLeaf,
            byte[] previousLeaf,
            byte[] root,
            uint index
       )
        {
            List<AccountMeta> metas = new()
            {
                AccountMeta.Writable(merkleTree, false),  // Merkle tree account (writable)
                AccountMeta.Writable(authority, true),      // Authority (signer)  
            };


            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = metas,
                Data = AccountCompressionProgramData.EncodeReplaceLeafData(newLeaf, previousLeaf, root, index)
            };
        }

        public static TransactionInstruction InsertOrAppend(
            PublicKey merkleTree,
            PublicKey authority,
            byte[] Leaf,
            byte[] root,
            uint index
       )
        {
            List<AccountMeta> metas = new()
            {
                AccountMeta.Writable(merkleTree, false),  // Merkle tree account (writable)
                AccountMeta.Writable(authority, true),      // Authority (signer)  
            };


            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = metas,
                Data = AccountCompressionProgramData.EncodeInsertOrAppendData(Leaf, root, index)
            };
        }

        public static TransactionInstruction TransferAuthority(
            PublicKey merkleTree,
            PublicKey authority,
            PublicKey newAuthority
       )
        {
            List<AccountMeta> metas = new()
            {
                AccountMeta.Writable(merkleTree, false),  // Merkle tree account (writable)
                AccountMeta.Writable(authority, true),      // Authority (signer)  
                AccountMeta.ReadOnly(newAuthority, false),      //new Authority  

            };


            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = metas,
                Data = AccountCompressionProgramData.EncodeTransferAuthorityData(newAuthority)
            };
        }

        public static TransactionInstruction VerifyLeaf(
            PublicKey merkleTree,
            PublicKey authority,
            byte[] root, byte[] leaf, uint index
       )
        {
            List<AccountMeta> metas = new()
            {
                AccountMeta.Writable(merkleTree, false),  // Merkle tree account (writable)
                AccountMeta.Writable(authority, true),      // Authority (signer) 

            };


            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = metas,
                Data = AccountCompressionProgramData.EncodeVerifyLeafData(root, leaf, index)
            };
        }

        public static DecodedInstruction Decode(ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            uint instruction = data.GetU32(AccountCompressionProgramData.MethodOffset);

            if (!Enum.IsDefined(typeof(NameServiceInstructions.Values), instruction))
            {
                return new()
                {
                    PublicKey = ProgramIdKey,
                    InstructionName = "Unknown Instruction",
                    ProgramName = ProgramName,
                    Values = new Dictionary<string, object>(),
                    InnerInstructions = new List<DecodedInstruction>()
                };
            }

            AccountCompressionProgramInstructions.Values instructionValue = (AccountCompressionProgramInstructions.Values)instruction;

            DecodedInstruction decodedInstruction = new()
            {
                PublicKey = ProgramIdKey,
                InstructionName = AccountCompressionProgramInstructions.Names[instructionValue],
                ProgramName = ProgramName,
                Values = new Dictionary<string, object>() { },
                InnerInstructions = new List<DecodedInstruction>()
            };

            switch (instructionValue)
            {
                case AccountCompressionProgramInstructions.Values.Append:
                    AccountCompressionProgramData.DecodeAppendLeafData(decodedInstruction, data, keys, keyIndices);
                    break;
                case AccountCompressionProgramInstructions.Values.InsertOrAppend:
                    AccountCompressionProgramData.DecodeInsertOrAppendData(decodedInstruction, data, keys, keyIndices);
                    break;
                case AccountCompressionProgramInstructions.Values.VerifyLeaf:
                    AccountCompressionProgramData.DecodeVerifyLeafData(decodedInstruction, data, keys, keyIndices);
                    break;
                case AccountCompressionProgramInstructions.Values.InitEmptyMerkleTree:
                    AccountCompressionProgramData.DecodeInitEmptyMerkleTreeData(decodedInstruction, data, keys, keyIndices);
                    break;
                case AccountCompressionProgramInstructions.Values.CloseEmptyTree:
                    AccountCompressionProgramData.DecodeCloseEmptyTreeData(decodedInstruction, data, keys, keyIndices);
                    break;
                case AccountCompressionProgramInstructions.Values.TransferAuthority:
                    AccountCompressionProgramData.DecodeTransferAuthorityInstruction(decodedInstruction, data, keys, keyIndices);
                    break;
                case AccountCompressionProgramInstructions.Values.ReplaceLeaf:
                    AccountCompressionProgramData.DecodeReplaceLeafData(decodedInstruction, data, keys, keyIndices);
                    break;
                
            }
            return decodedInstruction;
        }
    }
}
