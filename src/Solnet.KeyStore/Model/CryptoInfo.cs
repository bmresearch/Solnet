using Newtonsoft.Json;

namespace Solnet.KeyStore.Model
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKdfParams"></typeparam>
    public class CryptoInfo<TKdfParams> where TKdfParams : KdfParams
    {
        /// <summary>
        /// 
        /// </summary>
        public CryptoInfo()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipher"></param>
        /// <param name="cipherText"></param>
        /// <param name="iv"></param>
        /// <param name="mac"></param>
        /// <param name="salt"></param>
        /// <param name="kdfParams"></param>
        /// <param name="kdfType"></param>
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

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("cipher")] 
        public string Cipher { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("ciphertext")] 
        public string CipherText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("cipherparams")] 
        public CipherParams CipherParams { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("kdf")] 
        public string Kdf { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("mac")] 
        public string Mac { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("kdfparams")] 
        public TKdfParams Kdfparams { get; set; }
        
    }
}