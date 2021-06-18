using Solnet.Wallet;
using System;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Implements the account meta logic, which defines if an account represented by public key is a signer, a writable account or both.
    /// </summary>
    public class AccountMeta : IComparable<AccountMeta>
    {
        /// <summary>
        /// The public key as a byte array.
        /// </summary>
        internal byte[] PublicKeyBytes { get; }
        
        /// <summary>
        /// Get the public key encoded as base58.
        /// </summary>
        internal string PublicKey { get; }
        
        /// <summary>
        /// A boolean which defines if the account is a signer account.
        /// </summary>
        internal bool Signer { get; }

        /// <summary>
        /// A boolean which defines if the account is a writable account.
        /// </summary>
        internal bool Writable { get; }

        /// <summary>
        /// The account.
        /// </summary>
        internal Account Account { get; }

        /// <summary>
        /// Initialize the account meta with the passed account, being a signing account for the transaction.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="isWritable">If the account is writable.</param>
        public AccountMeta(Account account, bool isWritable)
        {
            PublicKey = account.PublicKey.Key;
            PublicKeyBytes = account.PublicKey.KeyBytes;
            Account = account;
            Signer = true;
            Writable = isWritable;
        }
        
        /// <summary>
        /// Initialize the account meta with the passed public key, being a non-signing account for the transaction.
        /// </summary>
        /// <param name="publicKey">The public key.</param>
        /// <param name="isWritable">If the account is writable.</param>
        public AccountMeta(PublicKey publicKey, bool isWritable)
        {
            Account = null;
            PublicKey = publicKey.Key;
            PublicKeyBytes = publicKey.KeyBytes;
            Signer = false;
            Writable = isWritable;
        }

        /// <summary>
        /// Compares the account meta instance with another account meta.
        /// </summary>
        /// <param name="other">The object to compare the base to.</param>
        /// <returns>
        /// Returns 0 if the objects are equal in terms of Signing and Writing,
        /// -1 if the base of the comparison is something the other is not, otherwise 1.
        /// </returns>
        /// <exception cref="NotImplementedException">Thrown when the object to compare with is null.</exception>
        public int CompareTo(AccountMeta other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            
            int cmpSigner = Signer == other.Signer ? 0 : Signer ? -1 : 1;
            if (cmpSigner != 0)
            {
                return cmpSigner;
            }

            int cmpWritable = Writable == other.Writable ? 0 : Writable ? -1 : 1;
            return cmpWritable != 0 ? cmpWritable : 0;
        }
    }
}