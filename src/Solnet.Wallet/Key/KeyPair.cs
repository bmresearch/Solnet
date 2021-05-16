using NBitcoin.DataEncoders;

namespace Solnet.Wallet.Key
{
    /// <summary>
    /// An Ed25519 key pair.
    /// </summary>
    internal class KeyPair
    {
        /// <summary>
        /// The private key.
        /// </summary>
        internal byte[] PrivateKey { get; init; }
        
        /// <summary>
        /// The public key.
        /// </summary>
        internal byte[] PublicKey { get; init; }
    }
}