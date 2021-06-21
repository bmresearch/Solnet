// unset

using Solnet.Programs;
using Solnet.Wallet;

namespace Solnet.Examples
{
    public class BonfidaTwitterVerificationExamples
    {
        
        /// <summary>
        /// The public key of the Twitter Verification Authority.
        /// </summary>
        public static readonly PublicKey TwitterVerificationAuthorityKey = new PublicKey("867BLob5b52i81SNaV9Awm5ejkZV6VGSv9SxLcwukDDJ");
        
        /// <summary>
        /// The public key of the Twitter Root Parent Registry.
        /// </summary>
        public static readonly PublicKey TwitterRootParentRegistryKey = new PublicKey("AFrGkxNmVLBn3mKhvfJJABvm8RJkTtRhHDoaF97pQZaA");

        /// <summary>
        /// Get the derived account address for the reverse lookup.
        /// </summary>
        /// <param name="publicKey">The public key.</param>
        /// <returns>The derived account public key.</returns>
        public static PublicKey GetReverseRegistryKey(string publicKey)
        {
            byte[] hashedName = NameServiceProgram.ComputeHashedName(publicKey);
            PublicKey nameAccountKey = NameServiceProgram.DeriveNameAccountKey(hashedName, TwitterVerificationAuthorityKey, null);
            return nameAccountKey;
        }
        
        /// <summary>
        /// Get the derived account address for the twitter handle registry.
        /// </summary>
        /// <param name="twitterHandle">The twitter handle.</param>
        /// <returns>The derived account public key.</returns>
        public static PublicKey GetTwitterHandleRegistryKey(string twitterHandle)
        {
            byte[] hashedName = NameServiceProgram.ComputeHashedName(twitterHandle);
            PublicKey nameAccountKey = NameServiceProgram.DeriveNameAccountKey(hashedName, null, TwitterRootParentRegistryKey);
            return nameAccountKey;
        }
    }
}