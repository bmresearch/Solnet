using System;
using System.Collections.Generic;

namespace Solnet.Programs.ComputeBudget
{
    /// <summary>
    /// Represents the instruction types for the <see cref="ComputeBudgetProgram"/> along with a friendly name so as not to use reflection.
    /// </summary>
    internal static class ComputeBudgetProgramInstructions
    {
        /// <summary>
        /// Represents the user-friendly names for the instruction types for the <see cref="ComputeBudgetProgram"/>.
        /// </summary>
        internal static readonly Dictionary<Values, string> Names = new()
        {
            { Values.RequestHeapFrame, "Request Heap Frame" },
            { Values.SetComputeUnitLimit, "Set Compute Unit Limit" },
            { Values.SetComputeUnitPrice, "Set Compute Unit Price" },
            { Values.SetLoadedAccountsDataSizeLimit, "Set Loaded Accounts Data Size Limit" },
        };

        /// <summary>
        /// Represents the instruction types for the <see cref="ComputeBudgetProgram"/>.
        /// </summary>
        internal enum Values : uint
        {
            /// <summary>
            /// Unused.
            /// Deprecated variant, reserved value.
            /// </summary>
            [Obsolete]
            Unused = 0,
            
            /// <summary>
            /// Request a heap frame.
            /// </summary>
            RequestHeapFrame = 1,

            /// <summary>
            /// Set compute unit limit.
            /// </summary>
            SetComputeUnitLimit = 2,

            /// <summary>
            /// Set compute unit price.
            /// </summary>
            SetComputeUnitPrice = 3,
            
            /// <summary>
            /// Set loaded accounts data size limit.
            /// </summary>
            SetLoadedAccountsDataSizeLimit = 4
        }
    }
}