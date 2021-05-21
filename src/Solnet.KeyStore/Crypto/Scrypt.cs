using System;
using System.Diagnostics;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;

namespace Solnet.KeyStore.Crypto
{
public class Scrypt
    {
        private static byte[] SingleIterationPbkdf2(byte[] p, byte[] s, int dkLen)
        {
            PbeParametersGenerator pGen = new Pkcs5S2ParametersGenerator(new Sha256Digest());
            pGen.Init(p, s, 1);
            KeyParameter key = (KeyParameter)pGen.GenerateDerivedMacParameters(dkLen * 8);
            return key.GetKey();
        }

        public unsafe static byte[] CryptoScrypt(byte[] password, byte[] salt, int n, int r, int p, int dkLen)
        {
            var ba = new byte[128 * r * p + 63];
            var xYa = new byte[256 * r + 63];
            var va = new byte[128 * r * n + 63];
            var buf = new byte[32];

            ba = SingleIterationPbkdf2(password, salt, p * 128 * r);

            fixed (byte* b = ba)
            fixed (void* v = va)
            fixed (void* xy = xYa)
            {
                /* 2: for i = 0 to p - 1 do */
                for (var i = 0; i < p; i++)
                {
                    /* 3: B_i <-- MF(B_i, N) */
                    SMix(&b[i * 128 * r], r, n, (uint*)v, (uint*)xy);
                }
            }

            /* 5: DK <-- PBKDF2(P, B, 1, dkLen) */
            return SingleIterationPbkdf2(password, ba, dkLen);
        }


        /// <summary>
        /// Copies a specified number of bytes from a source pointer to a destination pointer.
        /// </summary>
        private unsafe static void BulkCopy(void* dst, void* src, int len)
        {
            var d = (byte*)dst;
            var s = (byte*)src;

            while (len >= 8)
            {
                *(ulong*)d = *(ulong*)s;
                d += 8;
                s += 8;
                len -= 8;
            }
            if (len >= 4)
            {
                *(uint*)d = *(uint*)s;
                d += 4;

                s += 4;
                len -= 4;
            }
            if (len >= 2)
            {
                *(ushort*)d = *(ushort*)s;
                d += 2;
                s += 2;
                len -= 2;
            }
            if (len >= 1)
            {
                *d = *s;
            }
        }

        /// <summary>
        /// Copies a specified number of bytes from a source pointer to a destination pointer.
        /// </summary>
        private unsafe static void BulkXor(void* dst, void* src, int len)
        {
            var d = (byte*)dst;
            var s = (byte*)src;

            while (len >= 8)
            {
                *(ulong*)d ^= *(ulong*)s;
                d += 8;
                s += 8;
                len -= 8;
            }
            if (len >= 4)
            {
                *(uint*)d ^= *(uint*)s;
                d += 4;
                s += 4;
                len -= 4;
            }
            if (len >= 2)
            {
                *(ushort*)d ^= *(ushort*)s;
                d += 2;
                s += 2;
                len -= 2;
            }
            if (len >= 1)
            {
                *d ^= *s;
            }
        }

        /// <summary>
        /// Encode an integer to byte array on any alignment in little endian format.
        /// </summary>
        private unsafe static void Encode32(byte* p, uint x)
        {
            p[0] = (byte)(x & 0xff);
            p[1] = (byte)((x >> 8) & 0xff);
            p[2] = (byte)((x >> 16) & 0xff);
            p[3] = (byte)((x >> 24) & 0xff);
        }

        /// <summary>
        /// Decode an integer from byte array on any alignment in little endian format.
        /// </summary>
        private unsafe static uint Decode32(byte* p)
        {
            return
                ((uint)(p[0]) +
                ((uint)(p[1]) << 8) +
                ((uint)(p[2]) << 16) +
                ((uint)(p[3]) << 24));
        }

        /// <summary>
        /// Apply the salsa20/8 core to the provided block.
        /// </summary>
        private unsafe static void Salsa208(uint* b)
        {
            uint x0 = b[0];
            uint x1 = b[1];
            uint x2 = b[2];
            uint x3 = b[3];
            uint x4 = b[4];
            uint x5 = b[5];
            uint x6 = b[6];
            uint x7 = b[7];
            uint x8 = b[8];
            uint x9 = b[9];
            uint x10 = b[10];
            uint x11 = b[11];
            uint x12 = b[12];
            uint x13 = b[13];
            uint x14 = b[14];
            uint x15 = b[15];

            for (var i = 0; i < 8; i += 2)
            {
                //((x0 + x12) << 7) | ((x0 + x12) >> (32 - 7));
                /* Operate on columns. */
                x4 ^= R(x0 + x12, 7); x8 ^= R(x4 + x0, 9);
                x12 ^= R(x8 + x4, 13); x0 ^= R(x12 + x8, 18);

                x9 ^= R(x5 + x1, 7); x13 ^= R(x9 + x5, 9);
                x1 ^= R(x13 + x9, 13); x5 ^= R(x1 + x13, 18);

                x14 ^= R(x10 + x6, 7); x2 ^= R(x14 + x10, 9);
                x6 ^= R(x2 + x14, 13); x10 ^= R(x6 + x2, 18);

                x3 ^= R(x15 + x11, 7); x7 ^= R(x3 + x15, 9);
                x11 ^= R(x7 + x3, 13); x15 ^= R(x11 + x7, 18);

                /* Operate on rows. */
                x1 ^= R(x0 + x3, 7); x2 ^= R(x1 + x0, 9);
                x3 ^= R(x2 + x1, 13); x0 ^= R(x3 + x2, 18);

                x6 ^= R(x5 + x4, 7); x7 ^= R(x6 + x5, 9);
                x4 ^= R(x7 + x6, 13); x5 ^= R(x4 + x7, 18);

                x11 ^= R(x10 + x9, 7); x8 ^= R(x11 + x10, 9);
                x9 ^= R(x8 + x11, 13); x10 ^= R(x9 + x8, 18);

                x12 ^= R(x15 + x14, 7); x13 ^= R(x12 + x15, 9);
                x14 ^= R(x13 + x12, 13); x15 ^= R(x14 + x13, 18);
            }

            b[0] += x0;
            b[1] += x1;
            b[2] += x2;
            b[3] += x3;
            b[4] += x4;
            b[5] += x5;
            b[6] += x6;
            b[7] += x7;
            b[8] += x8;
            b[9] += x9;
            b[10] += x10;
            b[11] += x11;
            b[12] += x12;
            b[13] += x13;
            b[14] += x14;
            b[15] += x15;
        }

        /// <summary>
        /// Utility method for Salsa208.
        /// </summary>
        private unsafe static uint R(uint a, int b)
        {
            return (a << b) | (a >> (32 - b));
        }

        /// <summary>
        /// Compute Bout = BlockMix_{salsa20/8, r}(Bin).  The input Bin must be 128r
        /// bytes in length; the output Bout must also be the same size.
        /// The temporary space X must be 64 bytes.
        /// </summary>
        private unsafe static void BlockMix(uint* bin, uint* bout, uint* x, int r)
        {
            /* 1: X <-- B_{2r - 1} */
            BulkCopy(x, &bin[(2 * r - 1) * 16], 64);

            /* 2: for i = 0 to 2r - 1 do */
            for (var i = 0; i < 2 * r; i += 2)
            {
                /* 3: X <-- H(X \xor B_i) */
                BulkXor(x, &bin[i * 16], 64);
                Salsa208(x);

                /* 4: Y_i <-- X */
                /* 6: B' <-- (Y_0, Y_2 ... Y_{2r-2}, Y_1, Y_3 ... Y_{2r-1}) */
                BulkCopy(&bout[i * 8], x, 64);

                /* 3: X <-- H(X \xor B_i) */
                BulkXor(x, &bin[i * 16 + 16], 64);
                Salsa208(x);

                /* 4: Y_i <-- X */
                /* 6: B' <-- (Y_0, Y_2 ... Y_{2r-2}, Y_1, Y_3 ... Y_{2r-1}) */
                BulkCopy(&bout[i * 8 + r * 16], x, 64);
            }
        }

        /// <summary>
        /// Return the result of parsing B_{2r-1} as a little-endian integer.
        /// </summary>
        private unsafe static long Integerify(uint* b, int r)
        {
            var x = (uint*)(((byte*)b) + (2 * r - 1) * 64);

            return (((long)(x[1]) << 32) + x[0]);
        }

        /// <summary>
        /// Compute B = SMix_r(B, N).  The input B must be 128r bytes in length;
        /// the temporary storage V must be 128rN bytes in length; the temporary
        /// storage XY must be 256r + 64 bytes in length.  The value N must be a
        /// power of 2 greater than 1.  The arrays B, V, and XY must be aligned to a
        /// multiple of 64 bytes.
        /// </summary>
        private unsafe static void SMix(byte* b, int r, int n, uint* v, uint* xy)
        {
            var x = xy;
            var y = &xy[32 * r];
            var z = &xy[64 * r];

            /* 1: X <-- B */
            for (var k = 0; k < 32 * r; k++)
            {
                x[k] = Decode32(&b[4 * k]);
            }

            /* 2: for i = 0 to N - 1 do */
            for (var i = 0L; i < n; i += 2)
            {
                /* 3: V_i <-- X */
                BulkCopy(&v[i * (32 * r)], x, 128 * r);

                /* 4: X <-- H(X) */
                BlockMix(x, y, z, r);

                /* 3: V_i <-- X */
                BulkCopy(&v[(i + 1) * (32 * r)], y, 128 * r);

                /* 4: X <-- H(X) */
                BlockMix(y, x, z, r);
            }

            /* 6: for i = 0 to N - 1 do */
            for (var i = 0; i < n; i += 2)
            {
                /* 7: j <-- Integerify(X) mod N */
                var j = Integerify(x, r) & (n - 1);

                /* 8: X <-- H(X \xor V_j) */
                BulkXor(x, &v[j * (32 * r)], 128 * r);
                BlockMix(x, y, z, r);

                /* 7: j <-- Integerify(X) mod N */
                j = Integerify(y, r) & (n - 1);

                /* 8: X <-- H(X \xor V_j) */
                BulkXor(y, &v[j * (32 * r)], 128 * r);
                BlockMix(y, x, z, r);
            }

            /* 10: B' <-- X */
            for (var k = 0; k < 32 * r; k++)
            {
                Encode32(&b[4 * k], x[k]);
            }
        }

    }

    /// <summary>Implementation of the scrypt a password-based key derivation function.</summary>
    /// <remarks>
    /// Scrypt was created by Colin Percival and is specified in
    /// <a href="http://tools.ietf.org/html/draft-josefsson-scrypt-kdf-01">draft-josefsson-scrypt-kd</a>.
    /// </remarks>
    public class SCrypt
    {

        internal static uint R(uint x, int y)
        {
            return (x << y) | (x >> (32 - y));
        }

        internal static void SalsaCore(int rounds, uint[] input, uint[] x)
        {
            if (input.Length != 16)
                throw new ArgumentException();
            if (x.Length != 16)
                throw new ArgumentException();
            if (rounds % 2 != 0)
                throw new ArgumentException("Number of rounds must be even");

            uint x00 = input[0];
            uint x01 = input[1];
            uint x02 = input[2];
            uint x03 = input[3];
            uint x04 = input[4];
            uint x05 = input[5];
            uint x06 = input[6];
            uint x07 = input[7];
            uint x08 = input[8];
            uint x09 = input[9];
            uint x10 = input[10];
            uint x11 = input[11];
            uint x12 = input[12];
            uint x13 = input[13];
            uint x14 = input[14];
            uint x15 = input[15];

            for (int i = rounds; i > 0; i -= 2)
            {
                x04 ^= R((x00 + x12), 7);
                x08 ^= R((x04 + x00), 9);
                x12 ^= R((x08 + x04), 13);
                x00 ^= R((x12 + x08), 18);
                x09 ^= R((x05 + x01), 7);
                x13 ^= R((x09 + x05), 9);
                x01 ^= R((x13 + x09), 13);
                x05 ^= R((x01 + x13), 18);
                x14 ^= R((x10 + x06), 7);
                x02 ^= R((x14 + x10), 9);
                x06 ^= R((x02 + x14), 13);
                x10 ^= R((x06 + x02), 18);
                x03 ^= R((x15 + x11), 7);
                x07 ^= R((x03 + x15), 9);
                x11 ^= R((x07 + x03), 13);
                x15 ^= R((x11 + x07), 18);

                x01 ^= R((x00 + x03), 7);
                x02 ^= R((x01 + x00), 9);
                x03 ^= R((x02 + x01), 13);
                x00 ^= R((x03 + x02), 18);
                x06 ^= R((x05 + x04), 7);
                x07 ^= R((x06 + x05), 9);
                x04 ^= R((x07 + x06), 13);
                x05 ^= R((x04 + x07), 18);
                x11 ^= R((x10 + x09), 7);
                x08 ^= R((x11 + x10), 9);
                x09 ^= R((x08 + x11), 13);
                x10 ^= R((x09 + x08), 18);
                x12 ^= R((x15 + x14), 7);
                x13 ^= R((x12 + x15), 9);
                x14 ^= R((x13 + x12), 13);
                x15 ^= R((x14 + x13), 18);
            }

            x[0] = x00 + input[0];
            x[1] = x01 + input[1];
            x[2] = x02 + input[2];
            x[3] = x03 + input[3];
            x[4] = x04 + input[4];
            x[5] = x05 + input[5];
            x[6] = x06 + input[6];
            x[7] = x07 + input[7];
            x[8] = x08 + input[8];
            x[9] = x09 + input[9];
            x[10] = x10 + input[10];
            x[11] = x11 + input[11];
            x[12] = x12 + input[12];
            x[13] = x13 + input[13];
            x[14] = x14 + input[14];
            x[15] = x15 + input[15];
        }


        internal static byte[] UInt32_To_LE(uint n)
        {
            byte[] bs = new byte[4];
            UInt32_To_LE(n, bs, 0);
            return bs;
        }

        internal static void UInt32_To_LE(uint n, byte[] bs)
        {
            bs[0] = (byte)(n);
            bs[1] = (byte)(n >> 8);
            bs[2] = (byte)(n >> 16);
            bs[3] = (byte)(n >> 24);
        }

        internal static void UInt32_To_LE(uint n, byte[] bs, int off)
        {
            bs[off] = (byte)(n);
            bs[off + 1] = (byte)(n >> 8);
            bs[off + 2] = (byte)(n >> 16);
            bs[off + 3] = (byte)(n >> 24);
        }

        internal static byte[] UInt32_To_LE(uint[] ns)
        {
            byte[] bs = new byte[4 * ns.Length];
            UInt32_To_LE(ns, bs, 0);
            return bs;
        }

        internal static void UInt32_To_LE(uint[] ns, byte[] bs, int off)
        {
            for (int i = 0; i < ns.Length; ++i)
            {
                UInt32_To_LE(ns[i], bs, off);
                off += 4;
            }
        }

        internal static uint LE_To_UInt32(byte[] bs)
        {
            return (uint)bs[0]
                   | (uint)bs[1] << 8
                   | (uint)bs[2] << 16
                   | (uint)bs[3] << 24;
        }

        internal static uint LE_To_UInt32(byte[] bs, int off)
        {
            return (uint)bs[off]
                   | (uint)bs[off + 1] << 8
                   | (uint)bs[off + 2] << 16
                   | (uint)bs[off + 3] << 24;
        }

        internal static void LE_To_UInt32(byte[] bs, int off, uint[] ns)
        {
            for (int i = 0; i < ns.Length; ++i)
            {
                ns[i] = LE_To_UInt32(bs, off);
                off += 4;
            }
        }

        internal static void LE_To_UInt32(byte[] bs, int bOff, uint[] ns, int nOff, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                ns[nOff + i] = LE_To_UInt32(bs, bOff);
                bOff += 4;
            }
        }

        internal static uint[] LE_To_UInt32(byte[] bs, int off, int count)
        {
            uint[] ns = new uint[count];
            for (int i = 0; i < ns.Length; ++i)
            {
                ns[i] = LE_To_UInt32(bs, off);
                off += 4;
            }
            return ns;
        }

        /// <summary>Generate a key using the scrypt key derivation function.</summary>
        /// <param name="pBytes">the bytes of the pass phrase.</param>
        /// <param name="s">the salt to use for this invocation.</param>
        /// <param name="n">CPU/Memory cost parameter. Must be larger than 1, a power of 2 and less than
        ///     <code>2^(128 * r / 8)</code>.</param>
        /// <param name="r">the block size, must be >= 1.</param>
        /// <param name="p">Parallelization parameter. Must be a positive integer less than or equal to
        ///     <code>Int32.MaxValue / (128 * r * 8)</code>.</param>
        /// <param name="dkLen">the length of the key to generate.</param>
        /// <returns>the generated key.</returns>
        public static byte[] Generate(byte[] pBytes, byte[] s, int n, int r, int p, int dkLen)
        {
            if (pBytes == null)
                throw new ArgumentNullException(nameof(pBytes));
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (n <= 1 || !IsPowerOf2(n))
                throw new ArgumentException("Cost parameter N must be > 1 and a power of 2.");
            // Only value of r that cost (as an int) could be exceeded for is 1
            if (r == 1 && n >= 65536)
                
                if (r < 1)
                    throw new ArgumentException("Block size r must be >= 1.");
            int maxParallel = Int32.MaxValue / (128 * r * 8);
            if (p < 1 || p > maxParallel)
            {
                throw new ArgumentException("Parallelisation parameter p must be >= 1 and <= " + maxParallel
                                                                                               + " (based on block size r of " + r + ")");
            }
            if (dkLen < 1)
                throw new ArgumentException("Generated key length dkLen must be >= 1.");

            return MFcrypt(pBytes, s, n, r, p, dkLen);
        }

        private static byte[] MFcrypt(byte[] P, byte[] s, int n, int r, int p, int dkLen)
        {
            int mfLenBytes = r * 128;
            byte[] bytes = SingleIterationPbkdf2(P, s, p * mfLenBytes);

            uint[] b = null;

            try
            {
                int bLen = bytes.Length >> 2;
                b = new uint[bLen];

                LE_To_UInt32(bytes, 0, b);

                int mfLenWords = mfLenBytes >> 2;
                for (int bOff = 0; bOff < bLen; bOff += mfLenWords)
                {
                    // TODO These can be done in parallel threads
                    SMix(b, bOff, n, r);
                }

                UInt32_To_LE(b, bytes, 0);

                return SingleIterationPbkdf2(P, bytes, dkLen);
            }
            finally
            {
                ClearAll(bytes, b);
            }
        }

        private static byte[] SingleIterationPbkdf2(byte[] p, byte[] s, int dkLen)
        {
            PbeParametersGenerator pGen = new Pkcs5S2ParametersGenerator(new Sha256Digest());
            pGen.Init(p, s, 1);
            KeyParameter key = (KeyParameter)pGen.GenerateDerivedMacParameters(dkLen * 8);
            return key.GetKey();
        }

        private static void SMix(uint[] b, int bOff, int n, int r)
        {
            int bCount = r * 32;

            uint[] blockX1 = new uint[16];
            uint[] blockX2 = new uint[16];
            uint[] blockY = new uint[bCount];

            uint[] x = new uint[bCount];
            uint[][] v = new uint[n][];

            try
            {
                Array.Copy(b, bOff, x, 0, bCount);

                for (int i = 0; i < n; ++i)
                {
                    v[i] = (uint[])x.Clone();
                    BlockMix(x, blockX1, blockX2, blockY, r);
                }

                uint mask = (uint)n - 1;
                for (int i = 0; i < n; ++i)
                {
                    uint j = x[bCount - 16] & mask;
                    Xor(x, v[j], 0, x);
                    BlockMix(x, blockX1, blockX2, blockY, r);
                }

                Array.Copy(x, 0, b, bOff, bCount);
            }
            finally
            {
                ClearAll(v);
                ClearAll(x, blockX1, blockX2, blockY);
            }
        }

        private static void BlockMix(uint[] b, uint[] x1, uint[] x2, uint[] y, int r)
        {
            Array.Copy(b, b.Length - 16, x1, 0, 16);

            int bOff = 0, yOff = 0, halfLen = b.Length >> 1;

            for (int i = 2 * r; i > 0; --i)
            {
                Xor(x1, b, bOff, x2);

                SalsaCore(8, x2, x1);
                Array.Copy(x1, 0, y, yOff, 16);

                yOff = halfLen + bOff - yOff;
                bOff += 16;
            }

            Array.Copy(y, 0, b, 0, y.Length);
        }

        private static void Xor(uint[] a, uint[] b, int bOff, uint[] output)
        {
            for (int i = output.Length - 1; i >= 0; --i)
            {
                output[i] = a[i] ^ b[bOff + i];
            }
        }

        private static void Clear(Array array)
        {
            if (array != null)
            {
                Array.Clear(array, 0, array.Length);
            }
        }

        private static void ClearAll(params Array[] arrays)
        {
            foreach (Array array in arrays)
            {
                Clear(array);
            }
        }

        // note: we know X is non-zero
        private static bool IsPowerOf2(int x)
        {
            Debug.Assert(x != 0);

            return (x & (x - 1)) == 0;
        }
    }
}