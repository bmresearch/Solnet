using Solnet.Programs.Abstract;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Governance
{
    /// <summary>
    /// 
    /// </summary>
    public class GovernanceProgram : BaseProgram
    {

        /// <summary>
        /// The program's name.
        /// </summary>
        public const string GovernanceProgramName = "Governance Program";
        
        /// <summary>
        /// The program's public key.
        /// </summary>
        public static readonly PublicKey MainNetProgramIdKey = new("GovER5Lthms3bLBqWub97yVrMmEogzX7xNjdXpPPCVZw");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey"></param>
        /// <param name="programName"></param>
        public GovernanceProgram(PublicKey programIdKey, string programName = GovernanceProgramName) : base(programIdKey, programName)
        {
        }
    }
}
