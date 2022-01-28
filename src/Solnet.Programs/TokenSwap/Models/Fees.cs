using System;
using System.Buffers.Binary;
using Solnet.Programs.Utilities;

namespace Solnet.Programs.TokenSwap.Models
{
    /// <summary>
    /// Encapsulates all fee information and calculations for swap operations
    /// </summary>
    public class Fees
    {
        /// <summary>
        /// Trade fee numerator.
        /// </summary>
        public ulong TradeFeeNumerator;

        /// <summary>
        /// Trade fee denominator.
        /// </summary>
        public ulong TradeFeeDenominator;

        /// <summary>
        /// Owner trade fee numerator.
        /// </summary>
        public ulong OwnerTradeFeeNumerator;

        /// <summary>
        /// Owner trade fee denominator.
        /// </summary>
        public ulong OwnerTradeFeeDenomerator;

        /// <summary>
        /// Owner withdraw fee numerator.
        /// </summary>
        public ulong OwnerWithrawFeeNumerator;

        /// <summary>
        /// Owner withdraw fee denominator.
        /// </summary>
        public ulong OwnerWithrawFeeDenomerator;

        /// <summary>
        /// Host trading fee numerator.
        /// </summary>
        public ulong HostFeeNumerator;

        /// <summary>
        /// Host trading fee denominator.
        /// </summary>
        public ulong HostFeeDenomerator;
        
        /// <summary>
        /// Serialize the Fees
        /// </summary>
        /// <returns>Serialized Fees</returns>
        public Span<byte> Serialize()
        {
            var ret = new byte[64];
            ret.WriteU64(TradeFeeNumerator, 0);
            ret.WriteU64(TradeFeeDenominator, 8);
            ret.WriteU64(OwnerTradeFeeNumerator, 16);
            ret.WriteU64(OwnerTradeFeeDenomerator, 24);
            ret.WriteU64(OwnerWithrawFeeNumerator, 32);
            ret.WriteU64(OwnerWithrawFeeDenomerator, 40);
            ret.WriteU64(HostFeeNumerator, 48);
            ret.WriteU64(HostFeeDenomerator, 56);
            return new Span<byte>(ret);
        }

        public static Fees Deserialize(byte[] bytes)
        {
            var span = new Span<byte>(bytes);
            var f = new Fees()
            {
                TradeFeeNumerator = BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(0, 8)),
                TradeFeeDenominator = BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(8, 8)),
                OwnerTradeFeeNumerator = BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(16, 8)),
                OwnerTradeFeeDenomerator = BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(24, 8)),
                OwnerWithrawFeeNumerator = BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(32, 8)),
                OwnerWithrawFeeDenomerator = BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(40, 8)),
                HostFeeNumerator = BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(48, 8)),
                HostFeeDenomerator = BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(56, 8)),
            };
            return f;
        }
    }
}
