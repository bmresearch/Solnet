namespace Solnet.KeyStore.Serialization
{
    public class KeyStoreScryptDto : KeyStoreDtoBase
    {
        public KeyStoreScryptDto()
        {
            crypto = new CryptoInfoScryptDto();
        }

        public CryptoInfoScryptDto crypto { get; set; }
    }
}