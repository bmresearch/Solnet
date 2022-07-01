using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class TransactionBuilderTest
    {
        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray" +
            " forward deal onion eight catalog surface unit card window walnut wealth medal";

        private const string Blockhash = "5cZja93sopRB9Bkhckj5WzCxCaVyriv2Uh5fFDPDFFfj";

        private const string AddSignatureBlockHash = "F2EzHpSp2WYRDA1roBN2Q4Wzw7ePxU2z1zWfh8ejUEyh";
        private const string AddSignatureTransaction = "AThRcCA7YPqwXF1JrA3lTHKU0OTZdSbh1jn1oEUkOXh" +
            "lZlNfUZnJyC5I3h6ldRGY444BBKpjRNTYO2n5x8t9swABAAIER2mrlyBLqD+wyu4X94aPHgdOUhWBoNidlDedq" +
            "mW3F7J7rHLZwOnCKOnqrRmjOO1w2JcV0XhPLlWiw5thiFgQQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAABUpTUPhdyILWFKVWcniKKW3fHqur0KYGeIhJMvTu9qDQVQOHggZl4ubetKawWVznB6EGcsLPkeO3Skl7n" +
            "XGaZAICAgABDAIAAACAlpgAAAAAAAMBABVIZWxsbyBmcm9tIFNvbC5OZXQgOik=";
        private const string AddSignatureSignature = "28Jo82xATR1U2u1PfhEjhdn3m3ciXEbxi7SocxVaj9YvyxJHkZb3yyn9QYtAubqrTcXRqTvG8DKRLGnjs5mTi5yy";

        private const string ExpectedTransactionHashWithTransferAndMemo =
            "AV9Xyi1t5dscb5+097PVDAP8fq/6HDRoNTQx9ZD2picvZNDUy9seCEKgsTNKgeTXtQ+pNEYB" +
            "4PfxPX+9bQI7hgkBAAIEUy4zulRg8z2yKITZaNwcnq6G6aH8D0ITae862qbJ+3eE3M6r5DRw" +
            "ldquwlqOuXDDOWZagXmbHnAU3w5Dg44kogAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAABUpTUPhdyILWFKVWcniKKW3fHqur0KYGeIhJMvTu9qBEixald4nI54jqHpYLSWViej50" +
            "bnmzhen0yUOsH2zbbgICAgABDAIAAACAlpgAAAAAAAMBABVIZWxsbyBmcm9tIFNvbC5OZXQgOik=";

        private const string ExpectedTransactionHashCreateInitializeAndMintTo =
            "A056qhN8bf9baCZ6SwzUlM6ge4X19TzoKANpDjg9CUGQTvIOYu27MvTcscgGov0aMkuiM9N8g" +
            "1D2bMJSvYBpWwi2IP+9oPzCj4b0AWm6uLxLv+JrMwVB8gJBYf4JtXotWDY504QIm9IqEemgUK" +
            "vWkb+9dNatYsR3d9xcqxQ14mAEAq147oIAH+FQbHj2PhdP61KXqTN7T0EclKQMJLyhkqeyREF" +
            "10Ttg99bcwTuXMxfR5rstI/kg/0Cagr/Ua+SoAQMABAdHaauXIEuoP7DK7hf3ho8eB05SFYGg" +
            "2J2UN52qZbcXsk0+Jb2M++6vIpkqr8zv+aohVvbSqnzuJeRSoRYepWULT6cip03g/pgXJNLrh" +
            "xqTpZ3aHH1CxvB/iB89zlU8m8UAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAVKU1" +
            "D4XciC1hSlVnJ4iilt3x6rq9CmBniISTL07vagBqfVFxksXFEhjMlMPUrxf1ja7gibof1E49v" +
            "ZigAAAAAG3fbh12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqeD/Y3arpTMrvjv2uP0ZD3LV" +
            "kDTmRAfOpQ603IYXOGjCBgMCAAI0AAAAAGBNFgAAAAAAUgAAAAAAAAAG3fbh12Whk9nL4UbO6" +
            "3msHLSF7V9bN5E6jPWFfv8AqQYCAgVDAAJHaauXIEuoP7DK7hf3ho8eB05SFYGg2J2UN52qZb" +
            "cXsgFHaauXIEuoP7DK7hf3ho8eB05SFYGg2J2UN52qZbcXsgMCAAE0AAAAAPAdHwAAAAAApQA" +
            "AAAAAAAAG3fbh12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqQYEAQIABQEBBgMCAQAJB6hh" +
            "AAAAAAAABAEBEkhlbGxvIGZyb20gU29sLk5ldA==";

        private const string Nonce = "2S1kjspXLPs6jpNVXQfNMqZzzSrKLbGdr9Fxap5h1DLN";

        private static byte[] CompiledMessageBytes =
        {
            1, 0, 2, 5, 71, 105, 171, 151, 32, 75, 168, 63, 176, 202, 238, 23, 247, 134, 143, 30, 7, 78, 82, 21,
            129, 160, 216, 157, 148, 55, 157, 170, 101, 183, 23, 178, 132, 220, 206, 171, 228, 52, 112, 149, 218,
            174, 194, 90, 142, 185, 112, 195, 57, 102, 90, 129, 121, 155, 30, 112, 20, 223, 14, 67, 131, 142, 36,
            162, 223, 244, 229, 56, 86, 243, 0, 74, 86, 58, 56, 142, 17, 130, 113, 147, 61, 1, 136, 126, 243, 22,
            226, 173, 108, 74, 212, 104, 81, 199, 120, 180, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 167, 213, 23, 25, 44, 86, 142, 224, 138, 132, 95, 115, 210,
            151, 136, 207, 3, 92, 49, 69, 178, 26, 179, 68, 216, 6, 46, 169, 64, 0, 0, 21, 68, 15, 82, 0, 49, 0,
            146, 241, 176, 13, 84, 249, 55, 39, 9, 212, 80, 57, 8, 193, 89, 211, 49, 162, 144, 45, 140, 117, 21, 46,
            83, 2, 3, 3, 2, 4, 0, 4, 4, 0, 0, 0, 3, 2, 0, 1, 12, 2, 0, 0, 0, 0, 202, 154, 59, 0, 0, 0, 0
        };

        [TestMethod]
        public void TestTransactionBuilderBuild()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var fromAccount = wallet.GetAccount(0);
            var toAccount = wallet.GetAccount(1);
            var tx = new TransactionBuilder()
                .SetRecentBlockHash(Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(SystemProgram.Transfer(fromAccount, toAccount.PublicKey, 10000000))
                .AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)"))
                .Build(fromAccount);

            Assert.AreEqual(ExpectedTransactionHashWithTransferAndMemo, Convert.ToBase64String(tx));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestTransactionBuilderBuildNullBlockhashException()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var fromAccount = wallet.GetAccount(0);
            var toAccount = wallet.GetAccount(1);
            _ = new TransactionBuilder().SetFeePayer(fromAccount)
                .AddInstruction(SystemProgram.Transfer(fromAccount, toAccount.PublicKey, 10000000))
                .AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)"))
                .Build(fromAccount);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestTransactionBuilderBuildNullFeePayerException()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var fromAccount = wallet.GetAccount(0);
            var toAccount = wallet.GetAccount(1);
            _ = new TransactionBuilder()
                .SetRecentBlockHash(Blockhash)
                .AddInstruction(SystemProgram.Transfer(fromAccount, toAccount.PublicKey, 10000000))
                .AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)"))
                .Build(fromAccount);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestTransactionBuilderBuildEmptySignersException()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var fromAccount = wallet.GetAccount(0);
            var toAccount = wallet.GetAccount(1);
            _ = new TransactionBuilder()
                .SetRecentBlockHash(Blockhash)
                .AddInstruction(SystemProgram.Transfer(fromAccount, toAccount.PublicKey, 10000000))
                .AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)"))
                .Build(new List<Account>());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestTransactionBuilderBuildNullInstructionsException()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var fromAccount = wallet.GetAccount(0);
            _ = new TransactionBuilder().SetRecentBlockHash(Blockhash)
                .Build(fromAccount);
        }

        [TestMethod]
        public void CreateInitializeAndMintToTest()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var blockHash = "G9JC6E7LfG6ayxARq5zDV5RdDr6P8NJEdzTUJ8ttrSKs";
            var minBalanceForAccount = 2039280UL;
            var minBalanceForMintAccount = 1461600UL;

            var mintAccount = wallet.GetAccount(17);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(18);

            var tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(
                    SystemProgram.CreateAccount(
                    ownerAccount,
                    mintAccount,
                    minBalanceForMintAccount,
                    TokenProgram.MintAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(
                    TokenProgram.InitializeMint(
                    mintAccount.PublicKey,
                    2,
                    ownerAccount.PublicKey,
                    ownerAccount.PublicKey))
                .AddInstruction(
                    SystemProgram.CreateAccount(
                    ownerAccount,
                    initialAccount,
                    minBalanceForAccount,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(
                    TokenProgram.InitializeAccount(
                    initialAccount.PublicKey,
                    mintAccount.PublicKey,
                    ownerAccount.PublicKey))
                .AddInstruction(
                    TokenProgram.MintTo(
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    25000,
                    ownerAccount))
                .AddInstruction(MemoProgram.NewMemo(initialAccount, "Hello from Sol.Net"))
                .Build(new List<Account> { ownerAccount, mintAccount, initialAccount });

            var tx2 = Transaction.Deserialize(tx);
            var msg = tx2.CompileMessage();

            Assert.IsTrue(tx2.Signatures[0].PublicKey.Verify(msg, tx2.Signatures[0].Signature));

            Assert.AreEqual(ExpectedTransactionHashCreateInitializeAndMintTo, Convert.ToBase64String(tx));
        }

        [TestMethod]
        public void CompileMessageTest()
        {
            Wallet.Wallet wallet = new(MnemonicWords);

            Account ownerAccount = wallet.GetAccount(10);
            Account nonceAccount = wallet.GetAccount(1119);
            Account toAccount = wallet.GetAccount(1);
            NonceInformation nonceInfo = new()
            {
                Nonce = Nonce,
                Instruction = SystemProgram.AdvanceNonceAccount(
                    nonceAccount.PublicKey,
                    ownerAccount
                )
            };

            byte[] txBytes = new TransactionBuilder()
                .SetFeePayer(ownerAccount)
                .SetNonceInformation(nonceInfo)
                .AddInstruction(
                    SystemProgram.Transfer(
                        ownerAccount,
                        toAccount,
                        1_000_000_000)
                )
                .CompileMessage();

            CollectionAssert.AreEqual(CompiledMessageBytes, txBytes);
        }


        [TestMethod]
        public void TestTransactionInstructionTest()
        {
            Wallet.Wallet wallet = new(MnemonicWords);

            Account ownerAccount = wallet.GetAccount(10);
            var memo = MemoProgram.NewMemo(ownerAccount, "Hello");
            var created = TransactionInstructionFactory.Create(new PublicKey(memo.ProgramId), memo.Keys, memo.Data);

            Assert.AreEqual(Convert.ToBase64String(memo.ProgramId), Convert.ToBase64String(created.ProgramId));
            Assert.AreSame(memo.Keys, created.Keys);
            Assert.AreEqual(Convert.ToBase64String(memo.Data), Convert.ToBase64String(created.Data));

        }

        [TestMethod]
        public void TransactionBuilderAddSignatureTest()
        {
            Wallet.Wallet wallet = new(MnemonicWords);

            Account fromAccount = wallet.GetAccount(10);
            Account toAccount = wallet.GetAccount(8);

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetRecentBlockHash(AddSignatureBlockHash)
                .SetFeePayer(fromAccount)
                .AddInstruction(SystemProgram.Transfer(fromAccount.PublicKey, toAccount.PublicKey, 10000000))
                .AddInstruction(MemoProgram.NewMemo(fromAccount.PublicKey, "Hello from Sol.Net :)"));

            byte[] msgBytes = txBuilder.CompileMessage();
            byte[] signature = fromAccount.Sign(msgBytes);

            Assert.AreEqual(AddSignatureSignature, Encoders.Base58.EncodeData(signature));

            byte[] tx = txBuilder.AddSignature(signature)
                .Serialize();

            Assert.AreEqual(AddSignatureTransaction, Convert.ToBase64String(tx));
        }
    }
}