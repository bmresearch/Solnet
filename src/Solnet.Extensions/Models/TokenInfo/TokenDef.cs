using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Extensions.TokenInfo
{
    /// <summary>
    /// Token Definition object used by the TokenInfoResolver
    /// <para>TokenMint uniquely identifies a token on the Solana blockchain.
    /// Symbol is purley cosmetic and is not sufficient to uniquely identify a token by itself.</para>
    /// </summary>
    public class TokenDef
    {

        /// <summary>
        /// Constructs a TokenDef instance.
        /// </summary>
        /// <param name="mint">The public key of the token mint address.</param>
        /// <param name="name">The display name for this token.</param>
        /// <param name="symbol">The token symbol used to display balances of this token.</param>
        /// <param name="decimalPlaces"></param>
        public TokenDef(string mint, string name, string symbol, int decimalPlaces) {
            TokenMint = mint ?? throw new ArgumentNullException(nameof(mint));
            TokenName = name ?? throw new ArgumentNullException(nameof(name));
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            DecimalPlaces = decimalPlaces;
        }

        /// <summary>
        /// The public key of the token mint address.
        /// </summary>
        public string TokenMint { get; init; }

        /// <summary>
        /// The display name of this token, either user supplied or loaded from tokenlist.json
        /// </summary>
        public string TokenName { get; init; }

        /// <summary>
        /// The symbol to use when displaying balances of this token to a user.
        /// </summary>
        public string Symbol { get; init; }

        /// <summary>
        /// The number of decimal places this token uses.
        /// </summary>
        public int DecimalPlaces { get; init; }

    }

    /// <summary>
    /// Internal class used to deserialize tokenlist.json
    /// </summary>
    internal class TokenListDoc
    {
        public IList<TokenListItem> tokens { get; set; }
    }

    /// <summary>
    /// Internal class used to deserialize tokenlist.json
    /// </summary>
    internal class TokenListItem
    {
        public string Address { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public int Decimals { get; set; }
        public Dictionary<string, object> Extensions { get; set; }
    }

}
