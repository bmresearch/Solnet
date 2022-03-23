using Sol.Unity.Wallet;
using Sol.Unity.Wallet.Bip39;
using System;

namespace Sol.Unity.Examples
{
    public class SolanaKeygenWallet : IExample
    {
        public void Run()
        {
            const string expectedSolKeygenPublicKey = "AZzmpdbZWARkPzL8GKRHjjwY74st4URgk9v7QBubeWba";
            const string expectedSolKeygenPrivateKey = "2RitwnKZwoigHk9S3ftvFQhoTy5QQKAipNjZHDgCet8hyciUbJSuhMWDKRL8JKE784pK8jJPFaNerFsS6KXhY9K6";

            // mnemonic and passphrase to derive seed
            var passphrase = "thisiseightbytesithink";
            var mnemonic = new Mnemonic("route clerk disease box emerge airport loud waste attitude film army tray forward deal onion eight catalog surface unit card window walnut wealth medal", WordList.English);

            var solKeygenWallet = new Wallet.Wallet(mnemonic, passphrase, SeedMode.Bip39);

            Console.WriteLine($"SOLLET publicKey>b58 {solKeygenWallet.Account.PublicKey}");
            Console.WriteLine($"SOLLET privateKey>b58 {solKeygenWallet.Account.PrivateKey.Key}");

            if (solKeygenWallet.Account.PublicKey.Key != expectedSolKeygenPublicKey || solKeygenWallet.Account.PrivateKey.Key != expectedSolKeygenPrivateKey)
            {
                Console.WriteLine("NOT GOOD FOR THE SOL");
            }
            else
            {
                Console.WriteLine("GOOD FOR THE SOL");
            }

        }
    }
}