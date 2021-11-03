using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Rpc.Utilities;
using Solnet.Wallet.Utilities;
using System;
using System.Text;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class UtilitiesTest
    {
        private const string LoaderProgramId = "BPFLoader1111111111111111111111111111111111";


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCreateProgramAddressException()
        {
            _ = AddressExtensions.TryCreateProgramAddress(
                new[] { Encoding.UTF8.GetBytes("SeedPubey1111111111111111111111111111111111") },
                Encoding.UTF8.GetBytes(LoaderProgramId), out _);
        }

        [TestMethod]
        public void TestCreateProgramAddress()
        {
            var b58 = new Base58Encoder();
            var success = AddressExtensions.TryCreateProgramAddress(
                new[] { b58.DecodeData("SeedPubey1111111111111111111111111111111111") },
                b58.DecodeData(LoaderProgramId), out byte[] pubKey);

            Assert.IsTrue(success);
            CollectionAssert.AreEqual(
                b58.DecodeData("GUs5qLUfsEHkcMB9T38vjr18ypEhRuNWiePW2LoK4E3K"), pubKey);

            success = AddressExtensions.TryCreateProgramAddress(
                new[] { Encoding.UTF8.GetBytes(""), new byte[] { 1 } },
                b58.DecodeData(LoaderProgramId), out pubKey);

            Assert.IsTrue(success);
            CollectionAssert.AreEqual(
                b58.DecodeData("3gF2KMe9KiC6FNVBmfg9i267aMPvK37FewCip4eGBFcT"), pubKey);

            success = AddressExtensions.TryCreateProgramAddress(
                new[] { Encoding.UTF8.GetBytes("☉") },
                b58.DecodeData(LoaderProgramId), out pubKey);

            Assert.IsTrue(success);
            CollectionAssert.AreEqual(
                b58.DecodeData("7ytmC1nT1xY4RfxCV2ZgyA7UakC93do5ZdyhdF3EtPj7"), pubKey);
        }

        [TestMethod]
        public void TestFindProgramAddress()
        {
            var tryFindSuccess = AddressExtensions.TryFindProgramAddress(
                new[] { Encoding.UTF8.GetBytes("") },
                Encoding.UTF8.GetBytes(LoaderProgramId), out byte[] derivedAddress, out int derivationNonce);
            Assert.IsTrue(tryFindSuccess);

            var createProgSuccess = AddressExtensions.TryCreateProgramAddress(
                new[] { Encoding.UTF8.GetBytes(""), new[] { (byte)derivationNonce } },
                Encoding.UTF8.GetBytes(LoaderProgramId), out byte[] pubKey);
            Assert.IsTrue(createProgSuccess);
            CollectionAssert.AreEqual(
                derivedAddress, pubKey);
        }

    }
}