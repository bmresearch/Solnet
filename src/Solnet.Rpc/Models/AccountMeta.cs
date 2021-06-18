using Solnet.Wallet;
using Solnet.Wallet.Utilities;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Implements the account meta logic, which defines if an account represented by public key is a signer, a writable account or both.
    /// </summary>
    public class AccountMeta
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
        /// 
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="isWritable"></param>
        public AccountMeta(PublicKey publicKey, bool isWritable)
        {
            PublicKey = publicKey.Key;
            PublicKeyBytes = publicKey.KeyBytes;
            Signer = false;
            Writable = isWritable;
        }
        
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
            int cmpSigner = am1.Signer == am2.Signer ? 0 : am1.Signer ? -1 : 1;
            if (cmpSigner != 0)
            {
                return cmpSigner;
            }

            int cmpWritable = am1.Writable == am2.Writable ? 0 : am1.Writable ? -1 : 1;
            return cmpWritable != 0 ? cmpWritable : 0;
        }
    }
}