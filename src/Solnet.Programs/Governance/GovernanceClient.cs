using Solnet.Programs.Abstract;
using Solnet.Programs.Governance.Enums;
using Solnet.Programs.Governance.Models;
using Solnet.Programs.Models;
using Solnet.Rpc;
using Solnet.Rpc.Models;
using Solnet.Wallet.Utilities;
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
    public class GovernanceClient : BaseClient, IGovernanceClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rpcClient"></param>
        /// <param name="streamingRpcClient"></param>
        public GovernanceClient(IRpcClient rpcClient, IStreamingRpcClient streamingRpcClient) : base(rpcClient, streamingRpcClient)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ProgramAccountsResultWrapper<List<Realm>>> GetRealmsAsync()
        {
            var filters = new List<MemCmp>
            {
                new MemCmp{ Bytes = Encoders.Base58.EncodeData(new byte[]{ (byte)GovernanceAccountType.Realm }), Offset = GovernanceProgramAccount.Layout.AccountTypeOffset }
            };
            return await GetProgramAccounts<Realm>(GovernanceProgram.MainNetProgramIdKey, filters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ProgramAccountsResultWrapper<List<Realm>> GetRealms() => GetRealmsAsync().Result;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <returns></returns>
        public async Task<ProgramAccountsResultWrapper<List<GovernanceAccount>>> GetGovernanceAccountsAsync(string realm)
        {
            var filters = new List<MemCmp>
            {
                new MemCmp{ Bytes = Encoders.Base58.EncodeData(new byte[]{ (byte)GovernanceAccountType.AccountGovernance }), Offset = GovernanceProgramAccount.Layout.AccountTypeOffset },
                new MemCmp{ Bytes = realm, Offset = GovernanceAccount.ExtraLayout.RealmOffset }
            };
            return await GetProgramAccounts<GovernanceAccount>(GovernanceProgram.MainNetProgramIdKey, filters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <returns></returns>
        public ProgramAccountsResultWrapper<List<GovernanceAccount>> GetGovernanceAccounts(string realm) => GetGovernanceAccountsAsync(realm).Result;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <returns></returns>
        public async Task<ProgramAccountsResultWrapper<List<GovernanceAccount>>> GetMintGovernanceAccountsAsync(string realm)
        {
            var filters = new List<MemCmp>
            {
                new MemCmp{ Bytes = Encoders.Base58.EncodeData(new byte[]{ (byte)GovernanceAccountType.MintGovernance }), Offset = GovernanceProgramAccount.Layout.AccountTypeOffset },
                new MemCmp{ Bytes = realm, Offset = GovernanceAccount.ExtraLayout.RealmOffset }
            };
            return await GetProgramAccounts<GovernanceAccount>(GovernanceProgram.MainNetProgramIdKey, filters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <returns></returns>
        public ProgramAccountsResultWrapper<List<GovernanceAccount>> GetMintGovernanceAccounts(string realm) => GetMintGovernanceAccountsAsync(realm).Result;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <returns></returns>
        public async Task<ProgramAccountsResultWrapper<List<GovernanceAccount>>> GetProgramGovernanceAccountsAsync(string realm)
        {
            var filters = new List<MemCmp>
            {
                new MemCmp{ Bytes = Encoders.Base58.EncodeData(new byte[]{ (byte)GovernanceAccountType.ProgramGovernance }), Offset = GovernanceProgramAccount.Layout.AccountTypeOffset },
                new MemCmp{ Bytes = realm, Offset = GovernanceAccount.ExtraLayout.RealmOffset }
            };
            return await GetProgramAccounts<GovernanceAccount>(GovernanceProgram.MainNetProgramIdKey, filters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <returns></returns>
        public ProgramAccountsResultWrapper<List<GovernanceAccount>> GetProgramGovernanceAccounts(string realm) => GetProgramGovernanceAccountsAsync(realm).Result;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <returns></returns>
        public async Task<ProgramAccountsResultWrapper<List<GovernanceAccount>>> GetTokenGovernanceAccountsAsync(string realm)
        {
            var filters = new List<MemCmp>
            {
                new MemCmp{ Bytes = Encoders.Base58.EncodeData(new byte[]{ (byte)GovernanceAccountType.TokenGovernance }), Offset = GovernanceProgramAccount.Layout.AccountTypeOffset },
                new MemCmp{ Bytes = realm, Offset = GovernanceAccount.ExtraLayout.RealmOffset }
            };
            return await GetProgramAccounts<GovernanceAccount>(GovernanceProgram.MainNetProgramIdKey, filters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <returns></returns>
        public ProgramAccountsResultWrapper<List<GovernanceAccount>> GetTokenGovernanceAccounts(string realm) => GetTokenGovernanceAccountsAsync(realm).Result;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public async Task<ProgramAccountsResultWrapper<List<TokenOwnerRecord>>> GetTokenOwnerRecordsAsync(string realm, string owner)
        {
            var filters = new List<MemCmp>
            {
                new MemCmp{ Bytes = Encoders.Base58.EncodeData(new byte[]{ (byte)GovernanceAccountType.TokenOwnerRecord }), Offset = GovernanceProgramAccount.Layout.AccountTypeOffset },
                new MemCmp{ Bytes = realm, Offset = TokenOwnerRecord.ExtraLayout.RealmOffset },
                new MemCmp{ Bytes = owner, Offset = TokenOwnerRecord.ExtraLayout.GoverningTokenOwnerOffset }
            };
            return await GetProgramAccounts<TokenOwnerRecord>(GovernanceProgram.MainNetProgramIdKey, filters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public ProgramAccountsResultWrapper<List<TokenOwnerRecord>> GetTokenOwnerRecords(string realm, string owner) => GetTokenOwnerRecordsAsync(realm, owner).Result;

    }
}
