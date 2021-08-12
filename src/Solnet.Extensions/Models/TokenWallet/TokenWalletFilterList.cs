using Solnet.Extensions.TokenInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Extensions.Models
{
    public class TokenWalletFilterList : IEnumerable<TokenWalletAccount>
    {

        private IList<TokenWalletAccount> _list;

        public TokenWalletFilterList(IEnumerable<TokenWalletAccount> accounts)
        {
            _list = new List<TokenWalletAccount>(accounts);
        }

        public IEnumerator<TokenWalletAccount> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public TokenWalletFilterList ForToken(TokenDef token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            return new TokenWalletFilterList(_list.Where(x => x.TokenMint == token.TokenMint));
        }

        public TokenWalletFilterList WithSymbol(string symbol)
        {
            return new TokenWalletFilterList(_list.Where(x => x.Symbol == symbol));
        }

        public TokenWalletFilterList WithMint(string mint)
        {
            return new TokenWalletFilterList(_list.Where(x => x.TokenMint == mint));
        }

        public TokenWalletFilterList WithAtLeast(decimal minimumBalance)
        {
            return new TokenWalletFilterList(_list.Where(x => x.BalanceDecimal >= minimumBalance));
        }

        public TokenWalletFilterList WithAtLeast(ulong minimumBalance)
        {
            return new TokenWalletFilterList(_list.Where(x => x.BalanceRaw == minimumBalance));
        }

        public TokenWalletFilterList WhichAreAssociatedTokenAccounts()
        {
            return new TokenWalletFilterList(_list.Where(x => x.IsAssociatedTokenAccount));
        }

        /// <summary>
        /// Return the first associated account in the list if there is one or nulll
        /// </summary>
        /// <returns></returns>
        public TokenWalletAccount AssociatedTokenAccount()
        {
            var list = new TokenWalletFilterList(_list.Where(x => x.IsAssociatedTokenAccount));
            if (list.Count() == 1)
                return list.First();
            else
                return null;
        }



    }
}
