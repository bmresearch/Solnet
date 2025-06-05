using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.StakePool.Models
{
    /// <summary>
    /// Storage list for all validator stake accounts in the pool.
    /// </summary>
    public class ValidatorList
    {
        /// <summary>
        /// Data outside of the validator list, separated out for cheaper deserializations.
        /// </summary>
        public ValidatorListHeader Header { get; set; }

        /// <summary>
        /// List of stake info for each validator in the pool.
        /// </summary>
        public List<ValidatorStakeInfo> Validators { get; set; }

        /// <summary>
        /// Creates an empty instance containing space for <paramref name="maxValidators"/> validators.
        /// </summary>
        /// <param name="maxValidators">Maximum number of validators.</param>
        /// <returns>A new instance of <see cref="ValidatorList"/>.</returns>
        public static ValidatorList New(uint maxValidators)
        {
            return new ValidatorList
            {
                Header = new ValidatorListHeader
                {
                    AccountType = AccountType.ValidatorList,
                    MaxValidators = maxValidators
                },
                Validators = Enumerable.Repeat(new ValidatorStakeInfo(), (int)maxValidators).ToList()
            };
        }

        /// <summary>
        /// Calculate the number of validator entries that fit in the provided buffer length.
        /// Assumes that <see cref="ValidatorListHeader"/> defines a constant <c>LEN</c> for header length.
        /// </summary>
        /// <param name="bufferLength">The total buffer length.</param>
        /// <returns>The maximum number of validator entries.</returns>
        public static int CalculateMaxValidators(int bufferLength)
        {
            // Add 4 additional bytes to the serialized header length (as in the original Rust code).
            int headerSize = (new ValidatorListHeader()).GetSerializedLength() + 4;
            return (bufferLength - headerSize) / ValidatorStakeInfo.Length;
        }

        /// <summary>
        /// Checks if the list contains a validator with the given vote account address.
        /// </summary>
        /// <param name="voteAccountAddress">The vote account public key.</param>
        /// <returns><c>true</c> if found; otherwise, <c>false</c>.</returns>
        public bool Contains(PublicKey voteAccountAddress)
        {
            return Validators.Any(x => x.VoteAccountAddress.Equals(voteAccountAddress));
        }

        /// <summary>
        /// Finds a mutable reference to the <see cref="ValidatorStakeInfo"/> with the given vote account address.
        /// </summary>
        /// <param name="voteAccountAddress">The vote account public key.</param>
        /// <returns>
        /// A reference to the matching <see cref="ValidatorStakeInfo"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public ValidatorStakeInfo FindMut(PublicKey voteAccountAddress)
        {
            return Validators.FirstOrDefault(x => x.VoteAccountAddress.Equals(voteAccountAddress));
        }

        /// <summary>
        /// Finds an immutable reference to the <see cref="ValidatorStakeInfo"/> with the given vote account address.
        /// </summary>
        /// <param name="voteAccountAddress">The vote account public key.</param>
        /// <returns>
        /// A reference to the matching <see cref="ValidatorStakeInfo"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public ValidatorStakeInfo Find(PublicKey voteAccountAddress)
        {
            return Validators.FirstOrDefault(x => x.VoteAccountAddress.Equals(voteAccountAddress));
        }

        /// <summary>
        /// Checks if the list contains any validator with active stake.
        /// </summary>
        /// <returns><c>true</c> if any validator's active stake lamports are greater than zero; otherwise, <c>false</c>.</returns>
        public bool HasActiveStake()
        {
            return Validators.Any(x => x.ActiveStakeLamports > 0);
        }
    }
}
