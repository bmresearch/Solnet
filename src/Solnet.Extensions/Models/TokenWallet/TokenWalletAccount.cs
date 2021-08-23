using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Extensions
{
    /// <summary>
    /// A token balance for an individual token account.
    /// </summary>
    public class TokenWalletAccount : TokenWalletBalance
    {
        /// <summary>
        /// The public key of the account.
        /// </summary>
        public string PublicKey { get; init; }

        /// <summary>
        /// The owner public key of the account.
        /// </summary>
        public string Owner { get; init; }

        /// <summary>
        /// A flag to indicate whether this account is an Associated Token Account.
        /// </summary>
        public bool IsAssociatedTokenAccount { get; init; }

        /// <summary>
        /// Construct an instance of the TokenWalletAccount.
        /// </summary>
        /// <param name="tokenMint">The token mint public key address.</param>
        /// <param name="tokenSymbol">The symbol this token uses.</param>
        /// <param name="tokenName">The name of this token.</param>
        /// <param name="decimalPlaces">The number of decimal places this token uses.</param>
        /// <param name="balanceDecimal">Token balance in decimal.</param>
        /// <param name="balanceRaw">Token balance in raw ulong.</param>
        /// <param name="lamportsRaw">How many lamports does this balance represent.</param>
        /// <param name="publicKey">The public key of the account.</param>
        /// <param name="owner">The owner public key of the account.</param>
        /// <param name="isAta">A flag to indicate whether this account is an Associated Token Account.</param>
        internal TokenWalletAccount(string tokenMint, 
                                    string tokenSymbol, 
                                    string tokenName, 
                                    int decimalPlaces, 
                                    decimal balanceDecimal, 
                                    ulong balanceRaw,
                                    ulong lamportsRaw, 
                                    string publicKey, 
                                    string owner,
                                    bool isAta) : base(tokenMint, tokenSymbol, tokenName, decimalPlaces, balanceDecimal, balanceRaw, lamportsRaw, 1)
        {
            PublicKey = publicKey ?? throw new ArgumentNullException(nameof(publicKey));
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            IsAssociatedTokenAccount = isAta;
        }

        /// <summary>
        /// Provide a friendly to read balance with symbol and name and an ATA indicator.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{base.ToString()} {(IsAssociatedTokenAccount?"[ATA]":"")}";
        }

    }
}
