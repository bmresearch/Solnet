using System.Text.Json.Serialization;

namespace Solnet.KeyStore.Model
{
    public class ScryptParams : KdfParams
    {
        [JsonPropertyName("n")]
        public int N { get; init; }

        [JsonPropertyName("r")]
        public int R { get; init; }

        [JsonPropertyName("p")]
        public int P { get; init; }
    }
}