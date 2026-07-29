using System;

namespace Solnet.Programs.TokenSwap.Models
{
    /// <summary>
    /// Uniswap-style constant product curve, invariant = token_a_amount * token_b_amount
    /// </summary>
    public class ConstantProductCurve : ICurveCalculator
    {

        /// <summary>
        /// Serialize the Fees
        /// </summary>
        /// <returns>Serialized Fees</returns>
        public ReadOnlySpan<byte> Serialize()
        {
            return new Span<byte>([]);
        }

    }
}