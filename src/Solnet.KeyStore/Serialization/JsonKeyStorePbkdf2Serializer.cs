using Solnet.KeyStore.Model;

namespace Solnet.KeyStore.Serialization
{
    public static class JsonKeyStorePbkdf2Serializer
    {
        public static string SerialisePbkdf2(KeyStore<Pbkdf2Params> pbkdf2KeyStore)
        {
            return System.Text.Json.JsonSerializer.Serialize(pbkdf2KeyStore);
        }

        public static KeyStore<Pbkdf2Params> DeserializePbkdf2(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<KeyStore<Pbkdf2Params>>(json);
        }
    }
}