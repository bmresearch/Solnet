using System;
using Solnet.Wallet;

namespace Solnet.Programs.StakePool.Models
{
    /// <summary>
    /// Information about a validator in the pool.
    /// 
    /// NOTE: ORDER IS VERY IMPORTANT HERE, PLEASE DO NOT RE-ORDER THE FIELDS UNLESS
    /// THERE'S AN EXTREMELY GOOD REASON.
    ///
    /// To save on BPF instructions, the serialized bytes are reinterpreted with a
    /// bytemuck transmute, which means that this structure cannot have any
    /// undeclared alignment-padding in its representation.
    /// </summary>
    public class ValidatorStakeInfo
    {
        /// <summary>
        /// Represents the fixed length of the data structure.
        /// </summary>
        /// <remarks>This constant defines the length as 73 and is intended to be used wherever the fixed
        /// size is required.</remarks>
        public const int Length = 73;

        /// <summary>
        /// Amount of lamports on the validator stake account, including rent.
        /// </summary>
        public ulong ActiveStakeLamports { get; set; }

        /// <summary>
        /// Amount of transient stake delegated to this validator.
        /// </summary>
        public ulong TransientStakeLamports { get; set; }

        /// <summary>
        /// Last epoch the active and transient stake lamports fields were updated.
        /// </summary>
        public ulong LastUpdateEpoch { get; set; }

        /// <summary>
        /// Transient account seed suffix, used to derive the transient stake account address.
        /// </summary>
        public ulong TransientSeedSuffix { get; set; }

        /// <summary>
        /// Unused space, initially meant to specify the end of seed suffixes.
        /// </summary>
        public uint Unused { get; set; }

        /// <summary>
        /// Validator account seed suffix (0 means None).
        /// </summary>
        public uint ValidatorSeedSuffix { get; set; }

        /// <summary>
        /// Status of the validator stake account.
        /// </summary>
        public PodStakeStatus Status { get; set; }

        /// <summary>
        /// Validator vote account address.
        /// </summary>
        public PublicKey VoteAccountAddress { get; set; }

        /// <summary>
        /// Get the total lamports on this validator (active and transient).
        /// Returns null if overflow occurs.
        /// </summary>
        public ulong? StakeLamports()
        {
            try
            {
                checked
                {
                    return ActiveStakeLamports + TransientStakeLamports;
                }
            }
            catch (OverflowException)
            {
                return null;
            }
        }

        /// <summary>
        /// Performs a very cheap comparison, for checking if this validator stake info matches the vote account address.
        /// </summary>
        public static bool MemcmpPubkey(ReadOnlySpan<byte> data, PublicKey voteAddress)
        {
            // VoteAccountAddress is at offset 41, length 32
            return data.Slice(41, 32).SequenceEqual(voteAddress.KeyBytes);
        }

        /// <summary>
        /// Checks if this validator stake info has more active lamports than some limit.
        /// </summary>
        public static bool ActiveLamportsGreaterThan(ReadOnlySpan<byte> data, ulong lamports)
        {
            // ActiveStakeLamports is at offset 0, length 8
            ulong value = BitConverter.ToUInt64(data.Slice(0, 8));
            return value > lamports;
        }

        /// <summary>
        /// Checks if this validator stake info has more transient lamports than some limit.
        /// </summary>
        public static bool TransientLamportsGreaterThan(ReadOnlySpan<byte> data, ulong lamports)
        {
            // TransientStakeLamports is at offset 8, length 8
            ulong value = BitConverter.ToUInt64(data.Slice(8, 8));
            return value > lamports;
        }

        /// <summary>
        /// Check that the validator stake info is valid (not removed).
        /// </summary>
        public static bool IsNotRemoved(ReadOnlySpan<byte> data)
        {
            // Status is at offset 40, 1 byte
            return (StakeStatus)data[40] != StakeStatus.ReadyForRemoval;
        }

        /// <summary>
        /// Packs this instance into a 73-byte array.
        /// </summary>
        public byte[] Pack()
        {
            var data = new byte[Length];
            int offset = 0;

            BitConverter.GetBytes(ActiveStakeLamports).CopyTo(data, offset); offset += 8;
            BitConverter.GetBytes(TransientStakeLamports).CopyTo(data, offset); offset += 8;
            BitConverter.GetBytes(LastUpdateEpoch).CopyTo(data, offset); offset += 8;
            BitConverter.GetBytes(TransientSeedSuffix).CopyTo(data, offset); offset += 8;
            BitConverter.GetBytes(Unused).CopyTo(data, offset); offset += 4;
            BitConverter.GetBytes(ValidatorSeedSuffix).CopyTo(data, offset); offset += 4;
            data[offset++] = Status.Value;
            VoteAccountAddress.KeyBytes.CopyTo(data, offset);

            return data;
        }

        /// <summary>
        /// Unpacks a 73-byte array into a ValidatorStakeInfo instance.
        /// </summary>
        public static ValidatorStakeInfo Unpack(ReadOnlySpan<byte> data)
        {
            if (data.Length != Length)
                throw new ArgumentException($"Data must be {Length} bytes", nameof(data));

            int offset = 0;
            var info = new ValidatorStakeInfo
            {
                ActiveStakeLamports = BitConverter.ToUInt64(data.Slice(offset, 8)),
                TransientStakeLamports = BitConverter.ToUInt64(data.Slice(offset += 8, 8)),
                LastUpdateEpoch = BitConverter.ToUInt64(data.Slice(offset += 8, 8)),
                TransientSeedSuffix = BitConverter.ToUInt64(data.Slice(offset += 8, 8)),
                Unused = BitConverter.ToUInt32(data.Slice(offset += 8, 4)),
                ValidatorSeedSuffix = BitConverter.ToUInt32(data.Slice(offset += 4, 4)),
                Status = new PodStakeStatus(data[offset += 4]),
                VoteAccountAddress = new PublicKey(data.Slice(offset + 1, 32).ToArray())
            };
            return info;
        }
    }
}