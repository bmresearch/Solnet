namespace Solnet.KeyStore.Serialization
{
    public class ScryptParamsDto : KdfParamsDto
    {
        public int n { get; set; }
        public int r { get; set; }
        public int p { get; set; }
    }
}