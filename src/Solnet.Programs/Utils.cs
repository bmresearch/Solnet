namespace Solnet.Programs
{
    /// <summary>
    /// Utilities class for programs message encoding.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Write 4 bytes to the byte array (starting at the offset) as unsigned 32-bit integer in little endian format.
        /// </summary>
        /// <param name="val">The value to write.</param>
        /// <param name="array">The array to write in.</param>
        /// <param name="offset">The offset at which to start writing.</param>
        public static void Uint32ToByteArrayLe(long val, byte[] array, int offset) {
            array[offset] = (byte) (0xFF & val);
            array[offset + 1] = (byte) (0xFF & (val >> 8));
            array[offset + 2] = (byte) (0xFF & (val >> 16));
            array[offset + 3] = (byte) (0xFF & (val >> 24));
        }

        /// <summary>
        /// Write 8 bytes to the byte array (starting at the offset) as signed 64-bit integer in little endian format.
        /// </summary>
        /// <param name="val">The value to write.</param>
        /// <param name="array">The array to write in.</param>
        /// <param name="offset">The offset at which to start writing.</param>
        public static void Int64ToByteArrayLe(long val, byte[] array, int offset) {
            array[offset] = (byte) (0xFF & val);
            array[offset + 1] = (byte) (0xFF & (val >> 8));
            array[offset + 2] = (byte) (0xFF & (val >> 16));
            array[offset + 3] = (byte) (0xFF & (val >> 24));
            array[offset + 4] = (byte) (0xFF & (val >> 32));
            array[offset + 5] = (byte) (0xFF & (val >> 40));
            array[offset + 6] = (byte) (0xFF & (val >> 48));
            array[offset + 7] = (byte) (0xFF & (val >> 56));
        }
    }
}