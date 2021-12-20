using Solnet.Programs.Abstract;
using Solnet.Programs.Models;
using Solnet.Rpc;
using Solnet.Rpc.Models;
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
        public static PublicKey ProgramID => NameServiceProgram.ProgramIdKey;

        public static readonly PublicKey TokenTLD = new("6NSu2tci4apRKQtt257bAVcvqYjB3zV2H1dWo56vgpa6");

        public static readonly PublicKey TwitterTLD = new("4YcexoW3r78zz16J2aqmukBLRwGq6rAvWzJpkYAXqebv");
        public static readonly PublicKey TwitterVerificationAuthority = new("FvPH7PrVrLGKPfqaf3xJodFTjZriqrAXXLTVWEorTFBi");

        public static readonly PublicKey SolTLD = new("58PwtjSDuFHuUkYjH9BYnnQKHfwo9reZhC2zMJv9JPkx");
        public static readonly PublicKey ReverseSolTLD = new("33m47vH6Eav6jr5Ry86XjhRft2jRBLDnDgPSHoquXi2Z");

        public NameServiceClient(IRpcClient rpcClient, IStreamingRpcClient streamingRpcClient) : base(rpcClient, streamingRpcClient)
        {
        }

        public async Task<AccountResultWrapper<TokenNameRecord>> GetTokenFromMint(string key)
        {
            var hashedName = NameServiceProgram.ComputeHashedName(key);

            var pda = NameServiceProgram.DeriveNameAccountKey(hashedName, null, TokenTLD);


            return await GetAccount<TokenNameRecord>(pda);
        }

        public async Task<AccountResultWrapper<ReverseTokenNameRecord>> GetMintFromToken(string token)
        {
            var hashedName = NameServiceProgram.ComputeHashedName(token);

            var pda = NameServiceProgram.DeriveNameAccountKey(hashedName, null, TokenTLD);


            return await GetAccount<ReverseTokenNameRecord>(pda);
        }

        public async Task<AccountResultWrapper<NameRecord>> GetAddressFromTwitter(string handle)
        {
            var hashedName = NameServiceProgram.ComputeHashedName(handle);

            var pda = NameServiceProgram.DeriveNameAccountKey(hashedName, null, TwitterTLD);

            return await GetAccount<NameRecord>(pda);
        }

        public async Task<AccountResultWrapper<ReverseTwitterRecord>> GetTwitterFromAddressAsync(string address)
        {
            var hashedName = NameServiceProgram.ComputeHashedName(address);

            var pda = NameServiceProgram.DeriveNameAccountKey(hashedName, TwitterVerificationAuthority, TwitterTLD);

            return await GetAccount<ReverseTwitterRecord>(pda);
        }


        public async Task<AccountResultWrapper<NameRecord>> GetAddressFromName(string name)
        {
            if (name.EndsWith(".sol"))
            {
                name = name.Substring(0, name.Length - 4);
            }

            var hashedName = NameServiceProgram.ComputeHashedName(name);

            var pda = NameServiceProgram.DeriveNameAccountKey(hashedName, null, SolTLD);

            return await GetAccount<NameRecord>(pda);
        }

        public async Task<List<ReverseNameRecord>> GetNamesFromAddress(string address)
        {

            var res = await RpcClient.GetProgramAccountsAsync(ProgramID, Rpc.Types.Commitment.Confirmed, null,
                new List<MemCmp>() { new MemCmp() { Bytes = SolTLD, Offset = 0 }, new MemCmp() { Bytes = address, Offset = 32 }/*, new MemCmp() { Bytes = ReverseSolTLD, Offset = 64 }*/ });


            Dictionary<string, NameRecord> nameToRecordMap = new Dictionary<string, NameRecord>();

            foreach (var add in res.Result)
            {
                var name = NameRecord.Deserialize(Convert.FromBase64String(add.Account.Data[0]));

                //nameToRecordMap.Add(add.PublicKey, name);

                var hashedName = NameServiceProgram.ComputeHashedName(add.PublicKey);

                var pda = NameServiceProgram.DeriveNameAccountKey(hashedName, ReverseSolTLD, null);

                nameToRecordMap.Add(pda, name);
                //var res4 = await GetAccount<NameRecord<ReverseRecord>>(pda);
            }

            List<ReverseNameRecord> ret = new();
            var reverseNameAddresses = nameToRecordMap.Keys.ToList();

            var multipleAccs = await RpcClient.GetMultipleAccountsAsync(reverseNameAddresses, Rpc.Types.Commitment.Confirmed);

            for(int i = 0; i < reverseNameAddresses.Count; i++)
            {
                var nr = nameToRecordMap[reverseNameAddresses[i]];

                var rev = ReverseNameRecord.Deserialize(Convert.FromBase64String(multipleAccs.Result.Value[i].Data[0]));

                rev.Value = nr;

                ret.Add(rev);
            }
            
            return ret;
        }
    }
}
