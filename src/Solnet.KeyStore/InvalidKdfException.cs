using System;

namespace Solnet.KeyStore
{
    /// <summary>
    /// Represents an exception which is used when the key derivation function is invalid.
    /// </summary>
    public class InvalidKdfException : Exception
    {
        
        /// <summary>
        /// Initializes the exception for the passed kdf.
        /// </summary>
        /// <param name="kdf"></param>
        public InvalidKdfException(string kdf) : base($"Invalid kdf: {kdf}")
        {
        }

    }
}