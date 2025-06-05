using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.StakePool.Models
{
    /// <summary>  
    /// Represents the type of funding operation in the stake pool.  
    /// </summary>  
    public enum FundingType
    {
        /// <summary>  
        /// A deposit of a stake account.  
        /// </summary>  
        StakeDeposit = 0,

        /// <summary>  
        /// A deposit of SOL tokens.  
        /// </summary>  
        SolDeposit = 1,

        /// <summary>  
        /// A withdrawal of SOL tokens.  
        /// </summary>  
        SolWithdraw = 2,
    }
}
