using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Extensions.TokenInfo
{
    public class TokenInfo
    {
    
        public TokenInfo(string mint, string name, string symbol) {
            TokenMint = mint ?? throw new ArgumentNullException(nameof(mint));
            TokenName = name ?? throw new ArgumentNullException(nameof(name));
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        }

        public string TokenMint { get; init; }

        public string TokenName { get; init; }

        public string Symbol { get; init; }

    }
}
