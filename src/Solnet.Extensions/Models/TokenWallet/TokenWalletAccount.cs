using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Extensions
{
    public class TokenWalletAccount : TokenWalletBalance
    {

        public string Address { get; init; }

        public string Owner { get; init; }

        public bool IsAssociatedTokenAccount { get; init; }

        internal TokenWalletAccount(string tokenMint, 
                                    string tokenSymbol, 
                                    string tokenName, 
                                    int decimalPlaces, 
                                    decimal balanceDecimal, 
                                    ulong balanceRaw, 
                                    string address, 
                                    string owner,
                                    bool isAta) : base(tokenMint, tokenSymbol, tokenName, decimalPlaces, balanceDecimal, balanceRaw, 1)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            IsAssociatedTokenAccount = isAta;
        }

        public override string ToString()
        {
            return $"{base.ToString()} {(IsAssociatedTokenAccount?"[ATA]":"")}";
        }

        /// <summary>
        /// Helper method to convert a decimal value to ulong value used when building transaction instructions
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

    }
}
