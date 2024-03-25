using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.ComputeBudget
{
    internal static class   ComputeBudgetProgramInstructions
    {
        internal static readonly Dictionary<Values, string> Names = new()
        {
            { Values.RequestUnits, "Request Units" },
            { Values.RequestHeapFrame, "Request Heap Frame" },
            { Values.SetComputeUnitLimit, "Set Compute Unit Limit" },
            { Values.SetComputeUnitPrice, "Set Compute Unit Price" },
             
        };

       
        internal enum Values : byte
        {
            
            RequestUnits = 1,

           
            RequestHeapFrame = 2,

            
            SetComputeUnitLimit = 3,

             
            SetComputeUnitPrice = 4,

            
        }
    }
}
