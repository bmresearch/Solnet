using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Extensions.TokenMint
{
    /// <summary>
    /// Token Definition object used by the TokenMintResolver
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
        /// <param name="coinGeckoId">The token's CoinGecko ID.</param>
        /// <param name="decimalPlaces"></param>
        public TokenDef(string mint, string name, string symbol, int decimalPlaces, string coinGeckoId)
        {
            TokenMint = mint ?? throw new ArgumentNullException(nameof(mint));
            TokenName = name ?? throw new ArgumentNullException(nameof(name));
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            DecimalPlaces = decimalPlaces;
            CoinGeckoId = coinGeckoId;
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
        /// The token's coin gecko id.
        /// </summary>
        public string CoinGeckoId { get; init; }

        /// <summary>
        /// The number of decimal places this token uses.
        /// </summary>
        public int DecimalPlaces { get; init; }

        /// <summary>
        /// Create an instance of the TokenQuantity object with the raw token quanity value provided.
        /// </summary>
        /// <param name="valueDecimal">Value as decimal.</param>
        /// <param name="valueRaw">Value as ulong.</param>
        /// <returns>A TokenQuantity instance.</returns>
        public TokenQuantity CreateQuantity(decimal valueDecimal, ulong valueRaw)
        {
            return new TokenQuantity(TokenMint, Symbol, TokenName, CoinGeckoId, DecimalPlaces, valueDecimal, valueRaw);
        }

        /// <summary>
        /// Create an instance of the TokenQuantity object with the raw token quanity value provided.
        /// </summary>
        /// <param name="value">Value as ulong.</param>
        /// <returns>A TokenQuantity instance.</returns>
        public TokenQuantity CreateQuantityWithRaw(ulong value)
        {
            return CreateQuantity(ConvertUlongToDecimal(value), value);
        }

        /// <summary>
        /// Create an instance of the TokenQuantity object with the decimal token quanity value provided.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>A TokenQuantity instance.</returns>
        public TokenQuantity CreateQuantityWithDecimal(decimal value)
        {
            return CreateQuantity(value, ConvertDecimalToUlong(value));
        }

        /// <summary>
        /// Helper method to convert a decimal value to ulong value for this token's number of decimal places.
        /// based on the number of decimal places
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ulong ConvertDecimalToUlong(decimal value)
        {
            if (DecimalPlaces < 0) throw new ApplicationException($"DecimalPlaces is unknown for mint {TokenMint}");
            decimal impliedAmount = value;
            for (int ix = 0; ix < DecimalPlaces; ix++) impliedAmount = decimal.Multiply(impliedAmount, 10);
            ulong raw = Convert.ToUInt64(decimal.Floor(impliedAmount));
            return raw;
        }

        /// <summary>
        /// Helper method to convert a raw ulong to decimal value for this token's number of decimal places.
        /// based on the number of decimal places
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public decimal ConvertUlongToDecimal(ulong value)
        {
            if (DecimalPlaces < 0) throw new ApplicationException($"DecimalPlaces is unknown for mint {TokenMint}");
            decimal impliedAmount = value;
            for (int ix = 0; ix < DecimalPlaces; ix++) impliedAmount = decimal.Divide(impliedAmount, 10);
            return impliedAmount;
        }

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