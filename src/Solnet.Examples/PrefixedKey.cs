using System;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.DataEncoders;
using Org.BouncyCastle.Crypto.Digests;
using Solnet.Wallet;

namespace Solnet.Examples
{
    public class PrefixedKey
    {
        public class Generator
        {
            public readonly object indexLock = new object();
            public readonly object stopLock = new object();
            
            public long index;
            public bool stop;

            public Mnemonic _mnemonic;
            public Wallet.Wallet _wallet;
            public string _passphrase;

            public Generator(Mnemonic mnemonic, Wallet.Wallet wallet, string passphrase)
            {
                _mnemonic = mnemonic;
                _wallet = wallet;
                _passphrase = passphrase;
                
            }

            public long Index()
            {
                lock (indexLock)
                {
                    return index;
                }
            }

            public Account Generate()
            {
                lock (stopLock)
                {
                    if (stop) return null;
                }
                lock (indexLock)
                {
                    return _wallet.GetAccount((int)++index);
                }
            }
        }
        
        /// <summary>
        /// Performs sha256 and ripemd160 for bech32 address.
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static byte[] Hashed(byte[] publicKey)
        {
            var sha256 = new Sha256Digest();
            sha256.BlockUpdate(publicKey, 0, publicKey.Length);
            byte[] shaHash = new byte[sha256.GetDigestSize()];
            sha256.DoFinal (shaHash, 0);

            var ripe160 = new RipeMD160Digest();
            ripe160.BlockUpdate(shaHash, 0, shaHash.Length);
            byte[] ripHash = new byte[ripe160.GetDigestSize()];
            ripe160.DoFinal(ripHash, 0);
            return ripHash;
        }
        
        static async Task Prefixer(string[] args)
        {
            var b64Enc = new Base64Encoder();
            var passphrase = Hashed(Encoding.ASCII.GetBytes("hoaktrades"));
            var encodedPassphrase = b64Enc.EncodeData(passphrase);
            var mnemonic = new Mnemonic(Wordlist.English, WordCount.TwentyFour);
            var wallet = new Wallet.Wallet(mnemonic);
            var generator = new Generator(mnemonic, wallet, encodedPassphrase);
            Console.WriteLine($"Mnemonic: {mnemonic} Passphrase: {encodedPassphrase}");
            
            var tasks = new Task[24];
            
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() => Run(generator));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine($"Mnemonic: {mnemonic} Passphrase: {encodedPassphrase}");
        }

        static void Run(Generator generator)
        {
            Account acc = generator.Generate();
            
            while (acc != null && !acc.GetPublicKey.StartsWith("oak"))
            {
                acc = generator.Generate();
                Console.WriteLine($"Account: {generator.Index()} PrivateKey: {acc.GetPrivateKey} PublicKey {acc.GetPublicKey}");
            }
            lock (generator.stopLock)
            {
                generator.stop = true;
            }
            Console.WriteLine($"Account: {generator.Index()} PrivateKey: {acc.GetPrivateKey} PublicKey {acc.GetPublicKey}");
        }
    }
}