using System;

namespace Solnet.Programs.StakePool.Models
{
    /// <summary>
    /// Wrapper struct that can be used as a Pod, containing a byte that should be a valid StakeStatus underneath.
    /// </summary>
    public struct PodStakeStatus : IEquatable<PodStakeStatus>
    {
        public byte Value;

        /// <summary>
        /// Represents the status of a pod's stake as a byte value.
        /// </summary>
        /// <param name="value">The byte value representing the pod's stake status.</param>
        public PodStakeStatus(byte value)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a new instance of <see cref="PodStakeStatus"/> from a <see cref="StakeStatus"/> value.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static PodStakeStatus FromStakeStatus(StakeStatus status) => new PodStakeStatus((byte)status);

        /// <summary>
        /// Converts the current value to a <see cref="StakeStatus"/> enumeration.
        /// </summary>
        /// <returns>A <see cref="StakeStatus"/> value that corresponds to the current value.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the current value does not correspond to a valid <see cref="StakeStatus"/> enumeration value.</exception>
        public StakeStatus ToStakeStatus()
        {
            if (Enum.IsDefined(typeof(StakeStatus), Value))
                return (StakeStatus)Value;
            throw new InvalidOperationException("Invalid StakeStatus value.");
        }

        /// <summary>
        /// Downgrade the status towards ready for removal by removing the validator stake.
        /// </summary>
        public void RemoveValidatorStake()
        {
            var status = ToStakeStatus();
            StakeStatus newStatus = status switch
            {
                StakeStatus.Active or StakeStatus.DeactivatingTransient or StakeStatus.ReadyForRemoval => status,
                StakeStatus.DeactivatingAll => StakeStatus.DeactivatingTransient,
                StakeStatus.DeactivatingValidator => StakeStatus.ReadyForRemoval,
                _ => throw new InvalidOperationException("Invalid StakeStatus value.")
            };
            Value = (byte)newStatus;
        }

        /// <summary>
        /// Downgrade the status towards ready for removal by removing the transient stake.
        /// </summary>
        public void RemoveTransientStake()
        {
            var status = ToStakeStatus();
            StakeStatus newStatus = status switch
            {
                StakeStatus.Active or StakeStatus.DeactivatingValidator or StakeStatus.ReadyForRemoval => status,
                StakeStatus.DeactivatingAll => StakeStatus.DeactivatingValidator,
                StakeStatus.DeactivatingTransient => StakeStatus.ReadyForRemoval,
                _ => throw new InvalidOperationException("Invalid StakeStatus value.")
            };
            Value = (byte)newStatus;
        }

        /// <summary>
        /// Converts a <see cref="StakeStatus"/> instance to a <see cref="PodStakeStatus"/> instance.
        /// </summary>
        /// <param name="status">The <see cref="StakeStatus"/> instance to convert.</param>
        public static explicit operator PodStakeStatus(StakeStatus status) => FromStakeStatus(status);

        /// <summary>
        /// Converts a <see cref="PodStakeStatus"/> instance to a <see cref="StakeStatus"/> instance.
        /// </summary>
        /// <param name="pod">The <see cref="PodStakeStatus"/> instance to convert.</param>
        public static explicit operator StakeStatus(PodStakeStatus pod)
        {
            return pod.ToStakeStatus();
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><see langword="true"/> if the specified object is of type <c>PodStakeStatus</c> and is equal to the current
        /// instance; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj) => obj is PodStakeStatus other && Equals(other);

        /// <summary>
        /// Determines whether the current instance is equal to another <see cref="PodStakeStatus"/> instance.
        /// </summary>
        /// <param name="other">The <see cref="PodStakeStatus"/> instance to compare with the current instance.</param>
        /// <returns><see langword="true"/> if the <see cref="Value"/> of the current instance is equal to the <see
        /// cref="Value"/> of the specified instance; otherwise, <see langword="false"/>.</returns>
        public bool Equals(PodStakeStatus other) => Value == other.Value;

        /// <summary>
        /// Returns a hash code for the current object.
        /// </summary>
        /// <remarks>The hash code is derived from the <see cref="Value"/> property.  It is suitable for
        /// use in hashing algorithms and data structures such as hash tables.</remarks>
        /// <returns>An integer that represents the hash code for the current object.</returns>
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(PodStakeStatus left, PodStakeStatus right) => left.Equals(right);

        public static bool operator !=(PodStakeStatus left, PodStakeStatus right) => !(left == right);
    }
}
