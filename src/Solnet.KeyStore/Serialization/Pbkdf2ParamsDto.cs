namespace Solnet.KeyStore.Serialization
{
    public class Pbkdf2ParamsDto : KdfParamsDto
    {
        public int c { get; set; }
        public string prf { get; set; }
    }
}