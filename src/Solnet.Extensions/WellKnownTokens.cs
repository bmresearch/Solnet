using Solnet.Extensions.TokenInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Extensions
{
    public static class WellKnownTokens
    {

        public static TokenDef USDC = new TokenDef("EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v", "USD Coin", "USDC", 6);

        public static TokenDef USDT = new TokenDef("Es9vMFrzaCERmJfrF4H2FYD4KCoNkY11McCe8BenwNYB", "USDT", "USDT", 6);

        public static TokenDef SRM = new TokenDef("SRMuApVNdxXokk5GT7XD5cUUgXMBCoAz2LHeuAoKWRt", "Serum", "SRM", 6);

        public static TokenDef RAY = new TokenDef("4k3Dyjzvzp8eMZWUXbBCjEvwSkkk59S5iCNLY3QrkX6R", "Raydium", "RAY", 6);

    }
}
