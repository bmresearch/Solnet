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
        internal List<AccountMeta> AccountList
        {
            get
            {
                List<AccountMeta> res = _accounts.Select(acc => acc).ToList();

                res.Sort((x, y) =>
                {
                    if (x.IsSigner != y.IsSigner)
                    {
                        // Signers always come before non-signers
                        return x.IsSigner ? -1 : 1;
                    }
                    if (x.IsWritable != y.IsWritable)
                    {
                        // Writable accounts always come before read-only accounts
                        return x.IsWritable ? -1 : 1;
                    }
                    // Otherwise, sort by pubkey, stringwise.
                    return x.PublicKey.CompareTo(y.PublicKey);
                });

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
            else if (!accMeta.IsSigner && accountMeta.IsSigner)
            {
                accMeta.IsSigner = true;
                accMeta.IsWritable = accMeta.IsWritable || accountMeta.IsWritable;
            }
            else if(!accMeta.IsWritable && accountMeta.IsWritable)
            {
                accMeta.IsWritable = true;
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