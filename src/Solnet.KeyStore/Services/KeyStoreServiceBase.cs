using Solnet.KeyStore.Crypto;
using Solnet.KeyStore.Model;
using System;

namespace Solnet.KeyStore.Services
{
    public abstract class KeyStoreServiceBase<T> : ISecretKeyStoreService<T> where T : KdfParams
    {
        public const int CurrentVersion = 3;
        protected readonly KeyStoreCrypto KeyStoreCrypto;
        protected readonly IRandomBytesGenerator RandomBytesGenerator;

        protected KeyStoreServiceBase() : this(new RandomBytesGenerator(), new KeyStoreCrypto())
        {
        }

        protected KeyStoreServiceBase(IRandomBytesGenerator randomBytesGenerator, KeyStoreCrypto keyStoreCrypto)
        {
            RandomBytesGenerator = randomBytesGenerator;
            KeyStoreCrypto = keyStoreCrypto;
        }


        protected KeyStoreServiceBase(IRandomBytesGenerator randomBytesGenerator)
        {
            RandomBytesGenerator = randomBytesGenerator;
            KeyStoreCrypto = new KeyStoreCrypto();
        }

        public KeyStore<T> EncryptAndGenerateKeyStore(string password, byte[] privateKey, string address)
        {
            var kdfParams = GetDefaultParams();
            return EncryptAndGenerateKeyStore(password, privateKey, address, kdfParams);
        }

        public string EncryptAndGenerateKeyStoreAsJson(string password, byte[] privateKey, string address)
        {
            var keyStore = EncryptAndGenerateKeyStore(password, privateKey, address);
            return SerializeKeyStoreToJson(keyStore);
        }

        public abstract KeyStore<T> DeserializeKeyStoreFromJson(string json);
        public abstract string SerializeKeyStoreToJson(KeyStore<T> keyStore);

        public abstract byte[] DecryptKeyStore(string password, KeyStore<T> keyStore);

        public abstract string GetKdfType();

        public virtual string GetCipherType()
        {
            return "aes-128-ctr";
        }

        public KeyStore<T> EncryptAndGenerateKeyStore(string password, byte[] privateKey, string address, T kdfParams)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (privateKey == null) throw new ArgumentNullException(nameof(privateKey));
            if (address == null) throw new ArgumentNullException(nameof(address));
            if (kdfParams == null) throw new ArgumentNullException(nameof(kdfParams));

            var salt = RandomBytesGenerator.GenerateRandomSalt();

            var derivedKey = GenerateDerivedKey(password, salt, kdfParams);

            var cipherKey = KeyStoreCrypto.GenerateCipherKey(derivedKey);

            var iv = RandomBytesGenerator.GenerateRandomInitializationVector();

            var cipherText = GenerateCipher(privateKey, iv, cipherKey);

            var mac = KeyStoreCrypto.GenerateMac(derivedKey, cipherText);

            var cryptoInfo = new CryptoInfo<T>(GetCipherType(), cipherText, iv, mac, salt, kdfParams, GetKdfType());

            var keyStore = new KeyStore<T>
            {
                Version = CurrentVersion,
                Address = address,
                Id = Guid.NewGuid().ToString(),
                Crypto = cryptoInfo
            };

            return keyStore;
        }

        public string EncryptAndGenerateKeyStoreAsJson(string password, byte[] privateKey, string address, T kdfParams)
        {
            var keyStore = EncryptAndGenerateKeyStore(password, privateKey, address, kdfParams);
            return SerializeKeyStoreToJson(keyStore);
        }

        public byte[] DecryptKeyStoreFromJson(string password, string json)
        {
            var keyStore = DeserializeKeyStoreFromJson(json);
            return DecryptKeyStore(password, keyStore);
        }

        protected virtual byte[] GenerateCipher(byte[] privateKey, byte[] iv, byte[] cipherKey)
        {
            return KeyStoreCrypto.GenerateAesCtrCipher(iv, cipherKey, privateKey);
        }

        protected abstract byte[] GenerateDerivedKey(string password, byte[] salt, T kdfParams);

        protected abstract T GetDefaultParams();
    }
}