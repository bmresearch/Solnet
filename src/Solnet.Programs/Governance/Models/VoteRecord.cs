using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// Proposal VoteRecord
    /// </summary>
    public class VoteRecord : GovernanceProgramAccount
    {
        /// <summary>
        /// Proposal the signatory is assigned for.
        /// </summary>
        public PublicKey Proposal;

        /// <summary>
        /// The user who casted this vote.
        /// This is the Governing Token Owner who deposited governing tokens into the Realm.
        /// </summary>
        public PublicKey GoverningTokenOwner;

        /// <summary>
        /// Indicates whether the vote was relinquished by voter.
        /// </summary>
        public bool IsRelinquished;

        /// <summary>
        /// The weight of the user casting the vote.
        /// </summary>
        public ulong VoterWeight;

        /// <summary>
        /// Voter's vote.
        /// </summary>
        public Vote Vote;
    }
}
