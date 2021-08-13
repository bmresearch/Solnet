using Solnet.Extensions.TokenInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Extensions
{
    /// <summary>
    /// Defines well known tokens and their SPL Token Address, name, symbol and number of decimal places
    /// </summary>
    public static class WellKnownTokens
    {

        /// <summary>
        /// Wrapped SOL
        /// </summary>
        public static TokenDef WrappedSOL = new TokenDef("So11111111111111111111111111111111111111112", "Wrapped SOL", "SOL", 9);

        /// <summary>
        /// USDC
        /// </summary>
        public static TokenDef USDC = new TokenDef("EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v", "USD Coin", "USDC", 6);

        /// <summary>
        /// USDT
        /// </summary>
        public static TokenDef USDT = new TokenDef("Es9vMFrzaCERmJfrF4H2FYD4KCoNkY11McCe8BenwNYB", "USDT", "USDT", 6);

        /// <summary>
        /// SRM (Serum)
        /// </summary>
        public static TokenDef Serum = new TokenDef("SRMuApVNdxXokk5GT7XD5cUUgXMBCoAz2LHeuAoKWRt", "Serum", "SRM", 6);

        /// <summary>
        /// RAY (Radium)
        /// </summary>
        public static TokenDef Raydium = new TokenDef("4k3Dyjzvzp8eMZWUXbBCjEvwSkkk59S5iCNLY3QrkX6R", "Raydium", "RAY", 6);

    }
}
