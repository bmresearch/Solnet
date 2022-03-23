using System.Text.Json.Serialization;

namespace Sol.Unity.KeyStore.Model
{
    public class CipherParams
    {
        public CipherParams()
        {
        }

        public CipherParams(byte[] iv)
        {
            Iv = iv.ToHex();
        }

        [JsonPropertyName("iv")]
        // ReSharper disable once MemberCanBePrivate.Global
        public string Iv { get; init; }
    }
}