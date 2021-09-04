using Solnet.Extensions.TokenMint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Extensions
{
    /// <summary>
    /// Defines well known tokens and their SPL Token Address, name, symbol and number of decimal places
    /// </summary>
    public static class WellKnownTokens
    {
        private static List<TokenDef> _tokens;

        /// <summary>
        /// Discover all well known tokens on class load.
        /// </summary>
        static WellKnownTokens()
        {
            // reflect to discover all defined well known tokens by class loader
            var type = typeof(WellKnownTokens);
            var list = new List<TokenDef>();
            foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public))
                list.Add((TokenDef)field.GetValue(null));
            _tokens = list;
        }

        /// <summary>
        /// Get all TokenDefs in one list.
        /// </summary>
        /// <returns>A list of well known TokenDef</returns>
        public static IList<TokenDef> All()
        {
            // return new instance of list for immutability
            return new List<TokenDef>(_tokens);
        }

        /// <summary>
        /// Create a TokenMintResolver pre-loaded with well known tokens.
        /// </summary>
        /// <returns>An instance of the TokenMintResolver bootstrapped with the well known tokens.</returns>
        public static TokenMintResolver CreateTokenMintResolver() 
        {
            var resolver = new TokenMintResolver();
            foreach (var token in _tokens)
                resolver.Add(token);
            return resolver;
        }

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
        /// RAY (Raydium)
        /// </summary>
        public static TokenDef Raydium = new TokenDef("4k3Dyjzvzp8eMZWUXbBCjEvwSkkk59S5iCNLY3QrkX6R", "Raydium", "RAY", 6);

        /// <summary>
        /// FIDA (Bonfida)
        /// </summary>
        public static TokenDef Bonfida = new TokenDef("EchesyfXePKdLtoiZSL8pBe8Myagyy8ZRqsACNCFGnvp", "Bonfida", "FIDA", 6);

        /// <summary>
        /// COPE
        /// </summary>
        public static TokenDef Cope = new TokenDef("8HGyAAB1yoM1ttS7pXjHMa3dukTFGQggnFFH3hJZgzQh", "Cope", "COPE", 6);

        /// <summary>
        /// KIN
        /// </summary>
        public static TokenDef Kin = new TokenDef("kinXdEcpDQeHPEuQnqmUgtYykqKGVFq6CeVX5iAHJq6", "KIN", "KIN", 9);

        /// <summary>
        /// TULIP (Tulip/Solfarm)
        /// </summary>
        public static TokenDef Tulip = new TokenDef("TuLipcqtGVXP9XR62wM8WWCm6a9vhLs7T1uoWBk6FDs", "Tulip", "TULIP", 6);

        /// <summary>
        /// Orca
        /// </summary>
        public static TokenDef Orca = new TokenDef("orcaEKTdK7LKz57vaAYr9QeNsVEPfiu6QeMU1kektZE", "Orca", "ORCA", 6);

        /// <summary>
        /// MNGO (Mango Markets)
        /// </summary>
        public static TokenDef Mango = new TokenDef("MangoCzJ36AjZyKwVj3VnYU4GTonjfVEnJmvvWaxLac", "Mango", "MNGO", 6);

        /// <summary>
        /// SAMO (Samoyed Coin) 
        /// </summary>
        public static TokenDef Samoyed = new TokenDef("7xKXtg2CW87d97TXJSDpbD5jBkheTqA83TZRuJosgAsU", "Samoyed Coin", "SAMO", 9);

        /// <summary>
        /// SBR (Saber)
        /// </summary>
        public static TokenDef Saber = new TokenDef("Saber2gLauYim4Mvftnrasomsv6NvAuncvMEZwcLpD1", "Saber", "SBR", 6);

        /// <summary>
        /// FAB (Fabric Protocol)
        /// </summary>
        public static TokenDef Fabric = new TokenDef("EdAhkbj5nF9sRM7XN7ewuW8C9XEUMs8P7cnoQ57SYE96", "Fabric", "FAB", 9);

        /// <summary>
        /// BOP (Boring Protocol)
        /// </summary>
        public static TokenDef Boring = new TokenDef("BLwTnYKqf7u4qjgZrrsKeNs2EzWkMLqVCu6j8iHyrNA3", "Boring Protocol", "BOP", 9);

        /// <summary>
        /// LIQ (Liquid)
        /// </summary>
        public static TokenDef Liquid = new TokenDef("4wjPQJ6PrkC4dHhYghwJzGBVP78DkBzA2U3kHoFNBuhj", "LIQ Protocol", "LIQ", 6);

        /// <summary>
        /// Step
        /// </summary>
        public static TokenDef Step = new TokenDef("StepAscQoEioFxxWGnh2sLBDFp9d8rvKz2Yp39iDpyT", "Step", "STEP", 9);

        /// <summary>
        /// SLRS (Solrise Finance)
        /// </summary>
        public static TokenDef Solrise = new TokenDef("SLRSSpSLUTP7okbCUBYStWCo1vUgyt775faPqz8HUMr", "Solrise Finance", "SLRS", 6);

        /// <summary>
        /// LIKE (Only1)
        /// </summary>
        public static TokenDef Only1 = new TokenDef("3bRTivrVsitbmCTGtqwp7hxXPsybkjn4XLNtPsHqa3zR", "Only1", "LIKE", 9);

        /// <summary>
        /// ATLAS (Star Atlas)
        /// </summary>
        public static TokenDef StarAtlas = new TokenDef("ATLASXmbPQxBUYbxPsV97usA3fPQYEqzQBUHgiFCUsXx", "Star Atlas", "ATLAS", 8);

        /// <summary>
        /// POLIS (Star Atlas DAO)
        /// </summary>
        public static TokenDef StarAtlasDao = new TokenDef("poLisWXnNRwC6oBu1vHiuKQzFjGL4XDSu4g9qjz9qVk", "Star Atlas DAO", "POLIS", 8);

    }
}