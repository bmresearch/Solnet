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
    }
}