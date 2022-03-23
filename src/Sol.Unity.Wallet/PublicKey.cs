using Chaos.NaCl;
using Sol.Unity.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Sol.Unity.Wallet
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


        /// <summary>
        /// Checks if this object is a valid Ed25519 PublicKey.
        /// </summary>
        /// <returns>Returns true if it is a valid key, false otherwise.</returns>
        public bool IsOnCurve()
        {
            return Ed25519Extensions.IsOnCurve(KeyBytes);
        }

        /// <summary>
        /// Checks if this object is a valid Solana PublicKey.
        /// </summary>
        /// <returns>Returns true if it is a valid key, false otherwise.</returns>
        public bool IsValid()
        {
            return KeyBytes != null && KeyBytes.Length == PublicKeyLength;
        }

        /// <summary>
        /// Checks if a given string forms a valid PublicKey in base58.
        /// </summary>
        /// <remarks>
        /// Any set of 32 bytes can constitute a valid solana public key. However, not all 32-byte public keys are valid Ed25519 public keys. <br/>
        /// Two concrete examples: <br/>
        /// - A user wallet key must be on the curve (otherwise a user wouldn't be able to sign transactions).  <br/>
        /// - A program derived address must NOT be on the curve.
        /// </remarks>
        /// <param name="key">The base58 encoded public key.</param>
        /// <param name="validateCurve">Whether or not to validate if the public key belongs to the Ed25519 curve.</param>
        /// <returns>Returns true if the input is a valid key, false otherwise.</returns>
        public static bool IsValid(string key, bool validateCurve = false)
        {
            if(!string.IsNullOrEmpty(key))
            {
                try
                {
                    return IsValid(Encoders.Base58.DecodeData(key), validateCurve);
                }
                catch(Exception)
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if a given set of bytes forms a valid PublicKey.
        /// </summary>
        /// <remarks>
        /// Any set of 32 bytes can constitute a valid solana public key. However, not all 32-byte public keys are valid Ed25519 public keys. <br/>
        /// Two concrete examples: <br/>
        /// - A user wallet key must be on the curve (otherwise a user wouldn't be able to sign transactions).  <br/>
        /// - A program derived address must NOT be on the curve.
        /// </remarks>
        /// <param name="key">The key bytes.</param>
        /// <param name="validateCurve">Whether or not to validate if the public key belongs to the Ed25519 curve.</param>
        /// <returns>Returns true if the input is a valid key, false otherwise.</returns>
        public static bool IsValid(byte[] key, bool validateCurve = false)
        {
            return key != null && key.Length == PublicKeyLength && (!validateCurve || key.IsOnCurve());
        }
        
        /// <summary>
        /// Checks if a given set of bytes forms a valid PublicKey.
        /// </summary>
        /// <remarks>
        /// Any set of 32 bytes can constitute a valid solana public key. However, not all 32-byte public keys are valid Ed25519 public keys. <br/>
        /// Two concrete examples: <br/>
        /// - A user wallet key must be on the curve (otherwise a user wouldn't be able to sign transactions).  <br/>
        /// - A program derived address must NOT be on the curve.
        /// </remarks>
        /// <param name="key">The key bytes.</param>
        /// <param name="validateCurve">Whether or not to validate if the public key belongs to the Ed25519 curve.</param>
        /// <returns>Returns true if the input is a valid key, false otherwise.</returns>
        public static bool IsValid(ReadOnlySpan<byte> key, bool validateCurve = false)
        {
            return key != null && key.Length == PublicKeyLength && (!validateCurve || key.IsOnCurve());
        }

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
            MemoryStream buffer = new(PublicKeyLength * seeds.Count + ProgramDerivedAddressBytes.Length + programId.KeyBytes.Length);

            foreach (byte[] seed in seeds)
            {
                if (seed.Length > PublicKeyLength)
                {
                    throw new ArgumentException("max seed length exceeded", nameof(seeds));
                }
                buffer.Write(seed,0, seed.Length);
            }

            buffer.Write(programId.KeyBytes, 0, programId.KeyBytes.Length);
            buffer.Write(ProgramDerivedAddressBytes, 0, ProgramDerivedAddressBytes.Length);

            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(new ReadOnlySpan<byte>(buffer.GetBuffer(), 0, (int)buffer.Length).ToArray());

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
        /// <param name="bump">The bump used to derive the address, returned as inline out.</param>
        /// <returns>True whenever the address for a nonce was found, otherwise false.</returns>
        public static bool TryFindProgramAddress(IEnumerable<byte[]> seeds, PublicKey programId, out PublicKey address, out byte bump)
        {
            byte seedBump = 255;
            List<byte[]> buffer = seeds.ToList();
            var bumpArray = new byte[1];
            buffer.Add(bumpArray);

            while (seedBump != 0)
            {
                bumpArray[0] = seedBump;
                bool success = TryCreateProgramAddress(buffer, programId, out PublicKey derivedAddress);

                if (success)
                {
                    address = derivedAddress;
                    bump = seedBump;
                    return true;
                }

                seedBump--;
            }

            address = null;
            bump = 0;
            return false;
        }

        /// <summary>
        /// Derives a new public key from an existing public key and seed
        /// </summary>
        /// <param name="fromPublicKey">The extant pubkey</param>
        /// <param name="seed">The seed</param>
        /// <param name="programId">The programid</param>
        /// <param name="publicKeyOut">The derived public key</param>
        /// <returns>True whenever the address was successfully created, otherwise false.</returns>
        /// <remarks>To fail address creation, means the created address was a PDA.</remarks>
        public static bool TryCreateWithSeed(PublicKey fromPublicKey, string seed, PublicKey programId, out PublicKey publicKeyOut)
        {
            var b58 = new Base58Encoder();
            MemoryStream buffer = new();

            buffer.Write(fromPublicKey.KeyBytes, 0, fromPublicKey.KeyBytes.Length);
            buffer.Write(Encoding.UTF8.GetBytes(seed), 0, Encoding.UTF8.GetBytes(seed).Length);
            buffer.Write(programId.KeyBytes, 0, programId.KeyBytes.Length);

            var seeds = new ReadOnlySpan<byte>(buffer.GetBuffer(), 0, (int)buffer.Length);

            if(seeds.Length >= ProgramDerivedAddressBytes.Length)
            {
                var slice = seeds.Slice(seeds.Length - ProgramDerivedAddressBytes.Length);
                
                if(slice.SequenceEqual(ProgramDerivedAddressBytes))
                {
                    publicKeyOut = null;
                    return false;
                }
            }
            
            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(seeds.ToArray());
            publicKeyOut = new PublicKey(hash);
            return true;
        }

        #endregion
    }
}