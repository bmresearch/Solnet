namespace Solnet.KeyStore.Serialization
{
    public class CryptoInfoDtoBase
    {
        public CryptoInfoDtoBase()
        {
            cipherparams = new CipherParamsDto();
        }

        public string cipher { get; set; }
        public string cipherText { get; set; }
        public CipherParamsDto cipherparams { get; set; }
        public string kdf { get; set; }
        public string mac { get; set; }
    }
}