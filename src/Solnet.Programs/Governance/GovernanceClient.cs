using Solnet.Programs.Abstract;
using Solnet.Programs.Governance.Enums;
using Solnet.Programs.Governance.Models;
using Solnet.Programs.Models;
using Solnet.Rpc;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solnet.Programs.Governance
{
    /// <summary>
    /// Implements a client for the governance program.
    /// </summary>
    public class GovernanceClient : BaseClient
    {
        /// <summary>
        /// Initialize the governance client.
        /// </summary>
        /// <param name="rpcClient"></param>
        /// <param name="governanceProgramID"></param>
        public GovernanceClient(IRpcClient rpcClient, PublicKey governanceProgramID) : base(rpcClient, null, governanceProgramID) { }

        /// <summary>
        /// Gets all <see cref="Realm"/>s for the given program id. This is an asynchronous operation.
        /// </summary>
        /// <param name="programId">The deployed governance program id.</param>
        /// <returns>A task which may return the list of program accounts and their decoded structures.</returns>
        public async Task<ProgramAccountsResultWrapper<List<Realm>>> GetRealmsAsync(string programId)
        {
            var filters = new List<MemCmp>
            {
                new MemCmp{ Bytes = Encoders.Base58.EncodeData(new byte[]{ (byte)GovernanceAccountType.Realm }), Offset = GovernanceProgramAccount.Layout.AccountTypeOffset }
            };
            return await GetProgramAccounts<Realm>(programId, filters);
        }

        /// <summary>
        /// Gets all <see cref="Realm"/>s for the given program id.
        /// </summary>
        /// <returns>The list of program accounts and their decoded structures.</returns>
        public ProgramAccountsResultWrapper<List<Realm>> GetRealms(string programId) => GetRealmsAsync(programId).Result;

        /// <summary>
        /// Gets all <see cref="GovernanceAccount"/>s for the given program id. This is an asynchronous operation.
        /// </summary>
        /// <param name="programId">The deployed governance program id.</param>
        /// <param name="realm">The realm the governances belong to.</param>
        /// <returns>A task which may return the list of program accounts and their decoded structures.</returns>
        public async Task<ProgramAccountsResultWrapper<List<GovernanceAccount>>> GetGovernancesAsync(string programId, string realm)
        {
            var filters = new List<MemCmp>
            {
                new MemCmp{ Bytes = realm, Offset = GovernanceAccount.ExtraLayout.RealmOffset }
            };
            return await GetProgramAccounts<GovernanceAccount>(programId, filters);
        }

        /// <summary>
        /// Gets all <see cref="GovernanceAccount"/>s for the given program id.
        /// </summary>
        /// <param name="programId">The deployed governance program id.</param>
        /// <param name="realm">The realm the governances belong to.</param>
        /// <returns>The list of program accounts and their decoded structures.</returns>
        public ProgramAccountsResultWrapper<List<GovernanceAccount>> GetGovernances(string programId, string realm) => GetGovernancesAsync(programId, realm).Result;


        /// <summary>
        /// Gets all <see cref="GovernanceAccount"/>s of the type <see cref="GovernanceAccountType.MintGovernance"/> for the given program id. This is an asynchronous operation.
        /// </summary>
        /// <param name="programId">The deployed governance program id.</param>
        /// <param name="realm">The realm the governances belong to.</param>
        /// <returns>A task which may return the list of program accounts and their decoded structures.</returns>
        public async Task<ProgramAccountsResultWrapper<List<GovernanceAccount>>> GetMintGovernanceAccountsAsync(string programId, string realm)
        {
            var filters = new List<MemCmp>
            {
                new MemCmp{ Bytes = Encoders.Base58.EncodeData(new byte[]{ (byte)GovernanceAccountType.MintGovernance }), Offset = GovernanceProgramAccount.Layout.AccountTypeOffset },
                new MemCmp{ Bytes = realm, Offset = GovernanceAccount.ExtraLayout.RealmOffset }
            };
            return await GetProgramAccounts<GovernanceAccount>(programId, filters);
        }

        /// <summary>
        /// Gets all <see cref="GovernanceAccount"/>s of the type <see cref="GovernanceAccountType.MintGovernance"/> for the given program id.
        /// </summary>
        /// <param name="programId">The deployed governance program id.</param>
        /// <param name="realm">The realm the governances belong to.</param>
        /// <returns>The list of program accounts and their decoded structures.</returns>
        public ProgramAccountsResultWrapper<List<GovernanceAccount>> GetMintGovernanceAccounts(string programId, string realm) => GetMintGovernanceAccountsAsync(programId, realm).Result;


        /// <summary>
        /// Gets all <see cref="GovernanceAccount"/>s of the type <see cref="GovernanceAccountType.ProgramGovernance"/> for the given program id. This is an asynchronous operation.
        /// </summary>
        /// <param name="programId">The deployed governance program id.</param>
        /// <param name="realm">The realm the governances belong to.</param>
        /// <returns>A task which may return the list of program accounts and their decoded structures.</returns>
        public async Task<ProgramAccountsResultWrapper<List<GovernanceAccount>>> GetProgramGovernanceAccountsAsync(string programId, string realm)
        {
            var filters = new List<MemCmp>
            {
                new MemCmp{ Bytes = Encoders.Base58.EncodeData(new byte[]{ (byte)GovernanceAccountType.ProgramGovernance }), Offset = GovernanceProgramAccount.Layout.AccountTypeOffset },
                new MemCmp{ Bytes = realm, Offset = GovernanceAccount.ExtraLayout.RealmOffset }
            };
            return await GetProgramAccounts<GovernanceAccount>(programId, filters);
        }

        /// <summary>
        /// Gets all <see cref="GovernanceAccount"/>s of the type <see cref="GovernanceAccountType.ProgramGovernance"/> for the given program id.
        /// </summary>
        /// <param name="programId">The deployed governance program id.</param>
        /// <param name="realm">The realm the governances belong to.</param>
        /// <returns>The list of program accounts and their decoded structures.</returns>
        public ProgramAccountsResultWrapper<List<GovernanceAccount>> GetProgramGovernanceAccounts(string programId, string realm) => GetProgramGovernanceAccountsAsync(programId, realm).Result;


        /// <summary>
        /// Gets all <see cref="GovernanceAccount"/>s of the type <see cref="GovernanceAccountType.TokenGovernance"/> for the given program id. This is an asynchronous operation.
        /// </summary>
        /// <param name="programId">The deployed governance program id.</param>
        /// <param name="realm">The realm the governances belong to.</param>
        /// <returns>A task which may return the list of program accounts and their decoded structures.</returns>
        public async Task<ProgramAccountsResultWrapper<List<GovernanceAccount>>> GetTokenGovernanceAccountsAsync(string programId, string realm)
        {
            var filters = new List<MemCmp>
            {
                new MemCmp{ Bytes = Encoders.Base58.EncodeData(new byte[]{ (byte)GovernanceAccountType.TokenGovernance }), Offset = GovernanceProgramAccount.Layout.AccountTypeOffset },
                new MemCmp{ Bytes = realm, Offset = GovernanceAccount.ExtraLayout.RealmOffset }
            };
            return await GetProgramAccounts<GovernanceAccount>(programId, filters);
        }

        /// <summary>
        /// Gets all <see cref="GovernanceAccount"/>s of the type <see cref="GovernanceAccountType.TokenGovernance"/> for the given program id.
        /// </summary>
        /// <param name="programId">The deployed governance program id.</param>
        /// <param name="realm">The realm the governances belong to.</param>
        /// <returns>The list of program accounts and their decoded structures.</returns>
        public ProgramAccountsResultWrapper<List<GovernanceAccount>> GetTokenGovernanceAccounts(string programId, string realm) => GetTokenGovernanceAccountsAsync(programId, realm).Result;


        /// <summary>
        /// Gets all <see cref="GovernanceAccount"/>s of the type <see cref="GovernanceAccountType.AccountGovernance"/> for the given program id. This is an asynchronous operation.
        /// </summary>
        /// <param name="programId">The deployed governance program id.</param>
        /// <param name="realm">The realm the governances belong to.</param>
        /// <returns>A task which may return the list of program accounts and their decoded structures.</returns>
        public async Task<ProgramAccountsResultWrapper<List<GovernanceAccount>>> GetGenericGovernanceAccountsAsync(string programId, string realm)
        {
            var filters = new List<MemCmp>
            {
                new MemCmp{ Bytes = Encoders.Base58.EncodeData(new byte[]{ (byte)GovernanceAccountType.AccountGovernance }), Offset = GovernanceProgramAccount.Layout.AccountTypeOffset },
                new MemCmp{ Bytes = realm, Offset = GovernanceAccount.ExtraLayout.RealmOffset }
            };
            return await GetProgramAccounts<GovernanceAccount>(programId, filters);
        }

        /// <summary>
        /// Gets all <see cref="GovernanceAccount"/>s of the type <see cref="GovernanceAccountType.AccountGovernance"/> for the given program id.
        /// </summary>
        /// <param name="programId">The deployed governance program id.</param>
        /// <param name="realm">The realm the governances belong to.</param>
        /// <returns>The list of program accounts and their decoded structures.</returns>
        public ProgramAccountsResultWrapper<List<GovernanceAccount>> GetGenericGovernanceAccounts(string programId, string realm) => GetGenericGovernanceAccountsAsync(programId, realm).Result;

        /// <summary>
        /// Gets all <see cref="TokenOwnerRecord"/>s of the given owner for the given program id. This is an asynchronous operation.
        /// </summary>
        /// <param name="programId">The deployed governance program id.</param>
        /// <param name="realm">The realm the governances belong to.</param>
        /// <param name="owner">The owner of the token owner records.</param>
        /// <returns>A task which may return the list of program accounts and their decoded structures.</returns>
        public async Task<ProgramAccountsResultWrapper<List<TokenOwnerRecord>>> GetTokenOwnerRecordsAsync(string programId, string realm, string owner)
        {
            var filters = new List<MemCmp>
            {
                new MemCmp{ Bytes = Encoders.Base58.EncodeData(new byte[]{ (byte)GovernanceAccountType.TokenOwnerRecord }), Offset = GovernanceProgramAccount.Layout.AccountTypeOffset },
                new MemCmp{ Bytes = realm, Offset = TokenOwnerRecord.ExtraLayout.RealmOffset },
                new MemCmp{ Bytes = owner, Offset = TokenOwnerRecord.ExtraLayout.GoverningTokenOwnerOffset }
            };
            return await GetProgramAccounts<TokenOwnerRecord>(programId, filters);
        }

        /// <summary>
        /// Gets all <see cref="TokenOwnerRecord"/>s of the given owner for the given program id.
        /// </summary>
        /// <param name="programId">The deployed governance program id.</param>
        /// <param name="realm">The realm the governances belong to.</param>
        /// <param name="owner">The owner of the token owner records.</param>
        /// <returns>The list of program accounts and their decoded structures.</returns>
        public ProgramAccountsResultWrapper<List<TokenOwnerRecord>> GetTokenOwnerRecords(string programId, string realm, string owner) => GetTokenOwnerRecordsAsync(programId, realm, owner).Result;

        /// <summary>
        /// Gets all <see cref="ProposalV1"/>s of the giuven governance for the given program id. This is an asynchronous operation.
        /// </summary>
        /// <param name="programId">The deployed governance program id.</param>
        /// <param name="governance">The governance the proposals belong to.</param>
        /// <returns>A task which may return the list of program accounts and their decoded structures.</returns>
        public async Task<ProgramAccountsResultWrapper<List<ProposalV1>>> GetProposalsV1Async(string programId, string governance)
        {
            var filters = new List<MemCmp>
            {
                new MemCmp{ Bytes = Encoders.Base58.EncodeData(new byte[]{ (byte)GovernanceAccountType.ProposalV1 }), Offset = GovernanceProgramAccount.Layout.AccountTypeOffset },
                new MemCmp{ Bytes = governance, Offset = ProposalV2.ExtraLayout.GovernanceOffset },
            };
            return await GetProgramAccounts<ProposalV1>(programId, filters);
        }

        /// <summary>
        /// Gets all <see cref="ProposalV1"/>s of the giuven governance for the given program id.
        /// </summary>
        /// <param name="programId">The deployed governance program id.</param>
        /// <param name="governance">The governance the proposals belong to.</param>
        /// <returns>The list of program accounts and their decoded structures.</returns>
        public ProgramAccountsResultWrapper<List<ProposalV1>> GetProposalsV1(string programId, string governance) => GetProposalsV1Async(programId, governance).Result;

        /// <summary>
        /// Gets all <see cref="ProposalV2"/>s of the giuven governance for the given program id. This is an asynchronous operation.
        /// </summary>
        /// <param name="programId">The deployed governance program id.</param>
        /// <param name="governance">The governance the proposals belong to.</param>
        /// <returns>A task which may return the list of program accounts and their decoded structures.</returns>
        public async Task<ProgramAccountsResultWrapper<List<ProposalV2>>> GetProposalsV2Async(string programId, string governance)
        {
            var filters = new List<MemCmp>
            {
                new MemCmp{ Bytes = Encoders.Base58.EncodeData(new byte[]{ (byte)GovernanceAccountType.ProposalV1 }), Offset = GovernanceProgramAccount.Layout.AccountTypeOffset },
                new MemCmp{ Bytes = governance, Offset = ProposalV2.ExtraLayout.GovernanceOffset },
            };
            return await GetProgramAccounts<ProposalV2>(programId, filters);
        }

        /// <summary>
        /// Gets all <see cref="ProposalV2"/>s of the giuven governance for the given program id.
        /// </summary>
        /// <param name="programId">The deployed governance program id.</param>
        /// <param name="governance">The governance the proposals belong to.</param>
        /// <returns>The list of program accounts and their decoded structures.</returns>
        public ProgramAccountsResultWrapper<List<ProposalV2>> GetProposalsV2(string programId, string governance) => GetProposalsV2Async(programId, governance).Result;

    }
}
