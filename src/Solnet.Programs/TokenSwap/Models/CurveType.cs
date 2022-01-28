namespace Solnet.Programs.TokenSwap.Models
{
    /// <summary>
    /// Curve type enum for an instruction
    /// </summary>
    public enum CurveType
    {
        /// Uniswap-style constant product curve, invariant = token_a_amount * token_b_amount
        ConstantProduct = 0,
        /// Flat line, always providing 1:1 from one token to another
        ConstantPrice = 1,
        /// Stable, like uniswap, but with wide zone of 1:1 instead of one point
        Stable = 2,
        /// Offset curve, like Uniswap, but the token B side has a faked offset
        Offset = 3,
    }
}