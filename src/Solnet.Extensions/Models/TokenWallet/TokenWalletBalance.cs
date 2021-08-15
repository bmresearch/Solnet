using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Extensions
{
    /// <summary>
    /// A consolidated token balance for a number of accounts of a given mint.
    /// </summary>
    public class TokenWalletBalance
    {

        /// <summary>
        /// Constructs a token balance instance.
        /// </summary>
        /// <param name="tokenMint">The token mint public key address.</param>
        /// <param name="tokenSymbol">The symbol this token uses.</param>
        /// <param name="tokenName">The name of this token.</param>
        /// <param name="decimalPlaces">The number of decimal places this token uses.</param>
        /// <param name="balanceDecimal">Token balance in decimal.</param>
        /// <param name="balanceRaw">Token balance in raw ulong.</param>
        /// <param name="lamportsRaw">How many lamports does this balance represent.</param>
        /// <param name="accountCount">The number of accounts this balance represents. Start with 1.</param>
        internal TokenWalletBalance(string tokenMint, 
                                    string tokenSymbol, 
                                    string tokenName, 
                                    int decimalPlaces, 
                                    decimal balanceDecimal,
                                    ulong balanceRaw,
                                    ulong lamportsRaw,
                                    int accountCount)
        {
            TokenMint = tokenMint;
            Symbol = tokenSymbol;
            TokenName = tokenName;
            DecimalPlaces = decimalPlaces;
            BalanceDecimal = balanceDecimal;
            BalanceRaw = balanceRaw;
            Lamports = lamportsRaw;
            AccountCount = accountCount;
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
        /// The number of decimal places this token uses.
        /// </summary>
        public int DecimalPlaces { get; init; }

        /// <summary>
        /// Token balance in decimal.
        /// </summary>
        public decimal BalanceDecimal { get; init; }

        /// <summary>
        /// Token balance in raw ulong.
        /// </summary>
        public ulong BalanceRaw { get; init; }

        /// <summary>
        /// How many lamports does this balance represent.
        /// </summary>
        public ulong Lamports { get; init; }

        /// <summary>
        /// The number of accounts this balance represents. 
        /// </summary>
        public int AccountCount { get; init; }

        /// <summary>
        /// Provide a friendly to read balance with symbol and name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Symbol == TokenName)
                return $"{BalanceDecimal} {Symbol}";
            else
                return $"{BalanceDecimal} {Symbol} ({TokenName})";
        }

        /// <summary>
        /// Add the value of an account to this consolidated balance.
        /// </summary>
        /// <param name="valueDecimal">Number of tokens as decimal to add to this consolidated balance.</param>
        /// <param name="valueRaw">Number of tokens as ulong to add to this consolidated balance.</param>
        /// <param name="lamportsRaw">Number of lamports to add to this consolidated balance.</param>
        /// <param name="accountCount">Number of accounts to add to this consolidated balance.</param>
        /// <returns>A new instance with this account provdided added to the accumulators.</returns>
        internal TokenWalletBalance AddAccount(decimal valueDecimal, 
                                               ulong valueRaw,
                                               ulong lamportsRaw, 
                                               int accountCount) {

            return new TokenWalletBalance(TokenMint, Symbol, TokenName, 
                DecimalPlaces, BalanceDecimal + valueDecimal, 
                BalanceRaw + valueRaw, Lamports + lamportsRaw, 
                AccountCount + accountCount);

        }

    }

}
