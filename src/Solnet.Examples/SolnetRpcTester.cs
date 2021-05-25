using System;
using Solnet.Rpc;
using Solnet.Rpc.Core.Sockets;

namespace Solnet.Examples
{
    class SolnetRpcTester
    {
        static void Main(string[] args)
        {

            var c = new SolanaRpcClient("https://testnet.solana.com");

            //var accInfo = c.GetAccountInfo("4K1oSvRCvALnJAaQdyxXLenV4fcxHyXDY2nYY6WDyKZT");
            //var balance = c.GetBalance("9UGxCidmZtU1PM7Tbhv2twQ8ChsS6S3HdL1xo56fSVWn");
            //var accInfo = c.GetGenesisHash();

            //var blockCommitment = c.GetBlockCommitment(78561320);

            //var blockTime = c.GetBlockTime(78561320);

            var cn = c.GetClusterNodes();

            foreach (var cluster in cn.Result)
            {
                Console.WriteLine("cluster node " + cluster.PubKey );
            }
        }

        private static void Sub_SubscriptionChanged(object sender, SubscriptionEvent e)
        {
            Console.WriteLine("subcription changed to: " + e.Status);
        }
    }
}
