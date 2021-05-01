using Newtonsoft.Json;

namespace Solnet.KeyStore.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class CipherParams
    {
        /// <summary>
        /// 
        /// </summary>
        public CipherParams()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iv"></param>
        public CipherParams(byte[] iv)
        {
            Iv = iv.ToHex();
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("iv")]
        public string Iv { get; set; }
    }
}