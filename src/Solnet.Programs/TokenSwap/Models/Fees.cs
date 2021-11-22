using System;
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
    }
}
