using System;

namespace Solnet.KeyStore.Crypto
{
    /// <summary>
    /// Specifies an exception thrown during decryption.
    /// </summary>
    public class DecryptionException : Exception
    {

        /// <summary>
        /// Initialize the exception.
        /// </summary>
        /// <param name="msg">The message of the exception.</param>
        internal DecryptionException(string msg) : base(msg)
        {
        }
        
    }
}