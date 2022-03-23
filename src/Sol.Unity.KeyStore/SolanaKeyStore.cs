using Sol.Unity.Wallet;
using System.IO;
using System.Text;

namespace Sol.Unity.KeyStore
{
    /// <summary>
    /// Implements a keystore compatible with the solana-keygen made in rust.
    /// </summary>
    public class SolanaKeyStoreService
    {
        /// <summary>
        /// Restores a keypair from a keystore compatible with the solana-keygen made in rust.
        /// </summary>
        /// <param name="privateKey">The string with the private key bytes.</param>
        /// <param name="passphrase">The passphrase used while originally generating the keys.</param>
        public Unity.Wallet.Wallet RestoreKeystore(string privateKey, string passphrase = "")
        {
            return InitializeWallet(privateKey.FromStringByteArray(), passphrase);
        }

        /// <summary>
        /// Restores a keypair from a keystore compatible with the solana-keygen made in rust.
        /// </summary>
        /// <param name="path">The path to the keystore.</param>
        /// <param name="passphrase">The passphrase used while originally generating the keys.</param>
        public Unity.Wallet.Wallet RestoreKeystoreFromFile(string path, string passphrase = "")
        {
            var inputBytes = File.ReadAllText(path).FromStringByteArray();
            return InitializeWallet(inputBytes, passphrase);
        }

        /// <summary>
        /// Saves a keypair to a keystore compatible with the solana-keygen made in rust.
        /// </summary>
        /// <param name="path">The path to the keystore</param>
        /// <param name="wallet">The wallet to save to the keystore.</param>
        public void SaveKeystore(string path, Unity.Wallet.Wallet wallet)
        {
            File.WriteAllBytes(path, Encoding.ASCII.GetBytes(wallet.Account.PrivateKey.KeyBytes.ToStringByteArray()));
        }

        /// <summary>
        /// Initialize the wallet.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <param name="passphrase">The passphrase.</param>
        /// <returns>The wallet.</returns>
        private Unity.Wallet.Wallet InitializeWallet(byte[] seed, string passphrase = "")
        {
            return new Unity.Wallet.Wallet(seed, passphrase, SeedMode.Bip39);
        }
    }
}