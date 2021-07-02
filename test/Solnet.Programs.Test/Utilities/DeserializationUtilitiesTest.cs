using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Numerics;

namespace Solnet.Programs.Test.Utilities
{
    [TestClass]
    public class DeserializationUtilitiesTest
    {
        private static readonly byte[] PublicKeyBytes =
        {
            6, 221, 246, 225, 215, 101, 161, 147, 217, 203,
            225, 70, 206, 235, 121, 172, 28, 180, 133, 237,
            95, 91, 55, 145, 58, 140, 245, 133, 126, 255, 0, 169
        };

        private static readonly byte[] BigIntBytes =
        {
            153, 153, 153, 153, 153, 153, 153, 153,
            153, 153, 153, 153, 153, 153, 153, 25,
        };
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadU8Exception()
        {
            ReadOnlySpan<byte> readSpan = new byte[] {1}.AsSpan();
            byte value = readSpan.GetU8(1);
        }
        
        [TestMethod]
        public void TestReadU8()
        {
            ReadOnlySpan<byte> readSpan = new byte[] {1}.AsSpan();
            byte value = readSpan.GetU8(0);
            
            Assert.AreEqual(1, value);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadU16Exception()
        {
            ReadOnlySpan<byte> readSpan = new byte[] {1, 0}.AsSpan();
            uint value = readSpan.GetU16(1);
        }
        
        [TestMethod]
        public void TestReadU16()
        {
            ReadOnlySpan<byte> readSpan = new byte[] {1, 0}.AsSpan();
            uint value = readSpan.GetU16(0);
            
            Assert.AreEqual(1U, value);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadU32Exception()
        {
            ReadOnlySpan<byte> readSpan = new byte[] {1, 0, 0, 0}.AsSpan();
            uint value = readSpan.GetU32(1);
        }
        
        [TestMethod]
        public void TestReadU32()
        {
            ReadOnlySpan<byte> readSpan = new byte[] {1, 0, 0, 0}.AsSpan();
            uint value = readSpan.GetU32(0);
            
            Assert.AreEqual(1U, value);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadU64Exception()
        {
            ReadOnlySpan<byte> readSpan = new byte[] {1, 0, 0, 0, 0, 0, 0, 0}.AsSpan();
            ulong value = readSpan.GetU64(1);
        }
        
        [TestMethod]
        public void TestReadU64()
        {
            ReadOnlySpan<byte> readSpan = new byte[] {1, 0, 0, 0, 0, 0, 0, 0}.AsSpan();
            ulong value = readSpan.GetU64(0);
            
            Assert.AreEqual(1UL, value);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadS8Exception()
        {
            ReadOnlySpan<byte> readSpan = new byte[] {1}.AsSpan();
            sbyte value = readSpan.GetS8(1);
        }
        
        [TestMethod]
        public void TestReadS8()
        {
            ReadOnlySpan<byte> readSpan = new byte[] {1}.AsSpan();
            sbyte value = readSpan.GetS8(0);
            
            Assert.AreEqual(1, value);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadS16Exception()
        {
            ReadOnlySpan<byte> readSpan = new byte[] {1, 0}.AsSpan();
            int value = readSpan.GetS16(1);
        }
        
        [TestMethod]
        public void TestReadS16()
        {
            ReadOnlySpan<byte> readSpan = new byte[] {1, 0}.AsSpan();
            int value = readSpan.GetS16(0);
            
            Assert.AreEqual(1, value);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadS32Exception()
        {
            ReadOnlySpan<byte> readSpan = new byte[] {1, 0, 0, 0}.AsSpan();
            int value = readSpan.GetS32(1);
        }
        
        [TestMethod]
        public void TestReadS32()
        {
            ReadOnlySpan<byte> readSpan = new byte[] {1, 0, 0, 0}.AsSpan();
            int value = readSpan.GetS32(0);
            
            Assert.AreEqual(1, value);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadS64Exception()
        {
            ReadOnlySpan<byte> readSpan = new byte[] {1, 0, 0, 0, 0, 0, 0, 0}.AsSpan();
            long value = readSpan.GetS64(1);
        }
        
        [TestMethod]
        public void TestReadS64()
        {
            ReadOnlySpan<byte> readSpan = new byte[] {1, 0, 0, 0, 0, 0, 0, 0}.AsSpan();
            long value = readSpan.GetS64(0);
            
            Assert.AreEqual(1L, value);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadSpanException()
        {
            ReadOnlySpan<byte> readSpan = PublicKeyBytes.AsSpan();
            Span<byte> span = readSpan.GetSpan(1, 32);
        }
        
        [TestMethod]
        public void TestReadSpan()
        {
            ReadOnlySpan<byte> readSpan = PublicKeyBytes.AsSpan();
            Span<byte> span = readSpan.GetSpan(0, 32);
            
            CollectionAssert.AreEqual(PublicKeyBytes, span.ToArray());
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadPublicKeyException()
        {
            ReadOnlySpan<byte> span = PublicKeyBytes.AsSpan();
            PublicKey pk = span.GetPubKey(1);
        }
        
        [TestMethod]
        public void TestReadPublicKey()
        {
            ReadOnlySpan<byte> span = PublicKeyBytes.AsSpan();
            PublicKey pk = span.GetPubKey(0);
            
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", pk.Key);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReadBigIntegerException()
        {
            ReadOnlySpan<byte> span = BigIntBytes.AsSpan();
            BigInteger bi = span.GetBigInt(1, 16);
        }
        
        [TestMethod]
        public void TestReadBigInteger()
        {
            BigInteger actual = new(BigIntBytes);
            ReadOnlySpan<byte> span = BigIntBytes.AsSpan();
            BigInteger bi = span.GetBigInt(0, 16);
            Assert.AreEqual(actual, bi);
        }
    }
}