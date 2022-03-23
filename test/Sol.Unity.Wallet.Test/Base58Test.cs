using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sol.Unity.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sol.Unity.Wallet.Test
{
    [TestClass]
    public class Base58Test
    {
        public static IEnumerable<object[]> DataSet
        {
            get
            {
                return new[] {
                    new object[]{string.Empty, ""},
                    new object[]{"61", "2g"},
                    new object[]{"626262", "a3gV"},
                    new object[]{"636363", "aPEr"},
                    new object[]{"73696d706c792061206c6f6e6720737472696e67", "2cFupjhnEsSn59qHXstmK2ffpLv2"},
                    new object[]{"00eb15231dfceb60925886b67d065299925915aeb172c06647", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L"},
                    new object[]{"516b6fcd0f", "ABnLTmg"},
                    new object[]{"bf4f89001e670274dd", "3SEo3LWLoPntC"},
                    new object[]{"572e4794", "3EFU7m"},
                    new object[]{"ecac89cad93923c02321", "EJDM8drfXA6uyA"},
                    new object[]{"10c8511e", "Rt5zm"},
                    new object[]{"00000000000000000000", "1111111111"}
                };
            }
        }

        [TestMethod]
        public void ShouldEncodeProperly()
        {
            foreach (var i in DataSet)
            {
                string data = (string)i[0];
                string encoded = (string)i[1];
                var testBytes = FromHexString(data);
                Assert.AreEqual(encoded, Encoders.Base58.EncodeData(testBytes));
            }
        }



        [TestMethod]
        public void ShouldDecodeProperly()
        {
            foreach (var i in DataSet)
            {
                string data = (string)i[0];
                string encoded = (string)i[1];
                var testBytes = Encoders.Base58.DecodeData(encoded);
                CollectionAssert.AreEqual(FromHexString(data), testBytes);
            }
        }

        [TestMethod]
        public void ShouldThrowFormatExceptionOnInvalidBase58()
        {
            try
            {
                _ = Encoders.Base58.DecodeData("invalid");
                Assert.Fail("invalid base58 string");
            }
            catch
            {
            }

            try
            {
                _ = Encoders.Base58.DecodeData(" \t\n\v\f\r skip \r\f\v\n\t a");
                Assert.Fail("invalid base58 string");
            }
            catch
            {
            }

            var result = Encoders.Base58.DecodeData(" \t\n\v\f\r skip \r\f\v\n\t ");
            var expected2 = FromHexString("971a55");
            CollectionAssert.AreEqual(result, expected2);
        }


        public static byte[] FromHexString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return bytes; // returns: "Hello world" for "48656C6C6F20776F726C64"
        }

    }
}