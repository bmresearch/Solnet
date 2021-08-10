using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Solnet.Extensions.TokenInfo;

namespace Solnet.Extensions
{
    public class TokenInfoResolver : ITokenInfoResolver
    {
        private const string TOKENLIST_GITHUB_URL = "https://raw.githubusercontent.com/solana-labs/token-list/main/src/tokens/solana.tokenlist.json";

        private Dictionary<string, TokenListItem> _tokens;

        internal TokenInfoResolver(TokenListDoc tokenList)
        {
            _tokens = new Dictionary<string, TokenListItem>();
            foreach (var token in tokenList.tokens)
            {
                _tokens[token.Address] = token;
            }
        }

        public static TokenInfoResolver Load()
        {
            return LoadAsync().Result;
        }

        public static TokenInfoResolver Load(string url)
        {
            return LoadAsync(url).Result;
        }

        public static async Task<TokenInfoResolver> LoadAsync()
        {
            return await LoadAsync(TOKENLIST_GITHUB_URL);
        }

        public static async Task<TokenInfoResolver> LoadAsync(string url)
        {
            using (var wc = new WebClient())
            {
                var json = await wc.DownloadStringTaskAsync(url);
                return ParseTokenList(json);
            }
        }

        public static TokenInfoResolver ParseTokenList(string json)
        {
            if (json is null) throw new ArgumentNullException(nameof(json));
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            var tokenList = JsonSerializer.Deserialize<TokenListDoc>(json, options);
            return new TokenInfoResolver(tokenList);
        }

        public TokenDef Resolve(string mint)
        {
            if (_tokens.ContainsKey(mint))
            {
                var token = _tokens[mint];
                return new TokenDef(token.Address, token.Name, token.Symbol);
            }
            else
            {
                return new TokenDef(mint, $"Unknown {mint}", string.Empty);
            }
        }
    }

}
