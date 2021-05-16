using System.IO;
using System.Text;
using Chaos.NaCl;
using Solnet.Util;
using Solnet.Wallet;

namespace Solnet.KeyStore
{
    /// <summary>
    /// Implements a keystore compatible with the solana-keygen made in rust.
    /// </summary>
    public class SolanaKeyStore : IKeyStore
    {
        /// <summary>
        /// Restores a keypair from a keystore compatible with the solana-keygen made in rust.
        /// </summary>
        /// <param name="path">The path to the keystore</param>
        public Account RestoreKeystore(string path)
        {
            var inputBytes = File.ReadAllText(path).FromStringByteArray();

            var acc = new Account(inputBytes, Ed25519.PublicKeyFromSeed(inputBytes[..32]));
            
            return acc;
        }

        /// <summary>
        /// NOT IMPLEMENTED.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException">NOT IMPLEMENTED.</exception>
        public Account DecryptAndRestoreKeystore(string path)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Saves a keypair to a keystore compatible with the solana-keygen made in rust.
        /// </summary>
        /// <param name="path">The path to the keystore</param>
        /// <param name="account">The account to save to the keystore.</param>
        public void SaveKeystore(string path, Account account)
        {
            File.WriteAllBytes(path, Encoding.ASCII.GetBytes(account.PrivateKey.ToStringByteArray()));
        }

        /// <summary>
        /// NOT IMPLEMENTED.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="account">The account to save to the keystore.</param>
        /// <exception cref="System.NotImplementedException">NOT IMPLEMENTED.</exception>
        public void EncryptAndSaveKeystore(string path, Account account)
        {
            throw new System.NotImplementedException();
        }
    }
}