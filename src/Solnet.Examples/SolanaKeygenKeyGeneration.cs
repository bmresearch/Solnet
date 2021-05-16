using System;
using System.Collections.Generic;
using System.Diagnostics;
using NBitcoin;
using Solnet.Wallet;

namespace Solnet.Examples
{
    public class SolanaKeygenWallet
    {
        static void Example()
        {
            const string expectedSolKeygenPublicKey = "AZzmpdbZWARkPzL8GKRHjjwY74st4URgk9v7QBubeWba";
            const string expectedSolKeygenPrivateKey = "2RitwnKZwoigHk9S3ftvFQhoTy5QQKAipNjZHDgCet8hyciUbJSuhMWDKRL8JKE784pK8jJPFaNerFsS6KXhY9K6";
            
            // mnemonic and passphrase to derive seed
            var passphrase = "thisiseightbytesithink";
            var mnemonic = new Mnemonic("route clerk disease box emerge airport loud waste attitude film army tray forward deal onion eight catalog surface unit card window walnut wealth medal", Wordlist.English);

            var solKeygenWallet = new Wallet.Wallet(mnemonic, passphrase, SeedMode.Bip39);
            
            Console.WriteLine($"SOLLET publicKey>b58 {solKeygenWallet.Account.EncodedPublicKey}");
            Console.WriteLine($"SOLLET privateKey>b58 {solKeygenWallet.Account.EncodedPrivateKey}");
            
            Debug.Assert(solKeygenWallet.Account.EncodedPublicKey == expectedSolKeygenPublicKey && solKeygenWallet.Account.EncodedPrivateKey == expectedSolKeygenPrivateKey);

            if (solKeygenWallet.Account.EncodedPublicKey != expectedSolKeygenPublicKey ||
                solKeygenWallet.Account.EncodedPrivateKey != expectedSolKeygenPrivateKey)
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