using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using Solnet.KeyStore;
using Solnet.Wallet;

namespace Solnet.Examples
{
    public class SolletKeygenKeystore
    {
        static void Example(string[] args)
        {
            var expectedSolletAddresses = new List<string[]>
            {
                new []{"6bhhceZToGG9RsTe1nfNFXEMjavhj6CV55EsvearAt2z", "5S1UT7L6bQ8sVaPjpJyYFEEYh8HAXRXPFUEuj6kHQXs6ZE9F6a2wWrjdokAmSPP5HVP46bYxsrU8yr2FxxYmVBi6"},
                new []{"9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", "22J7rH3DFJb1yz8JuWUWfrpsQrNsvZKov8sznfwHbPGTznSgQ8u6LQ6KixPC2mYCJDsfzME1FbdX1x89zKq4MU3K"},
                new []{"3F2RNf2f2kWYgJ2XsqcjzVeh3rsEQnwf6cawtBiJGyKV", "5954a6aMxVnPTyMNdVKrSiqoVMRvZcwU7swGp9kHsV9HP9Eu81TebS4Mbq5ZGmZwUaJkkKoCJ2eJSY9cTdWzRXeF"},
                new []{"GyWQGKYpvzFmjhnG5Pfw9jfvgDM7LB31HnTRPopCCS9", "tUV1EeY6CARAbuEfVqKS46X136PRBea8PcmYfHRWNQc6yYB14GkSBZ6PTybUt5W14A7FSJ6Mm6NN22fLhUhDUGu"},
                new []{"GjtWSPacUQFVShQKRKPc342MLCdNiusn3WTJQKxuDfXi", "iLtErFEn6w5xbsUW63QLYMTJeX8TAgFTUDTgat3gxpaiN3AJbebv6ybtmTj1t1yvkqqY2k1uwFxaKZoCQAPcDZe"},
                new []{"DjGCyxjGxpvEo921Ad4tUUWquiRG6dziJUCk8HKZoaKK", "3uvEiJiMyXqQmELLjxV8r3E7CyRFg42LUAxzz6q7fPhzTCxCzPkaMCQ9ARpWYDNiDXhue2Uma1C7KR9AkiiWUS8y"},
                new []{"HU6aKFapq4RssJqV96rfE7vv1pepz5A5miPAMxGFso4X", "4xFZDEhhw3oVewE3UCvzLmhRWjjcqvVMxuYiETWiyaV2wJwEJ4ceDDE359NMirh43VYisViHAwsXjZ3F9fk6dAxB"},
                new []{"HunD57AAvhBiX2SxmEDMbrgQ9pcqrtRyWKy7dWPEWYkJ", "2Z5CFuVDPQXxrB3iw5g6SAnKqApE1djAqtTZDA83rLZ1NDi6z13rwDX17qdyUDCxK9nDwKAHdVuy3h6jeXspcYxA"},
                new []{"9KmfMX4Ne5ocb8C7PwjmJTWTpQTQcPhkeD2zY35mawhq", "c1BzdtL4RByNQnzcaUq3WuNLuyY4tQogGT7JWwy4YGBE8FGSgWUH8eNJFyJgXNYtwTKq4emhC4V132QX9REwujm"},
                new []{"7MrtfwpJBw2hn4eopB2CVEKR1kePJV5kKmKX3wUAFsJ9", "4skUmBVmaLoriN9Ge8xcF4xQFJmF554rnRRa2u1yDbre2zj2wUpgCXUaPETLSAWNudCkNAkWM5oJFJRaeZY1g9JR"}
            };
            const string _password = "password";
            const string mnemonicWords =
                "route clerk disease box emerge airport loud waste attitude film army tray forward deal onion eight catalog surface unit card window walnut wealth medal";
            
            var mnemonic = new Mnemonic(mnemonicWords, Wordlist.English);

            var keystoreService = new SecretKeyStoreService();
            
            // no passphrase to generate same keys as sollet
            var wallet = new Wallet.Wallet(mnemonic);
            var seed = wallet.DeriveMnemonicSeed();
            Account account = null;
            bool flag = true; // to check if the keys are the expected ones
            Console.WriteLine($"Seed: {seed.ToStringByteArray()}\nAddress: {wallet.Account.GetPublicKey}");
            
            /* 1. Encrypt mnemonic derived seed and generate keystore as json 
            var keystoreJson = keystoreService.EncryptAndGenerateDefaultKeyStoreAsJson(_password, seed, wallet.Account.EncodedPublicKey);
            */
            
            /* 2. Encrypt the mnemonic as bytes */
            var stringByteArray = Encoding.UTF8.GetBytes(mnemonic.ToString());
            var encryptedKeystoreJson = keystoreService.EncryptAndGenerateDefaultKeyStoreAsJson(_password, stringByteArray, wallet.Account.GetPublicKey);

            var keystoreJsonAddr = SecretKeyStoreService.GetAddressFromKeyStore(encryptedKeystoreJson);
            
            Console.WriteLine($"Keystore JSON: {encryptedKeystoreJson}\nKeystore Address: {keystoreJsonAddr}");
            
            /* 1. Decrypt mnemonic derived seed and generate wallet from it 
            var decryptedKeystore = keystoreService.DecryptKeyStoreFromJson(_password, keystoreJson);
            */
            
            /* 2. Decrypt the mnemonic as bytes */
            var decryptedKeystore = keystoreService.DecryptKeyStoreFromJson(_password, encryptedKeystoreJson);
            var mnemonicString = Encoding.UTF8.GetString(decryptedKeystore);
            
            /* 2. Restore the wallet from the restored mnemonic */
            var restoredMnemonic = new Mnemonic(mnemonicString);
            var restoredWallet = new Wallet.Wallet(restoredMnemonic);
            
            // no passphrase to generate same keys as sollet
            //var restoredWallet = new Wallet.Wallet(decryptedKeystore);
            var restoredSeed = restoredWallet.DeriveMnemonicSeed();
            
            Console.WriteLine($"Seed: {restoredSeed.ToStringByteArray()}");
            
            // Mimic sollet key generation
            for (int i = 0; i < 10; i++)
            {
                account = restoredWallet.GetAccount(i);
                
                Console.WriteLine($"RESTORED SOLLET address {account.GetPublicKey}");
                
                if (account.GetPublicKey != expectedSolletAddresses[i][0] || account.GetPrivateKey != expectedSolletAddresses[i][1]) flag = false;
            }
            Console.WriteLine(flag ? "GOOD RESTORE FOR THE SOLLET" : "NOT GOOD RESTORE FOR THE SOLLET");
        }
    }
}