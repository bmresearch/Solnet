using Solnet.Programs.Governance;
using Solnet.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Examples
{
    public class GovernanceProgramExamples : IExample
    {

        private static readonly IRpcClient mRpcClient = ClientFactory.GetClient(Cluster.MainNet);
        private GovernanceClient governanceClient;

        public GovernanceProgramExamples()
        {
            governanceClient = new GovernanceClient(mRpcClient, null);
        }

        public void Run()
        {
            var realms = governanceClient.GetRealms();

            for(int i = 0; i < realms.ParsedResult.Count; i++)
            {
                Console.WriteLine($"--------------------------------------\n" +
                    $"Realm: {realms.ParsedResult[i].Name}\n" +
                    $"Community Mint: {realms.ParsedResult[i].CommunityMint}\n" +
                    $"Authority: {realms.ParsedResult[i]?.Authority}\n" +
                    $"Council Mint: {realms.ParsedResult[i].Config?.CouncilMint}\n" +
                    $"Vote Weight Source: {realms.ParsedResult[i].Config.CommunityMintMaxVoteWeightSource}\n");
                var governances = governanceClient.GetGovernanceAccounts(realms.OriginalRequest.Result[i].PublicKey);
                Console.WriteLine($"Governance Accounts: {governances?.ParsedResult.Count}\n" +
                    $"--------------------------------------\n");
            }

        }
    }
}
