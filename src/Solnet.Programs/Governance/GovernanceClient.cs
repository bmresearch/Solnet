using Solnet.Programs.Abstract;
using Solnet.Programs.Governance.Models;
using Solnet.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Governance
{
    /// <summary>
    /// 
    /// </summary>
    public class GovernanceClient : BaseClient
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rpcClient"></param>
        /// <param name="streamingRpcClient"></param>
        public GovernanceClient(IRpcClient rpcClient, IStreamingRpcClient streamingRpcClient) : base(rpcClient, streamingRpcClient)
        {

        }


        public async Task<List<Realm>> GetRealms()
        {

        }
    }
}
