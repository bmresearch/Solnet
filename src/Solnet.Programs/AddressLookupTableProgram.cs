using Solnet.Programs.Utilities;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

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
        /// Derive the public key of an address lookup table for the given authority and recent slot.
        /// </summary>
        /// <param name="authority">The authority used in the derivation path.</param>
        /// <param name="recentSlot">The recent slot used in the derivation path.</param>
        /// <returns>The derived lookup table address, or null if no valid PDA could be found.</returns>
        public static PublicKey DeriveLookupTableAddress(PublicKey authority, ulong recentSlot)
        {
            return TryDeriveLookupTableAddress(authority, recentSlot, out PublicKey address, out _) ? address : null;
        }

        /// <summary>
        /// Derive the public key and bump seed of an address lookup table for the given authority and recent slot.
        /// </summary>
        /// <param name="authority">The authority used in the derivation path.</param>
        /// <param name="recentSlot">The recent slot used in the derivation path.</param>
        /// <param name="address">The derived address.</param>
        /// <param name="bump">The bump seed used to derive the address.</param>
        /// <returns>True if the lookup table PDA could be derived, otherwise false.</returns>
        public static bool TryDeriveLookupTableAddress(PublicKey authority, ulong recentSlot, out PublicKey address,
            out byte bump)
        {
            byte[] recentSlotBytes = new byte[8];
            recentSlotBytes.WriteU64(recentSlot, 0);

            return PublicKey.TryFindProgramAddress(
            [
                authority.KeyBytes,
                recentSlotBytes
            ], ProgramIdKey, out address, out bump);
        }

        /// <summary>
        /// Create a new address lookup table instruction and derive the lookup table address internally.
        /// </summary>
        /// <param name="authority">The authority used to derive and control the new lookup table.</param>
        /// <param name="payer">The account that funds the new lookup table.</param>
        /// <param name="recentSlot">The recent slot used in the derivation path.</param>
        /// <returns>The transaction instruction, or null if the lookup table PDA could not be derived.</returns>
        public static TransactionInstruction CreateLookupTable(PublicKey authority, PublicKey payer, ulong recentSlot)
        {
            if (!TryDeriveLookupTableAddress(authority, recentSlot, out PublicKey lookupTableAddress, out byte bump))
                return null;

            return CreateLookupTable(authority, payer, lookupTableAddress, bump, recentSlot);
        }

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
            PublicKey Authority, PublicKey Payer, PublicKey ALT, byte bump, ulong RecentSlot)
        {
            return CreateLookupTable(Authority, Payer, ALT, bump, RecentSlot);
        }

        /// <summary>
        /// Create new address lookup table instruction.
        /// </summary>
        /// <param name="authority">The authority used to derive and control the new lookup table.</param>
        /// <param name="payer">The account that funds the new lookup table.</param>
        /// <param name="lookupTable">The derived lookup table account.</param>
        /// <param name="bump">The bump seed used to derive the lookup table PDA.</param>
        /// <param name="recentSlot">The recent slot used in the derivation path.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction CreateLookupTable(
            PublicKey authority, PublicKey payer, PublicKey lookupTable, byte bump, ulong recentSlot)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(lookupTable, false),
                AccountMeta.ReadOnly(authority, false),
                AccountMeta.Writable(payer, true),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false)
            ];
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = AddressLookupTableProgramData.EncodeCreateAddressLookupTableData(recentSlot, bump)
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
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(LookupTable, false),
                AccountMeta.ReadOnly(Authority, true)
            ];
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = AddressLookupTableProgramData.EncodeFreezeLookupTableData()
            };
        }

        /// <summary>
        /// Extend lookup table instruction.
        /// </summary>
        /// <param name="lookupTable"></param>
        /// <param name="authority"></param>
        /// <param name="addresses"></param>
        /// <returns></returns>
        public static TransactionInstruction ExtendLookupTable(PublicKey lookupTable, PublicKey authority,
            List<PublicKey> addresses)
        {
            return ExtendLookupTableInternal(lookupTable, authority, null, addresses);
        }

        /// <summary>
        /// Extend lookup table instruction.
        /// </summary>
        /// <param name="LookupTable"></param>
        /// <param name="Authority"></param>
        /// <param name="Payer"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static TransactionInstruction ExtendLookupTable(PublicKey LookupTable, PublicKey Authority, PublicKey Payer, List<PublicKey> keys)
        {
            return ExtendLookupTableInternal(LookupTable, Authority, Payer, keys);
        }

        private static TransactionInstruction ExtendLookupTableInternal(PublicKey lookupTable, PublicKey authority,
            PublicKey payer, List<PublicKey> addresses)
        {
            List<AccountMeta> meta =
            [
                AccountMeta.Writable(lookupTable, false),
                AccountMeta.ReadOnly(authority, true)
            ];

            if (payer != null)
            {
                meta.Add(AccountMeta.Writable(payer, true));
                meta.Add(AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false));
            }

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = meta,
                Data = AddressLookupTableProgramData.EncodeExtendLookupTableData((ulong)addresses.Count, addresses)
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
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(LookupTable, false),
                AccountMeta.ReadOnly(Authority, true)
            ];
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
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(LookupTable, false),
                AccountMeta.ReadOnly(Authority, true),
                AccountMeta.Writable(Recipient, false)
            ];
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = AddressLookupTableProgramData.EncodeCloseLookupTableData()
            };
        }

        /// <summary>
        /// Decodes an instruction created by the address lookup table program.
        /// </summary>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        /// <returns>A decoded instruction.</returns>
        public static DecodedInstruction Decode(ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            uint instruction = data.GetU32(AddressLookupTableProgramData.MethodOffset);

            if (!Enum.IsDefined(typeof(AddressLookupTableProgramInstruction.Values), instruction))
            {
                return new()
                {
                    PublicKey = ProgramIdKey,
                    InstructionName = "Unknown Instruction",
                    ProgramName = ProgramName,
                    Values = [],
                    InnerInstructions = []
                };
            }

            AddressLookupTableProgramInstruction.Values instructionType =
                (AddressLookupTableProgramInstruction.Values)instruction;

            DecodedInstruction decodedInstruction = new()
            {
                PublicKey = ProgramIdKey,
                InstructionName = AddressLookupTableProgramInstruction.Names[instructionType],
                ProgramName = ProgramName,
                Values = [],
                InnerInstructions = []
            };

            switch (instructionType)
            {
                case AddressLookupTableProgramInstruction.Values.CreateLookupTable:
                    decodedInstruction.Values.Add("Lookup Table", keys[keyIndices[0]]);
                    decodedInstruction.Values.Add("Authority", keys[keyIndices[1]]);
                    decodedInstruction.Values.Add("Payer", keys[keyIndices[2]]);
                    decodedInstruction.Values.Add("System Program", keys[keyIndices[3]]);
                    decodedInstruction.Values.Add("Recent Slot", data.GetU64(4));
                    decodedInstruction.Values.Add("Bump Seed", data.GetU8(12));
                    break;
                case AddressLookupTableProgramInstruction.Values.FreezeLookupTable:
                    decodedInstruction.Values.Add("Lookup Table", keys[keyIndices[0]]);
                    decodedInstruction.Values.Add("Authority", keys[keyIndices[1]]);
                    break;
                case AddressLookupTableProgramInstruction.Values.ExtendLookupTable:
                    decodedInstruction.Values.Add("Lookup Table", keys[keyIndices[0]]);
                    decodedInstruction.Values.Add("Authority", keys[keyIndices[1]]);
                    if (keyIndices.Length > 2)
                    {
                        decodedInstruction.Values.Add("Payer", keys[keyIndices[2]]);
                    }

                    decodedInstruction.Values.Add("Addresses", AddressLookupTableProgramData.DecodeExtendLookupTableAddresses(data));
                    break;
                case AddressLookupTableProgramInstruction.Values.DeactivateLookupTable:
                    decodedInstruction.Values.Add("Lookup Table", keys[keyIndices[0]]);
                    decodedInstruction.Values.Add("Authority", keys[keyIndices[1]]);
                    break;
                case AddressLookupTableProgramInstruction.Values.CloseLookupTable:
                    decodedInstruction.Values.Add("Lookup Table", keys[keyIndices[0]]);
                    decodedInstruction.Values.Add("Authority", keys[keyIndices[1]]);
                    decodedInstruction.Values.Add("Recipient", keys[keyIndices[2]]);
                    break;
            }

            return decodedInstruction;
        }


    }
}