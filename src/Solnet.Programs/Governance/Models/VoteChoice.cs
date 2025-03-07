namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// Voter choice for a proposal option
    /// In the current version only 1) Single choice and 2) Multiple choices proposals are supported
    /// In the future versions we can add support for 1) Quadratic voting, 2) Ranked choice voting and 3) Weighted voting
    /// </summary>
    public class VoteChoice
    {
        /// <summary>
        /// The rank given to the choice by voter
        /// Note: The filed is not used in the current version
        /// </summary>
        public byte Rank;

        /// <summary>
        /// The voter's weight percentage given by the voter to the choice
        /// </summary>
        public byte WeightPercentage;
    }
}
