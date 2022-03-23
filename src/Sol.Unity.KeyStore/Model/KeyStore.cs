using System.Text.Json.Serialization;

namespace Sol.Unity.KeyStore.Model
{
    public class KeyStore<TKdfParams> where TKdfParams : KdfParams
    {
        [JsonPropertyName("crypto")]
        public CryptoInfo<TKdfParams> Crypto { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("version")]
        public int Version { get; set; }
    }
}