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
                                  decimal balanceDecimal,
                                  ulong balanceRaw,
                                  ulong lamportsRaw,
                                  int accountCount)
        {
            TokenMint = mint;
            Symbol = symbol;
            TokenName = name;
            DecimalPlaces = decimalPlaces;
            BalanceDecimal = balanceDecimal;
            BalanceRaw = balanceRaw;
            Lamports = lamportsRaw;
            AccountCount = accountCount;
        }

        public string TokenMint { get; init; }

        public string Symbol { get; init; }

        public string TokenName { get; init; }

        public int DecimalPlaces { get; init; }

        public decimal BalanceDecimal { get; init; }

        public ulong BalanceRaw { get; init; }

        public ulong Lamports { get; init; }

        public int AccountCount { get; init; }

        public override string ToString()
        {
            if (Symbol == TokenName)
                return $"{BalanceDecimal} {Symbol}";
            else
                return $"{BalanceDecimal} {Symbol} ({TokenName})";
        }

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
