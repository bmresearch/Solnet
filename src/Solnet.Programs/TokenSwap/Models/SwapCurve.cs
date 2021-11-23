using Solnet.Programs.Utilities;
using System;

namespace Solnet.Programs.TokenSwap.Models
{
    /// <summary>
    /// A swap curve type of a token swap. The static construction methods should be used to construct
    /// </summary>
    public class SwapCurve
    {
        /// <summary>
        /// The curve type.
        /// </summary>
        public CurveType CurveType { get; set; }

        /// <summary>
        /// The calculator used
        /// </summary>
        public CurveCalculator Calculator { get; set; }

        /// <summary>
        /// Create a swap curve class.  Protected as factory methods should be used to create
        /// </summary>
        protected SwapCurve() { }

        /// <summary>
        /// Serialize this swap curve for an instruction
        /// </summary>
        /// <returns></returns>
        public virtual ReadOnlySpan<byte> Serialize()
        {
            var ret = new byte[33];
            ret.WriteU8((byte)CurveType, 0);
            ret.WriteSpan(Calculator.Serialize(), 1);
            return new Span<byte>(ret);
        }

        /// <summary>
        /// The constant procuct curve
        /// </summary>
        public static SwapCurve ConstantProduct => new SwapCurve() { CurveType = CurveType.ConstantProduct, Calculator = new ConstantProductCurve() };
    }
}
