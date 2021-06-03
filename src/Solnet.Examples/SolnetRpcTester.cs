using System;
using Microsoft.Extensions.Logging;
using NBitcoin.Logging;
using Solnet.Rpc;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;

namespace Solnet.Examples
{
    class SolnetRpcTester
    {
        static void Main(string[] args)
        {
            var c = ClientFactory.GetClient(Cluster.MainNet);

            //var accInfo = c.GetGenesisHash();

            //var blockCommitment = c.GetBlockCommitment(78561320);

            //var blockTime = c.GetBlockTime(78561320);

            //var cn = c.GetClusterNodes();
            //var bh = c.GetBlockHeight();
            //Console.WriteLine(bh.Result);
            //var identity = c.GetIdentity();
            //Console.WriteLine(identity.Result.Identity);
            
            //var inflationGov = c.GetInflationGovernor();
            //Console.WriteLine(inflationGov.Result.Terminal);
            
            //var inflationRate = c.GetInflationRate();
            //Console.WriteLine(inflationRate.Result.Total);

            var v = c.GetVersion();
            
            Console.WriteLine(v.Result.SolanaCore);
            Console.WriteLine(v.Result.FeatureSet);
            
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

            //var tokenAccounts = c.GetTokenAccountsByOwner(
            //    "9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5",null, "TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA");            
            //foreach (var acc in tokenAccounts.Result.Value)
            //{
            //    Console.WriteLine("--------------------------------------------------------------------");
            //    var accInfo = c.GetAccountInfo(acc.PublicKey, BinaryEncoding.JsonParsed);
            //    TokenAccountData tokenAccData = null;
            //    var data = accInfo.Result.Value.TryGetAccountData(out tokenAccData);
            //    Console.WriteLine(
            //        $"Token Account:\n" +
            //        $"\tAccount PubKey: {acc.PublicKey}\n" +
            //        $"\tAccount Lamport Balance: {acc.Account.Lamports}\n" +
            //        $"\tAccount Encoded Data: {acc.Account.Data}\n" +
            //        $"Account Info for {acc.PublicKey}:\n" +
            //        $"\tAccount Owner: {tokenAccData.Parsed.Info.Owner}\n" +
            //        $"\tToken Balance: {tokenAccData.Parsed.Info.TokenAmount.UiAmountString}\n" +
            //        $"\tToken Mint: {tokenAccData.Parsed.Info.Mint}"
            //    );
            //    Console.WriteLine("--------------------------------------------------------------------");
            //}
            var tokenAccounts = c.GetTokenAccountsByOwner(
                "9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5", tokenProgramId: "TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA");
            
            Console.ReadKey();
        }
    }
}
