using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs
{
    /// <summary>
    /// Represents the instruction types for the <see cref="StakeProgram"/> ??? along with a friendly name so as not to use reflection ???.
    /// <remarks>
    /// For more information see:
    /// https://docs.solana.com/developing/runtime-facilities/programs#stake-program
    /// https://docs.rs/solana-sdk/1.7.14/solana_sdk/stake/instruction/enum.StakeInstruction.html
    /// </remarks>
    /// </summary>
    internal static class StakeProgramInstructions
    {
        /// <summary>
        /// Represents the user-friendly names for the instruction types for the <see cref="StakeProgram"/>.
        /// </summary>
        internal static readonly Dictionary<Values, string> Names = new()
        {
            { Values.Initialize, "Initialize" },
            { Values.Authorize, "Authorize" },
            { Values.DelegateStake, "Delegate Stake" },
            { Values.Split, "Split" },
            { Values.Withdraw, "Withdraw" },
            { Values.Deactivate, "Deactivate" },
            { Values.SetLockup, "Set Lockup" },
            { Values.Merge, "Merge" },
            { Values.AuthorizeWithSeed, "Authorize With Seed" },
            { Values.InitializeChecked, "Initialize Checked" },
            { Values.AuthorizeChecked, "Authorize Checked" },
            { Values.AuthorizeCheckedWithSeed, "Authorize Checked With Seed" },
            { Values.SetLockupChecked, "Set Lockup Checked"}
        };

        /// <summary>
        /// Represents the instruction types for the <see cref="StakeProgram"/>.
        /// </summary>
        internal enum Values : byte
        {
            Initialize = 0,
            Authorize = 1,
            DelegateStake =2,
            Split = 3,
            Withdraw = 4,
            Deactivate = 5,
            SetLockup = 6,
            Merge = 7,
            AuthorizeWithSeed = 8,
            InitializeChecked = 9,
            AuthorizeChecked = 10,
            AuthorizeCheckedWithSeed = 11,
            SetLockupChecked = 12
        }
    }
}
