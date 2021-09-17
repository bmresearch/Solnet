﻿using System;

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
        /// <param name="tokenMint">The token mint public key address.</param>
        /// <param name="tokenSymbol">The symbol this token uses.</param>
        /// <param name="tokenName">The name of this token.</param>
        /// <param name="coinGeckoId">The token's CoinGecko ID.</param>
        /// <param name="decimalPlaces">The number of decimal places this token uses.</param>
        /// <param name="balanceDecimal">Token balance in decimal.</param>
        /// <param name="balanceRaw">Token balance in raw ulong.</param>
        internal TokenQuantity(string tokenMint, string tokenSymbol, string tokenName, string coinGeckoId,
            int decimalPlaces, decimal balanceDecimal, ulong balanceRaw)
        {
            TokenMint = tokenMint ?? throw new ArgumentNullException(nameof(tokenMint));
            Symbol = tokenSymbol ?? throw new ArgumentNullException(nameof(tokenSymbol));
            TokenName = tokenName ?? throw new ArgumentNullException(nameof(tokenName));
            CoinGeckoId = coinGeckoId;
            DecimalPlaces = decimalPlaces;
            QuantityDecimal = balanceDecimal;
            QuantityRaw = balanceRaw;
        }

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
        /// The token's CoinGecko ID.
        /// </summary>
        public string CoinGeckoId { get; init; }

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
            return Symbol == TokenName ? $"{QuantityDecimal} {Symbol}" : $"{QuantityDecimal} {Symbol} ({TokenName})";
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
            return new TokenQuantity(TokenMint, Symbol, TokenName, CoinGeckoId,
                DecimalPlaces, QuantityDecimal + valueDecimal,
                QuantityRaw + valueRaw);
        }
    }
}