using System.Text.Json.Serialization;

namespace Solnet.KeyStore.Model
{
    public class CryptoInfo<TKdfParams> where TKdfParams : KdfParams
    {
        public CryptoInfo()
        {
        }

        public CryptoInfo(string cipher, byte[] cipherText, byte[] iv, byte[] mac, byte[] salt, TKdfParams kdfParams,
            string kdfType)
        {
            Cipher = cipher;
            CipherText = cipherText.ToHex();
            Mac = mac.ToHex();
            CipherParams = new CipherParams(iv);
            Kdfparams = kdfParams;
            Kdfparams.Salt = salt.ToHex();
            Kdf = kdfType;
        }

        [JsonPropertyName("cipher")]
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Cipher { get; }

        [JsonPropertyName("ciphertext")]
        // ReSharper disable once MemberCanBePrivate.Global
        public string CipherText { get; init; }

        // ReSharper disable once StringLiteralTypo
        [JsonPropertyName("cipherparams")]
        // ReSharper disable once MemberCanBePrivate.Global
        public CipherParams CipherParams { get; init; }

        [JsonPropertyName("kdf")]
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Kdf { get; }

        [JsonPropertyName("mac")]
        // ReSharper disable once MemberCanBePrivate.Global
        public string Mac { get; init; }

        // ReSharper disable once StringLiteralTypo
        [JsonPropertyName("kdfparams")]
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once MemberCanBePrivate.Global
        public TKdfParams Kdfparams { get; init; }
    }
}