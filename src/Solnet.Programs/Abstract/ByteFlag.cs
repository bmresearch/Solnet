namespace Solnet.Programs.Abstract
{
    /// <summary>
    /// Represents a flag using a byte for masking.
    /// </summary>
    public class ByteFlag : Flag<byte>
    {
        /// <summary>
        /// Check if the 1st bit is set.
        /// </summary>
        public bool Bit0 => IsKthBitSet(Value, 1);

        /// <summary>
        /// Check if the 2nd bit is set.
        /// </summary>
        public bool Bit1 => IsKthBitSet(Value, 2);

        /// <summary>
        /// Check if the 3rd bit is set.
        /// </summary>
        public bool Bit2 => IsKthBitSet(Value, 3);

        /// <summary>
        /// Check if the 4th bit is set.
        /// </summary>
        public bool Bit3 => IsKthBitSet(Value, 4);

        /// <summary>
        /// Check if the 5th bit is set.
        /// </summary>
        public bool Bit4 => IsKthBitSet(Value, 5);

        /// <summary>
        /// Check if the 6th bit is set.
        /// </summary>
        public bool Bit5 => IsKthBitSet(Value, 6);

        /// <summary>
        /// Check if the 7th bit is set.
        /// </summary>
        public bool Bit6 => IsKthBitSet(Value, 7);

        /// <summary>
        /// Check if the 8th bit is set.
        /// </summary>
        public bool Bit7 => IsKthBitSet(Value, 8);

        /// <summary>
        /// Initialize the flags with the given byte.
        /// </summary>
        /// <param name="mask">The byte to use.</param>
        public ByteFlag(byte mask) : base(mask) { }
    }
}