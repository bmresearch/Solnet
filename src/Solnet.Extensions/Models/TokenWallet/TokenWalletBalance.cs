using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Extensions
{
    public class TokenWalletBalance
    {

        public TokenWalletBalance(string mint, 
                                  string symbol, 
                                  string name, 
                                  int decimalPlaces, 
                                  decimal balance,
                                  int accountCount)
        {
            TokenMint = mint;
            Symbol = symbol;
            TokenName = name;
            DecimalPlaces = decimalPlaces;
            Balance = balance;
            AccountCount = accountCount;
        }

        public string TokenMint { get; init; }

        public string Symbol { get; init; }

        public string TokenName { get; init; }

        public int DecimalPlaces { get; init; }

        public decimal Balance { get; init; }

        public int AccountCount { get; init; }

        public override string ToString()
        {
            if (Symbol == TokenName)
                return $"{Balance} {Symbol}";
            else
                return $"{Balance} {Symbol} ({TokenName})";
        }

        internal TokenWalletBalance AddAccount(decimal value, int accountCount) {
            return new TokenWalletBalance(TokenMint, Symbol, TokenName, DecimalPlaces, Balance + value, AccountCount + accountCount);
        }

    }
}
