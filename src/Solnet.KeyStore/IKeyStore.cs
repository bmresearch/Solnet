using Solnet.Wallet;

namespace Solnet.KeyStore
{
    /// <summary>
    /// Specifies functionality for a key store service.
    /// </summary>
    public interface IKeyStore
    {
        /// <summary>
        /// Restore the keypair from the keystore specified by path.
        /// </summary>
        /// <param name="path">The path to the keystore.</param>
        Account RestoreKeystore(string path);

        /// <summary>
        /// TODO: Implement an encrypted keystore based on Web3 Secret Storage Definition.
        /// </summary>
        /// <param name="path">The path to the keystore.</param>
        Account DecryptAndRestoreKeystore(string path);
        
        
        /// <summary>
        /// Save the keypair to the keystore specified by path.
        /// </summary>
        /// <param name="path">The path to the keystore.</param>
        /// <param name="account">The account to save to the keystore.</param>
        void SaveKeystore(string path, Account account);
        
        
        /// <summary>
        /// TODO: Implement an encrypted keystore based on Web3 Secret Storage Definition.
        /// </summary>
        /// <param name="path">The path to the keystore.</param>
        /// <param name="account">The account to save to the keystore.</param>
        void EncryptAndSaveKeystore(string path, Account account);
    }
}