using System;
using System.Numerics;
using Solnet.RPC.Accounts;

namespace Solnet.Accounts
{
    /// <summary>
    /// 
    /// </summary>
    public class Account : IAccount
    {
        public BigInteger? ChainId { get; }

        public static Account LoadFromKeyStore(string json, string password, BigInteger? chainId = null)
        {
            var keyStoreService = new KeyStoreService();
            var key = keyStoreService.DecryptKeyStoreFromJson(password, json);
            return new Account(key, chainId);
        }

        public string PrivateKey { get; private set; }
        public string PublicKey { get; private set; }


        public Account(SolECKey key, BigInteger? chainId = null)
        {
            ChainId = chainId;
            Initialise(key);
        }

        public Account(string privateKey, BigInteger? chainId = null)
        {
            ChainId = chainId;
            Initialise(new SolECKey(privateKey));
        }

        public Account(byte[] privateKey, BigInteger? chainId = null)
        {
            ChainId = chainId;
            Initialise(new SolECKey(privateKey, true));
        }

        public Account(SolECKey key, Chain chain) : this(key, (int) chain)
        {
        }

        public Account(string privateKey, Chain chain) : this(privateKey, (int) chain)
        {
        }

        public Account(byte[] privateKey, Chain chain) : this(privateKey, (int) chain)
        {
        }

        private void Initialise(SolECKey key)
        {
            PrivateKey = key.GetPrivateKey();
            Address = key.GetPublicAddress();
            PublicKey = key.GetPubKey().ToHex();
            InitialiseDefaultTransactionManager();
        }

        protected virtual void InitialiseDefaultTransactionManager()
        {
            TransactionManager = new AccountSignerTransactionManager(null, this, ChainId);
        }

        public string Address { get; protected set; }
        public ITransactionManager TransactionManager { get; protected set; }
        
        public INonceService NonceService { get; set; }
    }
}