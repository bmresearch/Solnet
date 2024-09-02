#pragma warning disable CS1591
using Solnet.KeyStore.Model;

namespace Solnet.KeyStore.Serialization
{
    public static class JsonKeyStoreScryptSerializer
    {
        public static string SerializeScrypt(KeyStore<ScryptParams> scryptKeyStore)
        {
            return System.Text.Json.JsonSerializer.Serialize(scryptKeyStore);
        }

        public static KeyStore<ScryptParams> DeserializeScrypt(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<KeyStore<ScryptParams>>(json);
        }
    }
}