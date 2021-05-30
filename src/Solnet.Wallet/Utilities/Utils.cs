namespace Solnet.Wallet.Utilities
{
    public static class Utils
    {
        
        
        /// <summary>
        /// Slices the array, returning a new array starting at <c>start</c> index and ending at <c>end</c> index.
        /// </summary>
        /// <param name="source">The array to slice.</param>
        /// <param name="start">The starting index of the slicing.</param>
        /// <param name="end">The ending index of the slicing.</param>
        /// <typeparam name="T">The array type.</typeparam>
        /// <returns>The sliced array.</returns>
        public static T[] Slice<T>(this T[] source, int start, int end)
        {
            if (end < 0)
                end = source.Length;

            var len = end - start;

            // Return new array.
            var res = new T[len];
            for (var i = 0; i < len; i++) res[i] = source[i + start];
            return res;
        }

        /// <summary>
        /// Slices the array, returning a new array starting at <c>start</c>.
        /// </summary>
        /// <param name="source">The array to slice.</param>
        /// <param name="start">The starting index of the slicing.</param>
        /// <typeparam name="T">The array type.</typeparam>
        /// <returns>The sliced array.</returns>
        public static T[] Slice<T>(this T[] source, int start)
        {
            return Slice<T>(source, start, -1);
        }
    }
}