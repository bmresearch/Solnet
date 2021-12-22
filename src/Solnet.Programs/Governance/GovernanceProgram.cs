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
        public static readonly PublicKey MainNetProgramIdKey = new("GqTPL6qRf5aUuqscLh8Rg2HTxPUXfhhAXDptTLhp1t2J");

        /// <summary>
        /// 
        /// </summary>
        public static readonly PublicKey MangoGovernanceProgramIdKey = new ("GqTPL6qRf5aUuqscLh8Rg2HTxPUXfhhAXDptTLhp1t2J");

        /// <summary>
        /// 
        /// </summary>
        public static readonly PublicKey SerumGovernanceProgramIdKey = new ("AVoAYTs36yB5izAaBkxRG67wL1AMwG3vo41hKtUSb8is");

        /// <summary>
        /// 
        /// </summary>
        public static readonly PublicKey SoceanGovernanceProgramIdKey = new ("5hAykmD4YGcQ7Am3N7nC9kyELq6CThAkU82nhNKDJiCy");

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
