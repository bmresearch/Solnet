using Org.BouncyCastle.Crypto.Digests;
using Solnet.Wallet;
using Solnet.Wallet.Test;
using Solnet.Wallet.Utilities;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Solnet.Rpc.Utilities
{
    /// <summary>
    /// Contains helper methods to handle program addresses.
    /// </summary>
    public static class AddressExtensions
    {
        /// <summary>
        /// The bytes of the `ProgramDerivedAddress` string.
        /// </summary>
        private static readonly byte[] ProgramDerivedAddressBytes = Encoding.UTF8.GetBytes("ProgramDerivedAddress");
        private static byte[] EncodeRustString(string data)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(data);
            byte[] encoded = new byte[stringBytes.Length];

            encoded.WriteSpan(stringBytes,0);

            return encoded;
        }
        private static void WriteU32(this byte[] data, uint value, int offset)
        {
            if (offset + sizeof(uint) > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(offset, sizeof(uint)), value);
        }
        private static void WriteSpan(this byte[] data, ReadOnlySpan<byte> span, int offset)
        {
            if (offset + span.Length > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            span.CopyTo(data.AsSpan(offset, span.Length));
        }
        public static bool TryCreateWithSeed(
            PublicKey fromPublicKey, string seed, PublicKey programId, out PublicKey publicKeyOut)
        {
            Console.WriteLine("seed = " + seed);
            var b58 = new Base58Encoder();
            MemoryStream buffer = new();
            MemoryStream buffer2 = new();
            MemoryStream buffer3 = new();
            buffer.Write(fromPublicKey.KeyBytes);
            //Console.WriteLine("buffer array 1= " + Utils.ToStringByteArray(buffer.ToArray()));
            buffer.Write(EncodeRustString(seed));
            buffer2.Write(EncodeRustString(seed));
            //Console.WriteLine("buffer array 2= " + Utils.ToStringByteArray(buffer2.ToArray()));
            buffer.Write(programId.KeyBytes);
            buffer3.Write(programId.KeyBytes);
            //Console.WriteLine("buffer array 3= " + Utils.ToStringByteArray(buffer3.ToArray()));
            byte[] hash = Sha256(buffer.ToArray());
            var b58h = b58.EncodeData(hash);
            //Console.WriteLine("b58h=" + b58h);
            publicKeyOut = new PublicKey(hash);
            //Console.WriteLine("pukout = " + publicKeyOut);
            return true;
        }


        /// <summary>
        /// Derives a program address.
        /// </summary>
        /// <param name="seeds">The address seeds.</param>
        /// <param name="programId">The program Id.</param>
        /// <param name="publicKeyBytes">The derived public key bytes, returned as inline out.</param>
        /// <returns>true if it could derive the program address for the given seeds, otherwise false..</returns>
        /// <exception cref="ArgumentException">Throws exception when one of the seeds has an invalid length.</exception>
        public static bool TryCreateProgramAddress(IList<byte[]> seeds, byte[] programId, out byte[] publicKeyBytes)
        {
            MemoryStream buffer = new(32 * seeds.Count + ProgramDerivedAddressBytes.Length + programId.Length);

            foreach (byte[] seed in seeds)
            {
                if (seed.Length > 32)
                {
                    throw new ArgumentException("max seed length exceeded", nameof(seeds));
                }
                buffer.Write(seed);
            }

            buffer.Write(programId);
            buffer.Write(ProgramDerivedAddressBytes);

            byte[] hash = Sha256(buffer.ToArray());

            if (hash.IsOnCurve())
            {
                publicKeyBytes = null;
                return false;
            }
            publicKeyBytes = hash;
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
        public static bool TryFindProgramAddress(IEnumerable<byte[]> seeds, byte[] programId, out byte[] address, out int nonce)
        {
            int derivationNonce = 255;
            List<byte[]> buffer = seeds.ToList();

            while (derivationNonce != 0)
            {
                buffer.Add(new[] { (byte)derivationNonce });
                bool success = TryCreateProgramAddress(buffer, programId, out byte[] derivedAddress);

                if (success)
                {
                    address = derivedAddress;
                    nonce = derivationNonce;
                    return true;
                }

                buffer.RemoveAt(buffer.Count - 1);
                derivationNonce--;
            }

            address = null;
            nonce = 0;
            return false;
        }

        /// <summary>
        /// Calculates the SHA256 of the given data.
        /// </summary>
        /// <param name="data">The data to hash.</param>
        /// <returns>The hash.</returns>
        private static byte[] Sha256(byte[] data)
        {
            byte[] i = new byte[32];
            Sha256Digest digest = new();
            digest.BlockUpdate(data, 0, data.Length);
            digest.DoFinal(i, 0);
            return i;
        }
    }
}