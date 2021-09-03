namespace Solnet.Programs.Abstract
{

    /// <summary>
    /// Represents a flag using a long for masking.
    /// </summary>
    public class IntFlag : Flag<uint>
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
        /// Check if the 17th bit is set.
        /// </summary>
        public bool Bit16 => IsKthBitSet(Value, 17);

        /// <summary>
        /// Check if the 18th bit is set.
        /// </summary>
        public bool Bit17 => IsKthBitSet(Value, 18);

        /// <summary>
        /// Check if the 19th bit is set.
        /// </summary>
        public bool Bit18 => IsKthBitSet(Value, 19);

        /// <summary>
        /// Check if the 20th bit is set.
        /// </summary>
        public bool Bit19 => IsKthBitSet(Value, 20);

        /// <summary>
        /// Check if the 21st bit is set.
        /// </summary>
        public bool Bit20 => IsKthBitSet(Value, 21);

        /// <summary>
        /// Check if the 22nd bit is set.
        /// </summary>
        public bool Bit21 => IsKthBitSet(Value, 22);

        /// <summary>
        /// Check if the 23rd bit is set.
        /// </summary>
        public bool Bit22 => IsKthBitSet(Value, 23);

        /// <summary>
        /// Check if the 24th bit is set.
        /// </summary>
        public bool Bit23 => IsKthBitSet(Value, 24);

        /// <summary>
        /// Check if the 25th bit is set.
        /// </summary>
        public bool Bit24 => IsKthBitSet(Value, 25);

        /// <summary>
        /// Check if the 26th bit is set.
        /// </summary>
        public bool Bit25 => IsKthBitSet(Value, 26);

        /// <summary>
        /// Check if the 27th bit is set.
        /// </summary>
        public bool Bit26 => IsKthBitSet(Value, 27);

        /// <summary>
        /// Check if the 28th bit is set.
        /// </summary>
        public bool Bit27 => IsKthBitSet(Value, 28);

        /// <summary>
        /// Check if the 29th bit is set.
        /// </summary>
        public bool Bit28 => IsKthBitSet(Value, 29);

        /// <summary>
        /// Check if the 30th bit is set.
        /// </summary>
        public bool Bit29 => IsKthBitSet(Value, 30);

        /// <summary>
        /// Check if the 31st bit is set.
        /// </summary>
        public bool Bit30 => IsKthBitSet(Value, 31);

        /// <summary>
        /// Check if the 32nd bit is set.
        /// </summary>
        public bool Bit31 => IsKthBitSet(Value, 32);

        /// <summary>
        /// Initialize the flags with the given uint.
        /// </summary>
        /// <param name="mask">The uint to use.</param>
        public IntFlag(uint mask) : base(mask) { }
    }
}