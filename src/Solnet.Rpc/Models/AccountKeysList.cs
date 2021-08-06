using System;
using System.Collections.Generic;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// A wrapper around a dictionary of key-value pairs of public-keys - account metas to be used
    /// during transaction building. It checks for differences in account meta when adding to the dictionary
    /// and sorts the underlying list of values in the dictionary according to <see cref="AccountMeta.CompareTo"/>
    /// so they are ordered and compatible with Solana's transaction anatomy.
    /// </summary>
    internal class AccountKeysList
    {
        /// <summary>
        /// The dictionary with key-value pairs of public keys - account metas.
        /// </summary>
        private readonly Dictionary<string, AccountMeta> _accounts;

        /// <summary>
        /// Get the values of the accounts dictionary as a list.
        /// </summary>
        internal IList<AccountMeta> AccountList
        {
            get
            {
                List<AccountMeta> list = new (_accounts.Values);
                list.Sort();
                return list;
            }
        }

        /// <summary>
        /// Initialize the account keys list for use within transaction building.
        /// </summary>
        internal AccountKeysList()
        {
            _accounts = new Dictionary<string, AccountMeta>();
        }

        /// <summary>
        /// Add a list of account metas to the dictionary with key-value pairs of public keys - account metas.
        /// </summary>
        /// <param name="accountMeta">The account meta to add.</param>
        /// <exception cref="Exception">Throws exception when account meta is already present and couldn't overwrite.</exception>
        internal void Add(AccountMeta accountMeta)
        {
            if (_accounts.ContainsKey(accountMeta.PublicKey))
            {
                bool ok = _accounts.TryGetValue(accountMeta.PublicKey, out AccountMeta account);
                if (!ok) throw new Exception("account meta already exists but could not overwrite");
                if ((!accountMeta.IsWritable || account.IsWritable) && (!accountMeta.IsSigner || account.IsSigner))
                {
                    return;
                }

                _accounts.Remove(account.PublicKey);
                _accounts.Add(accountMeta.PublicKey, accountMeta);

            }
            else
            {
                _accounts.Add(accountMeta.PublicKey, accountMeta);
            }
        }

        /// <summary>
        /// Add a list of account metas to the dictionary with key-value pairs of public keys - account metas.
        /// </summary>
        /// <param name="accountMetas">The account meta to add.</param>
        internal void Add(IEnumerable<AccountMeta> accountMetas)
        {
            foreach (AccountMeta accountMeta in accountMetas)
            {
                Add(accountMeta);
            }
        }
    }
}