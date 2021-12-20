using Solnet.Programs.Governance.Enums;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// Proposal VoteRecord
    /// </summary>
    public class VoteRecord : GovernanceProgramAccount
    {
        /// <summary>
        /// 
        /// </summary>
        public static class ExtraLayout
        {
            /// <summary>
            /// 
            /// </summary>
            public const int ProposalOffset = 1;

            /// <summary>
            /// 
            /// </summary>
            public const int GoverningTokenOwnerOffset = 33;

            /// <summary>
            /// 
            /// </summary>
            public const int IsRelinquishedOffset = 65;

            /// <summary>
            /// 
            /// </summary>
            public const int VoterWeightOffset = 66;

            /// <summary>
            /// 
            /// </summary>
            public const int VoteOffset = 74;
        }
        
        /// <summary>
        /// Proposal the signatory is assigned for.
        /// </summary>
        public PublicKey Proposal;

        /// <summary>
        /// The user who casted this vote.
        /// This is the Governing Token Owner who deposited governing tokens into the Realm.
        /// </summary>
        public PublicKey GoverningTokenOwner;

        /// <summary>
        /// Indicates whether the vote was relinquished by voter.
        /// </summary>
        public bool IsRelinquished;

        /// <summary>
        /// The weight of the user casting the vote.
        /// </summary>
        public ulong VoterWeight;

        /// <summary>
        /// Voter's vote.
        /// </summary>
        public Vote Vote;

        /// <summary>
        /// 
        /// </summary>
        public List<VoteChoice> Choices;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static VoteRecord Deserialize(byte[] data)
        {
            ReadOnlySpan<byte> span = data.AsSpan();

            List<VoteChoice> choices = new();
            Vote vote = (Vote)Enum.Parse(typeof(Vote), span.GetU8(ExtraLayout.VoteOffset).ToString());

            if(vote == Vote.Approve)
            {
                int numChoices = (int)span.GetU32(ExtraLayout.VoteOffset + 1);
                for(int i = 0; i < numChoices; i++)
                {
                    var choiceBytes = span.GetSpan(ExtraLayout.VoteOffset + 5 + (i * 2), 2);
                    choices.Add(new VoteChoice
                    {
                        Rank = choiceBytes.GetU8(0),
                        WeightPercentage = choiceBytes.GetU8(1),
                    });
                }
            }

            return new VoteRecord
            {
                AccountType = (GovernanceAccountType)Enum.Parse(typeof(GovernanceAccountType), span.GetU8(Layout.AccountTypeOffset).ToString()),
                Proposal = span.GetPubKey(ExtraLayout.ProposalOffset),
                GoverningTokenOwner = span.GetPubKey(ExtraLayout.GoverningTokenOwnerOffset),
                IsRelinquished = span.GetBool(ExtraLayout.IsRelinquishedOffset),
                VoterWeight = span.GetU64(ExtraLayout.VoterWeightOffset),
                Vote = vote,
                Choices = choices,
            };
        }
    }
}
