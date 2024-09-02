#pragma warning disable CS1591
namespace Solnet.KeyStore.Crypto
{
    public interface IRandomBytesGenerator
    {


        byte[] GenerateRandomInitializationVector();
        byte[] GenerateRandomSalt();
    }
}