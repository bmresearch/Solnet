using Solnet.Programs.Governance.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// A user's vote.
    /// </summary>
    public class Vote
    {
        /// <summary>
        /// The vote type of the vote.
        /// </summary>
        public VoteType VoteType;

        /// <summary>
        /// The voter choice for a proposal option.
        /// </summary>
        public VoteChoice VoteChoice;
    }
}
