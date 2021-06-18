using Solnet.Wallet.Utilities;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Implements the account meta logic, which defines if an account represented by public key is a signer, a writable account or both.
    /// </summary>
    public class AccountMeta
    {
        /// <summary>
        /// The base58 encoder instance.
        /// </summary>
        private static readonly Base58Encoder Encoder = new();

        /// <summary>
        /// The public key.
        /// </summary>
        internal byte[] PublicKey;

        /// <summary>
        /// A boolean which defines if the account is a signer account.
        /// </summary>
        internal bool Signer { get; }

        /// <summary>
        /// A boolean which defines if the account is a writable account.
        /// </summary>
        internal bool Writable { get; }

        /// <summary>
        /// Initialize the account meta with the passed public key, and booleans defining if it is a signer and/or writable account.
        /// </summary>
        /// <param name="publicKey">The account's public key.</param>
        /// <param name="isSigner">If the account is a signer.</param>
        /// <param name="isWritable">If the account is writable.</param>
        public AccountMeta(byte[] publicKey, bool isSigner, bool isWritable)
        {
            PublicKey = publicKey;
            Signer = isSigner;
            Writable = isWritable;
        }


        /// <summary>
        /// Get the public key encoded as base58.
        /// </summary>
        public string GetPublicKey => Encoder.EncodeData(PublicKey);
    }

    /// <summary>
    /// Implements extensions to <see cref="AccountMeta"/>.
    /// </summary>
    internal static class AccountMetaExtensions
    {
        /// <summary>
        /// Compares two <see cref="AccountMeta"/> objects.
        /// </summary>
        /// <param name="am1">The base of the comparison.</param>
        /// <param name="am2">The object to compare the base to.</param>
        /// <returns>
        /// Returns 0 if the objects are equal in terms of Signing and Writing,
        /// -1 if the base of the comparison is something the other is not, otherwise 1.
        /// </returns>
        internal static int Compare(AccountMeta am1, AccountMeta am2)
        {
            var cmpSigner = am1.Signer == am2.Signer ? 0 : am1.Signer ? -1 : 1;
            if (cmpSigner != 0)
            {
                return cmpSigner;
            }

            var cmpWritable = am1.Writable == am2.Writable ? 0 : am1.Writable ? -1 : 1;
            return cmpWritable != 0 ? cmpWritable : 0;
        }
    }
}