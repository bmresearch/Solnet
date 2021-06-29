using Solnet.Rpc;
using Solnet.Rpc.Core.Sockets;
using System;

namespace Solnet.Examples
{
    public class SolnetStreamingRpcTester
    {
        static void Example(string[] args)
        {
            IStreamingRpcClient c = ClientFactory.GetStreamingClient(Cluster.MainNet);
            c.ConnectAsync().Wait();

            var sub = c.SubscribeAccountInfo(
                "4tSvZvnbyzHXLMTiFonMyxZoHmFqau1XArcRCVHLZ5gX",
                (s, data) =>
                {
                    // In a case where account data is received as jsonParsed
                    //TokenAccountData tokenAcc = null;
                    //var accData = data.Value.TryGetAccountData(out tokenAcc);
                    //if (accData)
                    //    Console.WriteLine(
                    //        $"Channel: {s.Channel} Slot: {data.Context.Slot} Lamports: {data.Value.Lamports} Account Owner: {tokenAcc.Parsed.Info.Owner}");

                    //// In a case where account data is received as base64
                    //string encodedData = null;
                    //var dataString = data.Value.TryGetAccountData(out encodedData);
                    //if (dataString)
                    //    Console.WriteLine(
                    //        $"Channel: {s.Channel} Slot: {data.Context.Slot} Lamports: {data.Value.Lamports} AccountData: {encodedData}");
                });

            sub.SubscriptionChanged += SubscriptionChanged;

            Console.ReadKey();
            Console.ReadKey();
        }

        private static void SubscriptionChanged(object sender, SubscriptionEvent e)
        {
            Console.WriteLine("Subscription changed to: " + e.Status);
        }
    }
}