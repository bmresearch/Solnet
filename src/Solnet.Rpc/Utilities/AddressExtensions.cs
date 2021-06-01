using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Solnet.Rpc.Utilities
{
    public static class AddressExtensions
    {
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
            var buffer = new MemoryStream();

            foreach (var seed in seeds)
            {
                if (seed.Length > 32)
                {
                    throw new ArgumentException("max seed length exceeded", nameof(seeds));
                }
                buffer.Write(seed);
            }
            
            buffer.Write(programId);
            buffer.Write(Encoding.ASCII.GetBytes("ProgramDerivedAddress"));

            var hash = SHA256.HashData(buffer.ToArray());

            if (hash.IsOnCurve() != 0)
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
        public static (byte[] Address, int Nonce) FindProgramAddress(IEnumerable<byte[]> seeds, byte[] programId)
        {
            var nonce = 255;
            var buffer = seeds.ToList();

            while (nonce-- != 0)
            {
                byte[] address;
                try
                {
                    buffer.Add(new [] { (byte) nonce });
                    address = CreateProgramAddress(buffer, programId);
                }
                catch (Exception)
                {
                    buffer.RemoveAt(buffer.Count - 1);
                    continue;
                }
                
                return (address, nonce);
            }

            throw new Exception("unable to find viable program address nonce");
        }
    }
}