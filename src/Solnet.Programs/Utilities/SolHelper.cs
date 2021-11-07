using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Utilities
{
    /// <summary>
    /// Helper class for conversion between Sol and Lamports.
    /// </summary>
    public class SolHelper
    {
        /// <summary>
        /// Number of Lamports per Sol.
        /// </summary>
        public const ulong LAMPORTS_PER_SOL = 1000000000;

        /// <summary>
        /// Convert Lamports value into Sol decimal value.
        /// </summary>
        /// <param name="lamports"></param>
        /// <returns></returns>
        public static decimal ConvertToSol(ulong lamports)
        {
            return Decimal.Round((decimal)lamports / (decimal)LAMPORTS_PER_SOL, 9);
        }

        /// <summary>
        /// Convert a decimal Sol value into Lamports ulong value.
        /// </summary>
        /// <param name="sol"></param>
        /// <returns></returns>
        public static ulong ConvertToLamports(decimal sol)
        {
            return (ulong)(sol * LAMPORTS_PER_SOL);
        }
    }
}
