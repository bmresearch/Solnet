using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Solnet.Wallet.Utilities;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Solnet.Wallet
{
    /// <summary>
    /// An implementation of Ed25519 based BIP32 key generation.
    /// </summary>
    public class Ed25519Bip32
    {
        /// <summary>
        /// The seed for the Ed25519 BIP32 HMAC-SHA512 master key calculation.
        /// </summary>
        private const string Curve = "ed25519 seed";

        /// <summary>
        /// 
        /// </summary>
        private const uint HardenedOffset = 0x80000000;

        /// <summary>
        /// The computed master key.
        /// </summary>
        private readonly byte[] _masterKey;

        /// <summary>
        /// The computed chain code.
        /// </summary>
        private readonly byte[] _chainCode;

        /// <summary>
        /// Initialize the ed25519 based bip32 key generator with the passed seed.
        /// </summary>
        /// <param name="seed">The seed.</param>
        public Ed25519Bip32(byte[] seed)
        {
            (_masterKey, _chainCode) = GetMasterKeyFromSeed(seed);
        }

        /// <summary>
        /// Gets the master key used for key generation from the passed seed.
        /// </summary>
        /// <param name="seed">The seed used to calculate the master key.</param>
        /// <returns>A tuple consisting of the key and corresponding chain code.</returns>
        private static (byte[] Key, byte[] ChainCode) GetMasterKeyFromSeed(byte[] seed)
            => HmacSha512(Encoding.UTF8.GetBytes(Curve), seed);

        /// <summary>
        /// Computes the child key.
        /// </summary>
        /// <param name="key">The key used to derive from.</param>
        /// <param name="chainCode">The chain code for derivation.</param>
        /// <param name="index">The index of the key to the derive.</param>
        /// <returns>A tuple consisting of the key and corresponding chain code.</returns>
        private static (byte[] Key, byte[] ChainCode) GetChildKeyDerivation(byte[] key, byte[] chainCode, uint index)
        {
            MemoryStream buffer = new();

            buffer.Write(new byte[] { 0 });
            buffer.Write(key);
            byte[] indexBytes = new byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(indexBytes, index);
            buffer.Write(indexBytes);

            return HmacSha512(chainCode, buffer.ToArray());
        }

        /// <summary>
        /// Computes the HMAC SHA 512 of the byte array passed into <c>data</c>.
        /// </summary>
        /// <param name="keyBuffer">The byte array to be used as the HMAC SHA512 key.</param>
        /// <param name="data">The data to calculate the HMAC SHA512 on.</param>
        /// <returns>A tuple consisting of the key and corresponding chain code.</returns>
        private static (byte[] Key, byte[] ChainCode) HmacSha512(byte[] keyBuffer, byte[] data)
        {
            byte[] i = new byte[64];
            Sha512Digest digest = new();
            HMac hmac = new(digest);

            hmac.Init(new KeyParameter(keyBuffer));
            hmac.BlockUpdate(data, 0, data.Length);
            hmac.DoFinal(i, 0);

            byte[] il = i.Slice(0, 32);
            byte[] ir = i.Slice(32);

            return (Key: il, ChainCode: ir);
        }

        /// <summary>
        /// Checks if the derivation path is valid.
        /// <remarks>Returns true if the path is valid, otherwise false.</remarks>
        /// </summary>
        /// <param name="path">The derivation path.</param>
        /// <returns>A boolean.</returns>
        private static bool IsValidPath(string path)
        {
            Regex regex = new("^m(\\/[0-9]+')+$");

            if (!regex.IsMatch(path))
                return false;

            bool valid = !(path.Split('/')
                .Slice(1)
                .Select(a => a.Replace("'", ""))
                .Any(a => !int.TryParse(a, out _)));

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

            IEnumerable<uint> segments = path
                .Split('/')
                .Slice(1)
                .Select(a => a.Replace("'", ""))
                .Select(a => Convert.ToUInt32(a, 10));

            (byte[] _masterKey, byte[] _chainCode) results = segments
                .Aggregate(
                    (_masterKey, _chainCode),
                    (masterKeyFromSeed, next) =>
                        GetChildKeyDerivation(masterKeyFromSeed._masterKey, masterKeyFromSeed._chainCode, next + HardenedOffset));

            return results;
        }
    }
}