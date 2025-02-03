using System.Collections.Generic;
using Solnet.Rpc.Models;
using Solnet.Wallet;

namespace Solnet.Programs
{
    /// <summary>
    /// Address lookup table program
    /// </summary>
    public static class AddressLookupTableProgram
    {
        /// <summary>
        /// The public key of the ATL Program.
        /// </summary>
        public static readonly PublicKey ProgramIdKey = new("AddressLookupTab1e1111111111111111111111111");

        /// <summary>
        /// The program's name.
        /// </summary>
        private const string ProgramName = "Address Lookup Table Program";

        /// <summary>
        /// Create New Address Lookup Table Instruction
        /// </summary>
        /// <param name="Authority"></param>
        /// <param name="Payer"></param>
        /// <param name="ALT"></param>
        /// <param name="bump"></param>
        /// <param name="RecentSlot"></param>
        /// <returns></returns>
        public static TransactionInstruction CreateAddressLookupTable(
            PublicKey Authority, PublicKey Payer, PublicKey ALT,byte bump, ulong RecentSlot)
        {
            //byte[] recentSlotBytes = BitConverter.GetBytes(RecentSlot);
            //byte[] seed = Authority.KeyBytes.Concat(recentSlotBytes).ToArray();
            //PublicKey.TryFindProgramAddress(new List<byte[]> { seed }, ProgramIdKey, out PublicKey ALTaddress, out byte bump);

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(ALT, false),
                AccountMeta.ReadOnly(Authority, false),
                AccountMeta.Writable(Payer, true),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = AddressLookupTableProgramData.EncodeCreateAddressLookupTableData(RecentSlot, bump)
            };
        }

        /// <summary>
        /// Freeze Lookup Table Instruction
        /// </summary>
        /// <param name="LookupTable"></param>
        /// <param name="Authority"></param>
        /// <returns></returns>
        public static TransactionInstruction FreezeLookupTable(PublicKey LookupTable, PublicKey Authority)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(LookupTable, false),
                AccountMeta.ReadOnly(Authority, true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = AddressLookupTableProgramData.EncodeFreezeLookupTableData()
            };
        }

        /// <summary>
        /// Extend Lookup Table Instruction
        /// </summary>
        /// <param name="LookupTable"></param>
        /// <param name="Authority"></param>
        /// <param name="Payer"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static TransactionInstruction ExtendLookupTable(PublicKey LookupTable, PublicKey Authority, PublicKey Payer, List<PublicKey> keys)
        {
            List<AccountMeta> meta = new()
            {
                AccountMeta.Writable(LookupTable, false),
                AccountMeta.ReadOnly(Authority, true),
                AccountMeta.Writable(Payer, true),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = meta,
                Data = AddressLookupTableProgramData.EncodeExtendLookupTableData((ulong)keys.Count, keys)
            };
        }

        /// <summary>
        /// Deactivate  Lookup Table Instruction
        /// </summary>
        /// <param name="LookupTable"></param>
        /// <param name="Authority"></param>
        /// <returns></returns>
        public static TransactionInstruction DeactivateLookupTable(PublicKey LookupTable, PublicKey Authority)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(LookupTable, false),
                AccountMeta.ReadOnly(Authority, true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = AddressLookupTableProgramData.EncodeDeactivateLookupTableData()
            };
        }

        /// <summary>
        /// Close Lookup Table Instruction
        /// </summary>
        /// <param name="LookupTable"></param>
        /// <param name="Authority"></param>
        /// <param name="Recipient"></param>
        /// <returns></returns>
        public static TransactionInstruction CloseLookupTable(PublicKey LookupTable, PublicKey Authority, PublicKey Recipient)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(LookupTable, false),
                AccountMeta.ReadOnly(Authority, true),
                AccountMeta.Writable(Recipient, false)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = AddressLookupTableProgramData.EncodeCloseLookupTableData()
            };
        }


    }
}
