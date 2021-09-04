namespace Solnet.Programs.Abstract
{
    /// <summary>
    /// Represents bitmask flags for various types of accounts within Solana Programs.
    /// </summary>
    public abstract class Flag<T>
    {
        /// <summary>
        /// The mask for the account flags.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Initialize the flags with the given mask.
        /// </summary>
        /// <param name="mask">The mask to use.</param>
        protected Flag(T mask)
        {
            Value = mask;
        }

        /// <summary>
        /// Checks whether the Kth bit for a given number N is set.
        /// </summary>
        /// <param name="n">The number to check against.</param>
        /// <param name="k">The bit to check.</param>
        /// <returns>true if it is, otherwise false.</returns>
        protected static bool IsKthBitSet(ulong n, int k) => (n & (1UL << (k - 1))) > 0;
    }
}