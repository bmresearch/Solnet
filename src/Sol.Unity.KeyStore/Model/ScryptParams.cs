using System.Text.Json.Serialization;

namespace Sol.Unity.KeyStore.Model
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