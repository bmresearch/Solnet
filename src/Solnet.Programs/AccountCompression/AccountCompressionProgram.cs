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
    /// <summary>
    /// Implements the Stake Program methods.
    /// <remarks>
    /// For more information see:
    /// https://docs.rs/spl-account-compression/latest/spl_account_compression/instruction/index.html
    /// https://github.com/solana-program/account-compression/blob/ac-mainnet-tag/account-compression/sdk/src/instructions/index.ts
    /// </remarks>
    /// </summary>
    public static class AccountCompressionProgram
    {
        /// <summary>
        /// The public key of the Stake Program.
        /// </summary>
        public static PublicKey ProgramIdKey = new ("compr6CUsB5m2jS4Y3831ztGSTnDpnKJTKS95d64XVq");

        /// <summary>
        /// The public key of the account compression program.
        /// </summary>
        public static readonly PublicKey ConfigKey = new("ComprConfig11111111111111111111111111111111");
        /// <summary>
        /// The program's name.
        /// </summary>
        private const string ProgramName = "Account Compression Program";



        /// <summary>
        /// Creates an instruction to append a leaf to a Merkle tree.
        /// </summary>
        /// <param name="merkleTree"></param>
        /// <param name="authority"></param>
        /// <param name="leaf"></param>
        /// <returns></returns>
        public static TransactionInstruction Append(
            PublicKey merkleTree,
            PublicKey authority,
            byte[] leaf)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(authority, true),      // Authority (signer)                
                AccountMeta.Writable(merkleTree, false),  // Merkle tree account (writable)
            };

           
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = AccountCompressionProgramData.EncodeAppendData(leaf)
            };
        }
        /// <summary>
        /// /// Creates an instruction to close an empty Merkle tree and transfer its lamports to a recipient.
        /// </summary>
        /// <param name="merkleTree"></param>
        /// <param name="authority"></param>
        /// <param name="recipient"></param>
        /// <returns></returns>
        public static TransactionInstruction CloseEmptyTree(
            PublicKey merkleTree,
            PublicKey authority,
            PublicKey recipient
        )
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(merkleTree, false),  // Merkle tree account (writable)
                AccountMeta.Writable(authority, true),      // Authority (signer)          
                AccountMeta.ReadOnly(recipient, false)  // recipient
            };


            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = AccountCompressionProgramData.EncodeCloseEmptyTreeData()
            };
        }
        /// <summary>
        /// /// Creates an instruction to initialize an empty Merkle tree.
        /// </summary>
        /// <param name="merkleTree"></param>
        /// <param name="authority"></param>
        /// <param name="maxDepth"></param>
        /// <param name="maxBufferSize"></param>
        /// <returns></returns>
        public static TransactionInstruction InitEmptyMerkleTree(
            PublicKey merkleTree,
            PublicKey authority,
            byte maxDepth, byte maxBufferSize
        )
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(merkleTree, false),  // Merkle tree account (writable)
                AccountMeta.Writable(authority, true),      // Authority (signer)  
            };


            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = AccountCompressionProgramData.EncodeInitEmptyMerkleTreeData(maxDepth, maxBufferSize)
            };
        }
        /// <summary>
        /// /// Creates an instruction to replace a leaf in a Merkle tree.
        /// </summary>
        /// <param name="merkleTree"></param>
        /// <param name="authority"></param>
        /// <param name="newLeaf"></param>
        /// <param name="previousLeaf"></param>
        /// <param name="root"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static TransactionInstruction ReplaceLeaf(
            PublicKey merkleTree,
            PublicKey authority,
            byte[] newLeaf,
            byte[] previousLeaf,
            byte[] root,
            uint index
       )
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(merkleTree, false),  // Merkle tree account (writable)
                AccountMeta.Writable(authority, true),      // Authority (signer)  
            };


            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = AccountCompressionProgramData.EncodeReplaceLeafData(newLeaf, previousLeaf, root, index)
            };
        }
        /// <summary>
        /// /// Creates an instruction to insert or append a leaf to a Merkle tree.
        /// </summary>
        /// <param name="merkleTree"></param>
        /// <param name="authority"></param>
        /// <param name="Leaf"></param>
        /// <param name="root"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static TransactionInstruction InsertOrAppend(
            PublicKey merkleTree,
            PublicKey authority,
            byte[] Leaf,
            byte[] root,
            uint index
       )
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(merkleTree, false),  // Merkle tree account (writable)
                AccountMeta.Writable(authority, true),      // Authority (signer)  
            };


            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = AccountCompressionProgramData.EncodeInsertOrAppendData(Leaf, root, index)
            };
        }
        /// <summary>
        /// Creates an instruction to transfer authority of a Merkle tree.
        /// </summary>
        /// <param name="merkleTree"></param>
        /// <param name="authority"></param>
        /// <param name="newAuthority"></param>
        /// <returns></returns>
        public static TransactionInstruction TransferAuthority(
            PublicKey merkleTree,
            PublicKey authority,
            PublicKey newAuthority
       )
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(merkleTree, false),  // Merkle tree account (writable)
                AccountMeta.Writable(authority, true),      // Authority (signer)  
                AccountMeta.ReadOnly(newAuthority, false),      //new Authority  

            };


            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = AccountCompressionProgramData.EncodeTransferAuthorityData(newAuthority)
            };
        }
        /// <summary>
        /// Creates an instruction to verify a leaf in a Merkle tree.
        /// </summary>
        /// <param name="merkleTree"></param>
        /// <param name="authority"></param>
        /// <param name="root"></param>
        /// <param name="leaf"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static TransactionInstruction VerifyLeaf(
            PublicKey merkleTree,
            PublicKey authority,
            byte[] root, byte[] leaf, uint index
       )
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(merkleTree, false),  // Merkle tree account (writable)
                AccountMeta.Writable(authority, true),      // Authority (signer) 

            };


            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = AccountCompressionProgramData.EncodeVerifyLeafData(root, leaf, index)
            };
        }
        /// <summary>
        /// Decodes the instruction data for the <see cref="AccountCompressionProgram"/>.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="keys"></param>
        /// <param name="keyIndices"></param>
        /// <returns></returns>
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
