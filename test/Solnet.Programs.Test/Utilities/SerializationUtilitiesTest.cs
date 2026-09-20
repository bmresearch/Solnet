using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Numerics;

namespace Solnet.Programs.Test.Utilities
{
    [TestClass]
    public class SerializationUtilitiesTest
    {
        private static readonly byte[] PublicKeyBytes =
        [
            6, 221, 246, 225, 215, 101, 161, 147, 217, 203,
            225, 70, 206, 235, 121, 172, 28, 180, 133, 237,
            95, 91, 55, 145, 58, 140, 245, 133, 126, 255, 0, 169
        ];

        private static readonly byte[] DoubleBytes =
        [
            108, 251, 85, 215, 136, 134, 245, 63
        ];

        private static readonly byte[] SingleBytes =
        [
            71, 52, 172, 63,
        ];

        private static readonly byte[] EncodedStringBytes =
        {
            21, 0, 0, 0, 0,0,0,0,116, 104, 105, 115, 32, 105, 115,
            32, 97, 32, 116, 101, 115, 116, 32, 115, 116, 114,
            105, 110, 103
        };

        [TestMethod]
        public void TestWriteU8Exception()
        {
            byte[] sut = new byte[1];
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => sut.WriteU8(1, 2));
        }

        [TestMethod]
        public void TestWriteU8()
        {
            byte[] sut = new byte[1];
            sut.WriteU8(1, 0);
            Assert.AreSequenceEqual(new byte[] { 1 }, sut);
        }

        [TestMethod]
        public void TestWriteU16Exception()
        {
            byte[] sut = new byte[2];
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => sut.WriteU16(1, 2));
        }

        [TestMethod]
        public void TestWriteU16()
        {
            byte[] sut = new byte[2];
            sut.WriteU16(1, 0);
            Assert.AreSequenceEqual(new byte[] { 1, 0 }, sut);
        }

        [TestMethod]
        public void TestWriteBoolException()
        {
            byte[] sut = new byte[2];
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => sut.WriteBool(true, 2));
        }

        [TestMethod]
        public void TestWriteBool()
        {
            byte[] sut = new byte[2];
            sut.WriteBool(true, 0);
            Assert.AreSequenceEqual(new byte[] { 1, 0 }, sut);
        }

        [TestMethod]
        public void TestWriteU32Exception()
        {
            byte[] sut = new byte[4];
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => sut.WriteU32(1, 4));
        }

        [TestMethod]
        public void TestWriteU32()
        {
            byte[] sut = new byte[4];
            sut.WriteU32(1, 0);
            Assert.AreSequenceEqual(new byte[] { 1, 0, 0, 0 }, sut);
        }

        [TestMethod]
        public void TestWriteU64Exception()
        {
            byte[] sut = new byte[8];
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => sut.WriteU64(1, 8));
        }

        [TestMethod]
        public void TestWriteU64()
        {
            byte[] sut = new byte[8];
            sut.WriteU64(1, 0);
            Assert.AreSequenceEqual(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0 }, sut);
        }

        [TestMethod]
        public void TestWriteS8Exception()
        {
            byte[] sut = new byte[1];
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => sut.WriteS8(1, 2));
        }

        [TestMethod]
        public void TestWriteS8()
        {
            byte[] sut = new byte[1];
            sut.WriteS8(1, 0);
            Assert.AreSequenceEqual(new byte[] { 1 }, sut);
        }

        [TestMethod]
        public void TestWriteS16Exception()
        {
            byte[] sut = new byte[2];
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => sut.WriteS16(1, 1));
        }

        [TestMethod]
        public void TestWriteS16()
        {
            byte[] sut = new byte[2];
            sut.WriteS16(1, 0);
            Assert.AreSequenceEqual(new byte[] { 1, 0 }, sut);
        }

        [TestMethod]
        public void TestWriteS32Exception()
        {
            byte[] sut = new byte[4];
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => sut.WriteS32(1, 1));
        }

        [TestMethod]
        public void TestWriteS32()
        {
            byte[] sut = new byte[4];
            sut.WriteS32(1, 0);
            Assert.AreSequenceEqual(new byte[] { 1, 0, 0, 0 }, sut);
        }

        [TestMethod]
        public void TestWriteS64Exception()
        {
            byte[] sut = new byte[8];
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => sut.WriteS64(1, 1));
        }

        [TestMethod]
        public void TestWriteS64()
        {
            byte[] sut = new byte[8];
            sut.WriteS64(1, 0);
            Assert.AreSequenceEqual(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0 }, sut);
        }

        [TestMethod]
        public void TestWriteSpanException()
        {
            byte[] sut = new byte[32];
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => sut.WriteSpan(PublicKeyBytes, 1));
        }

        [TestMethod]
        public void TestWriteSpan()
        {
            byte[] sut = new byte[32];
            sut.WriteSpan(PublicKeyBytes, 0);
            Assert.AreSequenceEqual(PublicKeyBytes, sut);
        }

        [TestMethod]
        public void TestWritePublicKeyException()
        {
            byte[] sut = new byte[32];
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => sut.WritePubKey(new PublicKey(PublicKeyBytes), 1));
        }

        [TestMethod]
        public void TestWritePublicKey()
        {
            byte[] sut = new byte[32];
            sut.WritePubKey(new PublicKey(PublicKeyBytes), 0);
            Assert.AreSequenceEqual(PublicKeyBytes, sut);
        }

        [TestMethod]
        public void TestWriteBigIntegerException()
        {
            byte[] sut = new byte[16];

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => sut.WriteBigInt(new BigInteger(15000000000000000000000000D), 8, 16));
        }

        [TestMethod]
        public void TestWriteBigIntegerException2()
        {
            BigInteger bi = BigInteger.Parse("34028236692093846346337460743176821145");

            byte[] buffer = new byte[10];

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => buffer.WriteBigInt(bi, 0, 10));
        }


        [TestMethod]
        public void TestWriteBigInteger()
        {
            byte[] sut = new byte[16];
            BigInteger bi = BigInteger.Parse("34028236692093846346337460743176821145");

            int written = sut.WriteBigInt(bi, 0, 16);

            Assert.AreEqual(bi.GetByteCount(), written);
            Assert.AreSequenceEqual(new byte[]
            {
                153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 25,
            }, sut);
        }

        [TestMethod]
        public void TestWriteDoubleException()
        {
            const double value = 1.34534534564565;
            byte[] bytes = new byte[8];
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => bytes.WriteDouble(value, 1));
        }

        [TestMethod]
        public void TestWriteDouble()
        {
            const double value = 1.34534534564565;
            byte[] bytes = new byte[8];
            bytes.WriteDouble(value, 0);
            Assert.AreSequenceEqual(DoubleBytes, bytes);
        }

        [TestMethod]
        public void TestWriteSingleException()
        {
            const float value = 1.34534534f;
            byte[] bytes = new byte[4];
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => bytes.WriteSingle(value, 1));
        }

        [TestMethod]
        public void TestWriteSingle()
        {
            const float value = 1.34534534f;
            byte[] bytes = new byte[4];
            bytes.WriteSingle(value, 0);
            Assert.AreSequenceEqual(SingleBytes, bytes);
        }

        [TestMethod]
        public void TestWriteRustString()
        {
            const string value = "this is a test string";

            byte[] encodedString = Serialization.EncodeBincodeString(value);

            Assert.AreSequenceEqual(EncodedStringBytes, encodedString);
        }
    }
}