using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs.Abstract;

namespace Solnet.Programs.Test.Abstract
{
    [TestClass]
    public class ByteFlagTest
    {
        [TestMethod]
        public void TestBit0()
        {
            ByteFlag sut = new (1);
            Assert.AreEqual(1, sut.Value);
            Assert.IsTrue(sut.Bit0);
            Assert.IsFalse(sut.Bit1);
            Assert.IsFalse(sut.Bit2);
            Assert.IsFalse(sut.Bit3);
            Assert.IsFalse(sut.Bit4);
            Assert.IsFalse(sut.Bit5);
            Assert.IsFalse(sut.Bit6);
            Assert.IsFalse(sut.Bit7);
        }
        
        [TestMethod]
        public void TestBit1()
        {
            ByteFlag sut = new (2);
            Assert.AreEqual(2, sut.Value);
            Assert.IsFalse(sut.Bit0);
            Assert.IsTrue(sut.Bit1);
            Assert.IsFalse(sut.Bit2);
            Assert.IsFalse(sut.Bit3);
            Assert.IsFalse(sut.Bit4);
            Assert.IsFalse(sut.Bit5);
            Assert.IsFalse(sut.Bit6);
            Assert.IsFalse(sut.Bit7);
        }
        
        [TestMethod]
        public void TestBit2()
        {
            ByteFlag sut = new (4);
            Assert.AreEqual(4, sut.Value);
            Assert.IsFalse(sut.Bit0);
            Assert.IsFalse(sut.Bit1);
            Assert.IsTrue(sut.Bit2);
            Assert.IsFalse(sut.Bit3);
            Assert.IsFalse(sut.Bit4);
            Assert.IsFalse(sut.Bit5);
            Assert.IsFalse(sut.Bit6);
            Assert.IsFalse(sut.Bit7);
        }
        
        [TestMethod]
        public void TestBit3()
        {
            ByteFlag sut = new (8);
            Assert.AreEqual(8, sut.Value);
            Assert.IsFalse(sut.Bit0);
            Assert.IsFalse(sut.Bit1);
            Assert.IsFalse(sut.Bit2);
            Assert.IsTrue(sut.Bit3);
            Assert.IsFalse(sut.Bit4);
            Assert.IsFalse(sut.Bit5);
            Assert.IsFalse(sut.Bit6);
            Assert.IsFalse(sut.Bit7);
        }
        
        [TestMethod]
        public void TestBit4()
        {
            ByteFlag sut = new (16);
            Assert.AreEqual(16, sut.Value);
            Assert.IsFalse(sut.Bit0);
            Assert.IsFalse(sut.Bit1);
            Assert.IsFalse(sut.Bit2);
            Assert.IsFalse(sut.Bit3);
            Assert.IsTrue(sut.Bit4);
            Assert.IsFalse(sut.Bit5);
            Assert.IsFalse(sut.Bit6);
            Assert.IsFalse(sut.Bit7);
        }
        
        [TestMethod]
        public void TestBit5()
        {
            ByteFlag sut = new (32);
            Assert.AreEqual(32, sut.Value);
            Assert.IsFalse(sut.Bit0);
            Assert.IsFalse(sut.Bit1);
            Assert.IsFalse(sut.Bit2);
            Assert.IsFalse(sut.Bit3);
            Assert.IsFalse(sut.Bit4);
            Assert.IsTrue(sut.Bit5);
            Assert.IsFalse(sut.Bit6);
            Assert.IsFalse(sut.Bit7);
        }
        
        [TestMethod]
        public void TestBit6()
        {
            ByteFlag sut = new (64);
            Assert.AreEqual(64, sut.Value);
            Assert.IsFalse(sut.Bit0);
            Assert.IsFalse(sut.Bit1);
            Assert.IsFalse(sut.Bit2);
            Assert.IsFalse(sut.Bit3);
            Assert.IsFalse(sut.Bit4);
            Assert.IsFalse(sut.Bit5);
            Assert.IsTrue(sut.Bit6);
            Assert.IsFalse(sut.Bit7);
        }
        
        [TestMethod]
        public void TestBit7()
        {
            ByteFlag sut = new (128);
            Assert.AreEqual(128, sut.Value);
            Assert.IsFalse(sut.Bit0);
            Assert.IsFalse(sut.Bit1);
            Assert.IsFalse(sut.Bit2);
            Assert.IsFalse(sut.Bit3);
            Assert.IsFalse(sut.Bit4);
            Assert.IsFalse(sut.Bit5);
            Assert.IsFalse(sut.Bit6);
            Assert.IsTrue(sut.Bit7);
        }
    }
}