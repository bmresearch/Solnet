using System;

namespace Solnet.Programs.StakePool.Models
{
    /// <summary>
    /// Wrapper type that "counts down" epochs, similar to Rust's Option, with three states.
    /// </summary>
    /// <typeparam name="T">The type of the value being wrapped.</typeparam>
    public abstract class FutureEpoch<T>
    {
        private FutureEpoch() { }

        /// <summary>
        /// Represents the None state (no value set).
        /// </summary>
        public sealed class None : FutureEpoch<T>
        {
            internal None() { }
        }

        /// <summary>
        /// Value is ready after the next epoch boundary.
        /// </summary>
        public sealed class One : FutureEpoch<T>
        {
            /// <summary>
            /// Gets the value stored in the current instance.
            /// </summary>
            public T Value { get; }
            /// <summary>
            /// Initializes a new instance of the <see cref="One{T}"/> class with the specified value.
            /// </summary>
            /// <param name="value">The value to be assigned to the instance.</param>
            public One(T value) => Value = value;
        }

        /// <summary>
        /// Value is ready after two epoch boundaries.
        /// </summary>
        public sealed class Two : FutureEpoch<T>
        {
            /// <summary>
            /// Gets the value stored in the current instance.
            /// </summary>
            public T Value { get; }
            /// <summary>
            /// Initializes a new instance of the <see cref="FutureEpoch{T}.Two"/> class with the specified value.
            /// </summary>
            /// <param name="value">The value to initialize the instance with.</param>
            public Two(T value) => Value = value;
        }

        /// <summary>
        /// Create a new value to be unlocked in two epochs.
        /// </summary>
        public static FutureEpoch<T> New(T value) => new Two(value);

        /// <summary>
        /// Returns the value if it's ready (i.e., in the One state), otherwise null.
        /// </summary>
        public T? Get()
        {
            return this is One one ? one.Value : default;
        }

        /// <summary>
        /// Update the epoch, to be done after getting the underlying value.
        /// </summary>
        public FutureEpoch<T> UpdateEpoch()
        {
            return this switch
            {
                None => this,
                One => new None(),
                Two two => new One(two.Value),
                _ => throw new InvalidOperationException()
            };
        }

        /// <summary>
        /// Converts the FutureEpoch to an Option-like value (null if None, value otherwise).
        /// </summary>
        public T? ToOption()
        {
            return this switch
            {
                None => default,
                One one => one.Value,
                Two two => two.Value,
                _ => default
            };
        }

        /// <summary>
        /// Returns a None instance.
        /// </summary>
        public static FutureEpoch<T> NoneValue { get; } = new None();
    }
}