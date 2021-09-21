using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Programs
{
    /// <summary>
    /// Implements the system program data encodings.
    /// </summary>
    internal static class SystemProgramData
    {
        /// <summary>
        /// The offset at which the value which defines the program method begins. 
        /// </summary>
        internal const int MethodOffset = 0;

        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.Values.CreateAccount"/> method.
        /// </summary>
        /// <param name="owner">The public key of the owner program account.</param>
        /// <param name="lamports">The number of lamports to fund the account.</param>
        /// <param name="space">The space to be allocated to the account.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeCreateAccountData(PublicKey owner, ulong lamports, ulong space)
        {
            byte[] data = new byte[52];

            data.WriteU32((uint)SystemProgramInstructions.Values.CreateAccount, MethodOffset);
            data.WriteU64(lamports, 4);
            data.WriteU64(space, 12);
            data.WritePubKey(owner, 20);

            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.Values.Assign"/> method.
        /// </summary>
        /// <param name="programId">The program id to set as the account owner.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeAssignData(PublicKey programId)
        {
            byte[] data = new byte[36];

            data.WriteU32((uint)SystemProgramInstructions.Values.Assign, MethodOffset);
            data.WritePubKey(programId, 4);

            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.Values.Transfer"/> method.
        /// </summary>
        /// <param name="lamports">The number of lamports to fund the account.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeTransferData(ulong lamports)
        {
            byte[] data = new byte[12];

            data.WriteU32((uint)SystemProgramInstructions.Values.Transfer, MethodOffset);
            data.WriteU64(lamports, 4);

            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.Values.CreateAccountWithSeed"/> method.
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
            byte[] encodedSeed = Serialization.EncodeRustString(seed);
            byte[] data = new byte[84 + encodedSeed.Length];

            data.WriteU32((uint)SystemProgramInstructions.Values.CreateAccountWithSeed, MethodOffset);
            data.WritePubKey(baseAccount, 4);
            data.WriteSpan(encodedSeed, 36);
            data.WriteU64(lamports, 36 + encodedSeed.Length);
            data.WriteU64(space, 44 + encodedSeed.Length);
            data.WritePubKey(owner, 52 + encodedSeed.Length);

            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.Values.AdvanceNonceAccount"/> method.
        /// </summary>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeAdvanceNonceAccountData()
        {
            byte[] data = new byte[4];

            data.WriteU32((uint)SystemProgramInstructions.Values.AdvanceNonceAccount, MethodOffset);

            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.Values.WithdrawNonceAccount"/> method.
        /// </summary>
        /// <param name="lamports"></param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeWithdrawNonceAccountData(ulong lamports)
        {
            byte[] data = new byte[12];

            data.WriteU32((uint)SystemProgramInstructions.Values.WithdrawNonceAccount, MethodOffset);
            data.WriteU64(lamports, 4);

            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.Values.InitializeNonceAccount"/> method.
        /// </summary>
        /// <param name="authorized">The public key of the entity authorized to execute nonce instructions on the account.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeInitializeNonceAccountData(PublicKey authorized)
        {
            byte[] data = new byte[36];

            data.WriteU32((uint)SystemProgramInstructions.Values.InitializeNonceAccount, MethodOffset);
            data.WritePubKey(authorized, 4);

            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.Values.AuthorizeNonceAccount"/> method.
        /// </summary>
        /// <param name="authorized">The public key of the entity authorized to execute nonce instructions on the account.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeAuthorizeNonceAccountData(PublicKey authorized)
        {
            byte[] data = new byte[36];

            data.WriteU32((uint)SystemProgramInstructions.Values.AuthorizeNonceAccount, MethodOffset);
            data.WritePubKey(authorized, 4);

            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.Values.Allocate"/> method.
        /// </summary>
        /// <param name="space">Number of bytes of memory to allocate.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeAllocateData(ulong space)
        {
            byte[] data = new byte[12];

            data.WriteU32((uint)SystemProgramInstructions.Values.Allocate, MethodOffset);
            data.WriteU64(space, 4);

            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.Values.AllocateWithSeed"/> method.
        /// </summary>
        /// <param name="baseAccount">The public key of the base account.</param>
        /// <param name="space">Number of bytes of memory to allocate.</param>
        /// <param name="owner">Owner to use to derive the funding account address.</param>
        /// <param name="seed">Seed to use to derive the funding account address.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeAllocateWithSeedData(
            PublicKey baseAccount, PublicKey owner, ulong space, string seed)
        {
            byte[] encodedSeed = Serialization.EncodeRustString(seed);
            byte[] data = new byte[76 + encodedSeed.Length];

            data.WriteU32((uint)SystemProgramInstructions.Values.AllocateWithSeed, MethodOffset);
            data.WritePubKey(baseAccount, 4);
            data.WriteSpan(encodedSeed, 36);
            data.WriteU64(space, 36 + encodedSeed.Length);
            data.WritePubKey(owner, 44 + encodedSeed.Length);

            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.Values.AssignWithSeed"/> method.
        /// </summary>
        /// <param name="baseAccount">The public key of the base account.</param>
        /// <param name="seed">Seed to use to derive the account address.</param>
        /// <param name="owner">The public key of the owner program account.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeAssignWithSeedData(
            PublicKey baseAccount, string seed, PublicKey owner)
        {
            byte[] encodedSeed = Serialization.EncodeRustString(seed);
            byte[] data = new byte[68 + encodedSeed.Length];

            data.WriteU32((uint)SystemProgramInstructions.Values.AssignWithSeed, MethodOffset);
            data.WritePubKey(baseAccount, 4);
            data.WriteSpan(encodedSeed, 36);
            data.WritePubKey(owner, 36 + encodedSeed.Length);

            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="SystemProgramInstructions.Values.TransferWithSeed"/> method.
        /// </summary>
        /// <param name="owner">Owner to use to derive the funding account address.</param>
        /// <param name="seed">Seed to use to derive the funding account address.</param>
        /// <param name="lamports">Amount of lamports to transfer.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeTransferWithSeedData(PublicKey owner, string seed, ulong lamports)
        {
            byte[] encodedSeed = Serialization.EncodeRustString(seed);
            byte[] data = new byte[44 + encodedSeed.Length];

            data.WriteU32((uint)SystemProgramInstructions.Values.TransferWithSeed, MethodOffset);
            data.WriteU64(lamports, 4);
            data.WriteSpan(encodedSeed, 12);
            data.WritePubKey(owner, 12 + encodedSeed.Length);

            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="SystemProgramInstructions.Values.CreateAccount"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeCreateAccountData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Owner Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("New Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Amount", data.GetU64(4));
            decodedInstruction.Values.Add("Space", data.GetU64(12));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="SystemProgramInstructions.Values.Assign"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeAssignData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Assign To", data.GetPubKey(4));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="SystemProgramInstructions.Values.Transfer"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeTransferData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("From Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("To Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Amount", data.GetU64(4));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="SystemProgramInstructions.Values.CreateAccountWithSeed"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeCreateAccountWithSeedData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("From Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("To Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Base Account", data.GetPubKey(4));
            (string createSeed, int createLength) = data.DecodeRustString(36);
            decodedInstruction.Values.Add("Seed", createSeed);
            decodedInstruction.Values.Add("Amount", data.GetU64(36 + createLength));
            decodedInstruction.Values.Add("Space", data.GetU64(44 + createLength));
            decodedInstruction.Values.Add("Owner", data.GetPubKey(52 + createLength));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="SystemProgramInstructions.Values.AdvanceNonceAccount"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeAdvanceNonceAccountData(DecodedInstruction decodedInstruction, IList<PublicKey> keys,
            byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Nonce Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Authority", keys[keyIndices[2]]);
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="SystemProgramInstructions.Values.WithdrawNonceAccount"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeWithdrawNonceAccountData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Nonce Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("To Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Authority", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Amount", data.GetU64(4));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="SystemProgramInstructions.Values.InitializeNonceAccount"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeInitializeNonceAccountData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Nonce Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Authority", data.GetPubKey(4));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="SystemProgramInstructions.Values.AuthorizeNonceAccount"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeAuthorizeNonceAccountData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Nonce Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Current Authority", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("New Authority", data.GetPubKey(4));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="SystemProgramInstructions.Values.Allocate"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeAllocateData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Space", data.GetU64(4));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="SystemProgramInstructions.Values.AllocateWithSeed"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeAllocateWithSeedData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Base Account", data.GetPubKey(4));
            (string allocateSeed, int allocateLength) = data.DecodeRustString(36);
            decodedInstruction.Values.Add("Seed", allocateSeed);
            decodedInstruction.Values.Add("Space", data.GetU64(36 + allocateLength));
            decodedInstruction.Values.Add("Owner", data.GetPubKey(44 + allocateLength));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="SystemProgramInstructions.Values.AssignWithSeed"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeAssignWithSeedData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Base Account", data.GetPubKey(4));
            (string assignSeed, int assignLength) = data.DecodeRustString(36);
            decodedInstruction.Values.Add("Seed", assignSeed);
            decodedInstruction.Values.Add("Owner", data.GetPubKey(36 + assignLength));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="SystemProgramInstructions.Values.TransferWithSeed"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeTransferWithSeedData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("From Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("From Base Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("To Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Amount", data.GetU64(4));
            (string transferSeed, int transferLength) = data.DecodeRustString(12);
            decodedInstruction.Values.Add("Seed", transferSeed);
            decodedInstruction.Values.Add("From Owner", data.GetPubKey(12 + transferLength));
        }

    }
}