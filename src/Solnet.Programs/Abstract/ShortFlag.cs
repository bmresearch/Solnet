namespace Solnet.Programs.Abstract
{
    /// <summary>
    /// Represents a flag using a short for masking.
    /// </summary>
    public class ShortFlag : Flag<ushort>
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
        /// Check if the 9th bit is set.
        /// </summary>
        public bool Bit8 => IsKthBitSet(Value, 9);

        /// <summary>
        /// Check if the 10th bit is set.
        /// </summary>
        public bool Bit9 => IsKthBitSet(Value, 10);

        /// <summary>
        /// Check if the 11th bit is set.
        /// </summary>
        public bool Bit10 => IsKthBitSet(Value, 11);

        /// <summary>
        /// Check if the 12th bit is set.
        /// </summary>
        public bool Bit11 => IsKthBitSet(Value, 12);

        /// <summary>
        /// Check if the 13th bit is set.
        /// </summary>
        public bool Bit12 => IsKthBitSet(Value, 13);

        /// <summary>
        /// Check if the 14th bit is set.
        /// </summary>
        public bool Bit13 => IsKthBitSet(Value, 14);

        /// <summary>
        /// Check if the 15th bit is set.
        /// </summary>
        public bool Bit14 => IsKthBitSet(Value, 15);

        /// <summary>
        /// Check if the 16th bit is set.
        /// </summary>
        public bool Bit15 => IsKthBitSet(Value, 16);

        /// <summary>
        /// Initialize the flags with the given ushort.
        /// </summary>
        /// <param name="mask">The ushort to use.</param>
        public ShortFlag(ushort mask) : base(mask) { }
    }
}