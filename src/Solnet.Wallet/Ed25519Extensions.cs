using Chaos.NaCl;

namespace Solnet.Wallet
{
    public static class Ed25519Extensions
    {
        /// <summary>
        /// Gets the corresponding ed25519 key pair from the passed seed.
        /// </summary>
        /// <param name="seed">The seed</param>
        /// <returns>The key pair.</returns>
        public static (byte[] privateKey, byte[] publicKey) EdKeyPairFromSeed(byte[] seed) =>
            new(Ed25519.ExpandedPrivateKeyFromSeed(seed), Ed25519.PublicKeyFromSeed(seed));
    }
}