using Solnet.KeyStore.Model;
using Solnet.KeyStore.Services;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Solnet.KeyStore
{
    /// <summary>
    /// Implements a keystore compatible with the web3 secret storage standard.
    /// </summary>
    public class SecretKeyStoreService
    {
        private readonly KeyStoreScryptService _keyStoreScryptService;
        private readonly KeyStorePbkdf2Service _keyStorePbkdf2Service;

        public SecretKeyStoreService()
        {
            _keyStorePbkdf2Service = new KeyStorePbkdf2Service();
            _keyStoreScryptService = new KeyStoreScryptService();
        }

        public SecretKeyStoreService(KeyStoreScryptService keyStoreScryptService, KeyStorePbkdf2Service keyStorePbkdf2Service)
        {
            _keyStoreScryptService = keyStoreScryptService;
            _keyStorePbkdf2Service = keyStorePbkdf2Service;
        }

        public static string GetAddressFromKeyStore(string json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));
            var keyStoreDocument = JsonSerializer.Deserialize<JsonDocument>(json);
            if (keyStoreDocument == null) throw new SerializationException("could not process json");

            var addrExist = keyStoreDocument.RootElement.TryGetProperty("address", out var address);
            if (!addrExist) throw new JsonException("could not get address from json");

            return address.GetString();
        }

        public static string GenerateUtcFileName(string address)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));
            return "utc--" + DateTime.UtcNow.ToString("O").Replace(":", "-") + "--" + address;
        }

        public byte[] DecryptKeyStoreFromFile(string password, string filePath)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));

            using var file = File.OpenText(filePath);
            var json = file.ReadToEnd();
            return DecryptKeyStoreFromJson(password, json);
        }

        public byte[] DecryptKeyStoreFromJson(string password, string json)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (json == null) throw new ArgumentNullException(nameof(json));

            var type = KeyStoreKdfChecker.GetKeyStoreKdfType(json);
            return type switch
            {
                KdfType.Pbkdf2 => _keyStorePbkdf2Service.DecryptKeyStoreFromJson(password, json),
                KdfType.Scrypt => _keyStoreScryptService.DecryptKeyStoreFromJson(password, json),
                _ => throw new Exception("Invalid kdf type")
            };
        }

        public string EncryptAndGenerateDefaultKeyStoreAsJson(string password, byte[] key, string address)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (address == null) throw new ArgumentNullException(nameof(address));

            return _keyStoreScryptService.EncryptAndGenerateKeyStoreAsJson(password, key, address);
        }
    }
}