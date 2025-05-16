using Solnet.Programs.StakePool.Models;
using Solnet.Programs.TokenSwap.Models;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Programs.StakePool
{
    internal static class StakePoolProgramData
    {
        /// <summary>
        /// The offset at which the value which defines the program method begins. 
        /// </summary>
        internal const int MethodOffset = 0;


        /// <summary>
        /// Encodes the 'Initialize' instruction data.
        /// </summary>
        /// <param name="fees"></param>
        /// <param name="withdrawalFee"></param>
        /// <param name="depositFee"></param>
        /// <param name="referralFee"></param>
        /// <param name="maxValidators"></param>
        /// <returns></returns>
        internal static byte[] EncodeInitializeData(Fees fees, Fees withdrawalFee, Fees depositFee, Fees referralFee, uint? maxValidators)
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.Initialize, MethodOffset);
            data.WriteSpan(fees.Serialize(), MethodOffset + 4);
            data.WriteSpan(withdrawalFee.Serialize(), MethodOffset + 12);
            data.WriteSpan(depositFee.Serialize(), MethodOffset + 20);
            data.WriteSpan(referralFee.Serialize(), MethodOffset + 28);
            data.WriteU32(maxValidators ?? 0, MethodOffset + 36);
            // Here you would implement the serialization of `initData` (e.g., using borsh or another method)
            return data; // Example: return the serialized byte array
        }


        /// <summary>
        /// Encodes the 'AddValidatorToPool' instruction data.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        internal static byte[] EncodeAddValidatorToPoolData(uint? seed)
        {
            byte[] data = new byte[4];
            var seedValue = seed ?? 0;
            data.WriteU32((uint)StakePoolProgramInstructions.Values.AddValidatorToPool, MethodOffset);
            data.WriteU32(seedValue, MethodOffset + 4);
            // Assuming the enum or data structure you're sending in the instruction is properly serialized here
            return data; // Example encoding, adjust based on actual data
        }

        /// <summary>
        /// Encodes the 'AddValidatorToPoolWithVote' instruction data.
        /// </summary>
        /// <returns></returns>
        internal static byte[] EncodeRemoveValidatorFromPoolData()
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.RemoveValidatorFromPool, MethodOffset);
            // Here you would implement the serialization of removeData (e.g., using Borsh or another method)
            return data; // Example: return the serialized byte array
        }

        /// <summary>
        /// Encodes the 'DecreaseValidatorStake' instruction data.
        /// </summary>
        internal static byte[] EncodeDecreaseValidatorStakeData(ulong lamports, ulong transientStakeSeed)
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.DecreaseValidatorStake, MethodOffset);
            data.WriteU64(lamports, MethodOffset + 4);
            data.WriteU64(transientStakeSeed, MethodOffset + 12);
            // Here you would implement the serialization of decreaseData (e.g., using Borsh or another method)
            return data; // Example: return the serialized byte array
        }


        /// <summary>
        /// Encodes the 'DecreaseAdditionalValidatorStake' instruction data.
        /// </summary>
        internal static byte[] EncodeDecreaseAdditionalValidatorStakeData(ulong lamports, ulong transientStakeSeed, ulong ephemeralStakeSeed)
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.DecreaseAdditionalValidatorStake, MethodOffset);
            data.WriteU64(lamports, MethodOffset + 4);
            data.WriteU64(transientStakeSeed, MethodOffset + 12);
            data.WriteU64(ephemeralStakeSeed, MethodOffset + 20);
            // Here you would implement the serialization of decreaseData (e.g., using Borsh or another method)
            return data; // Example: return the serialized byte array
        }

        /// <summary>
        /// Encodes the 'DecreaseValidatorStakeWithReserve' instruction data.
        /// </summary>
        internal static byte[] EncodeDecreaseValidatorStakeWithReserveData(ulong lamports, ulong transientStakeSeed)
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.DecreaseValidatorStakeWithReserve, MethodOffset);
            data.WriteU64(lamports, MethodOffset + 4);
            data.WriteU64(transientStakeSeed, MethodOffset + 12);
            // Implement the serialization of decreaseData (e.g., using Borsh or another method)
            return data; // Example: return the serialized byte array
        }

        /// <summary>
        /// Encodes the 'IncreaseValidatorStake' instruction data.
        /// </summary>
        /// <returns></returns>
        internal static byte[] EncodeIncreaseValidatorStakeData(ulong lamports, ulong transientStakeSeed)
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.IncreaseValidatorStake, MethodOffset);
            data.WriteU64(lamports, MethodOffset + 4);
            data.WriteU64(transientStakeSeed, MethodOffset + 12);
            // Implement the serialization of increaseData (e.g., using Borsh or another method)
            return data; // Example: return the serialized byte array
        }

        /// <summary>
        /// Encodes the 'IncreaseAdditionalValidatorStake' instruction data.
        /// </summary>
        internal static byte[] EncodeIncreaseAdditionalValidatorStakeData(ulong lamports, ulong transientStakeSeed, ulong ephemeralStakeSeed)
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.IncreaseValidatorStake, MethodOffset);
            data.WriteU64(lamports, MethodOffset + 4);
            data.WriteU64(transientStakeSeed, MethodOffset + 12);
            data.WriteU64(ephemeralStakeSeed, MethodOffset + 20);
            // Implement the serialization of increaseData (e.g., using Borsh or another method)
            return data; // Example: return the serialized byte array
        }

#nullable enable
        /// <summary>
        /// Encodes the 'SetPreferredDepositValidator' instruction data.
        /// </summary>
        internal static byte[] EncodeSetPreferredValidatorData(PreferredValidatorType validatorType, PublicKey? validatorVoteAddress)
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.SetPreferredValidator, MethodOffset);
            data.WriteU32((uint)validatorType, MethodOffset + 4);
            if (validatorVoteAddress != null)
            {
                data.WriteSpan(validatorVoteAddress.KeyBytes, MethodOffset + 8);
            }

            // Implement the serialization of setPreferredValidatorData (e.g., using Borsh or another method)
            return data; // Example: return the serialized byte array
        }

        /// <summary>
        /// Decodes the 'initialize' instruction data.
        /// </summary>
        internal static void DecodeInitializeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Fee assessed as percentage of perceived rewards", data.GetU64(4));
            decodedInstruction.Values.Add("Fee charged per withdrawal as percentage of withdrawal", data.GetU64(12));
            decodedInstruction.Values.Add("Fee charged per deposit as percentage of deposit", data.GetU64(20));
            decodedInstruction.Values.Add("Percentage [0-100] of deposit_fee that goes to referrer", data.GetU64(28));
            decodedInstruction.Values.Add("Maximum expected number of validators", data.GetU32(36));

        }

        /// <summary>
        /// Decodes the 'AddValidatorToPool' instruction data.
        /// </summary>
        internal static void DecodeAddValidatorToPoolData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            // The seed is at offset 4 (after the 4-byte method discriminator)
            decodedInstruction.Values.Add("Seed", data.GetU32(4));
        }

        /// <summary>
        /// Decodes the 'RemoveValidatorFromPool' instruction data.
        /// </summary>
        internal static void DecodeRemoveValidatorFromPoolData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            // No additional data to decode for this instruction
        }

        /// <summary>
        /// Decodes the 'DecreaseValidatorStake' instruction data.
        /// </summary>
        internal static void DecodeDecreaseValidatorStakeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Lamports", data.GetU64(4));
            decodedInstruction.Values.Add("Transient Stake Seed", data.GetU64(12));
        }

        /// <summary>
        /// Decodes the 'DecreaseAdditionalValidatorStake' instruction data.
        /// </summary>
        internal static void DecodeDecreaseAdditionalValidatorStakeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Lamports", data.GetU64(4));
            decodedInstruction.Values.Add("Transient Stake Seed", data.GetU64(12));
            decodedInstruction.Values.Add("Ephemeral Stake Seed", data.GetU64(20));
        }

        /// <summary>
        /// Decodes the 'DecreaseValidatorStakeWithReserve' instruction data.
        /// </summary>
        internal static void DecodeDecreaseValidatorStakeWithReserveData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Lamports", data.GetU64(4));
            decodedInstruction.Values.Add("Transient Stake Seed", data.GetU64(12));
        }

        /// <summary>
        /// Decodes the 'IncreaseValidatorStake' instruction data.
        /// </summary>
        internal static void DecodeIncreaseValidatorStakeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Lamports", data.GetU64(4));
            decodedInstruction.Values.Add("Transient Stake Seed", data.GetU64(12));
        }

        /// <summary>
        /// Decodes the 'IncreaseAdditionalValidatorStake' instruction data.
        /// </summary>
        internal static void DecodeIncreaseAdditionalValidatorStakeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Lamports", data.GetU64(4));
            decodedInstruction.Values.Add("Transient Stake Seed", data.GetU64(12));
            decodedInstruction.Values.Add("Ephemeral Stake Seed", data.GetU64(20));
        }

        /// <summary>
        /// Decodes the 'Redelegate' instruction data.
        /// </summary>
        internal static void DecodeRedelegateData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            // No additional data to decode for this instruction (based on encoding stub)
        }

        /// <summary>
        /// Decodes the 'SetPreferredDepositValidator' instruction data.
        /// </summary>
        internal static void DecodeSetPreferredValidatorData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Validator Type", (PreferredValidatorType)data.GetU32(4));
            if (data.Length >= 8 + PublicKey.PublicKeyLength)
            {
                var keyBytes = data.Slice(8, PublicKey.PublicKeyLength).ToArray();
                decodedInstruction.Values.Add("Validator Vote Address", new PublicKey(keyBytes));
            }
        }

    }
}
