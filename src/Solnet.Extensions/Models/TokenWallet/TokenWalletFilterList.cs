using Solnet.Extensions.TokenMint;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Extensions.Models
{
    /// <summary>
    /// A filterable list of TokenWalletAccounts.
    /// <para>Use the filter methods to select the accounts that you want to interact with.</para>
    /// <para>To filter a subset of accounts, see ForToken, WithSymbol, WithMint, WithAtLeast, WhichAreAssociatedTokenAccounts, </para>
    /// <para>To select individual accounts, see: WithPublicKey, AssociatedTokenAccount</para>
    /// </summary>
    public class TokenWalletFilterList : IEnumerable<TokenWalletAccount>
    {
        /// <summary>
        /// Private list storage.
        /// </summary>
        private IList<TokenWalletAccount> _list;

        /// <summary>
        /// Constructs an instance of TokenWalletFilterList with a list of accounts.
        /// </summary>
        /// <param name="accounts">Some accounts to add to the list.</param>
        public TokenWalletFilterList(IEnumerable<TokenWalletAccount> accounts)
        {
            _list = new List<TokenWalletAccount>(accounts ?? throw new ArgumentNullException(nameof(accounts)));
        }

        /// <summary>
        /// Get an enumerator for this list.
        /// </summary>
        /// <returns>An enumerator for this list.</returns>
        public IEnumerator<TokenWalletAccount> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Get an enumerator for this list.
        /// </summary>
        /// <returns>An enumerator for this list.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Keeps all accounts that match the TokenDef provided.
        /// </summary>
        /// <param name="token">An instance of TokenDef to use for filtering.</param>
        /// <returns>A filtered list of accounts that match the supplied TokenDef.</returns>
        public TokenWalletFilterList ForToken(TokenDef token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            return new TokenWalletFilterList(_list.Where(x => x.TokenMint == token.TokenMint));
        }

        /// <summary>
        /// Keeps all accounts with the token symbol supplied.
        /// <para>Be aware that token symbol does not guarentee you are interacting with the TokenMint you think. 
        /// It is much safer to identify tokens using their token mint public key address.</para>
        /// </summary>
        /// <param name="symbol">A token symbol, e.g. USDC</param>
        /// <returns>A filtered list of accounts for the given token symbol.</returns>
        public TokenWalletFilterList WithSymbol(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol)) throw new ArgumentException(nameof(symbol));
            return new TokenWalletFilterList(_list.Where(x => x.Symbol == symbol));
        }

        /// <summary>
        /// Get the TokenWalletAccount for the public key provided.
        /// </summary>
        /// <param name="publicKey">Public key for the account</param>
        /// <returns>The account with the matching public key or null if not found.</returns>
        public TokenWalletAccount WithPublicKey(string publicKey)
        {
            if (string.IsNullOrWhiteSpace(publicKey)) throw new ArgumentException(nameof(publicKey));
            return new TokenWalletFilterList(_list.Where(x => x.PublicKey == publicKey)).FirstOrDefault();
        }

        /// <summary>
        /// Keeps all accounts for the given token mint address.
        /// </summary>
        /// <param name="mint">Token mint public key address.</param>
        /// <returns>A filtered list of accounts for the given mint.</returns>
        public TokenWalletFilterList WithMint(string mint)
        {
            return new TokenWalletFilterList(_list.Where(x => x.TokenMint == mint));
        }

        /// <summary>
        /// Keeps all accounts with at least the supplied minimum balance.
        /// </summary>
        /// <param name="minimumBalance">A minimum balance value as decimal.</param>
        /// <returns>A filtered list of accounts with at least the balance as decimal supplied.</returns>
        public TokenWalletFilterList WithAtLeast(decimal minimumBalance)
        {
            return new TokenWalletFilterList(_list.Where(x => x.QuantityDecimal >= minimumBalance));
        }

        /// <summary>
        /// Keeps all accounts with at least the supplied minimum balance.
        /// </summary>
        /// <param name="minimumBalance">A minimum balance value as ulong.</param>
        /// <returns>A filtered list of accounts with at least the balance as raw ulong supplied.</returns>
        public TokenWalletFilterList WithAtLeast(ulong minimumBalance)
        {
            return new TokenWalletFilterList(_list.Where(x => x.QuantityRaw == minimumBalance));
        }

        /// <summary>
        /// Keeps all accounts with a non-zero balance.
        /// </summary>
        /// <returns>A filtered list of accounts with at least the balance as raw ulong supplied.</returns>
        public TokenWalletFilterList WithNonZero()
        {
            return new TokenWalletFilterList(_list.Where(x => x.QuantityRaw > 0));
        }

        /// <summary>
        /// Keeps all Associated Token Account instances in the list.
        /// </summary>
        /// <returns>A filtered list that only contains Associated Token Accounts.</returns>
        public TokenWalletFilterList WhichAreAssociatedTokenAccounts()
        {
            return new TokenWalletFilterList(_list.Where(x => x.IsAssociatedTokenAccount));
        }

        /// <summary>
        /// Return the first associated account found in the list or null.
        /// <para>Typically this would be used immediately after a WithMint 
        /// for ForToken filter to identify the Associated Token Account for that token.</para>
        /// </summary>
        /// <returns>The first matching Assocated Token Accounts in the list or null if none were found.</returns>
        public TokenWalletAccount AssociatedTokenAccount()
        {
            var list = WhichAreAssociatedTokenAccounts();
            if (list.Count() >= 1)
                return list.First();
            else
                return null;
        }

        /// <summary>
        /// Keeps all instances that satisfy the filter provided.
        /// </summary>
        /// <param name="filter">The filter to use.</param>
        /// <returns>A filtered list that only contains matching.</returns>
        public TokenWalletFilterList WithCustomFilter(Predicate<TokenWalletAccount> filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            return new TokenWalletFilterList(_list.Where(x => filter.Invoke(x)));
        }

    }
}