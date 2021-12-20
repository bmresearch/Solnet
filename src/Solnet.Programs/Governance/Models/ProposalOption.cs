using Solnet.Programs.Governance.Enums;
using Solnet.Programs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// Proposal Option
    /// </summary>
    public class ProposalOption
    {
        /// <summary>
        /// Option label
        /// </summary>
        public string Label;

        /// <summary>
        /// Vote weight for the option
        /// </summary>
        public ulong VoteWeight;

        /// <summary>
        /// Vote result for the option
        /// </summary>
        public OptionVoteResult VoteResult;

        /// <summary>
        /// The number of the instructions already executed
        /// </summary>
        public ushort InstructionsExecutedCount;

        /// <summary>
        /// The number of instructions included in the option
        /// </summary>
        public ushort InstructionsCount;

        /// <summary>
        /// The index of the the next instruction to be added
        /// </summary>
        public ushort InstructionsNextIndex;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ProposalOption Deserialize(ReadOnlySpan<byte> data)
        {
            int labelLength = data.GetString(0, out string label);
            ulong voteWeight = data.GetU64(0 + labelLength);
            OptionVoteResult voteResult = (OptionVoteResult)Enum.Parse(typeof(OptionVoteResult), data.GetU8(sizeof(ulong) + labelLength).ToString());

            return new ProposalOption
            {
                Label = label,
                VoteWeight = voteWeight,
                VoteResult = voteResult,

            };
        }
    }
}
