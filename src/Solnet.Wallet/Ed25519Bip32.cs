using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Solnet.Util;

namespace Solnet.Wallet
{ 
    /// <summary>
    /// An implementation of Ed25519 based BIP32 key generation.
    /// </summary>
    public class Ed25519Bip32
    {
        private readonly string curve = "ed25519 seed";
        private readonly uint hardenedOffset = 0x80000000;

        /// <summary>
        /// The seed used for key generation.
        /// </summary>
        private byte[] _seed;
        
        /// <summary>
        /// The computed master key.
        /// </summary>
        private byte[] _masterKey;
        
        /// <summary>
        /// The computed chain code.
        /// </summary>
        private byte[] _chainCode;

        /// <summary>
        /// Initialize the ed25519 based bip32 key generator with the passed seed.
        /// </summary>
        /// <param name="seed">The seed.</param>
        public Ed25519Bip32(byte[] seed)
        {
            _seed = seed;
            (_masterKey, _chainCode) = GetMasterKeyFromSeed(_seed);
        }

        /// <summary>
        /// Gets the master key used for key generation from the passed seed.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public (byte[] Key, byte[] ChainCode) GetMasterKeyFromSeed(byte[] seed)
        {
            using (HMACSHA512 hmacSha512 = new HMACSHA512(Encoding.UTF8.GetBytes(curve)))
            {
                var i = hmacSha512.ComputeHash(seed);

                var il = i.Slice(0, 32);
                var ir = i.Slice(32);

                return (Key: il, ChainCode: ir);
            }
        }       

        /// <summary>
        /// Computes the child key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="chainCode"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private (byte[] Key, byte[] ChainCode) GetChildKeyDerivation(byte[] key, byte[] chainCode, uint index)
        {
            BigEndianBuffer buffer = new BigEndianBuffer();

            buffer.Write(new byte[] { 0 });
            buffer.Write(key);
            buffer.WriteUInt(index);

            using (HMACSHA512 hmacSha512 = new HMACSHA512(chainCode))
            {
                var i = hmacSha512.ComputeHash(buffer.ToArray());

                var il = i.Slice(0, 32);
                var ir = i.Slice(32);

                return (Key: il, ChainCode: ir);
            }
        }

        /// <summary>
        /// Checks if the derivation path is valid.
        /// <remarks>Returns true if the path is valid, otherwise false.</remarks>
        /// </summary>
        /// <param name="path">The derivation path.</param>
        /// <returns>A boolean.</returns>
        private bool IsValidPath(string path)
        {
            var regex = new Regex("^m(\\/[0-9]+')+$");

            if (!regex.IsMatch(path))
                return false;

            var valid = !(path.Split('/')
                .Slice(1)
                .Select(a => a.Replace("'", ""))
                .Any(a => !Int32.TryParse(a, out _)));

            return valid;
        }

        /// <summary>
        /// Derives a child key from the passed derivation path.
        /// </summary>
        /// <param name="path">The derivation path.</param>
        /// <returns>The key and chaincode.</returns>
        /// <exception cref="FormatException">Thrown when the passed derivation path is invalid.</exception>
        public (byte[] Key, byte[] ChainCode) DerivePath(string path)
        {
            if (!IsValidPath(path))
                throw new FormatException("Invalid derivation path");

            var segments = path
                .Split('/')
                .Slice(1)
                .Select(a => a.Replace("'", ""))
                .Select(a => Convert.ToUInt32(a, 10));

            var results = segments
                .Aggregate(
                    (_masterKey, _chainCode), 
                    (masterKeyFromSeed, next) => 
                        GetChildKeyDerivation(masterKeyFromSeed._masterKey, masterKeyFromSeed._chainCode, next + hardenedOffset));

            return results;
        }
    }
}