namespace Solnet.Programs.Governance.Enums
{
    /// <summary>
    /// Defines all Governance accounts types
    /// </summary>
    public enum GovernanceAccountType : byte
    {
        /// <summary>
        /// Default uninitialized account state
        /// </summary>
        Uninitialized = 0,

        /// <summary>
        /// Top level aggregation for governances with Community Token (and optional Council Token)
        /// </summary>
        Realm = 1,

        /// <summary>
        /// Token Owner Record for given governing token owner within a Realm
        /// </summary>
        TokenOwnerRecord = 2,

        /// <summary>
        /// Generic Account Governance account
        /// </summary>
        AccountGovernance = 3,

        /// <summary>
        /// Program Governance account
        /// </summary>
        ProgramGovernance = 4,

        /// <summary>
        /// Proposal account for Governance account. A single Governance account can have multiple Proposal accounts
        /// </summary>
        ProposalV1 = 5,

        /// <summary>
        /// Proposal Signatory account
        /// </summary>
        SignatoryRecord = 6,

        /// <summary>
        /// Vote record account for a given Proposal.  Proposal can have 0..n voting records
        /// </summary>
        VoteRecordV1 = 7,

        /// <summary>
        /// ProposalInstruction account which holds an instruction to execute for Proposal
        /// </summary>
        ProposalInstructionV1 = 8,

        /// <summary>
        /// Mint Governance account
        /// </summary>
        MintGovernance = 9,

        /// <summary>
        /// Token Governance account
        /// </summary>
        TokenGovernance = 10,

        /// <summary>
        /// Realm config account
        /// </summary>
        RealmConfig = 11,

        /// <summary>
        /// Vote record account for a given Proposal.  Proposal can have 0..n voting records
        /// V2 adds support for multi option votes
        /// </summary>
        VoteRecordV2 = 12,

        /// <summary>
        /// ProposalInstruction account which holds an instruction to execute for Proposal
        /// V2 adds index for proposal option
        /// </summary>
        ProposalInstructionV2 = 13,

        /// <summary>
        /// Proposal account for Governance account. A single Governance account can have multiple Proposal accounts
        /// V2 adds support for multiple vote options
        /// </summary>
        ProposalV2 = 14,
    }
}
