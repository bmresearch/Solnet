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
        {
            6, 221, 246, 225, 215, 101, 161, 147, 217, 203,
            225, 70, 206, 235, 121, 172, 28, 180, 133, 237,
            95, 91, 55, 145, 58, 140, 245, 133, 126, 255, 0, 169
        };
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWriteU8Exception()
        {
            byte[] sut = new byte[1];
            sut.WriteU8(1, 2);
        }
        
        [TestMethod]
        public void TestWriteU8()
        {
            byte[] sut = new byte[1];
            sut.WriteU8(1, 0);
            CollectionAssert.AreEqual(new byte[]{ 1 }, sut);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWriteU16Exception()
        {
            byte[] sut = new byte[2];
            sut.WriteU16(1, 2);
        }

        [TestMethod]
        public void TestWriteU16()
        {
            byte[] sut = new byte[2];
            sut.WriteU16(1, 0);
            CollectionAssert.AreEqual(new byte[]{ 1, 0 }, sut);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWriteU32Exception()
        {
            byte[] sut = new byte[4];
            sut.WriteU32(1, 4);
        }

        [TestMethod]
        public void TestWriteU32()
        {
            byte[] sut = new byte[4];
            sut.WriteU32(1, 0);
            CollectionAssert.AreEqual(new byte[]{1, 0, 0, 0}, sut);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWriteU64Exception()
        {
            byte[] sut = new byte[8];
            sut.WriteU64(1, 8);
        }

        [TestMethod]
        public void TestWriteU64()
        {
            byte[] sut = new byte[8];
            sut.WriteU64(1, 0);
            CollectionAssert.AreEqual(new byte[]{1, 0, 0, 0, 0, 0, 0, 0}, sut);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWriteS8Exception()
        {
            byte[] sut = new byte[1];
            sut.WriteS8(1, 2);
        }
        
        [TestMethod]
        public void TestWriteS8()
        {
            byte[] sut = new byte[1];
            sut.WriteS8(1, 0);
            CollectionAssert.AreEqual(new byte[]{1}, sut);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWriteS16Exception()
        {
            byte[] sut = new byte[2];
            sut.WriteS16(1, 1);
        }

        [TestMethod]
        public void TestWriteS16()
        {
            byte[] sut = new byte[2];
            sut.WriteS16(1, 0);
            CollectionAssert.AreEqual(new byte[]{1, 0}, sut);
        }
                 
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWriteS32Exception()
        {
            byte[] sut = new byte[4];
            sut.WriteS32(1, 1);
        }
       
        [TestMethod]
        public void TestWriteS32()
        {
            byte[] sut = new byte[4];
            sut.WriteS32(1, 0);
            CollectionAssert.AreEqual(new byte[]{1, 0, 0, 0}, sut);
        }
                    
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWriteS64Exception()
        {
            byte[] sut = new byte[8];
            sut.WriteS64(1, 1);
        }
    
        [TestMethod]
        public void TestWriteS64()
        {
            byte[] sut = new byte[8];
            sut.WriteS64(1, 0);
            CollectionAssert.AreEqual(new byte[]{1, 0, 0, 0, 0, 0, 0, 0}, sut);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWriteSpanException()
        {
            byte[] sut = new byte[32];
            sut.WriteSpan(PublicKeyBytes, 1);
        }

        [TestMethod]
        public void TestWriteSpan()
        {
            byte[] sut = new byte[32];
            sut.WriteSpan(PublicKeyBytes, 0);
            CollectionAssert.AreEqual(PublicKeyBytes, sut);
        }
                
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWritePublicKeyException()
        {
            byte[] sut = new byte[32];
            sut.WritePubKey(new PublicKey(PublicKeyBytes), 1);
        }

        [TestMethod]
        public void TestWritePublicKey()
        {
            byte[] sut = new byte[32];
            sut.WritePubKey(new PublicKey(PublicKeyBytes), 0);
            CollectionAssert.AreEqual(PublicKeyBytes, sut);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWriteBigIntegerException()
        {
            byte[] sut = new byte[16];
            sut.WriteBigInt(new BigInteger(15000000000000000000000000D), 8);
        }
        
        [TestMethod]
        public void TestWriteBigInteger()
        {
            byte[] sut = new byte[16];
            BigInteger bi = BigInteger.Parse("34028236692093846346337460743176821145");
            
            int written = sut.WriteBigInt(bi, 0);
            
            Assert.AreEqual(bi.GetByteCount(), written);
            CollectionAssert.AreEqual(new byte[]
            {
                153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 25,
            }, sut);
        }
    }
}