using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;

namespace Solnet.Examples
{
    class SolnetRpcTester
    {

        static void Main(string[] args)
        {

            SolanaJsonRpcClient c = new SolanaJsonRpcClient();


            //var accInfo = c.GetAccountInfo("vines1vzrYbzLMRdu58ou5XTby4qAqVRLmqo36NKPTg");

            //var balance = c.GetBalance("9UGxCidmZtU1PM7Tbhv2twQ8ChsS6S3HdL1xo56fSVWn");
            //var accInfo = c.GetGenesisHash();

            //var blockCommitment = c.GetBlockCommitment(78561320);

            //var blockTime = c.GetBlockTime(78561320);

            SolanaStreamingClient c2 = new SolanaStreamingClient("wss://api.mainnet-beta.solana.com/");

            c2.Init().Wait();


            var sub = c2.SubscribeAccountInfo("9UGxCidmZtU1PM7Tbhv2twQ8ChsS6S3HdL1xo56fSVWn", (s, data) =>
            {
                Console.WriteLine("received data " + data.Value.Lamports);
            });

            sub.SubscriptionChanged += Sub_SubscriptionChanged;

            Console.ReadKey();
            Console.ReadKey();
        }

        private static void Sub_SubscriptionChanged(object sender, SubscriptionEvent e)
        {
            Console.WriteLine("subcription changed to: " + e.Status);
        }
    }
}
