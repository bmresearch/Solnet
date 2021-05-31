using System.Collections.Generic;

namespace Solnet.Rpc.Utilities
{
    public static class Ed25519Extensions
    {
        private static long[] Gf1 = new long[16] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

        private static readonly long[] D =
        {
            0x78a3, 0x1359, 0x4dca, 0x75eb,
            0xd8ab, 0x4141, 0x0a4d, 0x0070,
            0xe898, 0x7779, 0x4079, 0x8cc7,
            0xfe73, 0x2b6f, 0x6cee, 0x5203
        };

        private static readonly long[] I =
        {
            0xa0b0, 0x4a0e, 0x1b27, 0xc4ee,
            0xe478, 0xad2f, 0x1806, 0x2f43,
            0xd7a7, 0x3dfb, 0x0099, 0x2b4d,
            0xdf0b, 0x4fc1, 0x2480, 0x2b83
        };

        private static void Unpack25519(IList<long> o, IReadOnlyList<byte> n)
        {
            int i;
            for (i = 0; i < 16; i++) o[i] = (n[2 * i] & 0xff) + (long) ((n[2 * i + 1] << 8) & 0xffff);
            o[15] &= 0x7fff;
        }

        private static void Set25519(IList<long> r, IReadOnlyList<long> a)
        {
            int i;
            for (i = 0; i < 16; i++) r[i] = a[i];
        }

        private static void M(IList<long> o, IReadOnlyList<long> a, IReadOnlyList<long> b)
        {
            M(o, 0, a, 0, b, 0);
        }

        private static void M(IList<long> o, int oOff, IReadOnlyList<long> a, int aOff, IReadOnlyList<long> b, int bOff)
        {
            long t0 = 0,
                t1 = 0,
                t2 = 0,
                t3 = 0,
                t4 = 0,
                t5 = 0,
                t6 = 0,
                t7 = 0,
                t8 = 0,
                t9 = 0,
                t10 = 0,
                t11 = 0,
                t12 = 0,
                t13 = 0,
                t14 = 0,
                t15 = 0,
                t16 = 0,
                t17 = 0,
                t18 = 0,
                t19 = 0,
                t20 = 0,
                t21 = 0,
                t22 = 0,
                t23 = 0,
                t24 = 0,
                t25 = 0,
                t26 = 0,
                t27 = 0,
                t28 = 0,
                t29 = 0,
                t30 = 0,
                b0 = b[0 + bOff],
                b1 = b[1 + bOff],
                b2 = b[2 + bOff],
                b3 = b[3 + bOff],
                b4 = b[4 + bOff],
                b5 = b[5 + bOff],
                b6 = b[6 + bOff],
                b7 = b[7 + bOff],
                b8 = b[8 + bOff],
                b9 = b[9 + bOff],
                b10 = b[10 + bOff],
                b11 = b[11 + bOff],
                b12 = b[12 + bOff],
                b13 = b[13 + bOff],
                b14 = b[14 + bOff],
                b15 = b[15 + bOff];

            var v = a[0 + aOff];
            t0 += v * b0;
            t1 += v * b1;
            t2 += v * b2;
            t3 += v * b3;
            t4 += v * b4;
            t5 += v * b5;
            t6 += v * b6;
            t7 += v * b7;
            t8 += v * b8;
            t9 += v * b9;
            t10 += v * b10;
            t11 += v * b11;
            t12 += v * b12;
            t13 += v * b13;
            t14 += v * b14;
            t15 += v * b15;
            v = a[1 + aOff];
            t1 += v * b0;
            t2 += v * b1;
            t3 += v * b2;
            t4 += v * b3;
            t5 += v * b4;
            t6 += v * b5;
            t7 += v * b6;
            t8 += v * b7;
            t9 += v * b8;
            t10 += v * b9;
            t11 += v * b10;
            t12 += v * b11;
            t13 += v * b12;
            t14 += v * b13;
            t15 += v * b14;
            t16 += v * b15;
            v = a[2 + aOff];
            t2 += v * b0;
            t3 += v * b1;
            t4 += v * b2;
            t5 += v * b3;
            t6 += v * b4;
            t7 += v * b5;
            t8 += v * b6;
            t9 += v * b7;
            t10 += v * b8;
            t11 += v * b9;
            t12 += v * b10;
            t13 += v * b11;
            t14 += v * b12;
            t15 += v * b13;
            t16 += v * b14;
            t17 += v * b15;
            v = a[3 + aOff];
            t3 += v * b0;
            t4 += v * b1;
            t5 += v * b2;
            t6 += v * b3;
            t7 += v * b4;
            t8 += v * b5;
            t9 += v * b6;
            t10 += v * b7;
            t11 += v * b8;
            t12 += v * b9;
            t13 += v * b10;
            t14 += v * b11;
            t15 += v * b12;
            t16 += v * b13;
            t17 += v * b14;
            t18 += v * b15;
            v = a[4 + aOff];
            t4 += v * b0;
            t5 += v * b1;
            t6 += v * b2;
            t7 += v * b3;
            t8 += v * b4;
            t9 += v * b5;
            t10 += v * b6;
            t11 += v * b7;
            t12 += v * b8;
            t13 += v * b9;
            t14 += v * b10;
            t15 += v * b11;
            t16 += v * b12;
            t17 += v * b13;
            t18 += v * b14;
            t19 += v * b15;
            v = a[5 + aOff];
            t5 += v * b0;
            t6 += v * b1;
            t7 += v * b2;
            t8 += v * b3;
            t9 += v * b4;
            t10 += v * b5;
            t11 += v * b6;
            t12 += v * b7;
            t13 += v * b8;
            t14 += v * b9;
            t15 += v * b10;
            t16 += v * b11;
            t17 += v * b12;
            t18 += v * b13;
            t19 += v * b14;
            t20 += v * b15;
            v = a[6 + aOff];
            t6 += v * b0;
            t7 += v * b1;
            t8 += v * b2;
            t9 += v * b3;
            t10 += v * b4;
            t11 += v * b5;
            t12 += v * b6;
            t13 += v * b7;
            t14 += v * b8;
            t15 += v * b9;
            t16 += v * b10;
            t17 += v * b11;
            t18 += v * b12;
            t19 += v * b13;
            t20 += v * b14;
            t21 += v * b15;
            v = a[7 + aOff];
            t7 += v * b0;
            t8 += v * b1;
            t9 += v * b2;
            t10 += v * b3;
            t11 += v * b4;
            t12 += v * b5;
            t13 += v * b6;
            t14 += v * b7;
            t15 += v * b8;
            t16 += v * b9;
            t17 += v * b10;
            t18 += v * b11;
            t19 += v * b12;
            t20 += v * b13;
            t21 += v * b14;
            t22 += v * b15;
            v = a[8 + aOff];
            t8 += v * b0;
            t9 += v * b1;
            t10 += v * b2;
            t11 += v * b3;
            t12 += v * b4;
            t13 += v * b5;
            t14 += v * b6;
            t15 += v * b7;
            t16 += v * b8;
            t17 += v * b9;
            t18 += v * b10;
            t19 += v * b11;
            t20 += v * b12;
            t21 += v * b13;
            t22 += v * b14;
            t23 += v * b15;
            v = a[9 + aOff];
            t9 += v * b0;
            t10 += v * b1;
            t11 += v * b2;
            t12 += v * b3;
            t13 += v * b4;
            t14 += v * b5;
            t15 += v * b6;
            t16 += v * b7;
            t17 += v * b8;
            t18 += v * b9;
            t19 += v * b10;
            t20 += v * b11;
            t21 += v * b12;
            t22 += v * b13;
            t23 += v * b14;
            t24 += v * b15;
            v = a[10 + aOff];
            t10 += v * b0;
            t11 += v * b1;
            t12 += v * b2;
            t13 += v * b3;
            t14 += v * b4;
            t15 += v * b5;
            t16 += v * b6;
            t17 += v * b7;
            t18 += v * b8;
            t19 += v * b9;
            t20 += v * b10;
            t21 += v * b11;
            t22 += v * b12;
            t23 += v * b13;
            t24 += v * b14;
            t25 += v * b15;
            v = a[11 + aOff];
            t11 += v * b0;
            t12 += v * b1;
            t13 += v * b2;
            t14 += v * b3;
            t15 += v * b4;
            t16 += v * b5;
            t17 += v * b6;
            t18 += v * b7;
            t19 += v * b8;
            t20 += v * b9;
            t21 += v * b10;
            t22 += v * b11;
            t23 += v * b12;
            t24 += v * b13;
            t25 += v * b14;
            t26 += v * b15;
            v = a[12 + aOff];
            t12 += v * b0;
            t13 += v * b1;
            t14 += v * b2;
            t15 += v * b3;
            t16 += v * b4;
            t17 += v * b5;
            t18 += v * b6;
            t19 += v * b7;
            t20 += v * b8;
            t21 += v * b9;
            t22 += v * b10;
            t23 += v * b11;
            t24 += v * b12;
            t25 += v * b13;
            t26 += v * b14;
            t27 += v * b15;
            v = a[13 + aOff];
            t13 += v * b0;
            t14 += v * b1;
            t15 += v * b2;
            t16 += v * b3;
            t17 += v * b4;
            t18 += v * b5;
            t19 += v * b6;
            t20 += v * b7;
            t21 += v * b8;
            t22 += v * b9;
            t23 += v * b10;
            t24 += v * b11;
            t25 += v * b12;
            t26 += v * b13;
            t27 += v * b14;
            t28 += v * b15;
            v = a[14 + aOff];
            t14 += v * b0;
            t15 += v * b1;
            t16 += v * b2;
            t17 += v * b3;
            t18 += v * b4;
            t19 += v * b5;
            t20 += v * b6;
            t21 += v * b7;
            t22 += v * b8;
            t23 += v * b9;
            t24 += v * b10;
            t25 += v * b11;
            t26 += v * b12;
            t27 += v * b13;
            t28 += v * b14;
            t29 += v * b15;
            v = a[15 + aOff];
            t15 += v * b0;
            t16 += v * b1;
            t17 += v * b2;
            t18 += v * b3;
            t19 += v * b4;
            t20 += v * b5;
            t21 += v * b6;
            t22 += v * b7;
            t23 += v * b8;
            t24 += v * b9;
            t25 += v * b10;
            t26 += v * b11;
            t27 += v * b12;
            t28 += v * b13;
            t29 += v * b14;
            t30 += v * b15;

            t0 += 38 * t16;
            t1 += 38 * t17;
            t2 += 38 * t18;
            t3 += 38 * t19;
            t4 += 38 * t20;
            t5 += 38 * t21;
            t6 += 38 * t22;
            t7 += 38 * t23;
            t8 += 38 * t24;
            t9 += 38 * t25;
            t10 += 38 * t26;
            t11 += 38 * t27;
            t12 += 38 * t28;
            t13 += 38 * t29;
            t14 += 38 * t30;

            long c = 1;
            v = t0 + c + 65535;
            c = v >> 16;
            t0 = v - c * 65536;
            v = t1 + c + 65535;
            c = v >> 16;
            t1 = v - c * 65536;
            v = t2 + c + 65535;
            c = v >> 16;
            t2 = v - c * 65536;
            v = t3 + c + 65535;
            c = v >> 16;
            t3 = v - c * 65536;
            v = t4 + c + 65535;
            c = v >> 16;
            t4 = v - c * 65536;
            v = t5 + c + 65535;
            c = v >> 16;
            t5 = v - c * 65536;
            v = t6 + c + 65535;
            c = v >> 16;
            t6 = v - c * 65536;
            v = t7 + c + 65535;
            c = v >> 16;
            t7 = v - c * 65536;
            v = t8 + c + 65535;
            c = v >> 16;
            t8 = v - c * 65536;
            v = t9 + c + 65535;
            c = v >> 16;
            t9 = v - c * 65536;
            v = t10 + c + 65535;
            c = v >> 16;
            t10 = v - c * 65536;
            v = t11 + c + 65535;
            c = v >> 16;
            t11 = v - c * 65536;
            v = t12 + c + 65535;
            c = v >> 16;
            t12 = v - c * 65536;
            v = t13 + c + 65535;
            c = v >> 16;
            t13 = v - c * 65536;
            v = t14 + c + 65535;
            c = v >> 16;
            t14 = v - c * 65536;
            v = t15 + c + 65535;
            c = v >> 16;
            t15 = v - c * 65536;
            t0 += c - 1 + 37 * (c - 1);

            c = 1;
            v = t0 + c + 65535;
            c = v >> 16;
            t0 = v - c * 65536;
            v = t1 + c + 65535;
            c = v >> 16;
            t1 = v - c * 65536;
            v = t2 + c + 65535;
            c = v >> 16;
            t2 = v - c * 65536;
            v = t3 + c + 65535;
            c = v >> 16;
            t3 = v - c * 65536;
            v = t4 + c + 65535;
            c = v >> 16;
            t4 = v - c * 65536;
            v = t5 + c + 65535;
            c = v >> 16;
            t5 = v - c * 65536;
            v = t6 + c + 65535;
            c = v >> 16;
            t6 = v - c * 65536;
            v = t7 + c + 65535;
            c = v >> 16;
            t7 = v - c * 65536;
            v = t8 + c + 65535;
            c = v >> 16;
            t8 = v - c * 65536;
            v = t9 + c + 65535;
            c = v >> 16;
            t9 = v - c * 65536;
            v = t10 + c + 65535;
            c = v >> 16;
            t10 = v - c * 65536;
            v = t11 + c + 65535;
            c = v >> 16;
            t11 = v - c * 65536;
            v = t12 + c + 65535;
            c = v >> 16;
            t12 = v - c * 65536;
            v = t13 + c + 65535;
            c = v >> 16;
            t13 = v - c * 65536;
            v = t14 + c + 65535;
            c = v >> 16;
            t14 = v - c * 65536;
            v = t15 + c + 65535;
            c = v >> 16;
            t15 = v - c * 65536;
            t0 += c - 1 + 37 * (c - 1);

            o[0 + oOff] = t0;
            o[1 + oOff] = t1;
            o[2 + oOff] = t2;
            o[3 + oOff] = t3;
            o[4 + oOff] = t4;
            o[5 + oOff] = t5;
            o[6 + oOff] = t6;
            o[7 + oOff] = t7;
            o[8 + oOff] = t8;
            o[9 + oOff] = t9;
            o[10 + oOff] = t10;
            o[11 + oOff] = t11;
            o[12 + oOff] = t12;
            o[13 + oOff] = t13;
            o[14 + oOff] = t14;
            o[15 + oOff] = t15;
        }

        private static void S(IList<long> o, IReadOnlyList<long> a)
        {
            S(o, 0, a, 0);
        }

        private static void S(IList<long> o, int oOff, IReadOnlyList<long> a, int aOff)
        {
            M(o, oOff, a, aOff, a, aOff);
        }

        private static void Z(IList<long> o, IReadOnlyList<long> a, IReadOnlyList<long> b)
        {
            Z(o, 0, a, 0, b, 0);
        }

        private static void Z(IList<long> o, int oOff, IReadOnlyList<long> a, int aOff, IReadOnlyList<long> b, int bOff)
        {
            int i;
            for (i = 0; i < 16; i++) o[i + oOff] = a[i + aOff] - b[i + bOff];
        }

        private static void A(IList<long> o, IReadOnlyList<long> a, IReadOnlyList<long> b)
        {
            A(o, 0, a, 0, b, 0);
        }

        private static void A(IList<long> o, int oOff, IReadOnlyList<long> a, int aOff, IReadOnlyList<long> b, int bOff)
        {
            int i;
            for (i = 0; i < 16; i++) o[i + oOff] = a[i + aOff] + b[i + bOff];
        }

        private static void Car25519(long[] o)
        {
            int i;
            long v, c = 1;
            for (i = 0; i < 16; i++)
            {
                v = o[i] + c + 65535;
                c = v >> 16;
                o[i] = v - c * 65536;
            }

            o[0] += c - 1 + 37 * (c - 1);
        }

        private static void Sel25519(IList<long> p, IList<long> q, int b)
        {
            Sel25519(p, 0, q, 0, b);
        }

        private static void Sel25519(IList<long> p, int pOff, IList<long> q, int qOff, int b)
        {
            long t, c = ~(b - 1);
            for (var i = 0; i < 16; i++)
            {
                t = c & (p[i + pOff] ^ q[i + qOff]);
                p[i + pOff] ^= t;
                q[i + qOff] ^= t;
            }
        }

        private static void Pow2523(IList<long> o, IReadOnlyList<long> i)
        {
            var c = new long[16];
            int a;

            for (a = 0; a < 16; a++) c[a] = i[a];

            for (a = 250; a >= 0; a--)
            {
                S(c, 0, c, 0);
                if (a != 1) M(c, 0, c, 0, i, 0);
            }

            for (a = 0; a < 16; a++) o[a] = c[a];
        }

        private static int Vn(IReadOnlyList<byte> x, int xOff, IReadOnlyList<byte> y, int yOff, int n)
        {
            int i, d = 0;
            for (i = 0; i < n; i++) d |= (x[i + xOff] ^ y[i + yOff]) & 0xff;
            return (1 & (int) ((uint) (d - 1) >> 8)) - 1;
        }

        private static int CryptoVerify32(byte[] x, int xOff, byte[] y, int yOff)
        {
            return Vn(x, xOff, y, yOff, 32);
        }

        public static int CryptoVerify32(byte[] x, byte[] y)
        {
            return CryptoVerify32(x, 0, y, 0);
        }

        private static void Pack25519(IList<byte> o, IReadOnlyList<long> n, int nOff)
        {
            int i, j, b;
            long[] m = new long[16], t = new long[16];
            for (i = 0; i < 16; i++) t[i] = n[i + nOff];
            Car25519(t);
            Car25519(t);
            Car25519(t);
            for (j = 0; j < 2; j++)
            {
                m[0] = t[0] - 0xffed;
                for (i = 1; i < 15; i++)
                {
                    m[i] = t[i] - 0xffff - ((m[i - 1] >> 16) & 1);
                    m[i - 1] &= 0xffff;
                }

                m[15] = t[15] - 0x7fff - ((m[14] >> 16) & 1);
                b = (int) ((m[15] >> 16) & 1);
                m[14] &= 0xffff;
                Sel25519(t, 0, m, 0, 1 - b);
            }

            for (i = 0; i < 16; i++)
            {
                o[2 * i] = (byte) (t[i] & 0xff);
                o[2 * i + 1] = (byte) (t[i] >> 8);
            }
        }

        private static int Neq25519(IReadOnlyList<long> a, IReadOnlyList<long> b)
        {
            return Neq25519(a, 0, b, 0);
        }

        private static int Neq25519(IReadOnlyList<long> a, int aOff, IReadOnlyList<long> b, int bOff)
        {
            byte[] c = new byte[32], d = new byte[32];
            Pack25519(c, a, aOff);
            Pack25519(d, b, bOff);
            return CryptoVerify32(c, 0, d, 0);
        }

        public static int IsOnCurve(this byte[] p)
        {
            long[][] r =
            {
                new long [16], new long [16], new long [16], new long [16]
            };

            var t = new long [16];
            var chk = new long [16];
            var num = new long [16];
            var den = new long [16];
            var den2 = new long [16];
            var den4 = new long [16];
            var den6 = new long [16];

            Set25519(r[2], Gf1);
            Unpack25519(r[1], p);
            S(num, r[1]);
            M(den, num, D);
            Z(num, num, r[2]);
            A(den, r[2], den);

            S(den2, den);
            S(den4, den2);
            M(den6, den4, den2);
            M(t, den6, num);
            M(t, t, den);

            Pow2523(t, t);
            M(t, t, num);
            M(t, t, den);
            M(t, t, den);
            M(r[0], t, den);

            S(chk, r[0]);
            M(chk, chk, den);
            if (Neq25519(chk, num) != 0) M(r[0], r[0], I);

            S(chk, r[0]);
            M(chk, chk, den);
            return Neq25519(chk, num) != 0 ? 0 : 1;
        }
    }
}