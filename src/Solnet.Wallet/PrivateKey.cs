using Bifrost.Security;
using Solnet.Wallet.Utilities;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace Solnet.Wallet
{
    /// <summary>
    /// Implements the private key functionality.
    /// </summary>
    public sealed class PrivateKey : IEquatable<PrivateKey>
    {
        public const int PrivateKeyLength = 64;

        private readonly byte[] _keyBytes;
        private string _base58Cache;

        /// <summary>
        /// Accede a los bytes de la clave privada.
        /// </summary>
        public ReadOnlySpan<byte> KeyBytes => _keyBytes;

        /// <summary>
        /// Construye desde bytes de clave privada.
        /// </summary>
        /// <param name="keyBytes">Clave privada de 64 bytes.</param>
        public PrivateKey(ReadOnlySpan<byte> keyBytes)
        {
            if (keyBytes.Length != PrivateKeyLength)
                throw new ArgumentException($"Clave privada inválida, longitud esperada: {PrivateKeyLength} bytes", nameof(keyBytes));

            _keyBytes = new byte[PrivateKeyLength];
            keyBytes.CopyTo(_keyBytes);
        }

        /// <summary>
        /// Construye desde cadena Base58.
        /// </summary>
        public PrivateKey(string base58)
        {
            if (string.IsNullOrWhiteSpace(base58))
                throw new ArgumentNullException(nameof(base58));

            var bytes = Encoders.Base58.DecodeData(base58);
            if (bytes.Length != PrivateKeyLength)
                throw new ArgumentException($"Base58 inválido: se esperaban {PrivateKeyLength} bytes al decodificar", nameof(base58));

            _keyBytes = bytes;
            _base58Cache = base58;
        }

        /// <summary>
        /// Devuelve la representación Base58 de la clave.
        /// Solo se genera una vez y se cachea en memoria.
        /// </summary>
        public string GetBase58()
        {
            return _base58Cache ??= Encoders.Base58.EncodeData(_keyBytes);
        }

        /// <summary>
        /// Firma un mensaje con la clave privada.
        /// </summary>
        public byte[] Sign(ReadOnlySpan<byte> message)
        {
            var signature = new byte[64];
            Ed25519.Sign(signature, message.ToArray(), _keyBytes);
            return signature;
        }

        /// <summary>
        /// Compara con otra clave privada.
        /// </summary>
        public bool Equals(PrivateKey other)
        {
            if (other is null) return false;
            return _keyBytes.SequenceEqual(other._keyBytes);
        }

        public override bool Equals(object obj) => obj is PrivateKey other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                foreach (var b in _keyBytes)
                    hash = hash * 31 + b;
                return hash;
            }
        }

        /// <summary>
        /// Conversión explícita desde cadena base58.
        /// </summary>
        public static explicit operator PrivateKey(string base58) => new(base58);

        /// <summary>
        /// Conversión explícita desde bytes.
        /// </summary>
        public static explicit operator PrivateKey(byte[] keyBytes) => new(keyBytes);

        /// <summary>
        /// Conversión implícita a bytes.
        /// </summary>
        public static implicit operator byte[](PrivateKey privateKey) => [.. privateKey._keyBytes];
    }

    // Vulnerability #1 : DO NOT EXPOSE PRIVATE KEY.
    //[DebuggerDisplay("Key = {ToString()}")]
    //public class PrivateKey
    //{
    //    /// <summary>
    //    /// Private key length.
    //    /// </summary>
    //    public const int PrivateKeyLength = 64;

    //    private string _key;

    //    /// <summary>
    //    /// The key as base-58 encoded string.
    //    /// </summary>
    //    public string Key
    //    {
    //        get
    //        {
    //            if (_key == null)
    //            {
    //                Key = Encoders.Base58.EncodeData(KeyBytes);
    //            }
    //            return _key;
    //        }
    //        set => _key = value;
    //    }


    //    private byte[] _keyBytes;

    //    /// <summary>
    //    /// The bytes of the key.
    //    /// </summary>
    //    public byte[] KeyBytes
    //    {
    //        get
    //        {
    //            if (_keyBytes == null)
    //            {
    //                KeyBytes = Encoders.Base58.DecodeData(Key);
    //            }
    //            return _keyBytes;
    //        }
    //        set => _keyBytes = value;
    //    }


    //    /// <summary>
    //    /// Initialize the public key from the given byte array.
    //    /// </summary>
    //    /// <param name="key">The public key as byte array.</param>
    //    public PrivateKey(byte[] key)
    //    {
    //        if (key == null)
    //            throw new ArgumentNullException(nameof(key));
    //        if (key.Length != PrivateKeyLength)
    //            throw new ArgumentException("invalid key length", nameof(key));
    //        KeyBytes = new byte[PrivateKeyLength];
    //        Array.Copy(key, KeyBytes, PrivateKeyLength);
    //    }

    //    /// <summary>
    //    /// Initialize the public key from the given string.
    //    /// </summary>
    //    /// <param name="key">The public key as base58 encoded string.</param>
    //    public PrivateKey(string key)
    //    {
    //        Key = key ?? throw new ArgumentNullException(nameof(key));
    //    }

    //    /// <summary>
    //    /// Initialize the public key from the given string.
    //    /// </summary>
    //    /// <param name="key">The public key as base58 encoded string.</param>
    //    public PrivateKey(ReadOnlySpan<byte> key)
    //    {
    //        if (key.Length != PrivateKeyLength)
    //            throw new ArgumentException("invalid key length", nameof(key));
    //        KeyBytes = new byte[PrivateKeyLength];
    //        key.CopyTo(KeyBytes.AsSpan());
    //    }

    //    /// <summary>
    //    /// Sign the data.
    //    /// </summary>
    //    /// <param name="message">The data to sign.</param>
    //    /// <returns>The signature of the data.</returns>
    //    public byte[] Sign(byte[] message)
    //    {
    //        byte[] signature = new byte[64];
    //        Ed25519.Sign(signature, message, KeyBytes);
    //        return signature;
    //    }

    //    /// <inheritdoc cref="Equals(object)"/>
    //    public override bool Equals(object obj)
    //    {
    //        if (obj is PrivateKey pk) return pk.Key == this.Key;

    //        return false;
    //    }

    //    /// <summary>
    //    /// Conversion between a <see cref="PrivateKey"/> object and the corresponding base-58 encoded private key.
    //    /// </summary>
    //    /// <param name="privateKey">The PrivateKey object.</param>
    //    /// <returns>The base-58 encoded private key.</returns>
    //    public static implicit operator string(PrivateKey privateKey) => privateKey.Key;

    //    /// <summary>
    //    /// Conversion between a base-58 encoded private key and the <see cref="PrivateKey"/> object.
    //    /// </summary>
    //    /// <param name="address">The base-58 encoded private key.</param>
    //    /// <returns>The PrivateKey object.</returns>
    //    public static explicit operator PrivateKey(string address) => new(address);

    //    /// <summary>
    //    /// Conversion between a <see cref="PrivateKey"/> object and the private key as a byte array.
    //    /// </summary>
    //    /// <param name="privateKey">The PrivateKey object.</param>
    //    /// <returns>The private key as a byte array.</returns>
    //    public static implicit operator byte[](PrivateKey privateKey) => privateKey.KeyBytes;

    //    /// <summary>
    //    /// Conversion between a private key as a byte array and the corresponding <see cref="PrivateKey"/> object.
    //    /// </summary>
    //    /// <param name="keyBytes">The private key as a byte array.</param>
    //    /// <returns>The PrivateKey object.</returns>
    //    public static explicit operator PrivateKey(byte[] keyBytes) => new(keyBytes);

    //    ///// <inheritdoc cref="ToString"/>
    //    //public override string ToString() => Key;

    //    /// <inheritdoc cref="GetHashCode"/>
    //    public override int GetHashCode() => Key.GetHashCode();
    //}
}