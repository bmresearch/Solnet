using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Solnet.Extensions.TokenInfo
{
    public class TokenInfoResolver : ITokenInfoResolver
    {
        private const string TOKENLIST_GITHUB_URL = "https://raw.githubusercontent.com/solana-labs/token-list/main/src/tokens/solana.tokenlist.json";

        private Dictionary<string, SolanaMintToken> _tokens;

        internal TokenInfoResolver(SolanaMintList tokenList)
        {
            _tokens = new Dictionary<string, SolanaMintToken>();
            foreach (var token in tokenList.tokens)
            {
                _tokens[token.Address] = token;
            }
        }

        public static TokenInfoResolver Load()
        {
            return Load(TOKENLIST_GITHUB_URL);
        }

        public static TokenInfoResolver Load(string url)
        {
            using (var wc = new WebClient())
            {
                return ParseTokenList(wc.DownloadString(url));
            }
        }

        public static TokenInfoResolver ParseTokenList(string json)
        {
            if (json is null) throw new ArgumentNullException(nameof(json));
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            var tokenList = JsonSerializer.Deserialize<SolanaMintList>(json, options);
            return new TokenInfoResolver(tokenList);
        }

        public TokenInfo Resolve(string mint)
        {
            if (_tokens.ContainsKey(mint))
            {
                var token = _tokens[mint];
                return new TokenInfo(token.Address, token.Name, token.Symbol);
            }
            else
            {
                return new TokenInfo(mint, $"Unknown {mint}", string.Empty);
            }
        }
    }

    internal class SolanaMintList
    {
        public IList<SolanaMintToken> tokens { get; set; }
    }

    internal class SolanaMintToken
    {
        public string Address { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public int Decimals { get; set; }
        public Dictionary<string,object> Extensions { get; set; }
    }

}
