using System;
using Solnet.KeyStore.Crypto;
using Solnet.KeyStore.Model;

namespace Solnet.KeyStore
{
    /// <summary>
    /// Implements an abstract base class for the key store service.
    /// </summary>
    /// <typeparam name="T">The <c>KdfParams</c>.</typeparam>
    public abstract class KeyStoreServiceBase<T> : IKeyStoreService<T> where T: KdfParams
    {
        
        public const int CurrentVersion = 3;
        protected readonly KeyStoreCrypto KeyStoreCrypto;
        protected readonly IRandomBytesGenerator RandomBytesGenerator;

        protected KeyStoreServiceBase() : this(new RandomBytesGenerator(), new KeyStoreCrypto())
        {
        }

        protected KeyStoreServiceBase(IRandomBytesGenerator randomBytesGenerator, KeyStoreCrypto keyStoreCrypto)
        {
            RandomBytesGenerator = randomBytesGenerator;
            KeyStoreCrypto = keyStoreCrypto;
        }


        protected KeyStoreServiceBase(IRandomBytesGenerator randomBytesGenerator)
        {
            RandomBytesGenerator = randomBytesGenerator;
            KeyStoreCrypto = new KeyStoreCrypto();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="keyStore"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public byte[] DecryptKeyStore(string password, KeyStore<T> keyStore)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public KeyStore<T> DeserializeKeyStoreFromJson(string json)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="privateKey"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public KeyStore<T> EncryptAndGenerateKeyStore(string password, byte[] privateKey, string address)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="privateKey"></param>
        /// <param name="addresss"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string EncryptAndGenerateKeyStoreAsJson(string password, byte[] privateKey, string addresss)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string GetCipherType()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string GetKdfType()
        {
            throw new System.NotImplementedException();
        }
    }
}