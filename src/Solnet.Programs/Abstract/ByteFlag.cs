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
        public bool Bit0 => (Value & 0x01) != 0;
        
        /// <summary>
        /// Check if the 2nd bit is set.
        /// </summary>
        public bool Bit1 => (Value & 0x02) != 0;
        
        /// <summary>
        /// Check if the 3rc bit is set.
        /// </summary>
        public bool Bit2 => (Value & 0x04) != 0;
        
        /// <summary>
        /// Check if the 4th bit is set.
        /// </summary>
        public bool Bit3 => (Value & 0x08) != 0;
        
        /// <summary>
        /// Check if the 5th bit is set.
        /// </summary>
        public bool Bit4 => (Value & 0x10) != 0;
        
        /// <summary>
        /// Check if the 6th bit is set.
        /// </summary>
        public bool Bit5 => (Value & 0x20) != 0;
        
        /// <summary>
        /// Check if the 7th bit is set.
        /// </summary>
        public bool Bit6 => (Value & 0x40) != 0;
        
        /// <summary>
        /// Check if the 8th bit is set.
        /// </summary>
        public bool Bit7 => (Value & 0x80) != 0;
        
        /// <summary>
        /// Initialize the flags with the given byte.
        /// </summary>
        /// <param name="mask">The byte to use.</param>
        public ByteFlag(byte mask) : base(mask) { }
    }
}