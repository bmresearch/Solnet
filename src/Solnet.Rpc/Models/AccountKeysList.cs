using System;
using System.Collections.Generic;

namespace Solnet.Rpc.Models
{
    internal class AccountKeysList
    {
        private readonly Dictionary<string, AccountMeta> _accounts;

        internal IList<AccountMeta> AccountList
        {
            get
            {
                var list = new List<AccountMeta>(_accounts.Values);
                list.Sort(AccountMetaExtensions.Compare);
                return list;
            }
        }

        internal AccountKeysList()
        {
            _accounts = new Dictionary<string, AccountMeta>();
        }

        internal void Add(AccountMeta accountMeta)
        {
            if (_accounts.ContainsKey(accountMeta.GetPublicKey))
            {
                var ok = _accounts.TryGetValue(accountMeta.GetPublicKey, out var account);
                if (!ok) throw new Exception("account meta already exists but could not overwrite");
                if (accountMeta.Writable && !account.Writable)
                {
                    _accounts.Add(accountMeta.GetPublicKey, accountMeta);
                }
            }
            else
            {
                _accounts.Add(accountMeta.GetPublicKey, accountMeta);
            }
        }

        internal void Add(IEnumerable<AccountMeta> accountMetas)
        {
            foreach (var accountMeta in accountMetas)
            {
                Add(accountMeta);
            }
        }
    }
}