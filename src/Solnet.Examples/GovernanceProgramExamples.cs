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
            governanceClient = new GovernanceClient(mRpcClient, GovernanceProgram.MangoGovernanceProgramIdKey);
        }

        public void Run()
        {
            var realms = governanceClient.GetRealms(GovernanceProgram.MangoGovernanceProgramIdKey);

            for(int i = 0; i < realms.ParsedResult.Count; i++)
            {
                Console.WriteLine($"--------------------------------------\n" +
                    $"Realm: {realms.ParsedResult[i].Name}\n" +
                    $"Community Mint: {realms.ParsedResult[i].CommunityMint}\n" +
                    $"Authority: {realms.ParsedResult[i]?.Authority}\n" +
                    $"Council Mint: {realms.ParsedResult[i].Config?.CouncilMint}\n" +
                    $"Vote Weight Source: {realms.ParsedResult[i].Config.CommunityMintMaxVoteWeightSource}\n");

                var progGovernances = governanceClient.GetProgramGovernanceAccounts(realms.OriginalRequest.Result[i].PublicKey, realms.ParsedResult[i].Name);
                var mintGovernances = governanceClient.GetMintGovernanceAccounts(GovernanceProgram.MangoGovernanceProgramIdKey, realms.OriginalRequest.Result[i].PublicKey);
                var tokenGovernances = governanceClient.GetTokenGovernanceAccounts(GovernanceProgram.MangoGovernanceProgramIdKey, realms.OriginalRequest.Result[i].PublicKey);
                var genericGovernances = governanceClient.GetGenericGovernanceAccounts(GovernanceProgram.MangoGovernanceProgramIdKey, realms.OriginalRequest.Result[i].PublicKey);
                Console.WriteLine($"Program Governance Accounts: {progGovernances.ParsedResult?.Count}\n" +
                    $"Mint Governance Accounts: {mintGovernances.ParsedResult?.Count}\n" + 
                    $"Token Governance Accounts: {tokenGovernances.ParsedResult?.Count}\n" +
                    $"Generic Governance Accounts: {genericGovernances.ParsedResult?.Count}\n");

                for(int j = 0; j < progGovernances.ParsedResult?.Count; j++)
                {
                    var proposals = governanceClient.GetProposalsV1(GovernanceProgram.MangoGovernanceProgramIdKey, progGovernances.OriginalRequest.Result[j].PublicKey);
                    Console.WriteLine($"Program Governance: {progGovernances.OriginalRequest.Result[j].PublicKey}\n" +
                        $"Proposals: {proposals.OriginalRequest.Result.Count}");
                    for(int k = 0; k < proposals.ParsedResult?.Count; k++)
                    {
                        Console.WriteLine($"Proposal: {proposals.ParsedResult[k].Name}\n" +
                            $"Link: {proposals.ParsedResult[k].DescriptionLink}");
                    }
                }
            }

        }
    }
}
