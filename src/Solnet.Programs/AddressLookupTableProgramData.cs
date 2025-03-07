using System.Collections.Generic;
using Solnet.Programs.Utilities;
using Solnet.Wallet;

namespace Solnet.Programs
{
    internal static class AddressLookupTableProgramData
    {
        internal const int MethodOffset = 0;

        /// <summary>
        /// Encode transaction instruction data for the <see cref="AddressLookupTableProgramInstruction.Values.CreateLookupTable"/> method.
        /// </summary>
        /// <param name="RecentSlot"></param>
        /// <param name="bump"></param>
        /// <returns></returns>
        internal static byte[] EncodeCreateAddressLookupTableData( ulong RecentSlot,byte bump)
        {
            byte[] data = new byte[13];
            data.WriteU32((uint)AddressLookupTableProgramInstruction.Values.CreateLookupTable, MethodOffset);
            data.WriteU64(RecentSlot, 4);
            data.WriteU8(bump, 12);

            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="AddressLookupTableProgramInstruction.Values.FreezeLookupTable"/> method.
        /// </summary>
        /// <returns></returns>
        internal static byte[] EncodeFreezeLookupTableData()
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)AddressLookupTableProgramInstruction.Values.FreezeLookupTable, MethodOffset);
            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="AddressLookupTableProgramInstruction.Values.ExtendLookupTable"/> method.
        /// </summary>
        /// <param name="KeyCounts"></param>
        /// <param name="Keys"></param>
        /// <returns></returns>
        internal static byte[] EncodeExtendLookupTableData(ulong KeyCounts,List<PublicKey> Keys)
        {
            byte[] data = new byte[12 + Keys.Count * 32];
            data.WriteU32((uint)AddressLookupTableProgramInstruction.Values.ExtendLookupTable, MethodOffset);
            data.WriteU64(KeyCounts, 4);
            for (int i = 0; i < Keys.Count; i++)
            {
                data.WritePubKey(Keys[i], 12 + i * 32);
            }
            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="AddressLookupTableProgramInstruction.Values.DeactivateLookupTable"/> method.
        /// </summary>
        /// <returns></returns>
        internal static byte[] EncodeDeactivateLookupTableData()
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)AddressLookupTableProgramInstruction.Values.DeactivateLookupTable, MethodOffset);
            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="AddressLookupTableProgramInstruction.Values.CloseLookupTable"/> method.
        /// </summary>
        /// <returns></returns>
        internal static byte[] EncodeCloseLookupTableData()
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)AddressLookupTableProgramInstruction.Values.CloseLookupTable, MethodOffset);
            return data;
        }
    }
}
