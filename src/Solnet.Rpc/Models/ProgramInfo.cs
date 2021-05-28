// ReSharper disable ClassNeverInstantiated.Global

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents the program info.
    /// </summary>
    public class ProgramInfo
    {
        /// <summary>
        /// The base-58 encoded public key of the program.
        /// </summary>
        public string PubKey { get; set; }

        /// <summary>
        /// The account info associated with the program.
        /// </summary>
        public AccountInfo Account { get; set; }
    }
}
