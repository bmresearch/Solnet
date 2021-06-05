using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Solnet.Wallet.Test
{
    [TestClass]
    public class Ed25519Bip32Test
    {
        /// <summary>
        /// The valid derivation path.
        /// </summary>
        private const string DerivationPath = "m/44'/501'/0'/0'";

        /// <summary>
        /// The invalid derivation path.
        /// </summary>
        private const string InvalidDerivationPath = "m44/'501'//0'/0'";

        private static readonly byte[] SeedWithoutPassphrase =
        {
            124,36,217,106,151,19,165,102,96,101,74,81,
            237,254,232,133,28,167,31,35,119,188,66,40,
            101,104,25,103,139,83,57,7,19,215,6,113,22,
            145,107,209,208,107,159,40,223,19,82,53,136,
            255,40,171,137,93,9,205,28,7,207,88,194,91,
            219,232
        };

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestDerivePath()
        {
            var ed25519 = new Ed25519Bip32(SeedWithoutPassphrase);
            _ = ed25519.DerivePath(InvalidDerivationPath);
        }
    }
}