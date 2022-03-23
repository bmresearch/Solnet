using System.Text.Json.Serialization;

namespace Sol.Unity.KeyStore.Model
{
    public class KdfParams
    {
        // ReSharper disable once StringLiteralTypo
        [JsonPropertyName("dklen")]
        // ReSharper disable once IdentifierTypo
        public int Dklen { get; init; }

        [JsonPropertyName("salt")]
        public string Salt { get; set; }
    }
}