using Solnet.KeyStore.Model;

namespace Solnet.KeyStore.Services
{
    public interface ISecretKeyStoreService<T> where T : KdfParams
    {
        /// <summary>
        /// Decrypt the keystore.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="keyStore"></param>
        /// <returns></returns>
        byte[] DecryptKeyStore(string password, KeyStore<T> keyStore);

        /// <summary>
        /// Deserialize keystore from json.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        KeyStore<T> DeserializeKeyStoreFromJson(string json);

        /// <summary>
        /// Encrypt and generate the keystore.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="privateKey"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        KeyStore<T> EncryptAndGenerateKeyStore(string password, byte[] privateKey, string address);

        /// <summary>
        /// Encrypt and generate the keystore as json.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="privateKey"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        string EncryptAndGenerateKeyStoreAsJson(string password, byte[] privateKey, string address);

        /// <summary>
        /// Get keystore cipher type.
        /// </summary>
        /// <returns></returns>
        string GetCipherType();

        /// <summary>
        /// Get keystore key derivation function.
        /// </summary>
        /// <returns></returns>
        string GetKdfType();
    }
}