using Solnet.KeyStore.Model;

namespace Solnet.KeyStore
{
    public interface IKeyStoreService<T> where T : KdfParams
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="keyStore"></param>
        /// <returns></returns>
        byte[] DecryptKeyStore(string password, KeyStore<T> keyStore);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        KeyStore<T> DeserializeKeyStoreFromJson(string json);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="privateKey"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        KeyStore<T> EncryptAndGenerateKeyStore(string password, byte[] privateKey, string address);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="privateKey"></param>
        /// <param name="addresss"></param>
        /// <returns></returns>
        string EncryptAndGenerateKeyStoreAsJson(string password, byte[] privateKey, string addresss);
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetCipherType();
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetKdfType();
    }
}