using Solnet.Programs.Abstract;
using Solnet.Programs.Governance.Enums;
using Solnet.Programs.Governance.Models;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System.Collections.Generic;
using System.Text;

namespace Solnet.Programs.Governance
{
    /// <summary>
    /// Implements the governance program instructions.
    /// <remarks>
    /// For more information see:
    /// https://github.com/solana-labs/solana-program-library/blob/master/governance/
    /// </remarks>
    /// </summary>
    public class GovernanceProgram : BaseProgram
    {
        /// <summary>
        /// The seed used for the realm config address PDA.
        /// <remarks>
        /// Account PDA seeds: ['realm-config', realm]
        /// </remarks>
        /// </summary>
        public static readonly string RealmConfigSeed = "realm-config";

        /// <summary>
        /// The seed used for the realm config address PDA.
        /// <remarks>
        /// Account PDA seeds: ['realm-config', realm]
        /// </remarks>
        /// </summary>
        public static readonly string ProgramAuthoritySeed = "governance";

        /// <summary>
        /// The program's name.
        /// </summary>
        public const string GovernanceProgramName = "Governance Program";

        /// <summary>
        /// The program's public key.
        /// </summary>
        public static readonly PublicKey MainNetProgramIdKey = new("GqTPL6qRf5aUuqscLh8Rg2HTxPUXfhhAXDptTLhp1t2J");

        /// <summary>
        /// The public key of the governance program used by Mango Markets.
        /// </summary>
        public static readonly PublicKey MangoGovernanceProgramIdKey = new("GqTPL6qRf5aUuqscLh8Rg2HTxPUXfhhAXDptTLhp1t2J");

        /// <summary>
        /// The public key of the governance program used by Project Serum.
        /// </summary>
        public static readonly PublicKey SerumGovernanceProgramIdKey = new("AVoAYTs36yB5izAaBkxRG67wL1AMwG3vo41hKtUSb8is");

        /// <summary>
        /// The public key of the governance program used by Socean.
        /// </summary>
        public static readonly PublicKey SoceanGovernanceProgramIdKey = new("5hAykmD4YGcQ7Am3N7nC9kyELq6CThAkU82nhNKDJiCy");

        /// <summary>
        /// Initialize the governance program with the given program id.
        /// </summary>
        /// <param name="programIdKey">The program id.</param>
        public GovernanceProgram(PublicKey programIdKey) : base(programIdKey, GovernanceProgramName) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realmAuthority"></param>
        /// <param name="communityTokenMint"></param>
        /// <param name="payer"></param>
        /// <param name="name"></param>
        /// <param name="minCommunityTokensToCreateGovernance"></param>
        /// <param name="communityMintMaxVoteWeightSource"></param>
        /// <param name="councilTokenMint"></param>
        /// <param name="communityVoterWeightAddin"></param>
        /// <returns></returns>
        public TransactionInstruction CreateRealm(PublicKey realmAuthority, PublicKey communityTokenMint, PublicKey payer, string name,
            ulong minCommunityTokensToCreateGovernance, MintMaxVoteWeightSource communityMintMaxVoteWeightSource,
            PublicKey councilTokenMint = null, PublicKey communityVoterWeightAddin = null)
        {
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="governingTokenSource"></param>
        /// <param name="governingTokenOwner"></param>
        /// <param name="governingTokenTransferAuthority"></param>
        /// <param name="payer"></param>
        /// <param name="amount"></param>
        /// <param name="governingTokenMint"></param>
        /// <returns></returns>
        public TransactionInstruction DepositGoverningTokens(PublicKey realm, PublicKey governingTokenSource, PublicKey governingTokenOwner,
            PublicKey governingTokenTransferAuthority, PublicKey payer, ulong amount, PublicKey governingTokenMint)
        {

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="governingTokenDestination"></param>
        /// <param name="payer"></param>
        /// <param name="governingTokenMint"></param>
        /// <returns></returns>
        public TransactionInstruction WithdrawGoverningToken(PublicKey realm, PublicKey governingTokenDestination, PublicKey payer,
            PublicKey governingTokenMint)
        {
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="governanceAuthority"></param>
        /// <param name="governingTokenMint"></param>
        /// <param name="governingTokenOwner"></param>
        /// <param name="newGovernanceDelegate"></param>
        /// <returns></returns>
        public TransactionInstruction SetGovernanceDelegate(PublicKey realm, PublicKey governanceAuthority, PublicKey governingTokenMint,
            PublicKey governingTokenOwner, PublicKey newGovernanceDelegate = null)
        {
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="governedAccount"></param>
        /// <param name="tokenOwnerRecord"></param>
        /// <param name="payer"></param>
        /// <param name="governanceAuthority"></param>
        /// <param name="config"></param>
        /// <param name="voterWeightRecord"></param>
        /// <returns></returns>
        public TransactionInstruction CreateAccountGovernance(PublicKey realm, PublicKey governedAccount, PublicKey tokenOwnerRecord,
            PublicKey payer, PublicKey governanceAuthority, GovernanceConfig config, PublicKey voterWeightRecord = null)
        {
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="governedProgram"></param>
        /// <param name="governedProgramUpgradeAuthority"></param>
        /// <param name="tokenOwnerRecord"></param>
        /// <param name="payer"></param>
        /// <param name="governanceAuthority"></param>
        /// <param name="transferUpgradeAuthority"></param>
        /// <param name="config"></param>
        /// <param name="voterWeightRecord"></param>
        /// <returns></returns>
        public TransactionInstruction CreateProgramGovernance(PublicKey realm, PublicKey governedProgram, PublicKey governedProgramUpgradeAuthority,
            PublicKey tokenOwnerRecord, PublicKey payer, PublicKey governanceAuthority, bool transferUpgradeAuthority,
            GovernanceConfig config, PublicKey voterWeightRecord = null)
        {
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="governedMint"></param>
        /// <param name="governedMintAuthority"></param>
        /// <param name="tokenOwnerRecord"></param>
        /// <param name="payer"></param>
        /// <param name="governanceAuthority"></param>
        /// <param name="transferMintAuthority"></param>
        /// <param name="config"></param>
        /// <param name="voterWeightRecord"></param>
        /// <returns></returns>
        public TransactionInstruction CreateMintGovernance(PublicKey realm, PublicKey governedMint, PublicKey governedMintAuthority,
            PublicKey tokenOwnerRecord, PublicKey payer, PublicKey governanceAuthority, bool transferMintAuthority,
            GovernanceConfig config, PublicKey voterWeightRecord = null)
        {
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="governedToken"></param>
        /// <param name="governedTokenAuthority"></param>
        /// <param name="tokenOwnerRecord"></param>
        /// <param name="payer"></param>
        /// <param name="governanceAuthority"></param>
        /// <param name="transferTokenOwner"></param>
        /// <param name="config"></param>
        /// <param name="voterWeightRecord"></param>
        /// <returns></returns>
        public TransactionInstruction CreateTokenGovernance(PublicKey realm, PublicKey governedToken, PublicKey governedTokenAuthority,
            PublicKey tokenOwnerRecord, PublicKey payer, PublicKey governanceAuthority, bool transferTokenOwner,
            GovernanceConfig config, PublicKey voterWeightRecord = null)
        {
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="governance"></param>
        /// <param name="proposalOwnerRecord"></param>
        /// <param name="governanceAuthority"></param>
        /// <param name="payer"></param>
        /// <param name="governingTokenMint"></param>
        /// <param name="name"></param>
        /// <param name="descriptionLink"></param>
        /// <param name="voteType"></param>
        /// <param name="options"></param>
        /// <param name="useDenyOption"></param>
        /// <param name="proposalIndex"></param>
        /// <param name="voterWeightRecord"></param>
        /// <returns></returns>
        public TransactionInstruction CreateProposal(PublicKey realm, PublicKey governance, PublicKey proposalOwnerRecord,
            PublicKey governanceAuthority, PublicKey payer, PublicKey governingTokenMint, string name, string descriptionLink,
            VoteType voteType, List<string> options, bool useDenyOption, uint proposalIndex, PublicKey voterWeightRecord = null)
        {
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proposal"></param>
        /// <param name="tokenOwnerRecord"></param>
        /// <param name="governanceAuthority"></param>
        /// <param name="payer"></param>
        /// <param name="signatory"></param>
        /// <returns></returns>
        public TransactionInstruction AddSignatory(PublicKey proposal, PublicKey tokenOwnerRecord, PublicKey governanceAuthority,
            PublicKey payer, PublicKey signatory)
        {

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proposal"></param>
        /// <param name="tokenOwnerRecord"></param>
        /// <param name="governanceAuthority"></param>
        /// <param name="signatory"></param>
        /// <param name="beneficiary"></param>
        /// <returns></returns>
        public TransactionInstruction RemoveSignatory(PublicKey proposal, PublicKey tokenOwnerRecord, PublicKey governanceAuthority,
            PublicKey signatory, PublicKey beneficiary)
        {

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proposal"></param>
        /// <param name="signatory"></param>
        /// <returns></returns>
        public TransactionInstruction SignOffProposal(PublicKey proposal, PublicKey signatory)
        {

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="governance"></param>
        /// <param name="proposal"></param>
        /// <param name="proposalOwnerRecord"></param>
        /// <param name="voterTokenOwnerRecord"></param>
        /// <param name="governanceAuthority"></param>
        /// <param name="governingTokenMint"></param>
        /// <param name="payer"></param>
        /// <param name="vote"></param>
        /// <param name="voterWeightRecord"></param>
        /// <returns></returns>
        public TransactionInstruction CastVote(PublicKey realm, PublicKey governance, PublicKey proposal, PublicKey proposalOwnerRecord,
            PublicKey voterTokenOwnerRecord, PublicKey governanceAuthority, PublicKey governingTokenMint, PublicKey payer, Vote vote,
            PublicKey voterWeightRecord = null)
        {

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="governance"></param>
        /// <param name="proposal"></param>
        /// <param name="proposalOwnerRecord"></param>
        /// <param name="governingTokenMint"></param>
        /// <returns></returns>
        public TransactionInstruction FinalizeVote(PublicKey realm, PublicKey governance, PublicKey proposal, PublicKey proposalOwnerRecord,
            PublicKey governingTokenMint)
        {

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="governance"></param>
        /// <param name="proposal"></param>
        /// <param name="tokenOwnerRecord"></param>
        /// <param name="governingTokenMint"></param>
        /// <param name="governanceAuthority"></param>
        /// <param name="beneficiary"></param>
        /// <returns></returns>
        public TransactionInstruction RelinquishVote(PublicKey governance, PublicKey proposal, PublicKey tokenOwnerRecord,
            PublicKey governingTokenMint, PublicKey governanceAuthority = null, PublicKey beneficiary = null)
        {

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="governance"></param>
        /// <param name="proposal"></param>
        /// <param name="proposalOwnerRecord"></param>
        /// <param name="governanceAuthority"></param>
        /// <returns></returns>
        public TransactionInstruction CancelProposal(PublicKey governance, PublicKey proposal, PublicKey proposalOwnerRecord,
            PublicKey governanceAuthority)
        {

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// Create an instruction to execute a transaction.
        /// </summary>
        /// <param name="governance">The public key of the governance account.</param>
        /// <param name="proposal">The public key of the proposal account.</param>
        /// <param name="proposalInstruction">The public key of the proposal instruction account.</param>
        /// <param name="instructionProgramId">The instruction's program id.</param>
        /// <param name="instructionAccounts">The list of account metas necessary to execute the instruction.</param>
        /// <returns>The transaction instruction.</returns>
        public TransactionInstruction ExecuteInstruction(PublicKey governance, PublicKey proposal, PublicKey proposalInstruction,
            PublicKey instructionProgramId, List<AccountMeta> instructionAccounts)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(governance, false),
                AccountMeta.Writable(proposal, false),
                AccountMeta.Writable(proposalInstruction, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(instructionProgramId, false)
            };

            keys.AddRange(instructionAccounts);

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,
                Keys = keys,
                Data = GovernanceProgramData.EncodeExecuteInstructionData()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="governance"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public TransactionInstruction SetGovernanceConfig(PublicKey governance, GovernanceConfig config)
        {

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proposal"></param>
        /// <param name="tokenOwnerRecord"></param>
        /// <param name="governanceAuthority"></param>
        /// <param name="proposalInstruction"></param>
        /// <returns></returns>
        public TransactionInstruction FlagInstructionError(PublicKey proposal, PublicKey tokenOwnerRecord, PublicKey governanceAuthority,
            PublicKey proposalInstruction)
        {

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="realmAuthority"></param>
        /// <param name="newRealmAuthority"></param>
        /// <returns></returns>
        public TransactionInstruction SetRealmAuthority(PublicKey realm, PublicKey realmAuthority, PublicKey newRealmAuthority)
        {

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="realmAuthority"></param>
        /// <param name="payer"></param>
        /// <param name="minCommunityTokensToCreateGovernance"></param>
        /// <param name="communityMintMaxVoteWeightSource"></param>
        /// <param name="councilTokenMint"></param>
        /// <param name="communityVoterWeightAddin"></param>
        /// <returns></returns>
        public TransactionInstruction SetRealmConfig(PublicKey realm, PublicKey realmAuthority, PublicKey payer, 
            ulong minCommunityTokensToCreateGovernance, MintMaxVoteWeightSource communityMintMaxVoteWeightSource, 
            PublicKey councilTokenMint = null, PublicKey communityVoterWeightAddin = null)
        {

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="governingTokenOwner"></param>
        /// <param name="governingTokenMint"></param>
        /// <param name="payer"></param>
        /// <returns></returns>
        public TransactionInstruction CreateTokenOwnerRecord(PublicKey realm, PublicKey governingTokenOwner, PublicKey governingTokenMint,
            PublicKey payer)
        {

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey,

            };
        }

        /// <summary>
        /// Adds voter weight accounts to the instruction accounts if <c>voterWeightRecord</c> is not null.
        /// </summary>
        /// <param name="accounts">The instruction accounts.</param>
        /// <param name="realm">The governance realm.</param>
        /// <param name="voterWeightRecord">The voter weight record.</param>
        private void WithVoterWeightAccounts(List<AccountMeta> accounts, PublicKey realm, PublicKey voterWeightRecord)
        {
            if (voterWeightRecord != null)
            {
                PublicKey realmConfigAddress = GetRealmConfigAddress(ProgramIdKey, realm);
                accounts.Add(AccountMeta.ReadOnly(realmConfigAddress, false));
                accounts.Add(AccountMeta.ReadOnly(voterWeightRecord, false));
            }
        }

        /// <summary>
        /// Gets the realm config address.
        /// </summary>
        /// <param name="programId">The program id.</param>
        /// <param name="realm">The governance realm.</param>
        /// <returns>The public key of the realm config.</returns>
        public static PublicKey GetRealmConfigAddress(PublicKey programId, PublicKey realm)
        {
            bool success = PublicKey.TryFindProgramAddress(
                new List<byte[]> { Encoding.UTF8.GetBytes(RealmConfigSeed), realm },
                programId, out PublicKey realmConfigAddressBytes, out _);

            return success ? realmConfigAddressBytes : null;
        }

    }
}
