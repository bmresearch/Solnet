using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Rpc.Models
{
    public class TokenAccountInfo
    {
        public TokenBalance TokenAmount { get; set; }

        public string? Delegate { get; set; }

        public ulong DelegatedAmount { get; set; }

        public string State { get; set; }

        public bool IsNative { get; set; }

        public string Mint { get; set; }

        public string Owner { get; set; }
    }

    public class ParsedTokenAccountData
    {
        public string Type { get; set; }

        public TokenAccountInfo Info { get; set; }
    }

    public class TokenAccountData
    {
        public string Program { get; set; }

        public ParsedTokenAccountData Parsed { get; set; }
    }

    public class LargeAccount : TokenBalance
    {
        public string Address { get; set; }
    }

    public class TokenBalance
    {
        public string Amount { get; set; }

        public int Decimals { get; set; }

        public decimal? UiAmount { get; set; }

        public string UiAmountString { get; set; }
    }
}
