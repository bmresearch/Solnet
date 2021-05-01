using System;
using NBitcoin;
using Org.BouncyCastle.Security;

namespace Solnet.Wallet
{
    /// <summary>
    /// 
    /// </summary>
    public class Random : IRandom
    {
        /// <summary>
        /// The secure random instance.
        /// </summary>
        public SecureRandom SecureRandom = new SecureRandom();

        public Random(SecureRandom secureRandom = null)
        {
            if (secureRandom != null)
            {
                SecureRandom = secureRandom;
            }
        }

        public void GetBytes(byte[] output)
        {
            SecureRandom.NextBytes(output);
        }

        public void GetBytes(Span<byte> output)
        {
            SecureRandom.NextBytes(output);
        }
    }
}