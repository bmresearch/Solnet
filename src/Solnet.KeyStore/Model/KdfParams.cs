using System.Text.Json.Serialization;

namespace Solnet.KeyStore.Model
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