namespace Solnet.KeyStore.Crypto
{
    public interface IRandomBytesGenerator
    {
        byte[] GenerateRandomInitializationVector();
        byte[] GenerateRandomSalt();
    }
}