using Solnet.Extensions.TokenMint;
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
    public class TokenWalletBalance : TokenQuantity
    {

        /// <summary>
        /// Constructs a TokenWalletBalance instance.
        /// </summary>
        /// <param name="tokenDef">A TokenDef instance that describes this token.</param>
        /// <param name="balanceDecimal">Token balance in decimal.</param>
        /// <param name="balanceRaw">Token balance in raw ulong.</param>
        /// <param name="lamportsRaw">How many lamports does this balance represent.</param>
        /// <param name="accountCount">The number of accounts this balance represents. Start with 1.</param>
        internal TokenWalletBalance(TokenDef tokenDef,
                                    decimal balanceDecimal,
                                    ulong balanceRaw,
                                    ulong lamportsRaw,
                                    int accountCount) : base(tokenDef,
                                                             balanceDecimal,
                                                             balanceRaw)
        {
            Lamports = lamportsRaw;
            AccountCount = accountCount;
        }

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
                return $"{QuantityDecimal} {Symbol}";
            else
                return $"{QuantityDecimal} {Symbol} ({TokenName})";
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
                                               int accountCount)
        {

            return new TokenWalletBalance(
                this.TokenDef,
                QuantityDecimal + valueDecimal,
                QuantityRaw + valueRaw, Lamports + lamportsRaw,
                AccountCount + accountCount);

        }

    }

}