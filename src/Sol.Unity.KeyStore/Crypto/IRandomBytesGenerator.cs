namespace Sol.Unity.KeyStore.Crypto
{
    public interface IRandomBytesGenerator
    {
        byte[] GenerateRandomInitializationVector();
        byte[] GenerateRandomSalt();
    }
}