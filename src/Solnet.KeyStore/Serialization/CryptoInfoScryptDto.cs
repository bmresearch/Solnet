namespace Solnet.KeyStore.Serialization
{
    public class CryptoInfoScryptDto : CryptoInfoDtoBase
    { 
        public CryptoInfoScryptDto()
        {
            kdfparams = new ScryptParamsDto();
        }
        public ScryptParamsDto kdfparams { get; set; }
    }
}