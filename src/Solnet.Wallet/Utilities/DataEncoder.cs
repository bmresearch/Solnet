namespace Solnet.Wallet.Utilities
{
    /// <summary>
    /// Abstract data encoder class.
    /// </summary>
    public abstract class DataEncoder
    {
        /// <summary>
        /// Check if the character is a space...
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>True if it is, otherwise false.</returns>
        public static bool IsSpace(char c)
        {
            switch (c)
            {
                case ' ':
                case '\t':
                case '\n':
                case '\v':
                case '\f':
                case '\r':
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Initialize the data encoder.
        /// </summary>
        internal DataEncoder()
        {
        }

        /// <summary>
        /// Encode the data.
        /// </summary>
        /// <param name="data">The data to encode.</param>
        /// <returns>The data encoded.</returns>
        public string EncodeData(byte[] data)
        {
            return EncodeData(data, 0, data.Length);
        }

        /// <summary>
        /// Encode the data.
        /// </summary>
        /// <param name="data">The data to encode.</param>
        /// <param name="offset">The offset at which to start encoding.</param>
        /// <param name="count">The number of bytes to encode.</param>
        /// <returns>The encoded data.</returns>
        public abstract string EncodeData(byte[] data, int offset, int count);

        /// <summary>
        /// Decode the data.
        /// </summary>
        /// <param name="encoded">The data to decode.</param>
        /// <returns>The decoded data.</returns>
        public abstract byte[] DecodeData(string encoded);
    }

    /// <summary>
    /// A static encoder instance.
    /// </summary>
    public static class Encoders
    {
        /// <summary>
        /// The encoder.
        /// </summary>
        private static readonly Base58Encoder _base58 = new();

        /// <summary>
        /// The encoder.
        /// </summary>
        public static DataEncoder Base58 => _base58;
    }
}