using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.KeyStore.Exceptions;

namespace Solnet.KeyStore.Test
{
    [TestClass]
    public class KeyStoreKdfCheckerTest
    {
        private const string ValidKdfCheckerPath = "Resources/ValidKdfType.json";
        private const string InvalidKdfCheckerPath = "Resources/InvalidKdfType.json";

        private static readonly byte[] SeedWithPassphrase =
        {
            163,4,184,24,182,219,174,214,13,54,158,198,
            63,202,76,3,190,224,76,202,160,96,124,95,89,
            155,113,10,46,218,154,74,125,7,103,78,0,51,
            244,192,221,12,200,148,9,252,4,117,193,123,
            102,56,255,105,167,180,125,222,19,111,219,18,
            115,0
        };

        private static readonly SecretKeyStoreService KeyStore = new();

        [TestMethod]
        [ExpectedException(typeof(InvalidKdfException))]
        public void TestInvalidKdf()
        {
            _ = KeyStore.DecryptKeyStoreFromFile("randomPassword", InvalidKdfCheckerPath);
        }
    }
}