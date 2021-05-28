using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
{
    public class ClusterNode
    {
        public string Gossip { get; set; }
        
        [JsonPropertyName("pubkey")]
        public string PublicKey { get; set; }
        
        public string Rpc { get; set; }
        
        public string Tpu { get; set; }
        
        public string Version { get; set; }
        
        public ulong? FeatureSet { get; set; }
        
        public ulong ShredVersion { get; set; }
    }
}