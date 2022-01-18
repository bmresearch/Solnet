using Solnet.Wallet;
using System.Diagnostics;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Implements the account meta logic, which defines if an account represented by public key is a signer, a writable account or both.
    /// </summary>
    [DebuggerDisplay("PK = {" + nameof(PublicKey) + "}")]
    public class AccountMeta
    {
        /// <summary>
        /// The public key as a byte array.
        /// </summary>
        public byte[] PublicKeyBytes { get; }

        /// <summary>
        /// Get the public key encoded as base58.
        /// </summary>
        public string PublicKey { get; }

        /// <summary>
        /// A boolean which defines if the account is a signer account.
        /// </summary>
        public bool IsSigner { get; }

        /// <summary>
        /// A boolean which defines if the account is a writable account.
        /// </summary>
        public bool IsWritable { get; }

        /// <summary>
        /// Initialize the account meta with the passed public key, being a non-signing account for the transaction.
        /// </summary>
        /// <param name="publicKey">The public key.</param>
        /// <param name="isWritable">Whether the account is writable.</param>
        /// <param name="isSigner">Whether the account is a signer.</param>
        internal AccountMeta(PublicKey publicKey, bool isWritable, bool isSigner)
        {
            PublicKey = publicKey.Key;
            PublicKeyBytes = publicKey.KeyBytes;
            IsSigner = isSigner;
            IsWritable = isWritable;
        }

        /// <summary>
        /// Initializes an <see cref="AccountMeta"/> for a writable account with the given <see cref="PublicKey"/>
        /// and a bool that signals whether the account is a signer or not.
        /// </summary>
        /// <param name="publicKey">The public key.</param>
        /// <param name="isSigner">Whether the account is a signer.</param>
        /// <returns>The <see cref="AccountMeta"/> instance.</returns>
        public static AccountMeta Writable(PublicKey publicKey, bool isSigner) => new(publicKey, true, isSigner);

        /// <summary>
        /// Initializes an <see cref="AccountMeta"/> for a read-only account with the given <see cref="PublicKey"/>
        /// and a bool that signals whether the account is a signer or not.
        /// </summary>
        /// <param name="publicKey">The public key.</param>
        /// <param name="isSigner">Whether the account is a signer.</param>
        /// <returns>The <see cref="AccountMeta"/> instance.</returns>
        public static AccountMeta ReadOnly(PublicKey publicKey, bool isSigner) => new(publicKey, false, isSigner);
    }
}