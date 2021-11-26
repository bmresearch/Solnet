using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// Governance Realm Account
    /// Account PDA seeds" ['governance', name]
    /// </summary>
    public class Realm : GovernanceProgramAccount
    {
        /// <summary>
        /// Community mint
        /// </summary>
        public PublicKey CommunityMint;

        /// <summary>
        /// Configuration of the Realm
        /// </summary>
        public RealmConfig Config;

        /// <summary>
        /// Realm authority. The authority must sign transactions which update the realm config
        /// The authority can be transferer to Realm Governance and hence make the Realm self governed through proposals
        /// </summary>
        public PublicKey Authority;

        /// <summary>
        /// Governance Realm name
        /// </summary>
        public string Name;
    }
}
