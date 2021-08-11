using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Extensions.TokenInfo
{
    public class TokenDef
    {
    
        public TokenDef(string mint, string name, string symbol, int decimalPlaces) {
            TokenMint = mint ?? throw new ArgumentNullException(nameof(mint));
            TokenName = name ?? throw new ArgumentNullException(nameof(name));
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            DecimalPlaces = decimalPlaces;
        }

        public string TokenMint { get; init; }

        public string TokenName { get; init; }

        public string Symbol { get; init; }

        public int DecimalPlaces { get; init; }

    }

    internal class TokenListDoc
    {
        public IList<TokenListItem> tokens { get; set; }
    }

    internal class TokenListItem
    {
        public string Address { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public int Decimals { get; set; }
        public Dictionary<string, object> Extensions { get; set; }
    }

}
