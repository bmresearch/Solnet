using Solnet.Programs.Governance.Enums;
using Solnet.Programs.Utilities;
using System;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// Proposal Option
    /// </summary>
    public class ProposalOption
    {
        /// <summary>
        /// The layout of the structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the structure without taking into account the label string.
            /// </summary>
            public const int LengthWithoutLabel = 23;

            /// <summary>
            /// The offset at which the label string begins.
            /// </summary>
            public const int LabelOffset = 0;
        }

        /// <summary>
        /// Option label
        /// </summary>
        public string Label;

        /// <summary>
        /// The length of the label. This value is used for deserialization purposes.
        /// </summary>
        internal int LabelLength;

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
        /// Deserialize the data into the <see cref="ProposalOption"/> structure.
        /// </summary>
        /// <param name="data">The data to deserialize.</param>
        /// <returns>The <see cref="ProposalOption"/> structure.</returns>
        public static ProposalOption Deserialize(ReadOnlySpan<byte> data)
        {
            int labelLength = data.GetBorshString(Layout.LabelOffset, out string label);

            return new ProposalOption
            {
                Label = label,
                LabelLength = labelLength,
                VoteWeight = data.GetU64(labelLength),
                VoteResult = (OptionVoteResult)Enum.Parse(typeof(OptionVoteResult), data.GetU8(sizeof(ulong) + labelLength).ToString()),
                InstructionsExecutedCount = data.GetU16(sizeof(byte) + sizeof(ulong) + labelLength),
                InstructionsCount = data.GetU16(sizeof(byte) + sizeof(ulong) + sizeof(ushort) + labelLength),
                InstructionsNextIndex = data.GetU16(sizeof(byte) + sizeof(ulong) + (sizeof(ushort) * 2) + labelLength),
            };
        }
    }
}
