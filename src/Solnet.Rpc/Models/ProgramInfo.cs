// ReSharper disable ClassNeverInstantiated.Global

using System.Text.Json.Serialization;

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
        [JsonPropertyName("pubkey")]
        public string PublicKey { get; set; }

        /// <summary>
        /// The account info associated with the program.
        /// </summary>
        public ProgramData Account { get; set; }
    }

    /// <summary>
    /// Represents a program account details.
    /// </summary>
    public class ProgramData : AccountInfoBase
    {
        /// <summary>
        /// Contains the data associated with a given program.
        /// </summary>
        public string Data { get; set; }
    }
}
