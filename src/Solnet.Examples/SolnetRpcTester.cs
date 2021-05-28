using System;
using Solnet.Rpc;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Models;

namespace Solnet.Examples
{
    class SolnetRpcTester
    {
        static void Main(string[] args)
        {
            var c = new SolanaRpcClient("https://testnet.solana.com");

            //var accInfo = c.GetGenesisHash();

            //var blockCommitment = c.GetBlockCommitment(78561320);

            //var blockTime = c.GetBlockTime(78561320);

            //var cn = c.GetClusterNodes();

            /* Large accounts for Token Mint PubKey
            var largeAccounts = c.GetTokenLargestAccounts("7ugkvt26sFjMdiFQFP5AQX8m8UkxWaW7rk2nBk4R6Gf2");

            foreach (var acc in largeAccounts.Result.Value)
            {
                Console.WriteLine($"Acc: {acc.Address}  Balance: {acc.UiAmountString}");
            }
            */
            
            /* Token balance for Account PubKey
            var tokenBalance = c.GetTokenAccountBalance("7247amxcSBamBSKZJrqbj373CiJSa1v21cRav56C3WfZ");
            Console.WriteLine($"Token Balance: {tokenBalance.Result.Value.UiAmountString}");
            */

            var tokenAccounts = c.GetTokenAccountsByOwner(
                "9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5",null, "TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA");            
            foreach (var acc in tokenAccounts.Result.Value)
            {
                Console.WriteLine("--------------------------------------------------------------------");
                var accInfo = c.GetAccountInfoJson(acc.PublicKey);
                TokenAccountData tokenAccData = null;
                var data = accInfo.Result.Value.TryGetAccountData(out tokenAccData);
                Console.WriteLine(
                    $"Token Account:\n" +
                    $"\tAccount PubKey: {acc.PublicKey}\n" +
                    $"\tAccount Lamport Balance: {acc.Account.Lamports}\n" +
                    $"\tAccount Encoded Data: {acc.Account.Data}\n" +
                    $"Account Info for {acc.PublicKey}:\n" +
                    $"\tAccount Owner: {tokenAccData.Parsed.Info.Owner}\n" +
                    $"\tToken Balance: {tokenAccData.Parsed.Info.TokenAmount.UiAmountString}\n" +
                    $"\tToken Mint: {tokenAccData.Parsed.Info.Mint}"
                );
                Console.WriteLine("--------------------------------------------------------------------");
            }
        }
    }
}
