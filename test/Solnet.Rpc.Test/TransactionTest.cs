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
    public class TransactionTest
    {
        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

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

        private static byte[] CompiledAndSignedBytes =
        {
            1, 13, 18, 225, 68, 176, 254, 183, 157, 106, 29, 87, 152, 179, 104, 244, 139, 151, 193, 221, 38, 99,
            232, 152, 59, 58, 18, 54, 171, 174, 187, 41, 186, 131, 84, 185, 215, 182, 192, 38, 72, 229, 186, 195,
            119, 94, 63, 210, 160, 176, 79, 194, 101, 224, 221, 6, 127, 153, 218, 31, 223, 31, 118, 4, 6, 1, 0, 2,
            5, 71, 105, 171, 151, 32, 75, 168, 63, 176, 202, 238, 23, 247, 134, 143, 30, 7, 78, 82, 21, 129, 160,
            216, 157, 148, 55, 157, 170, 101, 183, 23, 178, 132, 220, 206, 171, 228, 52, 112, 149, 218, 174, 194,
            90, 142, 185, 112, 195, 57, 102, 90, 129, 121, 155, 30, 112, 20, 223, 14, 67, 131, 142, 36, 162, 223,
            244, 229, 56, 86, 243, 0, 74, 86, 58, 56, 142, 17, 130, 113, 147, 61, 1, 136, 126, 243, 22, 226, 173,
            108, 74, 212, 104, 81, 199, 120, 180, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 167, 213, 23, 25, 44, 86, 142, 224, 138, 132, 95, 115, 210, 151, 136,
            207, 3, 92, 49, 69, 178, 26, 179, 68, 216, 6, 46, 169, 64, 0, 0, 21, 68, 15, 82, 0, 49, 0, 146, 241,
            176, 13, 84, 249, 55, 39, 9, 212, 80, 57, 8, 193, 89, 211, 49, 162, 144, 45, 140, 117, 21, 46, 83, 2, 3,
            3, 2, 4, 0, 4, 4, 0, 0, 0, 3, 2, 0, 1, 12, 2, 0, 0, 0, 0, 202, 154, 59, 0, 0, 0, 0
        };

        private static byte[] CraftTransactionBytes =
        {
            3, 230, 155, 244, 7, 138, 48, 69, 134, 17, 31, 188, 153, 121, 114, 199, 185, 236, 232, 115, 176, 139, 
            150, 72, 167, 98, 110, 93, 170, 237, 95, 49, 204, 1, 100, 43, 189, 169, 146, 230, 50, 24, 217, 145, 178, 
            114, 72, 186, 85, 3, 184, 2, 122, 27, 231, 100, 58, 113, 159, 65, 161, 136, 77, 147, 5, 232, 59, 124, 
            162, 191, 129, 233, 0, 130, 46, 252, 112, 3, 165, 31, 143, 210, 206, 153, 164, 16, 118, 173, 237, 141, 
            185, 150, 235, 248, 205, 254, 173, 217, 139, 60, 16, 35, 126, 75, 111, 138, 38, 150, 13, 120, 180, 170, 
            187, 63, 131, 238, 189, 66, 63, 145, 33, 121, 136, 174, 102, 31, 66, 104, 13, 213, 251, 200, 222, 131, 69, 
            208, 88, 110, 109, 2, 115, 201, 248, 97, 158, 253, 222, 194, 201, 191, 121, 216, 69, 235, 79, 220, 19, 22, 
            172, 232, 229, 3, 76, 93, 12, 158, 231, 28, 107, 148, 8, 177, 6, 237, 80, 176, 106, 121, 65, 72, 91, 223, 
            34, 14, 213, 247, 191, 254, 72, 195, 99, 4, 13, 3, 0, 4, 7, 71, 105, 171, 151, 32, 75, 168, 63, 176, 202, 
            238, 23, 247, 134, 143, 30, 7, 78, 82, 21, 129, 160, 216, 157, 148, 55, 157, 170, 101, 183, 23, 178, 205, 
            251, 13, 211, 102, 148, 169, 147, 62, 156, 122, 35, 98, 20, 157, 88, 150, 56, 27, 74, 223, 168, 25, 163, 
            120, 95, 11, 3, 42, 184, 239, 59, 215, 137, 216, 107, 200, 181, 124, 152, 190, 73, 13, 182, 204, 46, 141, 
            8, 127, 222, 225, 79, 199, 135, 152, 53, 129, 239, 152, 82, 141, 143, 98, 133, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 74, 83, 80, 248, 93, 200, 130, 214, 
            20, 165, 86, 114, 120, 138, 41, 109, 223, 30, 171, 171, 208, 166, 6, 120, 136, 73, 50, 244, 238, 246, 160, 
            6, 167, 213, 23, 25, 44, 92, 81, 33, 140, 201, 76, 61, 74, 241, 127, 88, 218, 238, 8, 155, 161, 253, 68, 227, 
            219, 217, 138, 0, 0, 0, 0, 6, 221, 246, 225, 215, 101, 161, 147, 217, 203, 225, 70, 206, 235, 121, 172, 28, 
            180, 133, 237, 95, 91, 55, 145, 58, 140, 245, 133, 126, 255, 0, 169, 206, 78, 169, 189, 0, 235, 196, 10, 163, 
            190, 178, 243, 194, 80, 1, 89, 248, 166, 252, 150, 61, 65, 187, 142, 133, 205, 198, 253, 19, 241, 15, 248, 6, 
            3, 2, 0, 2, 52, 0, 0, 0, 0, 96, 77, 22, 0, 0, 0, 0, 0, 82, 0, 0, 0, 0, 0, 0, 0, 6, 221, 246, 225, 215, 101, 
            161, 147, 217, 203, 225, 70, 206, 235, 121, 172, 28, 180, 133, 237, 95, 91, 55, 145, 58, 140, 245, 133, 126, 
            255, 0, 169, 6, 2, 2, 5, 67, 0, 2, 71, 105, 171, 151, 32, 75, 168, 63, 176, 202, 238, 23, 247, 134, 143, 30, 
            7, 78, 82, 21, 129, 160, 216, 157, 148, 55, 157, 170, 101, 183, 23, 178, 1, 71, 105, 171, 151, 32, 75, 168, 
            63, 176, 202, 238, 23, 247, 134, 143, 30, 7, 78, 82, 21, 129, 160, 216, 157, 148, 55, 157, 170, 101, 183, 
            23, 178, 3, 2, 0, 1, 52, 0, 0, 0, 0, 240, 29, 31, 0, 0, 0, 0, 0, 165, 0, 0, 0, 0, 0, 0, 0, 6, 221, 246, 225, 
            215, 101, 161, 147, 217, 203, 225, 70, 206, 235, 121, 172, 28, 180, 133, 237, 95, 91, 55, 145, 58, 140, 245, 
            133, 126, 255, 0, 169, 6, 4, 1, 2, 0, 5, 1, 1, 6, 3, 2, 1, 0, 9, 7, 64, 66, 15, 0, 0, 0, 0, 0, 4, 1, 1, 18, 
            72, 101, 108, 108, 111, 32, 102, 114, 111, 109, 32, 83, 111, 108, 46, 78, 101, 116

        };

        private const string InvalidBase64Transaction =
            "AQ0S4USBAYBAAIFR2mrlyBLqD+wyu4X94aPHgdOUhWBoNidlDedqmW3F7KE3M6r5DRwldquwlqOuXDDOWZagXmbHnAU3w5Dg44kot" +
            "/05ThW8wBKVjo4jhGCcZM9AYh+8xbirWxK1GhRx3i0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGp9UXGSxWjuCKhF9" +
            "z0peIzwNcMUWyGrNE2AYuqUAAABVED1IAMQCS8bANVPk3JwnUUDkIwVnTMaKQLYx1FS5TAgMDAgQABAQAAAADAgABDAIAAAAAypo7AAAAAA==";

        private const string Base64Transaction =
            "AQ0S4USw/redah1XmLNo9IuXwd0mY+iYOzoSNquuuym6g1S517bAJkjlusN3Xj/SoLBPwmXg3QZ/mdof3x92BAYBAAIFR2mrlyBL" +
            "qD+wyu4X94aPHgdOUhWBoNidlDedqmW3F7KE3M6r5DRwldquwlqOuXDDOWZagXmbHnAU3w5Dg44kot/05ThW8wBKVjo4jhGCcZM9A" +
            "Yh+8xbirWxK1GhRx3i0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGp9UXGSxWjuCKhF9z0peIzwNcMUWyGrNE2AYuqU" +
            "AAABVED1IAMQCS8bANVPk3JwnUUDkIwVnTMaKQLYx1FS5TAgMDAgQABAQAAAADAgABDAIAAAAAypo7AAAAAA==";

        private const string Base64Message =
            "AwAEB0dpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyzfsN02aUqZM+nHojYhSdWJY4G0rfqBmjeF8LAyq47zvXidhry" +
            "LV8mL5JDbbMLo0If97hT8eHmDWB75hSjY9ihQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABUpTUPhdyILWFKVWcn" +
            "iKKW3fHqur0KYGeIhJMvTu9qAGp9UXGSxcUSGMyUw9SvF/WNruCJuh/UTj29mKAAAAAAbd9uHXZaGT2cvhRs7reawctIXtX1s" +
            "3kTqM9YV+/wCpzk6pvQDrxAqjvrLzwlABWfim/JY9QbuOhc3G/RPxD/gGAwIAAjQAAAAAYE0WAAAAAABSAAAAAAAAAAbd9uHX" +
            "ZaGT2cvhRs7reawctIXtX1s3kTqM9YV+/wCpBgICBUMAAkdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyAUdpq5cgS" +
            "6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyAwIAATQAAAAA8B0fAAAAAAClAAAAAAAAAAbd9uHXZaGT2cvhRs7reawctIXtX1" +
            "s3kTqM9YV+/wCpBgQBAgAFAQEGAwIBAAkHQEIPAAAAAAAEAQESSGVsbG8gZnJvbSBTb2wuTmV0";

        private const string PopulatedBase64MessageTx =
            "AywGaWtisQ1ssPzJjYz4MfsQYxFabAmqBN5ikzhLIVyI/78SFuYDjqcppmaVXUT7e5G0ibfcw566OrktXauH+wjrzZ7AK9Ct0hm" +
            "vDtESVp9qVGpmG0oVW2cRvdcHRifjLEAq+5SbQkIYKQ3oQqGXAii4YS5axTU0IjpaSM0E8isLcXjSX5ImjZkRIAKASKelgvYBdV" +
            "IdCFfw0rOqiQU4+YJaXDLL58bSlYU6KCknhqcgBJ280w2Mo1ftFgE7UzI1CQMABAdHaauXIEuoP7DK7hf3ho8eB05SFYGg2J2UN5" +
            "2qZbcXsteJ2GvItXyYvkkNtswujQh/3uFPx4eYNYHvmFKNj2KFzfsN02aUqZM+nHojYhSdWJY4G0rfqBmjeF8LAyq47zsAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAan1RcZLFxRIYzJTD1K8X9Y2u4Im6H9ROPb2YoAAAAABt324ddloZPZy+FGzut5rB" +
            "y0he1fWzeROoz1hX7/AKkFSlNQ+F3IgtYUpVZyeIopbd8eq6vQpgZ4iEky9O72oNPCkiUmUmY38UxCplFPvX1XWa8viIXmbvwSYH" +
            "8MpQXtBgMCAAE0AAAAAGBNFgAAAAAAUgAAAAAAAAAG3fbh12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqQUCAQRDAAJHaauXIE" +
            "uoP7DK7hf3ho8eB05SFYGg2J2UN52qZbcXsgFHaauXIEuoP7DK7hf3ho8eB05SFYGg2J2UN52qZbcXsgMCAAI0AAAAAPAdHwAAAA" +
            "AApQAAAAAAAAAG3fbh12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqQUEAgEABAEBBQMBAgAJB0BCDwAAAAAABgECEkhlbGxvIG" +
            "Zyb20gU29sLk5ldA==";

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TransactionDeserializeExceptionTest()
        {
            _ = Transaction.Deserialize(InvalidBase64Transaction);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransactionDeserializeArgumentNullExceptionTest()
        {
            _ = Transaction.Deserialize((string)null);
        }

        [TestMethod]
        public void TransactionDurableNonceDeserializeTest()
        {
            Transaction tx = Transaction.Deserialize(Base64Transaction);

            Assert.IsNotNull(tx);
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", tx.FeePayer.Key);
            Assert.AreEqual("2S1kjspXLPs6jpNVXQfNMqZzzSrKLbGdr9Fxap5h1DLN", tx.RecentBlockHash);

            Assert.AreEqual(1, tx.Signatures.Count);
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", tx.Signatures[0].PublicKey.Key);
            Assert.AreEqual("GAJa8rLiVeTHYTcbwjLmVnxVH986Vwxz4PXDEPaZKz4BEcmv9rvMF2Sw2xLzbu8mwNHA8ZZ6Es5Thf8yQrwjLv9",
                Encoders.Base58.EncodeData(tx.Signatures[0].Signature));

            // This is 1 because the transaction uses durable nonce.
            Assert.AreEqual(1, tx.Instructions.Count);

            // Assert the durable nonce data
            Assert.AreEqual("2S1kjspXLPs6jpNVXQfNMqZzzSrKLbGdr9Fxap5h1DLN", tx.NonceInformation.Nonce);
            Assert.AreEqual(3, tx.NonceInformation.Instruction.Keys.Count);
            Assert.AreEqual("11111111111111111111111111111111",
                Encoders.Base58.EncodeData(tx.NonceInformation.Instruction.ProgramId));
            Assert.AreEqual("G5EWCBwDM5GzVNwrG9LbgpTdQBD9PEAaey82ttuJJ7Qo",
                tx.NonceInformation.Instruction.Keys[0].PublicKey);
            Assert.IsTrue(tx.NonceInformation.Instruction.Keys[0].IsWritable);
            Assert.IsFalse(tx.NonceInformation.Instruction.Keys[0].IsSigner);
            Assert.AreEqual("SysvarRecentB1ockHashes11111111111111111111",
                tx.NonceInformation.Instruction.Keys[1].PublicKey);
            Assert.IsFalse(tx.NonceInformation.Instruction.Keys[1].IsWritable);
            Assert.IsFalse(tx.NonceInformation.Instruction.Keys[1].IsSigner);
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj",
                tx.NonceInformation.Instruction.Keys[2].PublicKey);
            Assert.IsTrue(tx.NonceInformation.Instruction.Keys[2].IsWritable);
            Assert.IsTrue(tx.NonceInformation.Instruction.Keys[2].IsSigner);
            Assert.AreEqual("BAAAAA==", Convert.ToBase64String(tx.NonceInformation.Instruction.Data));

            // Assert the instruction data that's not related to durable nonce
            Assert.AreEqual(2, tx.Instructions[0].Keys.Count);
            Assert.AreEqual("11111111111111111111111111111111",
                Encoders.Base58.EncodeData(tx.Instructions[0].ProgramId));
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", tx.Instructions[0].Keys[0].PublicKey);
            Assert.IsTrue(tx.Instructions[0].Keys[0].IsWritable);
            Assert.IsTrue(tx.Instructions[0].Keys[0].IsSigner);
            Assert.AreEqual("9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", tx.Instructions[0].Keys[1].PublicKey);
            Assert.IsTrue(tx.Instructions[0].Keys[1].IsWritable);
            Assert.IsFalse(tx.Instructions[0].Keys[1].IsSigner);
            Assert.AreEqual("AgAAAADKmjsAAAAA", Convert.ToBase64String(tx.Instructions[0].Data));
        }

        [TestMethod]
        public void PopulateTest()
        {
            Transaction tx = Transaction.Populate(Base64Message, new List<byte[]>()
            {
                Encoders.Base58.DecodeData("5cR7atqKv6zc73VFCK5iF58ytoaGMa9fFrY1JG95yn5QyyFPDxCtBmuBLvpZXF5o79YZ1phSoDD4ELqnJy468Ktg"),
                Encoders.Base58.DecodeData("5eJHTqGiAq4AGdJDT35A8zxJv9FjYfFd4YM1KivWYxxSVUmc9f5iFLbGbJiup7EhuMuYRHNXuV12BbWe1bcivLKN"),
                Encoders.Base58.DecodeData("5H8vKk6PGJKJxYxFfRkUnNwwJv9HaaZzmPfBSxJkvMpAgknm3JakR77WchVfHMTCHdG9mCWULjc8jEvHJb7wwXQG"),
            });

            byte[] txBytes = tx.Serialize();

            CollectionAssert.AreEqual(CraftTransactionBytes, txBytes);
        }

        [TestMethod]
        public void CompileMessageTest()
        {
            Message msg = Message.Deserialize(CompiledMessageBytes);
            Transaction tx = Transaction.Populate(msg);

            byte[] txBytes = tx.CompileMessage();

            CollectionAssert.AreEqual(CompiledMessageBytes, txBytes);
        }

        [TestMethod]
        public void SignTest()
        {
            Wallet.Wallet wallet = new(MnemonicWords);
            Account ownerAccount = wallet.GetAccount(10);

            Message msg = Message.Deserialize(CompiledMessageBytes);
            Transaction tx = Transaction.Populate(msg);

            Assert.IsTrue(tx.Sign(ownerAccount));

            byte[] txBytes = tx.Serialize();

            CollectionAssert.AreEqual(CompiledAndSignedBytes, txBytes);
        }

        [TestMethod]
        public void BuildTest()
        {
            Wallet.Wallet wallet = new(MnemonicWords);
            Account ownerAccount = wallet.GetAccount(10);

            Message msg = Message.Deserialize(CompiledMessageBytes);
            Transaction tx = Transaction.Populate(msg);

            byte[] txBytes = tx.Build(ownerAccount);

            CollectionAssert.AreEqual(CompiledAndSignedBytes, txBytes);
        }

        [TestMethod]
        public void AddSignatureTest()
        {
            Wallet.Wallet wallet = new(MnemonicWords);
            Account ownerAccount = wallet.GetAccount(10);

            Message msg = Message.Deserialize(CompiledMessageBytes);
            Transaction tx = Transaction.Populate(msg);

            byte[] signature = ownerAccount.Sign(tx.CompileMessage());

            tx.AddSignature(ownerAccount.PublicKey, signature);

            byte[] txBytes = tx.Serialize();

            CollectionAssert.AreEqual(CompiledAndSignedBytes, txBytes);
        }

        [TestMethod]
        public void AddInstructionsTest()
        {
            Wallet.Wallet wallet = new(MnemonicWords);
            Account ownerAccount = wallet.GetAccount(10);
            Account mintAccount = wallet.GetAccount(1002);
            Account initialAccount = wallet.GetAccount(1102);

            Transaction tx = new()
            {
                FeePayer = ownerAccount,
                RecentBlockHash = "EtLZEUfN1sSsaHRzTtrGW6N62hagTXjc5jokiWqZ9qQ3"
            };

            byte[] txBytes = tx
                .Add(SystemProgram.CreateAccount(
                    ownerAccount,
                    mintAccount,
                    1461600UL,
                    TokenProgram.MintAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .Add(TokenProgram.InitializeMint(
                    mintAccount.PublicKey,
                    2,
                    ownerAccount.PublicKey,
                    ownerAccount.PublicKey))
                .Add(SystemProgram.CreateAccount(
                    ownerAccount,
                    initialAccount,
                    2039280UL,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .Add(TokenProgram.InitializeAccount(
                    initialAccount.PublicKey,
                    mintAccount.PublicKey,
                    ownerAccount.PublicKey))
                .Add(TokenProgram.MintTo(
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    1_000_000,
                    ownerAccount))
                .Add(MemoProgram.NewMemo(initialAccount, "Hello from Sol.Net"))
                .CompileMessage();

            CollectionAssert.AreEqual(CraftTransactionBytes[193..], txBytes);
        }

        [TestMethod]
        public void PartialSignTest()
        {
            Wallet.Wallet wallet = new(MnemonicWords);
            Account ownerAccount = wallet.GetAccount(10);
            Account mintAccount = wallet.GetAccount(1002);
            Account initialAccount = wallet.GetAccount(1102);

            Transaction tx = new()
            {
                FeePayer = ownerAccount,
                RecentBlockHash = "EtLZEUfN1sSsaHRzTtrGW6N62hagTXjc5jokiWqZ9qQ3"
            };

            byte[] txBytes = tx
                .Add(SystemProgram.CreateAccount(
                    ownerAccount,
                    mintAccount,
                    1461600UL,
                    TokenProgram.MintAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .Add(TokenProgram.InitializeMint(
                    mintAccount.PublicKey,
                    2,
                    ownerAccount.PublicKey,
                    ownerAccount.PublicKey))
                .Add(SystemProgram.CreateAccount(
                    ownerAccount,
                    initialAccount,
                    2039280UL,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .Add(TokenProgram.InitializeAccount(
                    initialAccount.PublicKey,
                    mintAccount.PublicKey,
                    ownerAccount.PublicKey))
                .Add(TokenProgram.MintTo(
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    1_000_000,
                    ownerAccount))
                .Add(MemoProgram.NewMemo(initialAccount, "Hello from Sol.Net"))
                .CompileMessage();

            tx.PartialSign(new List<Account> { ownerAccount, ownerAccount });
            tx.PartialSign(mintAccount);

            tx.AddSignature(initialAccount.PublicKey, initialAccount.Sign(txBytes));

            byte[] serializedBytes = tx.Serialize();
            
            CollectionAssert.AreEqual(CraftTransactionBytes, serializedBytes);
        }
    }
}