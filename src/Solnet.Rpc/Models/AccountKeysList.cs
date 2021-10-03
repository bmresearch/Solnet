using System.Collections.Generic;
using System.Linq;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// A wrapper around a list of <see cref="AccountMeta"/>s that takes care of deduplication and ordering according to 
    /// the wire format specification.
    /// </summary>
    internal class AccountKeysList
    {
        /// <summary>
        /// The account metas list.
        /// </summary>
        private readonly List<AccountMeta> _accounts;

        /// <summary>
        /// Get the accounts as a list.
        /// </summary>
        internal IList<AccountMeta> AccountList
        {
            get
            {
                List<AccountMeta> res = new(_accounts.Count);
                for (int i = 0; i < _accounts.Count; i++)
                {
                    if (_accounts[i].IsSigner)
                    {
                        res.Add(_accounts[i]);
                    }
                }

                for (int i = 0; i < _accounts.Count; i++)
                {
                    if (!_accounts[i].IsSigner && _accounts[i].IsWritable)
                    {
                        res.Add(_accounts[i]);
                    }
                }

                for (int i = 0; i < _accounts.Count; i++)
                {
                    if (!_accounts[i].IsSigner && !_accounts[i].IsWritable)
                    {
                        res.Add(_accounts[i]);
                    }
                }

                return res;
            }
        }

        /// <summary>
        /// Initialize the account keys list for use within transaction building.
        /// </summary>
        internal AccountKeysList()
        {
            _accounts = new List<AccountMeta>();
        }

        /// <summary>
        /// Add an account meta to the list of accounts.
        /// </summary>
        /// <param name="accountMeta">The account meta to add.</param>
        internal void Add(AccountMeta accountMeta)
        {
            AccountMeta accMeta = _accounts.FirstOrDefault(x => x.PublicKey == accountMeta.PublicKey);

            if (accMeta == null)
            {
                _accounts.Add(accountMeta);
            }
            else if (!accMeta.IsSigner && accountMeta.IsSigner || !accMeta.IsWritable && accountMeta.IsWritable)
            {
                var idx = _accounts.IndexOf(accMeta);
                _accounts[idx] = accountMeta;
            }

        }

        /// <summary>
        /// Add a list of account metas to the list of accounts.
        /// </summary>
        /// <param name="accountMetas">The account metas to add.</param>
        internal void Add(IEnumerable<AccountMeta> accountMetas)
        {
            foreach (AccountMeta accountMeta in accountMetas)
            {
                Add(accountMeta);
            }
        }
    }
}