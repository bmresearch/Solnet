using Newtonsoft.Json;

namespace Solnet.KeyStore.Model
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKdfParams"></typeparam>
    public class KeyStore<TKdfParams> where TKdfParams : KdfParams
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("crypto")]
        public CryptoInfo<TKdfParams> Crypto { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("version")]
        public int Version { get; set; }
    }
}