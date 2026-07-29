using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System.Collections.Generic;

namespace Solnet.Programs.Test
{
    [TestClass]
    public class AddressLookupTableProgramTest
    {
        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        private static readonly byte[] ProgramIdBytes = AddressLookupTableProgram.ProgramIdKey.KeyBytes;

        [TestMethod]
        public void CreateLookupTableDerivesLookupTableAddressTest()
        {
            Wallet.Wallet wallet = new(MnemonicWords);
            Account authority = wallet.GetAccount(0);
            Account payer = wallet.GetAccount(1);
            const ulong recentSlot = 123456;

            TransactionInstruction txInstruction = AddressLookupTableProgram.CreateLookupTable(
                authority.PublicKey,
                payer.PublicKey,
                recentSlot);

            bool success = AddressLookupTableProgram.TryDeriveLookupTableAddress(
                authority.PublicKey,
                recentSlot,
                out PublicKey lookupTableAddress,
                out byte bump);

            Assert.IsTrue(success);
            Assert.AreEqual(4, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(ProgramIdBytes, txInstruction.ProgramId);
            Assert.AreEqual(lookupTableAddress, new PublicKey(txInstruction.Keys[0].PublicKey));
            Assert.AreEqual(authority.PublicKey, new PublicKey(txInstruction.Keys[1].PublicKey));
            Assert.AreEqual(payer.PublicKey, new PublicKey(txInstruction.Keys[2].PublicKey));
            Assert.AreEqual(SystemProgram.ProgramIdKey, new PublicKey(txInstruction.Keys[3].PublicKey));
            Assert.AreEqual(recentSlot, Solnet.Programs.Utilities.Deserialization.GetU64(txInstruction.Data, 4));
            Assert.AreEqual(bump, Solnet.Programs.Utilities.Deserialization.GetU8(txInstruction.Data, 12));
        }

        [TestMethod]
        public void ExtendLookupTableWithoutPayerOmitsFundingAccountsTest()
        {
            Wallet.Wallet wallet = new(MnemonicWords);
            Account authority = wallet.GetAccount(0);
            Account addressOne = wallet.GetAccount(2);
            Account addressTwo = wallet.GetAccount(3);

            TransactionInstruction txInstruction = AddressLookupTableProgram.ExtendLookupTable(
                wallet.GetAccount(4).PublicKey,
                authority.PublicKey,
                [addressOne.PublicKey, addressTwo.PublicKey]);

            Assert.AreEqual(2, txInstruction.Keys.Count);
            Assert.AreEqual(2UL, Solnet.Programs.Utilities.Deserialization.GetU64(txInstruction.Data, 4));
        }

        [TestMethod]
        public void InstructionDecoderDecodesCreateLookupTableTest()
        {
            Wallet.Wallet wallet = new(MnemonicWords);
            Account authority = wallet.GetAccount(0);
            Account payer = wallet.GetAccount(1);
            const ulong recentSlot = 123456;

            TransactionInstruction txInstruction = AddressLookupTableProgram.CreateLookupTable(
                authority.PublicKey,
                payer.PublicKey,
                recentSlot);

            DecodedInstruction decodedInstruction = InstructionDecoder.Decode(
                AddressLookupTableProgram.ProgramIdKey,
                txInstruction.Data,
                [
                    new(txInstruction.Keys[0].PublicKey),
                    new(txInstruction.Keys[1].PublicKey),
                    new(txInstruction.Keys[2].PublicKey),
                    new(txInstruction.Keys[3].PublicKey)
                ],
                [0, 1, 2, 3]);

            Assert.IsNotNull(decodedInstruction);
            Assert.AreEqual("Create Lookup Table", decodedInstruction.InstructionName);
            Assert.AreEqual("Address Lookup Table Program", decodedInstruction.ProgramName);
            Assert.AreEqual(recentSlot, decodedInstruction.Values["Recent Slot"]);
        }
    }
}