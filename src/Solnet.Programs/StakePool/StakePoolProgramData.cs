using Solnet.Programs.StakePool.Models;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Text;

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
        /// <param name="fee"></param>
        /// <param name="withdrawalFee"></param>
        /// <param name="depositFee"></param>
        /// <param name="referralFee"></param>
        /// <param name="maxValidators"></param>
        /// <returns></returns>
        internal static byte[] EncodeInitializeData(Fee fee, Fee withdrawalFee, Fee depositFee, Fee referralFee, uint? maxValidators)
        {
            byte[] data = new byte[72];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.Initialize, MethodOffset);
            data.WriteU64(fee.Numerator, MethodOffset + 4); // Serialize Fee as Numerator and Denominator
            data.WriteU64(fee.Denominator, MethodOffset + 12);
            data.WriteU64(withdrawalFee.Numerator, MethodOffset + 20);
            data.WriteU64(withdrawalFee.Denominator, MethodOffset + 28);
            data.WriteU64(depositFee.Numerator, MethodOffset + 36);
            data.WriteU64(depositFee.Denominator, MethodOffset + 44);
            data.WriteU64(referralFee.Numerator, MethodOffset + 52);
            data.WriteU64(referralFee.Denominator, MethodOffset + 60);
            data.WriteU32(maxValidators ?? 0, MethodOffset + 68);
            return data;
        }


        /// <summary>
        /// Encodes the 'AddValidatorToPool' instruction data.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        internal static byte[] EncodeAddValidatorToPoolData(uint? seed)
        {
            byte[] data = new byte[8];
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
            byte[] data = new byte[20];
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
            byte[] data = new byte[28];
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
            byte[] data = new byte[20];
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
            byte[] data = new byte[28];
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
            byte[] data = new byte[28];
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
        internal static byte[] EncodeSetPreferredValidatorData(PreferredValidatorType validatorType)
        {
            // Allocate 8 bytes: 4 bytes for the discriminator and 4 bytes for the validator type.
            byte[] data = new byte[8];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.SetPreferredValidator, MethodOffset);
            data.WriteU32((uint)validatorType, MethodOffset + 4);
            return data;
        }

        internal static byte[] EncodeSetFeeData(FeeType feeType)
        {
            // 4 bytes for method, 1 for discriminant, up to 16 for Fee, or 1 for byte
            var buffer = new List<byte>();
            buffer.AddRange(BitConverter.GetBytes((uint)StakePoolProgramInstructions.Values.SetFee));

            // Discriminant and value
            switch (feeType)
            {
                case FeeType.SolReferral solReferral:
                    buffer.Add(0); // Discriminant for SolReferral
                    buffer.Add(solReferral.Percentage);
                    break;
                case FeeType.StakeReferral stakeReferral:
                    buffer.Add(1);
                    buffer.Add(stakeReferral.Percentage);
                    break;
                case FeeType.Epoch epoch:
                    buffer.Add(2);
                    buffer.AddRange(BitConverter.GetBytes(epoch.Fee.Numerator));
                    buffer.AddRange(BitConverter.GetBytes(epoch.Fee.Denominator));
                    break;
                case FeeType.StakeWithdrawal stakeWithdrawal:
                    buffer.Add(3);
                    buffer.AddRange(BitConverter.GetBytes(stakeWithdrawal.Fee.Numerator));
                    buffer.AddRange(BitConverter.GetBytes(stakeWithdrawal.Fee.Denominator));
                    break;
                case FeeType.SolWithdrawal solWithdrawal:
                    buffer.Add(4);
                    buffer.AddRange(BitConverter.GetBytes(solWithdrawal.Fee.Numerator));
                    buffer.AddRange(BitConverter.GetBytes(solWithdrawal.Fee.Denominator));
                    break;
                case FeeType.SolDeposit solDeposit:
                    buffer.Add(5);
                    buffer.AddRange(BitConverter.GetBytes(solDeposit.Fee.Numerator));
                    buffer.AddRange(BitConverter.GetBytes(solDeposit.Fee.Denominator));
                    break;
                case FeeType.StakeDeposit stakeDeposit:
                    buffer.Add(6);
                    buffer.AddRange(BitConverter.GetBytes(stakeDeposit.Fee.Numerator));
                    buffer.AddRange(BitConverter.GetBytes(stakeDeposit.Fee.Denominator));
                    break;
                default:
                    throw new ArgumentException("Unknown FeeType variant");
            }

            return buffer.ToArray();
        }

        /// <summary>
        /// Encodes the 'SetFee' instruction data using a Fee object.
        /// </summary>
        /// <param name="fee">The fee to set.</param>
        /// <returns>A byte array containing the encoded instruction data.</returns>
        internal static byte[] EncodeSetFee(Fee fee)
        {
            // Allocate 20 bytes: 4 bytes for the discriminator and 16 bytes for the fee (8 for numerator and 8 for denominator).
            byte[] data = new byte[20];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.SetFee, MethodOffset);
            data.WriteU64(fee.Numerator, MethodOffset + 4);
            data.WriteU64(fee.Denominator, MethodOffset + 12);
            return data;
        }

        /// <summary>
        /// Encodes the 'Redelegate' instruction data.
        /// </summary>
        internal static byte[] EncodeRedelegateData(ulong lamports, ulong sourceTransientStakeSeed, ulong ephemeralStakeSeed, ulong destinationTransientStakeSeed)
        {
            // 4 bytes for discriminator + 8 bytes each for 4 fields = 36 bytes total.
            byte[] data = new byte[36];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.Redelegate, MethodOffset);
            data.WriteU64(lamports, MethodOffset + 4);
            data.WriteU64(sourceTransientStakeSeed, MethodOffset + 12);
            data.WriteU64(ephemeralStakeSeed, MethodOffset + 20);
            data.WriteU64(destinationTransientStakeSeed, MethodOffset + 28);
            return data;
        }

        /// <summary>
        /// Encodes the 'UpdateStakePoolBalance' instruction data.
        /// </summary>
        internal static byte[] EncodeUpdateStakePoolBalance()
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.UpdateStakePoolBalance, MethodOffset);
            return data;
        }

        /// <summary>
        /// Encodes the 'UpdateValidatorListBalance' instruction data.
        /// </summary>
        /// <param name="startIndex">The starting index in the validator list.</param>
        /// <param name="noMerge">If true, merging is disabled.</param>
        /// <returns>A byte array containing the encoded instruction data.</returns>
        internal static byte[] EncodeUpdateValidatorListBalance(uint startIndex, bool noMerge)
        {
            // Allocate 9 bytes: 4 for the method discriminator,
            // 4 for the start index, and 1 for the no-merge flag.
            byte[] data = new byte[9];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.UpdateValidatorListBalance, MethodOffset);
            data.WriteU32(startIndex, MethodOffset + 4);
            data[MethodOffset + 8] = noMerge ? (byte)1 : (byte)0;
            return data;
        }

        /// <summary>
        /// Encodes the 'CleanupRemovedValidatorEntries' instruction data.
        /// </summary>
        /// <returns>A byte array containing the encoded instruction data.</returns>
        internal static byte[] EncodeCleanupRemovedValidatorEntries()
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.CleanupRemovedValidatorEntries, MethodOffset);
            return data;
        }

        /// <summary>
        /// Encodes the 'DepositStakeWithSlippage' instruction data.
        /// </summary>
        /// <param name="minimumPoolTokensOut">The minimum pool tokens expected on deposit (slippage constraint).</param>
        /// <returns>A byte array containing the encoded instruction data.</returns>
        internal static byte[] EncodeDepositStakeWithSlippage(ulong minimumPoolTokensOut)
        {
            // Allocate 12 bytes:
            // 4 bytes for the instruction discriminator and 8 bytes for the minimum pool tokens out value.
            byte[] data = new byte[12];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.DepositStakeWithSlippage, MethodOffset);
            data.WriteU64(minimumPoolTokensOut, MethodOffset + 4);
            return data;
        }

        /// <summary>
        /// Encodes the 'DepositStake' instruction data.
        /// </summary>
        /// <returns>A byte array containing the encoded instruction data for DepositStake.</returns>
        internal static byte[] EncodeDepositStake()
        {
            // Allocate 4 bytes for the instruction discriminator.
            byte[] data = new byte[4];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.DepositStake, MethodOffset);
            return data;
        }

        /// <summary>
        /// Encodes the 'DepositSol' instruction data (without slippage).
        /// </summary>
        /// <param name="lamportsIn">The amount of SOL lamports being deposited.</param>
        /// <returns>A byte array containing the encoded instruction data.</returns>
        internal static byte[] EncodeDepositSol(ulong lamportsIn)
        {
            // Allocate 12 bytes: 4 for the discriminator and 8 for lamportsIn.
            byte[] data = new byte[12];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.DepositSol, MethodOffset);
            data.WriteU64(lamportsIn, MethodOffset + 4);
            return data;
        }

        /// <summary>
        /// Encodes the 'DepositSolWithSlippage' instruction data.
        /// </summary>
        /// <param name="lamportsIn">The amount of SOL lamports being deposited.</param>
        /// <param name="minimumPoolTokensOut">The minimum pool tokens expected on deposit.</param>
        /// <returns>A byte array containing the encoded instruction data.</returns>
        internal static byte[] EncodeDepositSolWithSlippage(ulong lamportsIn, ulong minimumPoolTokensOut)
        {
            // Allocate 20 bytes: 4 bytes for the discriminator, 8 for lamportsIn, 8 for minimumPoolTokensOut.
            byte[] data = new byte[20];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.DepositSolWithSlippage, MethodOffset);
            data.WriteU64(lamportsIn, MethodOffset + 4);
            data.WriteU64(minimumPoolTokensOut, MethodOffset + 12);
            return data;
        }

        /// <summary>
        /// Encodes the 'CreateTokenMetadata' instruction data.
        /// </summary>
        /// <param name="name">The name of the token.</param>
        /// <param name="symbol">The token symbol.</param>
        /// <param name="uri">The URI for the token metadata.</param>
        /// <returns>A byte array containing the encoded instruction data.</returns>
        internal static byte[] EncodeCreateTokenMetadata(string name, string symbol, string uri)
        {
            // Encode string fields as UTF8 byte arrays.
            byte[] nameBytes = Encoding.UTF8.GetBytes(name);
            byte[] symbolBytes = Encoding.UTF8.GetBytes(symbol);
            byte[] uriBytes = Encoding.UTF8.GetBytes(uri);

            // Total length:
            // 4 bytes for discriminator +
            // 4 bytes (name length) + name bytes +
            // 4 bytes (symbol length) + symbol bytes +
            // 4 bytes (uri length) + uri bytes.
            int totalLength = 4 + (4 + nameBytes.Length) + (4 + symbolBytes.Length) + (4 + uriBytes.Length);
            byte[] data = new byte[totalLength];

            int offset = 0;
            // Write the discriminator.
            data.WriteU32((uint)StakePoolProgramInstructions.Values.CreateTokenMetadata, offset);
            offset += 4;

            // Write the name.
            data.WriteU32((uint)nameBytes.Length, offset);
            offset += 4;
            nameBytes.CopyTo(data, offset);
            offset += nameBytes.Length;

            // Write the symbol.
            data.WriteU32((uint)symbolBytes.Length, offset);
            offset += 4;
            symbolBytes.CopyTo(data, offset);
            offset += symbolBytes.Length;

            // Write the URI.
            data.WriteU32((uint)uriBytes.Length, offset);
            offset += 4;
            uriBytes.CopyTo(data, offset);

            return data;
        }

        /// <summary>
        /// Encodes the 'UpdateTokenMetadata' instruction data.
        /// </summary>
        /// <param name="name">The new name for the pool token.</param>
        /// <param name="symbol">The new symbol for the pool token.</param>
        /// <param name="uri">The new URI for the pool token metadata.</param>
        /// <returns>A byte array containing the encoded instruction data.</returns>
        internal static byte[] EncodeUpdateTokenMetadata(string name, string symbol, string uri)
        {
            // Convert the string fields to UTF8 byte arrays.
            byte[] nameBytes = Encoding.UTF8.GetBytes(name);
            byte[] symbolBytes = Encoding.UTF8.GetBytes(symbol);
            byte[] uriBytes = Encoding.UTF8.GetBytes(uri);

            // Compute total length:
            // 4 bytes for discriminator +
            // 4 bytes (name length) + name bytes +
            // 4 bytes (symbol length) + symbol bytes +
            // 4 bytes (uri length) + uri bytes.
            int totalLength = 4 + (4 + nameBytes.Length) + (4 + symbolBytes.Length) + (4 + uriBytes.Length);
            byte[] data = new byte[totalLength];

            int offset = 0;
            // Write the discriminator. Ensure that your StakePoolProgramInstructions enum has a value for UpdateTokenMetadata.
            data.WriteU32((uint)StakePoolProgramInstructions.Values.UpdateTokenMetadata, offset);
            offset += 4;

            // Write 'name'.
            data.WriteU32((uint)nameBytes.Length, offset);
            offset += 4;
            nameBytes.CopyTo(data, offset);
            offset += nameBytes.Length;

            // Write 'symbol'.
            data.WriteU32((uint)symbolBytes.Length, offset);
            offset += 4;
            symbolBytes.CopyTo(data, offset);
            offset += symbolBytes.Length;

            // Write 'uri'.
            data.WriteU32((uint)uriBytes.Length, offset);
            offset += 4;
            uriBytes.CopyTo(data, offset);

            return data;
        }

        /// <summary>
        /// Encodes the 'SetFundingAuthority' instruction data.
        /// </summary>
        /// <param name="fundingType">The funding type to be set.</param>
        /// <param name="newAuthority">The new authority public key.</param>
        /// <returns>A byte array containing the encoded instruction data.</returns>
        internal static byte[] EncodeSetFundingAuthority(FundingType fundingType, PublicKey newAuthority)
        {
            // Allocate 37 bytes:
            // 4 bytes for the discriminator,
            // 1 byte for the funding type (as a byte),
            // 32 bytes for the new authority public key.
            byte[] data = new byte[37];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.SetFundingAuthority, MethodOffset);

            // Write funding type enum value as a byte.
            data[MethodOffset + 4] = (byte)fundingType;

            // Write the new authority public key.
            newAuthority.KeyBytes.CopyTo(data, MethodOffset + 5);

            return data;
        }

        /// <summary>
        /// Encodes the 'SetFundingAuthority' instruction data.
        /// </summary>
        /// <param name="fundingType">The funding type.</param>
        /// <returns>A byte array containing the encoded instruction data.</returns>
        internal static byte[] EncodeSetFundingAuthority(FundingType fundingType)
        {
            // Allocate 5 bytes: 4 bytes for the method discriminator and 1 byte for the funding type.
            byte[] data = new byte[5];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.SetFundingAuthority, MethodOffset);
            data[MethodOffset + 4] = (byte)fundingType;
            return data;
        }

        /// <summary>
        /// Encodes the 'SetStaker' instruction data.
        /// </summary>
        /// <returns>A byte array containing the encoded instruction data.</returns>
        internal static byte[] EncodeSetStaker()
        {
            // Allocate 4 bytes for the discriminator only.
            byte[] data = new byte[4];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.SetStaker, MethodOffset);
            return data;
        }

        /// <summary>
        /// Encodes the 'SetManager' instruction data.
        /// </summary>
        /// <returns>A byte array containing the encoded instruction data.</returns>
        internal static byte[] EncodeSetManager()
        {
            // Allocate 4 bytes for the instruction discriminator.
            byte[] data = new byte[4];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.SetManager, MethodOffset);
            return data;
        }

        /// <summary>
        /// Encodes the 'WithdrawSolWithSlippage' instruction data.
        /// </summary>
        /// <param name="poolTokensIn">The amount of pool tokens being withdrawn.</param>
        /// <param name="minimumLamportsOut">The minimum lamports expected on withdrawal.</param>
        /// <returns>A byte array containing the encoded instruction data.</returns>
        internal static byte[] EncodeWithdrawSolWithSlippage(ulong poolTokensIn, ulong minimumLamportsOut)
        {
            // Allocate 20 bytes:
            // 4 bytes for the instruction discriminator,
            // 8 bytes for the poolTokensIn amount,
            // 8 bytes for the minimum lamports out value.
            byte[] data = new byte[20];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.WithdrawSolWithSlippage, MethodOffset);
            data.WriteU64(poolTokensIn, MethodOffset + 4);
            data.WriteU64(minimumLamportsOut, MethodOffset + 12);
            return data;
        }

        /// <summary>
        /// Encodes the 'WithdrawSol' instruction data (without slippage).
        /// </summary>
        /// <param name="poolTokensIn">The amount of pool tokens to be redeemed.</param>
        /// <returns>a byte array containing the encoded instruction data.</returns>
        internal static byte[] EncodeWithdrawSol(ulong poolTokensIn)
        {
            // Allocate 12 bytes: 4 bytes for the instruction discriminator and 8 for poolTokensIn.
            byte[] data = new byte[12];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.WithdrawSol, MethodOffset);
            data.WriteU64(poolTokensIn, MethodOffset + 4);
            return data;
        }

        /// <summary>
        /// Encodes the 'WithdrawStake' instruction data (without slippage).
        /// </summary>
        /// <param name="poolTokensIn">The amount of pool tokens to redeem.</param>
        /// <returns>A byte array containing the encoded instruction data.</returns>
        internal static byte[] EncodeWithdrawStake(ulong poolTokensIn)
        {
            // Allocate 12 bytes: 4 bytes for the instruction discriminator and 8 for poolTokensIn.
            byte[] data = new byte[12];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.WithdrawStake, MethodOffset);
            data.WriteU64(poolTokensIn, MethodOffset + 4);
            return data;
        }

        /// <summary>
        /// Encodes the 'WithdrawStakeWithSlippage' instruction data.
        /// </summary>
        /// <param name="poolTokensIn">The amount of pool tokens to redeem.</param>
        /// <param name="minimumLamportsOut">The minimum lamports expected on withdrawal.</param>
        /// <returns>A byte array containing the encoded instruction data.</returns>
        internal static byte[] EncodeWithdrawStakeWithSlippage(ulong poolTokensIn, ulong minimumLamportsOut)
        {
            // Allocate 20 bytes:
            // 4 bytes for the instruction discriminator,
            // 8 bytes for poolTokensIn,
            // 8 bytes for minimumLamportsOut.
            byte[] data = new byte[20];
            data.WriteU32((uint)StakePoolProgramInstructions.Values.WithdrawStakeWithSlippage, MethodOffset);
            data.WriteU64(poolTokensIn, MethodOffset + 4);
            data.WriteU64(minimumLamportsOut, MethodOffset + 12);
            return data;
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

        /// <summary>
        /// Decodes the 'UpdateValidatorListBalance' instruction data.
        /// </summary>
        /// <param name="decodedInstruction">The <see cref="DecodedInstruction"/> to populate.</param>
        /// <param name="data">The instruction data as a read-only span.</param>
        /// <param name="keys">The list of account public keys associated with the instruction.</param>
        /// <param name="keyIndices">Indices of the keys related to the instruction.</param>
        internal static void DecodeUpdateValidatorListBalance(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            uint startIndex = data.GetU32(MethodOffset + 4);
            bool noMerge = data[MethodOffset + 8] != 0;
            decodedInstruction.Values.Add("Start Index", startIndex);
            decodedInstruction.Values.Add("No Merge", noMerge);
        }

        /// <summary>
        /// Decodes the 'DepositStakeWithSlippage' instruction data.
        /// </summary>
        /// <param name="decodedInstruction">The <see cref="DecodedInstruction"/> to populate.</param>
        /// <param name="data">The instruction data as a read-only span.</param>
        /// <param name="keys">The list of account public keys associated with the instruction.</param>
        /// <param name="keyIndices">Indices of the keys related to the instruction.</param>
        internal static void DecodeDepositStakeWithSlippage(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            // The minimum pool tokens out is stored 4 bytes after the discriminator.
            ulong minimumPoolTokensOut = data.GetU64(MethodOffset + 4);
            decodedInstruction.Values.Add("Minimum Pool Tokens Out", minimumPoolTokensOut);
        }

        /// <summary>
        /// Decodes the 'SetFundingAuthority' instruction data.
        /// </summary>
        /// <param name="decodedInstruction">The <see cref="DecodedInstruction"/> to populate.</param>
        /// <param name="data">The instruction data as a read-only span.</param>
        /// <param name="keys">The list of account public keys associated with the instruction.</param>
        /// <param name="keyIndices">Indices of the keys related to the instruction.</param>
        internal static void DecodeSetFundingAuthority(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            // The funding type is stored at offset 4 (after the 4-byte discriminator)
            decodedInstruction.Values.Add("Funding Type", (FundingType)data[MethodOffset + 4]);
        }

        /// <summary>
        /// Decodes the 'SetStaker' instruction data.
        /// </summary>
        /// <param name="decodedInstruction">The <see cref="DecodedInstruction"/> to populate.</param>
        /// <param name="data">The instruction data as a read-only span.</param>
        /// <param name="keys">The list of account public keys associated with the instruction.</param>
        /// <param name="keyIndices">Indices of the keys related to the instruction.</param>
        internal static void DecodeSetStaker(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            // Extract the new staker public key from offset 4.
            var keyBytes = data.Slice(MethodOffset + 4, PublicKey.PublicKeyLength).ToArray();
            decodedInstruction.Values.Add("New Staker", new PublicKey(keyBytes));
        }
    }
}
