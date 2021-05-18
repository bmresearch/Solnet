using Newtonsoft.Json;
using Solnet.Util;

namespace Solnet.KeyStore.Model
{
    public class CipherParams
    {
        public CipherParams()
        {
        }

        public CipherParams(byte[] iv)
        {
            Iv = iv.ToHex();
        }

        [JsonProperty("iv")]
        public string Iv { get; set; }
    }
}