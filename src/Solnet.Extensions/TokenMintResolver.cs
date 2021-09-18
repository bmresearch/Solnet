using Solnet.Extensions.TokenMint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Solnet.Extensions
{
    /// <summary>
    /// The default implementation of the TokenMintResolver.
    /// <para>You can create your own by implementing ITokenMintResolver.</para>
    /// <para>You can use the Load method to load the Solana ecosystem's standard token list or 
    /// populate your own instance with TokenDef objects.</para>
    /// </summary>
    public class TokenMintResolver : ITokenMintResolver
    {

        /// <summary>
        /// The URL of the standard token list
        /// </summary>
        private const string TOKENLIST_GITHUB_URL = "https://raw.githubusercontent.com/solana-labs/token-list/main/src/tokens/solana.tokenlist.json";

        /// <summary>
        /// Internal lookfor for resolving mint public key addresses to TokenDef objects.
        /// </summary>
        private Dictionary<string, TokenDef> _tokens;

        /// <summary>
        /// Constructs an empty TokenMintResolver object.
        /// </summary>
        public TokenMintResolver()
        {
            _tokens = new Dictionary<string, TokenDef>();
        }

        /// <summary>
        /// Constructs an empty TokenMintResolver and populates with deserialized TokenListDoc.
        /// </summary>
        /// <param name="tokenList">A deserialised token list.</param>
        internal TokenMintResolver(TokenListDoc tokenList) : this()
        {
            foreach (var token in tokenList.tokens)
            {
                Add(token);
            }
        }

        /// <summary>
        /// Return an instance of the TokenMintResolver loaded with the Solana token list.
        /// </summary>
        /// <returns>An instance of the TokenMintResolver populated with Solana token list definitions.</returns>
        public static TokenMintResolver Load()
        {
            return LoadAsync().Result;
        }

        /// <summary>
        /// Return an instance of the TokenMintResolver loaded dererialised token list JSON from the specified URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>An instance of the TokenMintResolver populated with Solana token list definitions.</returns>
        public static TokenMintResolver Load(string url)
        {
            return LoadAsync(url).Result;
        }

        /// <summary>
        /// Return an instance of the TokenMintResolver loaded with the Solana token list.
        /// </summary>
        /// <returns>A task that will result in an instance of the TokenMintResolver populated with Solana token list definitions.</returns>
        public static async Task<TokenMintResolver> LoadAsync()
        {
            return await LoadAsync(TOKENLIST_GITHUB_URL);
        }

        /// <summary>
        /// Return an instance of the TokenMintResolver loaded with the Solana token list.
        /// </summary>
        /// <returns>A task that will result in an instance of the TokenMintResolver populated with Solana token list definitions.</returns>
        public static async Task<TokenMintResolver> LoadAsync(string url)
        {
            using (var wc = new WebClient())
            {
                var json = await wc.DownloadStringTaskAsync(url);
                return ParseTokenList(json);
            }
        }

        /// <summary>
        /// Return an instance of the TokenMintResolver loaded with the dererialised JSON string supplied.
        /// </summary>
        /// <param name="json">The JSON to parse - should be shaped the same as the Solana token list.</param>
        /// <returns>An instance of the TokenMintResolver populated with the deserialized JSON provided.</returns>
        public static TokenMintResolver ParseTokenList(string json)
        {
            if (json is null) throw new ArgumentNullException(nameof(json));
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            var tokenList = JsonSerializer.Deserialize<TokenListDoc>(json, options);
            return new TokenMintResolver(tokenList);
        }

        /// <summary>
        /// Resolve a token mint public key address into the token def.
        /// <para>
        /// If a token is not known, a default Unknown TokenDef instance with be created for that mint and stashed for any future lookups.
        /// </para>
        /// <para>
        /// Unknown tokens will have decimal places of -1 by design. 
        /// This will prevent their use when converting decimal balance values into lamports for TransactionBuilder.
        /// It is unlikely this scenario will be encountered often as Unknown token are encounted by the TokenWallet Load method
        /// when processing TokenAccountInfo RPC results that do contain the decimal places.
        /// </para>
        /// </summary>
        /// <param name="tokenMint"></param>
        /// <returns></returns>
        public TokenDef Resolve(string tokenMint)
        {
            if (tokenMint == null) throw new ArgumentNullException(nameof(tokenMint));
            if (_tokens.ContainsKey(tokenMint))
            {
                return _tokens[tokenMint];
            }
            else
            {
                var unknown = new TokenDef(tokenMint, $"Unknown {tokenMint}", string.Empty, -1);
                _tokens[tokenMint] = unknown;
                return unknown;
            }
        }

        /// <summary>
        /// Add a token to the TokenMintResolver lookup.
        /// Any collisions on token mint will replace the previous instance.
        /// </summary>
        /// <param name="token">An instance of TokenDef to be added.</param>
        public void Add(TokenDef token)
        {
            if (token is null) throw new ArgumentNullException(nameof(token));
            _tokens[token.TokenMint] = token;
        }

        /// <summary>
        /// Construct a TokenDef instance populated with extension goodies from the Solana token list.
        /// </summary>
        /// <param name="tokenItem">A TokenListItem instance.</param>
        internal void Add(TokenListItem tokenItem)
        {
            if (tokenItem is null) throw new ArgumentNullException(nameof(tokenItem));

            // pick out the token logo or null
            string logoUrl = tokenItem.LogoUri;

            // pick out the coingecko identifier if available
            string coingeckoId = null;
            if (tokenItem.Extensions.ContainsKey("coingeckoId")) coingeckoId = ((JsonElement) tokenItem.Extensions["coingeckoId"]).GetString();

            // pick out the project website if available
            string projectUrl = null;
            if (tokenItem.Extensions.ContainsKey("website")) projectUrl = ((JsonElement)tokenItem.Extensions["website"]).GetString();

            // construct the TokenDef instance
            var token = new TokenDef(tokenItem.Address, tokenItem.Name, tokenItem.Symbol, tokenItem.Decimals)
            {
                CoinGeckoId = coingeckoId,
                TokenLogoUrl = logoUrl,
                TokenProjectUrl = projectUrl
            };

            // stash it
            _tokens[token.TokenMint] = token;
        }


    }

}