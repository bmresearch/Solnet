namespace Solnet.KeyStore.Crypto
{
    /// <summary>
    /// Specifies functionality for a random bytes generator.
    /// </summary>
    public interface IRandomBytesGenerator
    {
        
        /// <summary>
        /// Generate a random initialization vector with 16 bytes.
        /// </summary>
        /// <returns>A byte array.</returns>
        byte[] GenerateRandomInitialisationVector();
        
        
        /// <summary>
        /// Generates a random salt with 32 bytes.
        /// </summary>
        /// <returns>A byte array.</returns>
        byte[] GenerateRandomSalt();
    }
}