using System.Collections.Generic;

namespace Solnet.Programs.Governance
{

    /// <summary>
    /// Represents the instruction types for the <see cref="GovernanceProgram"/> along with a friendly name so as not to use reflection.
    /// <remarks>
    /// For more information see:
    /// https://github.com/solana-labs/solana-program-library/blob/master/governance/
    /// </remarks>
    /// </summary>
    internal static class GovernanceProgramInstructions
    {
        /// <summary>
        /// Represents the user-friendly names for the instruction types for the <see cref="GovernanceProgram"/>.
        /// </summary>
        internal static readonly Dictionary<Values, string> Names = new()
        {
            { Values.CreateRealm, "Create Realm" },
            { Values.DepositGoverningTokens, "Deposit Governing Tokens" },
            { Values.WithdrawGoverningTokens, "Withdraw Governing Tokens" },
            { Values.SetGovernanceDelegate, "Set Governance Delegate" },
            { Values.CreateAccountGovernance, "Create Account Governance" },
            { Values.CreateProgramGovernance, "Create Program Governance" },
            { Values.CreateProposal, "Create Proposal" },
            { Values.AddSignatory, "Add Signatory" },
            { Values.RemoveSignatory, "Remove Signatory" },
            { Values.InsertInstruction, "Insert Instruction" },
            { Values.RemoveInstruction, "Remove Instruction" },
            { Values.CancelProposal, "Cancel Proposal" },
            { Values.SignOffProposal, "Sign Off Proposal" },
            { Values.CastVote, "Cast Vote" },
            { Values.FinalizeVote, "Finalize Vote" },
            { Values.RelinquishVote, "Relinquish Vote" },
            { Values.ExecuteInstruction, "Execute Instruction" },
            { Values.CreateMintGovernance, "Create Mint Governance" },
            { Values.CreateTokenGovernance, "Create Token Governance" },
            { Values.SetGovernanceConfig, "Set Governance Config" },
            { Values.FlagInstructionError, "Flag Instruction Error" },
            { Values.SetRealmAuthority, "Set Realm Authority" },
            { Values.SetRealmConfig, "Set Realm Config" },
            { Values.CreateTokenOwnerRecord, "Create Token Owner Record" },
        };

        /// <summary>
        /// Represents the instruction types for the <see cref="GovernanceProgram"/>.
        /// </summary>
        internal enum Values : byte
        {
            /// <summary>
            /// Creates Governance Realm account which aggregates governances for given Community Mint and optional Council Mint
            /// </summary>
            CreateRealm = 0,

            /// <summary>
            /// Deposits governing tokens (Community or Council) to Governance Realm and establishes your voter weight to be used for voting within the Realm
            /// Note: If subsequent (top up) deposit is made and there are active votes for the Voter then the vote weights won't be updated automatically
            /// It can be done by relinquishing votes on active Proposals and voting again with the new weight
            /// </summary>
            DepositGoverningTokens = 1,

            /// <summary>
            /// Withdraws governing tokens (Community or Council) from Governance Realm and downgrades your voter weight within the Realm
            /// Note: It's only possible to withdraw tokens if the Voter doesn't have any outstanding active votes
            /// If there are any outstanding votes then they must be relinquished before tokens could be withdrawn
            /// </summary>
            WithdrawGoverningTokens = 2,

            /// <summary>
            /// Sets Governance Delegate for the given Realm and Governing Token Mint (Community or Council)
            /// The Delegate would have voting rights and could vote on behalf of the Governing Token Owner
            /// The Delegate would also be able to create Proposals on behalf of the Governing Token Owner
            /// Note: This doesn't take voting rights from the Token Owner who still can vote and change governance_delegate
            /// </summary>
            SetGovernanceDelegate = 3,

            /// <summary>
            /// Creates Account Governance account which can be used to govern an arbitrary account
            /// </summary>
            CreateAccountGovernance = 4,

            /// <summary>
            /// Creates Program Governance account which governs an upgradable program
            /// </summary>
            CreateProgramGovernance = 5,

            /// <summary>
            /// Creates Proposal account for Instructions that will be executed at some point in the future
            /// </summary>
            CreateProposal = 6,

            /// <summary>
            /// Adds a signatory to the Proposal which means this Proposal can't leave Draft state until yet another Signatory signs
            /// </summary>
            AddSignatory = 7,

            /// <summary>
            /// Removes a Signatory from the Proposal
            /// </summary>
            RemoveSignatory = 8,

            /// <summary>
            /// Inserts an instruction for the Proposal at the given index position
            /// New Instructions must be inserted at the end of the range indicated by Proposal instructions_next_index
            /// If an Instruction replaces an existing Instruction at a given index then the old one must be removed using RemoveInstruction first
            /// </summary>
            InsertInstruction = 9,

            /// <summary>
            /// Removes instruction from the Proposal
            /// </summary>
            RemoveInstruction = 10,

            /// <summary>
            /// Cancels Proposal by changing its state to Canceled
            /// </summary>
            CancelProposal = 11,

            /// <summary>
            /// Signs off Proposal indicating the Signatory approves the Proposal
            /// When the last Signatory signs the Proposal state moves to Voting state
            /// </summary>
            SignOffProposal = 12,

            /// <summary>
            ///  Uses your voter weight (deposited Community or Council tokens) to cast a vote on a Proposal
            ///  By doing so you indicate you approve or disapprove of running the Proposal set of instructions
            ///  If you tip the consensus then the instructions can begin to be run after their hold up time
            /// </summary>
            CastVote = 13,

            /// <summary>
            /// Finalizes vote in case the Vote was not automatically tipped within max_voting_time period
            /// </summary>
            FinalizeVote = 14,

            /// <summary>
            ///  Relinquish Vote removes voter weight from a Proposal and removes it from voter's active votes
            ///  If the Proposal is still being voted on then the voter's weight won't count towards the vote outcome
            ///  If the Proposal is already in decided state then the instruction has no impact on the Proposal
            ///  and only allows voters to prune their outstanding votes in case they wanted to withdraw Governing tokens from the Realm
            /// </summary>
            RelinquishVote = 15,

            /// <summary>
            /// Executes an instruction in the Proposal
            /// Anybody can execute transaction once Proposal has been voted Yes and transaction_hold_up time has passed
            /// The actual instruction being executed will be signed by Governance PDA the Proposal belongs to
            /// For example to execute Program upgrade the ProgramGovernance PDA would be used as the singer
            /// </summary>
            ExecuteInstruction = 16,

            /// <summary>
            /// Creates Mint Governance account which governs a mint
            /// </summary>
            CreateMintGovernance = 17,

            /// <summary>
            /// Creates Token Governance account which governs a token account
            /// </summary>
            CreateTokenGovernance = 18,

            /// <summary>
            /// Sets GovernanceConfig for a Governance
            /// </summary>
            SetGovernanceConfig = 19,

            /// <summary>
            /// Flags an instruction and its parent Proposal with error status
            /// It can be used by Proposal owner in case the instruction is permanently broken and can't be executed
            /// Note: This instruction is a workaround because currently it's not possible to catch errors from CPI calls
            ///       and the Governance program has no way to know when instruction failed and flag it automatically
            /// </summary>
            FlagInstructionError = 20,

            /// <summary>
            /// Sets new Realm authority
            /// </summary>
            SetRealmAuthority = 21,

            /// <summary>
            /// Sets realm config
            /// </summary>
            SetRealmConfig = 22,

            /// <summary>
            /// Creates TokenOwnerRecord with 0 deposit amount
            /// It's used to register TokenOwner when voter weight addin is used and the Governance program doesn't take deposits
            /// </summary>
            CreateTokenOwnerRecord = 23
        }
    }
}
