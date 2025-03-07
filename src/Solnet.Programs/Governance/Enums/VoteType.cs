namespace Solnet.Programs.Governance.Enums
{
    /// <summary>
    /// Proposal vote type
    /// </summary>
    public enum VoteType : byte
    {
        /// <summary>
        /// Single choice vote with mutually exclusive choices
        /// In the SingeChoice mode there can ever be a single winner
        /// If multiple options score the same highest vote then the Proposal is not resolved and considered as Failed
        /// Note: Yes/No vote is a single choice (Yes) vote with the deny option (No)
        /// </summary>
        SingleChoice = 0,

        /// <summary>
        /// Multiple options can be selected with up to N choices per voter
        /// By default N equals to the number of available options
        /// Note: In the current version the N limit is not supported and not enforced yet
        /// </summary>
        MultiChoice = 1,
    }
}
