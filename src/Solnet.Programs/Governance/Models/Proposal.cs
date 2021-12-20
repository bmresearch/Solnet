using Solnet.Programs.Governance.Enums;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Proposal : GovernanceProgramAccount
    {
        /// <summary>
        /// 
        /// </summary>
        public static class ExtraLayout
        {

        }

        public PublicKey Governance;

        public PublicKey GoverningTokenMint;

        public ProposalState State;

        public PublicKey TokenOwnerRecord;

        public byte SignatoriesCount;

        public byte SignatoriesSignedOffCount;

        public VoteType VoteType;

        public List<ProposalOption> Options;
    }
}
