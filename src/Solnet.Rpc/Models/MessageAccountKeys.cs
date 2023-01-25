using Solnet.Wallet;
using System.Collections.Generic;
using System.Linq;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// A wrapper around a list of <see cref="AccountMeta"/>s that takes care of deduplication and ordering according to 
    /// the wire format specification.
    /// </summary>
    internal class MessageAccountKeys
    {
        /// <summary>
        /// The static account metas list.
        /// </summary>
        private readonly List<PublicKey> _staticAccounts;

        private AccountKeysFromLookups _accountKeysFromLookups;


        internal List<PublicKey> KeySegments
        {
            get 
            {
                var segments = _staticAccounts.ToList();
                if (_accountKeysFromLookups != null)
                {
                    segments.AddRange(_accountKeysFromLookups.Writables);
                    segments.AddRange(_accountKeysFromLookups.Readonly);
                }

                return segments;
            }
        }

        /// <summary>
        /// Initialize the account keys list for use within transaction building.
        /// </summary>
        internal MessageAccountKeys(List<PublicKey> staticAccounts, AccountKeysFromLookups accountKeysFromLookups = null)
        {
            _staticAccounts = staticAccounts;
            _accountKeysFromLookups = accountKeysFromLookups;
        }

        public PublicKey Get(int index)
        {
            if (index < KeySegments.Count)
            {
                return KeySegments[index];
            }

            return null;
        }       
    }
}