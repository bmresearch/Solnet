using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBitcoin.DataEncoders;
using Solnet.Rpc.Utilities;

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
            _ = AddressExtensions.CreateProgramAddress(
                new[] { Encoding.UTF8.GetBytes("SeedPubey1111111111111111111111111111111111") },
                Encoding.UTF8.GetBytes(LoaderProgramId));
        }
        
        /*
        [TestMethod]
        public void TestCreateProgramAddress()
        {
            var b58 = new Base58Encoder();
            var programAddress = AddressExtensions.CreateProgramAddress(
                new[] { b58.DecodeData("SeedPubey1111111111111111111111111111111111") },
                Encoding.UTF8.GetBytes(LoaderProgramId));
            
            CollectionAssert.AreEqual(
                Encoding.UTF8.GetBytes("GUs5qLUfsEHkcMB9T38vjr18ypEhRuNWiePW2LoK4E3K"), programAddress);
            
            programAddress = AddressExtensions.CreateProgramAddress(
                new[] { Encoding.UTF8.GetBytes(""), new byte[] { 1 } },
                Encoding.UTF8.GetBytes(LoaderProgramId));
            
            CollectionAssert.AreEqual(
                Encoding.UTF8.GetBytes("3gF2KMe9KiC6FNVBmfg9i267aMPvK37FewCip4eGBFcT"), programAddress);
            
            programAddress = AddressExtensions.CreateProgramAddress(
                new[] {Encoding.UTF8.GetBytes("☉")},
                Encoding.UTF8.GetBytes(LoaderProgramId));
            
            CollectionAssert.AreEqual(
                Encoding.UTF8.GetBytes("7ytmC1nT1xY4RfxCV2ZgyA7UakC93do5ZdyhdF3EtPj7"), programAddress);
        }*/
        
        
        [TestMethod]
        public void TestFindProgramAddress()
        {
            var programAddress = AddressExtensions.FindProgramAddress(
                new[] { Encoding.UTF8.GetBytes("") },
                Encoding.UTF8.GetBytes(LoaderProgramId));

            CollectionAssert.AreEqual(
                programAddress.Address, 
                AddressExtensions.CreateProgramAddress(
                    new[] { Encoding.UTF8.GetBytes(""), new []{ (byte) programAddress.Nonce } },
                    Encoding.UTF8.GetBytes(LoaderProgramId)));
        }
    }
}