namespace Solnet.Rpc.Core
{
    /// <summary>
    /// Id generator.
    /// </summary>
    internal class IdGenerator
    {

        /// <summary>
        /// The id of the last request performed
        /// </summary>
        private int _id;

        /// <summary>
        /// Gets the id of the next request.
        /// </summary>
        /// <returns>The id.</returns>
        internal int GetNextId()
        {
            lock (this)
            {
                return _id++;
            }
        }
    }
}