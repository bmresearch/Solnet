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
                                    decimal balance, 
                                    string address, 
                                    string owner,
                                    bool isAta) : base(tokenMint, tokenSymbol, tokenName, decimalPlaces, balance, 1)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            IsAssociatedTokenAccount = isAta;
        }

        public override string ToString()
        {
            return $"{base.ToString()} {(IsAssociatedTokenAccount?"[ATA]":"")}";
        }

    }
}
