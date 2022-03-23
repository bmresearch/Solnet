using Sol.Unity.KeyStore.Model;

namespace Sol.Unity.KeyStore.Serialization
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