using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solnet.Rpc.Http;

namespace Solnet.Examples
{
    class SolnetRpcTester
    {

        static void Example(string[] args)
        {

            SolanaJsonRpcClient c = new SolanaJsonRpcClient();


            //var accInfo = c.GetAccountInfo("vines1vzrYbzLMRdu58ou5XTby4qAqVRLmqo36NKPTg");

            //var balance = c.GetBalance("9UGxCidmZtU1PM7Tbhv2twQ8ChsS6S3HdL1xo56fSVWn");
            //var accInfo = c.GetGenesisHash();

            //var blockCommitment = c.GetBlockCommitment(78561320);

            var blockTime = c.GetBlockTime(78561320);
        }
    }
}
