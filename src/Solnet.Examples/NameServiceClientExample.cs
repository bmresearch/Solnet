using Solnet.Programs.Clients;
using Solnet.Rpc;
using System;

namespace Solnet.Examples
{

    public class NameServiceClientExample : IExample
    {
        public void Run()
        {
            var rpc = ClientFactory.GetClient(Cluster.MainNet);
            var wrpc = ClientFactory.GetStreamingClient(Cluster.MainNet);

            var c = new NameServiceClient(rpc);

            var test1 = c.GetAddressFromNameAsync("bonfida.sol").Result;
            if (test1.WasSuccessful)
            {
                Console.WriteLine($"Name: 'bonfida.sol' \t Address: {test1.ParsedResult.Header.Owner}");
                Console.WriteLine($"Account Contents: ");
                Console.WriteLine("------------------------------------ BASE 64 ---------------------------------------");
                Console.WriteLine(System.Convert.ToBase64String(test1.ParsedResult.Value));
                Console.WriteLine("------------------------------------------------------------------------------------");
                Console.WriteLine("--------------------------- TENTATIVE UTF8 DECODING --------------------------------");
                Console.WriteLine(System.Text.Encoding.UTF8.GetString(test1.ParsedResult.Value));
                Console.WriteLine("------------------------------------------------------------------------------------");
            }
            else
            {
                Console.WriteLine($"Unable to resolve name.");
            }

            var test2 = c.GetNamesFromAddressAsync("BriW4tTAiAm541uB2Fua3dUNoGayRa8Wt7pZUshUbrPB").Result;
            if (test2 != null && test2.Count > 0)
            {
                foreach (var item in test2)
                {
                    Console.WriteLine($"Record Type: {item.Type} \t Name: {item.Name} \t Address: {item.AccountAddress} ");
                }
            }

            var test3 = c.GetAddressFromTwitterHandleAsync("bonfida").Result;
            if (test3.WasSuccessful)
            {
                Console.WriteLine($"Twitter: '@bonfida' \t Address: {test3.ParsedResult.Header.Owner}");
                Console.WriteLine($"Account Contents: ");
                Console.WriteLine("------------------------------------ BASE 64 ---------------------------------------");
                Console.WriteLine(System.Convert.ToBase64String(test3.ParsedResult.Value));
                Console.WriteLine("------------------------------------------------------------------------------------");
                Console.WriteLine("--------------------------- TENTATIVE UTF8 DECODING --------------------------------");
                Console.WriteLine(System.Text.Encoding.UTF8.GetString(test3.ParsedResult.Value));
                Console.WriteLine("------------------------------------------------------------------------------------");
            }
            else
            {
                Console.WriteLine($"Unable to resolve name.");
            }

            var test4 = c.GetTwitterHandleFromAddressAsync("FidaeBkZkvDqi1GXNEwB8uWmj9Ngx2HXSS5nyGRuVFcZ").Result;
            if (test4.WasSuccessful)
            {
                Console.WriteLine($"Address: {test4.ParsedResult.Header.Owner} \t Twitter: {test4.ParsedResult.TwitterHandle}");
            }
            else
            {
                Console.WriteLine($"Unable to resolve name.");
            }

            var test5 = c.GetAllNamesByOwnerAsync("FidaeBkZkvDqi1GXNEwB8uWmj9Ngx2HXSS5nyGRuVFcZ").Result;
            if (test5 != null && test5.Count > 0)
            {
                foreach (var item in test5)
                {
                    Console.WriteLine($"Record Type: {item.Type} \t Address: {item.AccountAddress} \t Value: {item.GetValue()} ");
                }
            }

            var test6 = c.GetTokenInfoFromMintAsync("EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v").Result;
            if (test6.WasSuccessful)
            {
                Console.WriteLine($"Mint Address: {test6.ParsedResult.Value.Mint} \t Ticker: ${test6.ParsedResult.Value.Ticker} \t Name: {test6.ParsedResult.Value.Name} \t  Name: {test6.ParsedResult.Value.Website}");
            }
            else
            {
                Console.WriteLine($"Unable to resolve token mint.");
            }

            var test7 = c.GetMintFromTokenTickerAsync("USDC").Result;
            if (test7.WasSuccessful)
            {
                Console.WriteLine($"Ticker: ${test6.ParsedResult.Value.Ticker} \t Mint Address: {test7.ParsedResult.Value} \t ");
            }
            else
            {
                Console.WriteLine($"Unable to resolve token mint.");
            }
        }
    }
}
