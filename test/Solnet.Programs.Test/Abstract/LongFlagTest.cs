using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs.Abstract;
using System;
using System.Reflection;

namespace Solnet.Programs.Test.Abstract
{
    [TestClass]
    public class LongFlagTest
    {
        [TestMethod]
        public void TestAllBitsSet()
        {
            LongFlag sut = new(ulong.MaxValue);

            PropertyInfo[] props = sut.GetType().GetProperties();

            foreach (PropertyInfo prop in props)
            {
                MethodInfo getMethod = prop.GetGetMethod();
                Assert.IsNotNull(getMethod);

                string methodName = getMethod.ToString();
                Assert.IsNotNull(methodName);

                if (!methodName.Contains("Bit"))
                    continue;

                object isBitSet = getMethod.Invoke(sut, null);
                Assert.IsNotNull(isBitSet);
                Assert.IsTrue((bool)isBitSet);
            }
        }

        [TestMethod]
        public void TestNoBitsSet()
        {
            LongFlag sut = new(ulong.MinValue);

            PropertyInfo[] props = sut.GetType().GetProperties();

            foreach (PropertyInfo prop in props)
            {
                MethodInfo getMethod = prop.GetGetMethod();
                Assert.IsNotNull(getMethod);

                string methodName = getMethod.ToString();
                Assert.IsNotNull(methodName);

                if (!methodName.Contains("Bit"))
                    continue;

                object isBitSet = getMethod.Invoke(sut, null);
                Assert.IsNotNull(isBitSet);
                Assert.IsFalse((bool)isBitSet);
            }
        }

        [TestMethod]
        public void TestIndividualBitSet()
        {
            PropertyInfo[] props = typeof(LongFlag).GetProperties();

            foreach (PropertyInfo prop in props)
            {
                MethodInfo getMethod = prop.GetGetMethod();
                Assert.IsNotNull(getMethod);

                string methodName = getMethod.ToString();
                Assert.IsNotNull(methodName);

                if (!methodName.Contains("Bit"))
                    continue;

                byte bitNumber = byte.Parse(methodName.Split("_Bit")[1].Split("()")[0]);
                double bitMaskValue = Math.Pow(2, bitNumber);

                LongFlag sut = new((ulong)bitMaskValue);

                object isBitSet = getMethod.Invoke(sut, null);
                Assert.IsNotNull(isBitSet);
                Assert.IsTrue((bool)isBitSet);
            }
        }
    }
}