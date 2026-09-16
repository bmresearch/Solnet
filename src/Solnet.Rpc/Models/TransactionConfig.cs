namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents the configuration options for a transaction.
    /// </summary>
    public sealed class TransactionConfig
    {
        /// <summary>
        /// Gets or sets the priority fee for the transaction.
        /// </summary>
        public ulong? PriorityFee { get; set; }

        /// <summary>
        /// Gets or sets the compute unit limit for the transaction.
        /// </summary>
        public uint? ComputeUnitLimit { get; set; }

        /// <summary>
        /// Gets or sets the loaded accounts data size limit for the transaction.
        /// </summary>
        public uint? LoadedAccountsDataSizeLimit { get; set; }

        /// <summary>
        /// Gets or sets the heap size for the transaction.
        /// </summary>
        public uint? HeapSize { get; set; }

        /// <summary>
        /// Calculates the size of the transaction configuration based on the set options.
        /// </summary>
        /// <returns>The size of the transaction configuration in bytes.</returns>
        public int Size()
        {
            int size = 0;

            if (PriorityFee.HasValue)
                size += sizeof(ulong);

            if (ComputeUnitLimit.HasValue)
                size += sizeof(uint);

            if (LoadedAccountsDataSizeLimit.HasValue)
                size += sizeof(uint);

            if (HeapSize.HasValue)
                size += sizeof(uint);

            return size;
        }
    }

    /// <summary>
    /// Represents a mask for the transaction configuration options.
    /// </summary>
    /// 
    public readonly struct TransactionConfigMask
    {
        /// <summary>
        /// Represents the mask for the priority fee option.
        /// </summary>
        public const uint PriorityFee = 0b11;
        /// <summary>
        /// Represents the mask for the compute unit limit option.
        /// </summary>
        public const uint ComputeUnitLimit = 0b100;
        
        /// <summary>
        /// Represents the mask for the loaded accounts data size option.
        /// </summary>
        public const uint LoadedAccountsDataSize = 0b1000;
        /// <summary>
        /// Represents the mask for the heap size option.
        /// </summary>
        public const uint HeapSize = 0b10000;

        /// <summary>
        /// Represents the mask for all known transaction configuration options.
        /// </summary>
        public const uint KnownBits =
            PriorityFee |
            ComputeUnitLimit |
            LoadedAccountsDataSize |
            HeapSize;

        /// <summary>
        /// Gets the value of the transaction configuration mask.
        /// </summary>
        public uint Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionConfigMask"/> struct with the specified value.
        /// </summary>
        /// <param name="value"></param>
        public TransactionConfigMask(uint value)
        {
            Value = value;
        }
        /// <summary>
        /// Creates a <see cref="TransactionConfigMask"/> from a <see cref="TransactionConfig"/> instance.
        /// </summary>
        /// <param name="config">The <see cref="TransactionConfig"/> instance to create the mask from.</param>
        /// <returns>A <see cref="TransactionConfigMask"/> representing the set options in the provided <see cref="TransactionConfig"/>.</returns>
        public static TransactionConfigMask FromConfig(TransactionConfig config)
        {
            uint mask = 0;

            if (config.PriorityFee.HasValue)
                mask |= PriorityFee;

            if (config.ComputeUnitLimit.HasValue)
                mask |= ComputeUnitLimit;

            if (config.LoadedAccountsDataSizeLimit.HasValue)
                mask |= LoadedAccountsDataSize;

            if (config.HeapSize.HasValue)
                mask |= HeapSize;

            return new TransactionConfigMask(mask);
        }
        /// <summary>
        /// Determines whether the transaction configuration mask contains any unknown bits.
        /// </summary>
        /// <returns></returns>
        public bool HasUnknownBits()
        {
            return (Value | KnownBits) != KnownBits;
        }
        /// <summary>
        /// Determines whether the transaction configuration mask has invalid priority fee bits.
        /// </summary>
        /// <returns></returns>
        public bool HasInvalidPriorityFeeBits()
        {
            uint bits = Value & PriorityFee;

            return bits != 0 &&
                   bits != PriorityFee;
        }

        /// <summary>
        /// Determines whether the transaction configuration mask has a priority fee set.
        /// </summary>
        /// <returns></returns>
        public bool HasPriorityFee()
        {
            return (Value & PriorityFee) == PriorityFee;
        }
        /// <summary>
        /// Determines whether the transaction configuration mask has a compute unit limit set.
        /// </summary>
        /// <returns></returns>
        public bool HasComputeUnitLimit()
        {
            return (Value & ComputeUnitLimit) != 0;
        }
        /// <summary>
        /// Determines whether the transaction configuration mask has a loaded accounts data size limit set.
        /// </summary>
        /// <returns></returns>
        public bool HasLoadedAccountsDataSize()
        {
            return (Value & LoadedAccountsDataSize) != 0;
        }
        /// <summary>
        /// Determines whether the transaction configuration mask has a heap size set.
        /// </summary>
        /// <returns></returns>
        public bool HasHeapSize()
        {
            return (Value & HeapSize) != 0;
        }
        /// <summary>
        /// Calculates the size of the transaction configuration based on the set options in the mask.
        /// </summary>
        /// <returns></returns>
        public int SizeOfConfig()
        {
            int size = 0;

            if (HasPriorityFee())
                size += sizeof(ulong);

            if (HasComputeUnitLimit())
                size += sizeof(uint);

            if (HasLoadedAccountsDataSize())
                size += sizeof(uint);

            if (HasHeapSize())
                size += sizeof(uint);

            return size;
        }
    }

}
