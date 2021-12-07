using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using static Solnet.Programs.Models.Stake.State;

namespace Solnet.Programs
{
    /// <summary>
    /// Implements the stake program data encodings.
    /// </summary>
    internal static class StakeProgramData
    {
        /// <summary>
        /// The offset at which the value which defines the program method begins. 
        /// </summary>
        internal const int MethodOffset = 0;

        static public string ToReadableByteArray(byte[] bytes)
        {
            return string.Join(", ", bytes);
        }

        /// <summary>
        /// Summary text here
        /// </summary>
        internal static byte[] EncodeInitializeData(Authorized authorized, Lockup lockup)
        {
            byte[] data = new byte[116];

            data.WriteU32((uint)StakeProgramInstructions.Values.Initialize, MethodOffset);
            data.WritePubKey(authorized.Staker, 4);
            data.WritePubKey(authorized.Withdrawer, 36);
            data.WriteS64(lockup.UnixTimestamp, 68);
            data.WriteU64(lockup.Epoch, 76);
            data.WritePubKey(lockup.Custodian, 84);

            var k = ToReadableByteArray(data);

            return data;
        }
        internal static byte[] EncodeAuthorizeData(PublicKey newAuthorizedPubkey, StakeAuthorize stakeAuthorize)
        {
            byte[] data = new byte[68];

            data.WriteU32((uint)StakeProgramInstructions.Values.Authorize, MethodOffset);
            data.WritePubKey(newAuthorizedPubkey, 4);
            data.WriteU32((uint)stakeAuthorize, 36);
            
            return data;
        }

        internal static byte[] EncodeDelegateStakeData()
        {
            byte[] data = new byte[4];

            data.WriteU32((uint)StakeProgramInstructions.Values.DelegateStake, MethodOffset);
            return data;
        }
        internal static byte[] EncodeSplitData(ulong lamports)
        {
            byte[] data = new byte[12];

            data.WriteU32((uint)StakeProgramInstructions.Values.Split, MethodOffset);
            data.WriteU64(lamports, 4);
            return data;
        }

        internal static byte[] EncodeWithdrawData(ulong lamports)
        {
            byte[] data = new byte[12];

            data.WriteU32((uint)StakeProgramInstructions.Values.Withdraw, MethodOffset);
            data.WriteU64(lamports, 4);
            return data;
        }

        internal static byte[] EncodeDeactivateData()
        {
            byte[] data = new byte[4];

            data.WriteU32((uint)StakeProgramInstructions.Values.Deactivate, MethodOffset);
            return data;
        }

        internal static byte[] EncodeSetLockupData(Lockup lockup)
        {
            byte[] data = new byte[52];

            data.WriteU32((uint)StakeProgramInstructions.Values.SetLockup, MethodOffset);
            data.WriteS64(lockup.UnixTimestamp, 4);
            data.WriteU64(lockup.Epoch, 12);
            data.WritePubKey(lockup.Custodian, 20);
            return data;
        }

        internal static byte[] EncodeMergeData()
        {
            byte[] data = new byte[4];

            data.WriteU32((uint)StakeProgramInstructions.Values.Merge, MethodOffset);
            return data;
        }

        internal static byte[] EncodeAuthorizeWithSeedData(string authoritySeed, PublicKey newAuthorizedPubKey, StakeAuthorize stakeAuthorize, PublicKey authorityOwner)
        {
            byte[] encodedSeed = Serialization.EncodeRustString(authoritySeed);
            byte[] data = new byte[72 + encodedSeed.Length];

            data.WriteU32((uint)StakeProgramInstructions.Values.AuthorizeWithSeed, MethodOffset);
            data.WritePubKey(newAuthorizedPubKey, 4);
            data.WriteU32((uint)stakeAuthorize, 36);
            data.WriteSpan(encodedSeed, 40);
            data.WritePubKey(authorityOwner, 40 + encodedSeed.Length);
            return data;
        }

        internal static byte[] EncodeInitializeCheckedData()
        {
            byte[] data = new byte[4];

            data.WriteU32((uint)StakeProgramInstructions.Values.InitializeChecked, MethodOffset);
            return data;
        }

        internal static byte[] EncodeAuthorizeCheckedData(StakeAuthorize stakeAuthorize)
        {
            byte[] data = new byte[8];

            data.WriteU32((uint)StakeProgramInstructions.Values.Authorize, MethodOffset);
            data.WriteU32((uint)stakeAuthorize, 4);
            return data;
        }

        internal static byte[] EncodeAuthorizeCheckedWithSeedData(string authoritySeed, PublicKey authorityOwner, StakeAuthorize stakeAuthorize)
        {
            byte[] encodedSeed = Serialization.EncodeRustString(authoritySeed);
            byte[] data = new byte[40 + encodedSeed.Length];

            data.WriteU32((uint)StakeProgramInstructions.Values.AuthorizeCheckedWithSeed, MethodOffset);
            data.WriteSpan(encodedSeed, 4);
            data.WriteU32((uint)stakeAuthorize, 4 + encodedSeed.Length);
            data.WritePubKey(authorityOwner, 8 + encodedSeed.Length);
            return data;
        }
        internal static byte[] EncodeSetLockupCheckedData(Lockup lockup)
        {
            byte[] data = new byte[20];

            data.WriteU32((uint)StakeProgramInstructions.Values.SetLockup, MethodOffset);
            data.WriteS64(lockup.UnixTimestamp, 4);
            data.WriteU64(lockup.Epoch, 12);
            return data;
        }
        internal static void DecodeInitializeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Authorized Withdraw Account", data.GetPubKey(4));
            decodedInstruction.Values.Add("Authorized Stake Account", data.GetPubKey(36));
            decodedInstruction.Values.Add("Lockup Timestamp", data.GetS64(68));
            decodedInstruction.Values.Add("Lockup Epoch", data.GetU64(76));
            decodedInstruction.Values.Add("Lockup Custodian", data.GetPubKey(84));
        }
        internal static void DecodeAuthorizeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Authorized Account", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Custodian Account", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("New Authorized Account", data.GetPubKey(4));
            decodedInstruction.Values.Add("Stake Authorize", Enum.Parse(typeof(StakeAuthorize), data.GetU8(36).ToString()));
        }
        internal static void DecodeDelegateStakeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Vote Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Authorized Account", keys[keyIndices[5]]);
        }
        internal static void DecodeSplitStakeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Split Stake Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Authorized Account", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Amount", data.GetU64(4));
        }
        internal static void DecodeWithdrawStakeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("To Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Withdraw Account", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Custodian Account", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Amount", data.GetU64(4));
        }
        internal static void DecodeDeactivateStakeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Authorized Account", keys[keyIndices[2]]);
        }
        internal static void DecodeSetLockupStakeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Custodian Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Lockup Timestamp", data.GetS64(4));
            decodedInstruction.Values.Add("Lockup Epoch", data.GetU64(12));
            decodedInstruction.Values.Add("Lockup Custodian", data.GetPubKey(20));
        }
        internal static void DecodeMergeStakeData(DecodedInstruction decodedInstruction,
                                                  ReadOnlySpan<byte> data,
                                                  IList<PublicKey> keys,
                                                  byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Destination Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Source Stake Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Authorized Account", keys[keyIndices[4]]);
        }
        internal static void DecodeAuthorizeWithSeedStakeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Authority Base Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Custodian Account", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("New Authorized Account", data.GetPubKey(4));
            decodedInstruction.Values.Add("Stake Authorize", Enum.Parse(typeof(StakeAuthorize), data.GetU8(36).ToString()));
            (string authoritySeed, int seedLength) = data.DecodeRustString(37);
            decodedInstruction.Values.Add("Authority Seed", authoritySeed);
            decodedInstruction.Values.Add("Authority Owner Account", data.GetPubKey(37 + seedLength));
        }  
        internal static void DecodeInitializeCheckedStakeData(DecodedInstruction decodedInstruction,
                                                  ReadOnlySpan<byte> data,
                                                  IList<PublicKey> keys,
                                                  byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Authorized Staker Account", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Authorized Withdrawer Account", keys[keyIndices[3]]);
        }
        internal static void DecodeAuthorizeCheckedStakeData(DecodedInstruction decodedInstruction,
                                                  ReadOnlySpan<byte> data,
                                                  IList<PublicKey> keys,
                                                  byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Authorized Account", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Custodian Account", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("New Authorized Account", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Stake Authorize", Enum.Parse(typeof(StakeAuthorize), data.GetU8(4).ToString()));
        }
        internal static void DecodeAuthorizeCheckedWithSeedStakeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Authority Base Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Custodian Account", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("New Authorized Account", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Stake Authorize", Enum.Parse(typeof(StakeAuthorize), data.GetU8(4).ToString()));
            (string authoritySeed, int seedLength) = data.DecodeRustString(5);
            decodedInstruction.Values.Add("Authority Seed", authoritySeed);
            decodedInstruction.Values.Add("Authority Owner Account", data.GetPubKey(5 + seedLength));
        }
        internal static void DecodeSetLockupCheckedStakeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Stake Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Custodian Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Lockup Timestamp", data.GetS64(4));
            decodedInstruction.Values.Add("Lockup Epoch", data.GetU64(12));
            decodedInstruction.Values.Add("Lockup Custodian", keys[keyIndices[2]]);
        }
    }
}
