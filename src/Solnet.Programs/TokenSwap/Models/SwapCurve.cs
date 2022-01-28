using Solnet.Programs.Utilities;
using System;
using System.Buffers.Binary;

namespace Solnet.Programs.TokenSwap.Models
{
    /// <summary>
    /// A swap curve type of a token swap. The static construction methods should be used to construct
    /// </summary>
    public class SwapCurve
    {
        /// <summary>
        /// The constant procuct curve
        /// </summary>
        public static SwapCurve ConstantProduct => new SwapCurve() { CurveType = CurveType.ConstantProduct, Calculator = new ConstantProductCurve() };

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

        public static SwapCurve Deserialize(byte[] bytes)
        {
            var s = new SwapCurve()
            {
                CurveType = (CurveType)bytes[0],
                //todo other curves
                Calculator = new ConstantProductCurve()
            };
            if (s.CurveType != CurveType.ConstantProduct)
            {
                throw new NotSupportedException("Only constant product curves are supported by Solnet currently");
            }
            return s;
        }
    }
}
