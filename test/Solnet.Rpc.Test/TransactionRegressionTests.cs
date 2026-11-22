using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Rpc.Models;
using Solnet.Programs;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class TransactionRegressionTests
    {
        private const string V1 = "gQEAAg8AAAAXdVBRU6o+csjYxFfP2uox3EI97dLi8dTKQJQkEQvBrAEE0p/7hGd1vQG2gT9OV2BqGTqNwNnrNJYwnOu7DfhhZNoukLW9WNDWad3rp/Q0nDMHMGndhSMCoqDeY6ggDT3Hkgbd9uHudY/eGEJdvORszdq2GvxNg7kNJ/69+SjYoYv8e4vtdLwF7d7WFoT+auIMPfng7MksoleL4tylbX1erce8AgAAAAAAADB1AAAAADMAAgQBAAEAAAMJuxM0qooYTRViAQI0vknT2j0FxjgYZKVKugsZcySW9adpB2wrJxcV/CptFV/XFKAjrwrBbQoD3EekMqSBgX/tDQ==";
        private const string EmptyMemo = "AQ61qZFf1Eq5tooVLXUOnm2pwa9WQufJ5dwvWe/6eO9FqRUrDCpWSXDWQXvu0Llq9fY2xHPWS2mVOco7cTIhyQ4BAAIEV9P/AiUTCbWOL7dsUx8qrRgqD0O0wGr1p24RvVPEZQh17HBNmbFCfrltyiXNmNbTZ227SBJqFVcV1Dn+xgICEQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABUpTWpkpIQZNJOhxYNo4fHw1td28kruB5B+oQEEFRI3Dj8pEevGnpgEIHbHXXZrjjFDtYQHS/aUrvQpUwKZn9gICAgABDAIAAAABAAAAAAAAAAMAAA==";
        private const string Partial = "AhHNGbKS9kYa72neOFht8lgm+br3BuE82KnX4IniFJRH0/iK+dq2N6epdQmniVU7N0Dw68BIet4hfx/5HRpFCQEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgABAwTuSAD8UuCl3/7wC2PUz+2cy5nXD8wtlPjTiVpjODMIF7C6Rh1+AGhZRJSRGfnvGTkRZTWbOiSsBpE7pCCeSSQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAK29C5kFdKmOB9sujrWPVtkOgX6z15iqXA1H8JUpe0v8AQICAQAMAgAAAJuhVgoAAAAA";

        [TestMethod]
        public void V1RoundTripsThroughBaseType()
        {
            byte[] input = Convert.FromBase64String(V1);
            Transaction transaction = Transaction.Deserialize(input);
            Assert.IsInstanceOfType<VersionedTransaction>(transaction);
            Assert.AreEqual(1, transaction.Signatures.Count);
            Assert.AreEqual(1, transaction.Instructions.Count);
            CollectionAssert.AreEqual(input, transaction.Serialize());
        }

        [TestMethod]
        public void V1BuildUsesVersionedFraming()
        {
            Account payer = new();
            Transaction transaction = new VersionedTransaction
            {
                Version = 1,
                FeePayer = payer.PublicKey,
                RecentBlockHash = "11111111111111111111111111111111",
                Instructions = new List<TransactionInstruction>
                {
                    SystemProgram.Transfer(payer.PublicKey, new Account().PublicKey, 1)
                }
            };
            byte[] output = transaction.Build(payer);
            Assert.AreEqual((byte)0x81, output[0]);
            Transaction decoded = Transaction.Deserialize(output);
            Assert.IsTrue(decoded.VerifySignatures());
            CollectionAssert.AreEqual(output, decoded.Serialize());
        }

        [TestMethod]
        [DoNotParallelize]
        public void V1SerializationDoesNotWriteToConsole()
        {
            TextWriter original = Console.Out;
            using StringWriter captured = new();
            try
            {
                Console.SetOut(captured);
                Transaction.Deserialize(Convert.FromBase64String(V1)).Serialize();
                Assert.AreEqual(string.Empty, captured.ToString());
            }
            finally
            {
                Console.SetOut(original);
            }
        }

        [TestMethod]
        public void V1RejectsTrailingAndMissingSignatureBytes()
        {
            byte[] input = Convert.FromBase64String(V1);
            Assert.ThrowsExactly<FormatException>(() => VersionedTransaction.Deserialize(input.Concat(new byte[] { 0 }).ToArray()));
            Assert.ThrowsExactly<FormatException>(() => VersionedTransaction.Deserialize(input[..^1]));
            byte[] signatureFirst = new byte[] { 1 }.Concat(input[^64..]).Concat(input[..^64]).ToArray();
            Assert.ThrowsExactly<FormatException>(() => VersionedTransaction.Deserialize(signatureFirst));
        }

        [TestMethod]
        public void EmptyFinalInstructionRoundTrips()
        {
            byte[] input = Convert.FromBase64String(EmptyMemo);
            Transaction transaction = Transaction.Deserialize(input);
            Assert.AreEqual(2, transaction.Instructions.Count);
            Assert.IsEmpty(transaction.Instructions[1].Keys);
            Assert.IsEmpty(transaction.Instructions[1].Data);
            CollectionAssert.AreEqual(input, transaction.Serialize());
        }

        [TestMethod]
        public void PlaceholderSignatureSlotsRoundTrip()
        {
            byte[] input = Convert.FromBase64String(Partial);
            Transaction transaction = Transaction.Deserialize(input);
            Assert.AreEqual(2, transaction.Signatures.Count);
            Assert.IsFalse(transaction.VerifySignatures());
            CollectionAssert.AreEqual(new byte[64], transaction.Signatures[1].Signature);
            CollectionAssert.AreEqual(input, transaction.Serialize());
        }

        [TestMethod]
        public void SigningFillsExistingSlotsInMessageOrder()
        {
            Account payer = new();
            Account other = new();
            Transaction transaction = new()
            {
                FeePayer = payer.PublicKey,
                RecentBlockHash = "11111111111111111111111111111111",
                Instructions = new List<TransactionInstruction>
                {
                    SystemProgram.Transfer(payer.PublicKey, other.PublicKey, 1),
                    SystemProgram.Transfer(other.PublicKey, payer.PublicKey, 1)
                }
            };
            transaction = Transaction.Populate(Message.Deserialize(transaction.CompileMessage()), new List<byte[]> { new byte[64], new byte[64] });
            transaction.PartialSign(other);
            transaction = Transaction.Deserialize(transaction.Serialize());
            Assert.IsTrue(transaction.Sign(payer));
            Assert.AreEqual(2, transaction.Signatures.Count);
            Assert.AreEqual(payer.PublicKey.Key, transaction.Signatures[0].PublicKey.Key);
            Assert.IsTrue(Transaction.Deserialize(transaction.Serialize()).VerifySignatures());
        }
    }
}
