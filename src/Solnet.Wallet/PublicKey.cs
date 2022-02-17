using Chaos.NaCl;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Solnet.Wallet
{
    /// <summary>
    /// Implements the public key functionality.
    /// </summary>
    [DebuggerDisplay("Key = {ToString()}")]
    public class PublicKey
    {
        /// <summary>
        /// Public key length.
        /// </summary>
        public const int PublicKeyLength = 32;

        private string _key;

        /// <summary>
        /// The key as base-58 encoded string.
        /// </summary>
        public string Key
        {
            get
            {
                if (_key == null)
                {
                    Key = Encoders.Base58.EncodeData(KeyBytes);
                }
                return _key;
            }
            set => _key = value;
        }


        private byte[] _keyBytes;

        /// <summary>
        /// The bytes of the key.
        /// </summary>
        public byte[] KeyBytes
        {
            get
            {
                if (_keyBytes == null)
                {
                    KeyBytes = Encoders.Base58.DecodeData(Key);
                }
                return _keyBytes;
            }
            set => _keyBytes = value;
        }


        /// <summary>
        /// Initialize the public key from the given byte array.
        /// </summary>
        /// <param name="key">The public key as byte array.</param>
        public PublicKey(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (key.Length != PublicKeyLength)
                throw new ArgumentException("invalid key length", nameof(key));
            KeyBytes = new byte[PublicKeyLength];
            Array.Copy(key, KeyBytes, PublicKeyLength);
        }

        /// <summary>
        /// Initialize the public key from the given string.
        /// </summary>
        /// <param name="key">The public key as base58 encoded string.</param>
        public PublicKey(string key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        /// <summary>
        /// Initialize the public key from the given string.
        /// </summary>
        /// <param name="key">The public key as base58 encoded string.</param>
        public PublicKey(ReadOnlySpan<byte> key)
        {
            if (key.Length != PublicKeyLength)
                throw new ArgumentException("invalid key length", nameof(key));
            KeyBytes = new byte[PublicKeyLength];
            key.CopyTo(KeyBytes.AsSpan());
        }

        /// <summary>
        /// Verify the signed message.
        /// </summary>
        /// <param name="message">The signed message.</param>
        /// <param name="signature">The signature of the message.</param>
        /// <returns></returns>
        public bool Verify(byte[] message, byte[] signature)
        {
            return Ed25519.Verify(signature, message, KeyBytes);
        }

        /// <inheritdoc cref="Equals(object)"/>
        public override bool Equals(object obj)
        {
            if (obj is PublicKey pk) return pk.Key == this.Key;

            return false;
        }

        /// <inheritdoc />
        public static bool operator ==(PublicKey lhs, PublicKey rhs)
        {

            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        /// <inheritdoc />
        public static bool operator !=(PublicKey lhs, PublicKey rhs) => !(lhs == rhs);

        /// <summary>
        /// Conversion between a <see cref="PublicKey"/> object and the corresponding base-58 encoded public key.
        /// </summary>
        /// <param name="publicKey">The PublicKey object.</param>
        /// <returns>The base-58 encoded public key.</returns>
        public static implicit operator string(PublicKey publicKey) => publicKey.Key;

        /// <summary>
        /// Conversion between a base-58 encoded public key and the <see cref="PublicKey"/> object.
        /// </summary>
        /// <param name="address">The base-58 encoded public key.</param>
        /// <returns>The PublicKey object.</returns>
        public static explicit operator PublicKey(string address) => new(address);

        /// <summary>
        /// Conversion between a <see cref="PublicKey"/> object and the public key as a byte array.
        /// </summary>
        /// <param name="publicKey">The PublicKey object.</param>
        /// <returns>The public key as a byte array.</returns>
        public static implicit operator byte[](PublicKey publicKey) => publicKey.KeyBytes;

        /// <summary>
        /// Conversion between a public key as a byte array and the corresponding <see cref="PublicKey"/> object.
        /// </summary>
        /// <param name="keyBytes">The public key as a byte array.</param>
        /// <returns>The PublicKey object.</returns>
        public static explicit operator PublicKey(byte[] keyBytes) => new(keyBytes);

        /// <inheritdoc cref="ToString"/>
        public override string ToString() => Key;

        /// <inheritdoc cref="GetHashCode"/>
        public override int GetHashCode() => Key.GetHashCode();

        #region KeyDerivation

        /// <summary>
        /// The bytes of the `ProgramDerivedAddress` string.
        /// </summary>
        private static readonly byte[] ProgramDerivedAddressBytes = Encoding.UTF8.GetBytes("ProgramDerivedAddress");

        /// <summary>
        /// Derives a program address.
        /// </summary>
        /// <param name="seeds">The address seeds.</param>
        /// <param name="programId">The program Id.</param>
        /// <param name="publicKey">The derived public key, returned as inline out.</param>
        /// <returns>true if it could derive the program address for the given seeds, otherwise false..</returns>
        /// <exception cref="ArgumentException">Throws exception when one of the seeds has an invalid length.</exception>
        public static bool TryCreateProgramAddress(ICollection<byte[]> seeds, PublicKey programId, out PublicKey publicKey)
        {
            MemoryStream buffer = new(32 * seeds.Count + ProgramDerivedAddressBytes.Length + programId.KeyBytes.Length);

            foreach (byte[] seed in seeds)
            {
                if (seed.Length > 32)
                {
                    throw new ArgumentException("max seed length exceeded", nameof(seeds));
                }
                buffer.Write(seed);
            }

            buffer.Write(programId.KeyBytes);
            buffer.Write(ProgramDerivedAddressBytes);

            byte[] hash = SHA256.HashData(buffer.ToArray());

            if (hash.IsOnCurve())
            {
                publicKey = null;
                return false;
            }
            publicKey = new(hash);
            return true;
        }

        /// <summary>
        /// Attempts to find a program address for the passed seeds and program Id.
        /// </summary>
        /// <param name="seeds">The address seeds.</param>
        /// <param name="programId">The program Id.</param>
        /// <param name="address">The derived address, returned as inline out.</param>
        /// <param name="nonce">The nonce used to derive the address, returned as inline out.</param>
        /// <returns>true whenever the address for a nonce was found, otherwise false.</returns>
        public static bool TryFindProgramAddress(IEnumerable<byte[]> seeds, PublicKey programId, out PublicKey address, out int nonce)
        {
            byte derivationNonce = 255;
            List<byte[]> buffer = seeds.ToList();
            var nonceArray = new byte[1];
            buffer.Add(nonceArray);

            while (derivationNonce != 0)
            {
                nonceArray[0] = derivationNonce;
                bool success = TryCreateProgramAddress(buffer, programId, out PublicKey derivedAddress);

                if (success)
                {
                    address = derivedAddress;
                    nonce = derivationNonce;
                    return true;
                }

                derivationNonce--;
            }

            address = null;
            nonce = 0;
            return false;
        }

        /// <summary>
        /// Derives a new public key from an existing public key and seed
        /// </summary>
        /// <param name="fromPublicKey">The extant pubkey</param>
        /// <param name="seed">The seed</param>
        /// <param name="programId">The programid</param>
        /// <param name="publicKeyOut">The derived public key</param>
        /// <returns></returns>
        public static bool TryCreateWithSeed(PublicKey fromPublicKey, string seed, PublicKey programId, out PublicKey publicKeyOut)
        {
            var b58 = new Base58Encoder();
            MemoryStream buffer = new();

            buffer.Write(fromPublicKey.KeyBytes);
            buffer.Write(Encoding.UTF8.GetBytes(seed));
            buffer.Write(programId.KeyBytes);

            var seeds = buffer.ToArray();

            if(seeds.Length >= ProgramDerivedAddressBytes.Length)
            {
                var slice = seeds.AsSpan(seeds.Length - ProgramDerivedAddressBytes.Length);
                
                if(slice.SequenceEqual(ProgramDerivedAddressBytes))
                {
                    publicKeyOut = null;
                    return false;
                }
            }

            byte[] hash = SHA256.HashData(seeds);
            publicKeyOut = new PublicKey(hash);
            return true;
        }

        #endregion
    }
}