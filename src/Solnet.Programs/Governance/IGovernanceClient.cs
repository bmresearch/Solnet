using Solnet.Programs.Governance.Models;
using Solnet.Programs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solnet.Programs.Governance
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGovernanceClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <returns></returns>
        ProgramAccountsResultWrapper<List<GovernanceAccount>> GetGovernanceAccounts(string realm);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <returns></returns>
        Task<ProgramAccountsResultWrapper<List<GovernanceAccount>>> GetGovernanceAccountsAsync(string realm);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <returns></returns>
        ProgramAccountsResultWrapper<List<GovernanceAccount>> GetMintGovernanceAccounts(string realm);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <returns></returns>
        Task<ProgramAccountsResultWrapper<List<GovernanceAccount>>> GetMintGovernanceAccountsAsync(string realm);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <returns></returns>
        ProgramAccountsResultWrapper<List<GovernanceAccount>> GetProgramGovernanceAccounts(string realm);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <returns></returns>
        Task<ProgramAccountsResultWrapper<List<GovernanceAccount>>> GetProgramGovernanceAccountsAsync(string realm);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ProgramAccountsResultWrapper<List<Realm>> GetRealms();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<ProgramAccountsResultWrapper<List<Realm>>> GetRealmsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <returns></returns>
        ProgramAccountsResultWrapper<List<GovernanceAccount>> GetTokenGovernanceAccounts(string realm);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <returns></returns>
        Task<ProgramAccountsResultWrapper<List<GovernanceAccount>>> GetTokenGovernanceAccountsAsync(string realm);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        ProgramAccountsResultWrapper<List<TokenOwnerRecord>> GetTokenOwnerRecords(string realm, string owner);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        Task<ProgramAccountsResultWrapper<List<TokenOwnerRecord>>> GetTokenOwnerRecordsAsync(string realm, string owner);
    }
}