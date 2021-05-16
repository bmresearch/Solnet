using Solnet.Wallet;
using Solnet.Wallet.Key;

namespace Solnet.KeyStore
{
    /// <summary>
    /// Implements the keystore abstraction for different types.
    /// </summary>
    public class KeyStore : IKeyStore
    {
        /// <summary>
        /// The keystore type.
        /// </summary>
        public KeyStoreType KeyStoreType { get; set; }
        
        /// <summary>
        /// The keystore object.
        /// </summary>
        private IKeyStore _keyStore { get; init; }

        /// <summary>
        /// Initialize the keystore of the passed type.
        /// </summary>
        /// <param name="keyStoreType">The keystore type.</param>
        public KeyStore(KeyStoreType keyStoreType)
        {
            KeyStoreType = keyStoreType;

            switch (keyStoreType)
            {
                case KeyStoreType.SolanaKeygen:
                    _keyStore = new SolanaKeyStore();
                    break;
                default:
                    _keyStore = new SolanaKeyStore();
                    break;
            }
        }
        
        /// <summary>
        /// Restore keypair from the keystore.
        /// </summary>
        /// <param name="path">The path of the keystore.</param>
        /// <returns>The keypair</returns>
        public Account RestoreKeystore(string path)
        {
            return _keyStore.RestoreKeystore(path);
        }

        /// <summary>
        /// Decrypt and restore keypair from the keystore.
        /// </summary>
        /// <param name="path">The path of the keystore.</param>
        /// <returns>The keypair</returns>
        public Account DecryptAndRestoreKeystore(string path)
        {
            return _keyStore.DecryptAndRestoreKeystore(path);
        }

        /// <summary>
        /// Save keypair to the keystore.
        /// </summary>
        /// <param name="path">The path of the keystore.</param>
        /// <param name="account">The keypair to save.</param>
        public void SaveKeystore(string path, Account account)
        {
            _keyStore.SaveKeystore(path, account);
        }

        /// <summary>
        /// Encrypt and save keypair to the keystore.
        /// </summary>
        /// <param name="path">The path of the keystore.</param>
        /// <param name="account">The keypair to save.</param>
        public void EncryptAndSaveKeystore(string path, Account account)
        {
            _keyStore.EncryptAndSaveKeystore(path, account);
        }
    }
}