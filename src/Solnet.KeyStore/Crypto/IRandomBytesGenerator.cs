namespace Solnet.KeyStore.Exceptions
{
    public interface IRandomBytesGenerator
    {
        byte[] GenerateRandomInitialisationVector();
        byte[] GenerateRandomSalt();
    }
}