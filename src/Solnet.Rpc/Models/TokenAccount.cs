using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
{    
    public class TokenAccount
    {
        public AccountInfo Account { get; set; }
        
        [JsonPropertyName("pubkey")]
        public string PublicKey { get; set; }
    }
}