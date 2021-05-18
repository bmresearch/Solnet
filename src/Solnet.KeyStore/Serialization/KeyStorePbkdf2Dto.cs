namespace Solnet.KeyStore.Serialization
{
    public class KeyStorePbkdf2Dto : KeyStoreDtoBase
    {
        public KeyStorePbkdf2Dto()
        {
            crypto = new CryptoInfoPbkdf2Dto();
        }
        public CryptoInfoPbkdf2Dto crypto { get; set; }
    }
}