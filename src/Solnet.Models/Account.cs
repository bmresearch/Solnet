using System;
using System.Numerics;

namespace Solnet.Models
{
    /// <summary>
    /// Specifies an <see cref="Account"/> within the Solana ecossystem.
    /// <para>
    /// This account can be characterized by a pair of public and private keys.
    /// </para>
    /// </summary>
    public class Account
    {
        /// <summary>
        /// 
        /// </summary>
        public BigInteger Nonce { get; set; }
        
        /// <summary>
        /// A scalar value equal to the amount of SOL owned by the <see cref="Account"/>.
        /// </summary>
        public BigInteger Balance { get; set; }
        
        /// <summary>
        /// A byte-array that represents the <see cref="Account"/>'s address' public key.
        /// </summary>
        public byte[] PublicKey { get; set; }
        
        
        /// <summary>
        /// A byte-array that represents the <see cref="Account"/>'s address' private key.
        /// </summary>
        public byte[] PrivateKey { get; set; }
    }
}