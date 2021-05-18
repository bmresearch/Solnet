namespace Solnet.KeyStore.Serialization
{
    public class KeyStoreDtoBase
    {
        public string id { get; set; }
        public string address { get; set; }
        public int version { get; set; }
    }
}