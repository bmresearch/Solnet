using Newtonsoft.Json;

namespace Solnet.KeyStore.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class KdfParams
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("dklen")]
        public int Dklen { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("salt")]
        public string Salt { get; set; }
    }
}