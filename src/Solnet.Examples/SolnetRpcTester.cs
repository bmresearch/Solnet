using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            var c = ClientFactory.GetClient(Cluster.TestNet);

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


            //var last = c.GetBlock(79662905);
            //var t = c.GetBlockHeight();
            //var x = c.GetBlockProduction();
            //var x2 = c.GetBlockProduction("Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu");
            //var x3 = c.GetBlockProduction("Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu", 79_000_000);
            //var x5 = c.GetBlockProduction(79714135);
            //var x4 = c.GetBlockProduction("Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu", 79_000_000, 79_500_000);

            //var res = c.GetTransaction("5as3w4KMpY23MP5T1nkPVksjXjN7hnjHKqiDxRMxUNcw5XsCGtStayZib1kQdyR2D9w8dR11Ha9Xk38KP3kbAwM1");

            //var res = c.GetBlocks(79_499_950, 79_500_000);
            //var res = c.GetBlocksWithLimit(79_699_950, 2);
            //var res = c.GetFirstAvailableBlock();
            //var res = c.GetHealth();

            //var res = c.GetLeaderSchedule();
            //var res2 = c.GetLeaderSchedule(79_700_000);
            //var res3 = c.GetLeaderSchedule(identity: "Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu");
            //var res4 = c.GetLeaderSchedule(79_700_000, "Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu");

            var accs = c.GetMultipleAccounts(new List<string> { "Bbe9EKucmRtJr2J4dd5Eb5ybQmY7Fm7jYxKXxmmkLFsu", "9we6kjtbcZ2vy3GSLLsZTEhbAqXPTRvEyoxa8wxSqKp5" });

            Console.ReadKey();
        }
    }
}
