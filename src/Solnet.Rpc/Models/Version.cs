using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents the current solana versions running on the node.
    /// </summary>
    public class NodeVersion
    {
        /// <summary>
        /// Software version of solana-core.
        /// </summary>
        [JsonPropertyName("solana-core")]
        public string SolanaCore { get; set; }

        /// <summary>
        /// unique identifier of the current software's feature set.
        /// </summary>
        [JsonPropertyName("feature-set")]
        public ulong? FeatureSet { get; set; }
    }
}