using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBitcoin.DataEncoders;

namespace Solnet.Programs.Test
{
    [TestClass]
    public class SystemProgramTest
    {
        private static readonly Base58Encoder Encoder = new();

        private const string CreateAccountInstructionBase64 =
            "AAAAAPAdHwAAAAAApQAAAAAAAAAG3fbh12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqQ==";

        private static readonly byte[] SystemProgramIdBytes =
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        private static readonly byte[] CreateAccountInstructionBytes =
        {
            0, 0, 0, 0, 240, 29, 31, 0, 0, 0, 0, 0, 165,
            0, 0, 0, 0, 0, 0, 0, 6, 221, 246, 225, 215,
            101, 161, 147, 217, 203, 225, 70, 206,
            235, 121, 172, 28, 180, 133, 237, 95, 91,
            55, 145, 58, 140, 245, 133, 126, 255, 0, 169
        };

        private static readonly byte[] TransferInstructionBytes = {2, 0, 0, 0, 128, 150, 152, 0, 0, 0, 0, 0};

        private const long BalanceForRentExemption = 2039280L;

        public static string ToStringByteArray(IEnumerable<byte> bytes) => "[" + string.Join(",", bytes) + "]";

        [TestMethod]
        public void TestSystemProgramTransfer()
        {
            var wallet =
                new Wallet.Wallet(
                    "route clerk disease box emerge airport loud waste attitude film army tray forward deal onion eight catalog surface unit card window walnut wealth medal");

            var fromAccount = wallet.GetAccount(0);
            var toAccount = wallet.GetAccount(1);

            var txInstruction = SystemProgram.Transfer(fromAccount.GetPublicKey, toAccount.GetPublicKey, 10000000);
            
            Assert.AreEqual(2, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TransferInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(SystemProgramIdBytes, txInstruction.ProgramId);
        }

        [TestMethod]
        public void TestSystemProgramCreateAccount()
        {
            var wallet =
                new Wallet.Wallet(
                    "route clerk disease box emerge airport loud waste attitude film army tray forward deal onion eight catalog surface unit card window walnut wealth medal");

            var mintAccount = wallet.GetAccount(3);
            var ownerAccount = wallet.GetAccount(4);

            var txInstruction = SystemProgram.CreateAccount(
                ownerAccount.GetPublicKey,
                mintAccount.GetPublicKey,
                BalanceForRentExemption,
                SystemProgram.AccountDataSize,
                TokenProgram.ProgramId);

            Assert.AreEqual(2, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(CreateAccountInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(SystemProgramIdBytes, txInstruction.ProgramId);
        }
    }
}