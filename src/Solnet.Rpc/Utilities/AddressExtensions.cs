using Org.BouncyCastle.Crypto.Digests;
using System;
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

        /// <summary>
        /// Derives a program address.
        /// </summary>
        /// <param name="seeds">The address seeds.</param>
        /// <param name="programId">The program Id.</param>
        /// <returns>The address derived.</returns>
        /// <exception cref="ArgumentException">Throws exception when one of the seeds has an invalid length.</exception>
        /// <exception cref="Exception">Throws exception when the resulting address doesn't fall off the Ed25519 curve.</exception>
        public static byte[] CreateProgramAddress(IList<byte[]> seeds, byte[] programId)
        {
            MemoryStream buffer = new (32 * seeds.Count + ProgramDerivedAddressBytes.Length + programId.Length);

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
                throw new Exception("invalid seeds, address must fall off curve");
            }

            return hash;
        }

        /// <summary>
        /// Attempts to find a program address for the passed seeds and program Id.
        /// </summary>
        /// <param name="seeds">The address seeds.</param>
        /// <param name="programId">The program Id.</param>
        /// <returns>A tuple corresponding to the address and nonce found.</returns>
        /// <exception cref="Exception">Throws exception when it is unable to find a viable nonce for the address.</exception>
        public static (byte[] Address, int Nonce) FindProgramAddress(IList<byte[]> seeds, byte[] programId)
        {
            int nonce = 255;

            while (nonce-- != 0)
            {
                byte[] address;
                try
                {
                    seeds.Add(new[] { (byte)nonce });
                    address = CreateProgramAddress(seeds, programId);
                }
                catch (Exception)
                {
                    seeds.RemoveAt(seeds.Count - 1);
                    continue;
                }

                return (address, nonce);
            }

            throw new Exception("unable to find viable program address nonce");
        }
        
        /// <summary>
        /// Calculates the SHA256 of the given data.
        /// </summary>
        /// <param name="data">The data to hash.</param>
        /// <returns>The hash.</returns>
        private static byte[] Sha256(byte[] data)
        {
            byte[] i = new byte[32];
            Sha256Digest digest = new ();
            digest.BlockUpdate(data, 0, data.Length);
            digest.DoFinal(i, 0);
            return i;
        }
    }
}