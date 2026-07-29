using System;

namespace Solnet.Programs.TokenSwap.Models
{
    /// <summary>
    /// A curve calculator must serialize itself to 32 bytes
    /// </summary>
    public interface ICurveCalculator
    {
        /// <summary>
        /// Serialize this calculator type
        /// </summary>
        /// <returns></returns>
        ReadOnlySpan<byte> Serialize();
    }
}