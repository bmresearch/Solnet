using Solnet.Programs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.ComputeBudget
{
    internal static class ComputeBudgetProgramData
    {
        internal static byte[] RequestUnits(byte method, ulong units, ulong additionalFee)
        {
            byte[] methodBuffer = new byte[17];

            methodBuffer.WriteU8(method, 0); 
            methodBuffer.WriteU32((byte)units, 1); 
            methodBuffer.WriteU32((byte)additionalFee, 8); 

            return methodBuffer;
        }


        internal static byte[] SetComputeUnitLimit(byte method, ulong units)
        {
            byte[] methodBuffer = new byte[9];

            methodBuffer.WriteU8(method, 0);
            methodBuffer.WriteU32((uint)units, 1);
            return methodBuffer;
        }

        internal static byte[] SetComputeUnitPrice(byte method, UInt64 microLamports)
        {
            byte[] methodBuffer = new byte[9];

            methodBuffer.WriteU8(method, 0);
            methodBuffer.WriteU64((UInt64)microLamports, 1);
            return methodBuffer; 
        }


        internal static byte[] SetComputeUnitLimit(ulong units)
           => SetComputeUnitLimit((byte)2, units);

        internal static byte[] SetComputeUnitPrice(UInt64 microLamports)
           => SetComputeUnitPrice((byte)3, microLamports);
    }
}
