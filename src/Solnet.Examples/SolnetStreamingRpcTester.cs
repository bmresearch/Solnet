using System;
using System.Text;
using NBitcoin.DataEncoders;
using Solnet.Rpc;
using Solnet.Rpc.Core.Sockets;

namespace Solnet.Examples
{
    public class SolnetStreamingRpcTester
    {
        static void Example(string[] args)
        {
            SolanaStreamingRpcClient c = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/");
            var b64Dec = new Base64Encoder();
            c.Init().Wait();

            var sub = c.SubscribeAccountInfo(
                "4tSvZvnbyzHXLMTiFonMyxZoHmFqau1XArcRCVHLZ5gX", 
                (s, data) =>
                {
                    Console.WriteLine($"Channel: {s.Channel} Slot: {data.Context.Slot} Lamports: {data.Value.Lamports} AccountData: {data.Value.Data[0]} Encoding: {data.Value.Data[1]}");
                    //Console.WriteLine($"AccountInfo Decoded: {Encoding.UTF8.GetString(b64Dec.DecodeData(data.Value.Data[0]))}");
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