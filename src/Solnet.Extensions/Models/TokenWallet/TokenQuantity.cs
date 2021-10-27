using Solnet.Extensions.TokenMint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Extensions
{
    /// <summary>
    /// Represents a token quantity of a known mint with a known number of decimal places.
    /// </summary>
    public class TokenQuantity
    {

        /// <summary>
        /// Constructs a TokenQuantity instance.
        /// </summary>
        /// <param name="tokenDef">A TokenDef instance that describes this token.</param>
        /// <param name="balanceDecimal">Token balance in decimal.</param>
        /// <param name="balanceRaw">Token balance in raw ulong.</param>
        internal TokenQuantity(TokenDef tokenDef,
                               decimal balanceDecimal,
                               ulong balanceRaw)
        {
            TokenDef = tokenDef ?? throw new ArgumentNullException(nameof(tokenDef));
            Symbol = tokenDef.Symbol;
            TokenName = tokenDef.TokenName;
            TokenMint = tokenDef.TokenMint;
            DecimalPlaces = tokenDef.DecimalPlaces;
            QuantityDecimal = balanceDecimal;
            QuantityRaw = balanceRaw;
        }

        /// <summary>
        /// The origin TokenDef instance
        /// </summary>
        public TokenDef TokenDef { get; init; }

        /// <summary>
        /// The token mint public key address.
        /// </summary>
        public string TokenMint { get; init; }

        /// <summary>
        /// The symbol this token uses.
        /// </summary>
        public string Symbol { get; init; }

        /// <summary>
        /// The name of this token.
        /// </summary>
        public string TokenName { get; init; }

        /// <summary>
        /// The number of decimal places this token uses.
        /// </summary>
        public int DecimalPlaces { get; init; }

        /// <summary>
        /// Token balance in decimal.
        /// </summary>
        public decimal QuantityDecimal { get; init; }

        /// <summary>
        /// Token balance in raw ulong.
        /// </summary>
        public ulong QuantityRaw { get; init; }

        /// <summary>
        /// Provide a friendly to read balance with symbol and name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Symbol == TokenName)
                return $"{QuantityDecimal} {Symbol}";
            else
                return $"{QuantityDecimal} {Symbol} ({TokenName})";
        }

        /// <summary>
        /// Add the value of another TokenQuantity to this TokenQuantity.
        /// </summary>
        /// <param name="valueDecimal">Number of tokens as decimal to add to this TokenQuantity.</param>
        /// <param name="valueRaw">Number of tokens as ulong to add to this TokenQuantity.</param>
        /// <returns>A new instance with this TokenQuantity added to the accumulators.</returns>
        internal TokenQuantity AddQuantity(decimal valueDecimal,
                                           ulong valueRaw)
        {

            return new TokenQuantity(this.TokenDef,
                QuantityDecimal + valueDecimal,
                QuantityRaw + valueRaw);

        }

    }

}