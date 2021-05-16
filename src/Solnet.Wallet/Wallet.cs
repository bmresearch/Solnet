using System;
using Chaos.NaCl;
using NBitcoin;

namespace Solnet.Wallet
{
    /// <summary>
    /// Represents a wallet.
    /// </summary>
    public class Wallet
    {
        /// <summary>
        /// The derivation path.
        /// </summary>
        private const string DerivationPath = "m/44'/501'/x'/0'";

        /// <summary>
        /// The seed derived from the mnemonic and/or passphrase.
        /// </summary>
        private byte[] _seed;
        
        /// <summary>
        /// The seed mode used for key generation.
        /// </summary>
        private SeedMode _seedMode;

        /// <summary>
        /// The method used for <see cref="SeedMode.Ed25519Bip32"/> key generation.
        /// </summary>
        private Ed25519Bip32 _ed25519Bip32;

        /// <summary>
        /// Initialize a wallet with the passed passphrase and seed mode.
        /// <remarks>See <see cref="SeedMode"/>.</remarks>
        /// </summary>
        /// <param name="passphrase">The passphrase.</param>
        /// <param name="seedMode">The seed mode.</param>
        public Wallet(string passphrase = "", SeedMode seedMode = SeedMode.Ed25519Bip32)
        {
            Mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            
            _seedMode = seedMode;
            _seed = InitializeSeed(Mnemonic, passphrase);
        }
        
        /// <summary>
        /// Initialize a wallet from passed word count and word list for the mnemonic and passphrase.
        /// </summary>
        /// <param name="wordCount"></param>
        /// <param name="wordlist"></param>
        /// <param name="passphrase">The passphrase.</param>
        /// <param name="seedMode">The seed generation mode.</param>
        public Wallet(WordCount wordCount, Wordlist wordlist, string passphrase = "", SeedMode seedMode = SeedMode.Ed25519Bip32)
        {
            Mnemonic = new Mnemonic(wordlist, wordCount);
            
            _seedMode = seedMode;
            _seed = InitializeSeed(Mnemonic, passphrase);
        }
        
        /// <summary>
        /// Initialize a wallet from the passed mnemonic and passphrase.
        /// </summary>
        /// <param name="mnemonic"></param>
        /// <param name="passphrase">The passphrase.</param>
        /// <param name="seedMode">The seed generation mode.</param>
        public Wallet(Mnemonic mnemonic, string passphrase = "", SeedMode seedMode = SeedMode.Ed25519Bip32)
        {
            Mnemonic = mnemonic;
            
            _seedMode = seedMode;
            _seed = InitializeSeed(mnemonic, passphrase);
        }
        
        /// <summary>
        /// Initializes a wallet from the passed account.
        /// </summary>
        /// <param name="account">The account holding the keypair.</param>
        public Wallet(Account account)
        {
            Account = account;
        }
        
        /// <summary>
        /// Verify the signed message.
        /// </summary>
        /// <param name="message">The signed message.</param>
        /// <param name="signature">The signature of the message.</param>
        /// <param name="accountIndex">The index of the account used to verify the signed message.</param>
        /// <returns></returns>
        public bool Verify(byte[] message, byte[] signature, int accountIndex)
        {
            if (_seedMode != SeedMode.Ed25519Bip32)
                throw new Exception("cannot verify bip39 signatures using ed25519 based bip32 keys");
            
            var account = GetAccount(accountIndex);
            
            return Ed25519.Verify(signature, message, account.PublicKey);
        }

        /// <summary>
        /// Verify the signed message.
        /// </summary>
        /// <param name="message">The signed message.</param>
        /// <param name="signature">The signature of the message.</param>
        /// <returns></returns>
        public bool Verify(byte[] message, byte[] signature)
        {
            if (_seedMode == SeedMode.Ed25519Bip32)
                throw new Exception("cannot verify ed25519 based bip32 signatures using bip39 keys");
            
            return Ed25519.Verify(signature, message, Account.PublicKey);
        }
        
        /// <summary>
        /// Sign the data.
        /// </summary>
        /// <param name="message">The data to sign.</param>
        /// <param name="accountIndex">The account used to sign the data.</param>
        /// <returns>The signature of the data.</returns>
        public byte[] Sign(byte[] message, int accountIndex)
        {
            if (_seedMode != SeedMode.Ed25519Bip32)
                throw new Exception("cannot compute bip39 signature using ed25519 based bip32 keys ");
            
            var account = GetAccount(accountIndex);
            
            var signature = new ArraySegment<byte>();
            Ed25519.Sign(signature, message, account.PrivateKey);
            return signature.ToArray();
        }

        /// <summary>
        /// Sign the data.
        /// </summary>
        /// <param name="message">The data to sign.</param>
        /// <returns>The signature of the data.</returns>
        public byte[] Sign(byte[] message)
        {
            if (_seedMode == SeedMode.Ed25519Bip32)
                throw new Exception("cannot compute ed25519 based bip32 signature using bip39 keys");

            var signature = new ArraySegment<byte>();
            Ed25519.Sign(signature, message, Account.PrivateKey);
            return signature.ToArray();
        }

        /// <summary>
        /// Gets the account at the passed index using the ed25519 bip32 derivation path.
        /// </summary>
        /// <param name="index">The index of the account.</param>
        /// <returns>The account.</returns>
        public Account GetAccount(int index)
        {
            var path = DerivationPath.Replace("x", index.ToString());
            var (account, chain) = _ed25519Bip32.DerivePath(path);
            var (privateKey, publicKey) = EdKeyPairFromSeed(account);
            return new(privateKey, publicKey);
        }

        /// <summary>
        /// Gets the corresponding ed25519 key pair from the passed seed.
        /// </summary>
        /// <param name="seed">The seed</param>
        /// <returns>The key pair.</returns>
        private static (byte[] privateKey, byte[] publicKey) EdKeyPairFromSeed(byte[] seed) =>
            new(Ed25519.ExpandedPrivateKeyFromSeed(seed), Ed25519.PublicKeyFromSeed(seed));

        /// <summary>
        /// Generate the keypair for the passed mnemonic and passphrase.
        /// </summary>
        /// <param name="mnemonic">The mnemonic</param>
        /// <param name="passphrase">The passphrase.</param>
        /// <returns>The key pair.</returns>
        private byte[] InitializeSeed(Mnemonic mnemonic, string passphrase = "")
        {
            var seed = DeriveSeed(mnemonic, passphrase);

            if (_seedMode == SeedMode.Ed25519Bip32)
            {
                _ed25519Bip32 = new Ed25519Bip32(seed);
                Account = GetAccount(0);
            }
            else
            {
                var (privateKey, publicKey) = EdKeyPairFromSeed(seed[..32]);
                Account = new Account(privateKey, publicKey);
            }

            return seed;
        }

        /// <summary>
        /// Derive a seed from the passed mnemonic and passphrase.
        /// </summary>
        /// <param name="mnemonic">The mnemonic</param>
        /// <param name="passphrase">The passphrase.</param>
        /// <returns>The seed.</returns>
        private byte[] DeriveSeed(Mnemonic mnemonic, string passphrase = "")
        {
            switch (_seedMode)
            {
                case SeedMode.Ed25519Bip32:
                    return mnemonic.DeriveSeed();
                case SeedMode.Bip39:
                    return mnemonic.DeriveSeed(passphrase);
                default:
                    return mnemonic.DeriveSeed();
            }
        }

        /// <summary>
        /// The key pair.
        /// </summary>
        public Account Account { get; private set; }
        
        /// <summary>
        /// The mnemonic words.
        /// </summary>
        public Mnemonic Mnemonic { get; }
    }
}