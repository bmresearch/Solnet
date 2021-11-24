using Solnet.Programs.Abstract;
using Solnet.Programs.Models;
using Solnet.Programs.TokenLending.Models;
using Solnet.Rpc;
using Solnet.Rpc.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solnet.Programs.TokenLending
{
    /// <summary>
    /// Implements functionality for the Token Lending Program client.
    /// </summary>
    public class TokenLendingClient : BaseClient, ITokenLendingClient
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rpcClient"></param>
        /// <param name="streamingRpcClient"></param>
        public TokenLendingClient(IRpcClient rpcClient, IStreamingRpcClient streamingRpcClient)
            : base(rpcClient, streamingRpcClient)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ProgramAccountsResultWrapper<List<LendingMarket>>> GetLendingMarketsAsync(Commitment commitment = Commitment.Finalized)
        {
            return await GetProgramAccounts<LendingMarket>(TokenLendingProgram.TokenLendingProgramIdKey, null,
                LendingMarket.Layout.Length, commitment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ProgramAccountsResultWrapper<List<LendingMarket>> GetLendingMarkets(Commitment commitment = Commitment.Finalized)
            => GetLendingMarketsAsync(commitment).Result;
    }
}
