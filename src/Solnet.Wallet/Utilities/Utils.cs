using Bifrost.Security;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Solnet.Wallet.Utilities
{
    /// <summary>
    /// Implements utility methods to be used in the wallet.
    /// </summary>
    internal static class Utils
    {

        /// <summary>
        /// Gets the corresponding ed25519 key pair from the passed seed.
        /// </summary>
        /// <param name="seed">The seed</param>
        /// <returns>The key pair.</returns>
        internal static (byte[] privateKey, byte[] publicKey) EdKeyPairFromSeed(byte[] seed) =>
            new(Ed25519.ExpandedPrivateKeyFromSeed(seed), Ed25519.PublicKeyFromSeed(seed));

    }
}
