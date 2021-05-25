using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Solnet.Rpc;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;

namespace Solnet.Examples
{
    class SolnetRpcTester
    {
        static void Example(string[] args)
        {

            var c = new SolanaRpcClient("https://testnet.solana.com");


            var accInfo = c.GetAccountInfo("4K1oSvRCvALnJAaQdyxXLenV4fcxHyXDY2nYY6WDyKZT");
            Console.WriteLine("acc info " + accInfo.Result.Value.Lamports);
            //var balance = c.GetBalance("9UGxCidmZtU1PM7Tbhv2twQ8ChsS6S3HdL1xo56fSVWn");
            //var accInfo = c.GetGenesisHash();

            //var blockCommitment = c.GetBlockCommitment(78561320);

            //var blockTime = c.GetBlockTime(78561320);

            SolanaStreamingRpcClient c2 = new SolanaStreamingRpcClient("wss://testnet.solana.com/");

            c2.Init().Wait();


            var sub = c2.SubscribeAccountInfo("4K1oSvRCvALnJAaQdyxXLenV4fcxHyXDY2nYY6WDyKZT", (s, data) =>
            {
                Console.WriteLine("received data " + data.Value.Lamports);
            });

            sub.SubscriptionChanged += Sub_SubscriptionChanged;

            Console.ReadKey();

            sub.Unsubscribe();

            Console.ReadKey();
        }

        private static void Sub_SubscriptionChanged(object sender, SubscriptionEvent e)
        {
            Console.WriteLine("subcription changed to: " + e.Status);
        }
    }
}
