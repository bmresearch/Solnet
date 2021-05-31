using System.IO;
using System.Text;
using Solnet.Wallet;

namespace Solnet.KeyStore
{
    /// <summary>
    /// Implements a keystore compatible with the solana-keygen made in rust.
    /// </summary>
    public class SolanaKeyStoreService
    {
        
        /// <summary>
        /// Restores a keypair from a keystore compatible with the solana-keygen made in rust.
        /// </summary>
        /// <param name="path">The path to the keystore.</param>
        /// <param name="passphrase">The passphrase used while originally generating the keys.</param>
        public Wallet.Wallet RestoreKeystore(string path, string passphrase = "")
        {
            var inputBytes = File.ReadAllText(path).FromStringByteArray();

            var wallet = new Wallet.Wallet(inputBytes, passphrase, SeedMode.Bip39);
            
            return wallet;
        }

        /// <summary>
        /// NOT IMPLEMENTED.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException">NOT IMPLEMENTED.</exception>
        public Wallet.Wallet DecryptAndRestoreKeystore(string path)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Saves a keypair to a keystore compatible with the solana-keygen made in rust.
        /// </summary>
        /// <param name="path">The path to the keystore</param>
        /// <param name="wallet">The wallet to save to the keystore.</param>
        public void SaveKeystore(string path, Wallet.Wallet wallet)
        {
            File.WriteAllBytes(path, Encoding.ASCII.GetBytes(wallet.DeriveMnemonicSeed().ToStringByteArray()));
        }

        /// <summary>
        /// NOT IMPLEMENTED.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="wallet">The wallet to save to the keystore.</param>
        /// <exception cref="System.NotImplementedException">NOT IMPLEMENTED.</exception>
        public void EncryptAndSaveKeystore(string path, Wallet.Wallet wallet)
        {
            throw new System.NotImplementedException();
        }
    }
}