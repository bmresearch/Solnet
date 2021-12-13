using Solnet.Programs.Abstract;
using Solnet.Programs.Models;
using Solnet.Rpc;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Clients
{
    public class NameServiceClient : BaseClient
    {
        public static readonly PublicKey TokenTLD = new("6NSu2tci4apRKQtt257bAVcvqYjB3zV2H1dWo56vgpa6");

        public NameServiceClient(IRpcClient rpcClient, IStreamingRpcClient streamingRpcClient) : base(rpcClient, streamingRpcClient)
        {
        }

        public async Task<AccountResultWrapper<TokenData>> GetTokenFromMint(string key)
        {
            var hashedName = NameServiceProgram.ComputeHashedName(key);

            var pda = NameServiceProgram.DeriveNameAccountKey(hashedName, null, TokenTLD);


            return await GetAccount<TokenData>(pda);
        }

        public void GetMintFromToken(string token)
        {
        }
    }
}
