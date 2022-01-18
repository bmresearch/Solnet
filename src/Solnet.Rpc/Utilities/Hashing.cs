using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Rpc.Utilities
{
    /// <summary>
    /// Contains helper methods to perform hashing operations
    /// </summary>
    public static  class Hashing
    {
        /// <summary>
        /// Calculates the SHA256 of the given data.
        /// </summary>
        /// <param name="data">The data to hash.</param>
        /// <returns>The hash.</returns>
        public static byte[] Sha256(byte[] data)
        {
            byte[] i = new byte[32];
            Sha256Digest digest = new();
            digest.BlockUpdate(data, 0, data.Length);
            digest.DoFinal(i, 0);
            return i;
        }
    }
}
