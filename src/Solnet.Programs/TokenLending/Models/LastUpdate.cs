using Solnet.Programs.Utilities;
using System;

namespace Solnet.Programs.TokenLending.Models
{
    /// <summary>
    /// The last update state.
    /// </summary>
    public class LastUpdate
    {
        /// <summary>
        /// The layout of the <see cref="LastUpdate"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the <see cref="LastUpdate"/> structure.
            /// </summary>
            public const int Length = 9;

            /// <summary>
            /// The offset of the slot property.
            /// </summary>
            public const int SlotOffset = 0;

            /// <summary>
            /// The offset of the stale property.
            /// </summary>
            public const int StaleOffset = 8;
        }

        /// <summary>
        /// The slot where the update occurred.
        /// </summary>
        public ulong Slot;

        /// <summary>
        /// True when marked stale, false if the slot is updated.
        /// </summary>
        public bool Stale;

        /// <summary>
        /// Initialize the <see cref="LastUpdate"/> with the given data.
        /// </summary>
        /// <param name="data">The byte array to deserialize.</param>
        public LastUpdate(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"{nameof(data)} has wrong size. Expected {Layout.Length} bytes, actual {data.Length} bytes.");

            Slot = data.GetU64(Layout.SlotOffset);
            Stale = data.GetU8(Layout.StaleOffset) == 1;
        }

        /// <summary>
        /// Deserialize a byte array into the <see cref="LastUpdate"/> structure.
        /// </summary>
        /// <param name="data">The byte array to deserialize.</param>
        /// <returns>The <see cref="LastUpdate"/> structure.</returns>
        public static LastUpdate Deserialize(byte[] data)
             => new(data.AsSpan());
    }
}
