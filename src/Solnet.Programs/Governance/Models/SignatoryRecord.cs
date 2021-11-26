using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// Account PDA seeds: ['governance', proposal, signatory]
    /// </summary>
    public class SignatoryRecord : GovernanceProgramAccount
    {
        /// <summary>
        /// Proposal the signatory is assigned for
        /// </summary>
        public PublicKey Proposal;

        /// <summary>
        /// The account of the signatory who can sign off the proposal
        /// </summary>
        public PublicKey Signatory;

        /// <summary>
        /// Indicates whether the signatory signed off the proposal
        /// </summary>
        public bool SignedOff;
    }
}
