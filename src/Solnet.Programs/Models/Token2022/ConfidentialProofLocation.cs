using Solnet.Wallet;
using System;

namespace Solnet.Programs.Models.Token2022
{
    /// <summary>References a verified proof context account or a proof instruction in the same transaction.</summary>
    public sealed class ConfidentialProofLocation
    {
        /// <summary>Relative instruction offset. Zero denotes a context state account.</summary>
        public sbyte InstructionOffset { get; }

        /// <summary>Proof context state account, or null for a relative instruction reference.</summary>
        public PublicKey ContextStateAccount { get; }

        private ConfidentialProofLocation(sbyte offset, PublicKey account)
        {
            InstructionOffset = offset;
            ContextStateAccount = account;
        }

        /// <summary>References a proof verification instruction by its nonzero offset relative to the token instruction.</summary>
        public static ConfidentialProofLocation FromInstructionOffset(int offset)
        {
            if (offset == 0 || offset < sbyte.MinValue || offset > sbyte.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(offset), "Proof instruction offsets must be nonzero signed bytes.");
            return new ConfidentialProofLocation((sbyte)offset, null);
        }

        /// <summary>References an already verified, correctly typed proof context state account.</summary>
        public static ConfidentialProofLocation FromContextStateAccount(PublicKey account)
        {
            ArgumentNullException.ThrowIfNull(account);
            return new ConfidentialProofLocation(0, account);
        }
    }
}
