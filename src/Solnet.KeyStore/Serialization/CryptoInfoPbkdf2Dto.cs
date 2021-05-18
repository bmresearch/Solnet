namespace Solnet.KeyStore.Serialization
{
    public class CryptoInfoPbkdf2Dto : CryptoInfoDtoBase
    {
        public CryptoInfoPbkdf2Dto()
        {
            kdfparams = new Pbkdf2ParamsDto();
        }

        public Pbkdf2ParamsDto kdfparams { get; set; }
    }
}