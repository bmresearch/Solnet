using System.Numerics;

namespace Solnet.Rpc.Utilities
{
    /* Ported and refactored from Java to C# by Hans Wolff, 10/10/2013
     * Released to the public domain
     * /
    /* Java code written by k3d3
     * Source: https://github.com/k3d3/ed25519-java/blob/master/ed25519.java
     * Released to the public domain
     */

    /// <summary>
    /// Helper methods for ED25519 checks
    /// Edwards-curve Digital Signature Algorithm (EdDSA)
    /// https://en.wikipedia.org/wiki/EdDSA#Ed25519
    /// </summary>
    public static class Ed25519Extensions
    {
        private static BigInteger ExpMod(BigInteger number, BigInteger exponent, BigInteger modulo)
        {
            if (exponent.Equals(BigInteger.Zero))
            {
                return BigInteger.One;
            }
            BigInteger t = BigInteger.Pow(ExpMod(number, exponent / Two, modulo), 2).Mod(modulo);
            if (!exponent.IsEven)
            {
                t *= number;
                t = t.Mod(modulo);
            }
            return t;
        }

        private static BigInteger Inv(BigInteger x)
        {
            return ExpMod(x, Qm2, Q);
        }

        private static BigInteger RecoverX(BigInteger y)
        {
            BigInteger y2 = y * y;
            BigInteger xx = (y2 - 1) * Inv(D * y2 + 1);
            BigInteger x = ExpMod(xx, Qp3 / Eight, Q);
            if (!(x * x - xx).Mod(Q).Equals(BigInteger.Zero))
            {
                x = (x * I).Mod(Q);
            }
            if (!x.IsEven)
            {
                x = Q - x;
            }
            return x;
        }

        private static bool IsOnCurve(BigInteger x, BigInteger y)
        {
            BigInteger xx = x * x;
            BigInteger yy = y * y;
            BigInteger dxxyy = D * yy * xx;
            return (yy - xx - dxxyy - 1).Mod(Q).Equals(BigInteger.Zero);
        }

        /// <summary>
        /// Checks whether the PublicKey bytes are 'On The Curve'
        /// </summary>
        /// <param name="key">PublicKey as byte array</param>
        /// <returns></returns>
        public static bool IsOnCurve(this byte[] key)
        {
            BigInteger y = new BigInteger(key) & Un;
            BigInteger x = RecoverX(y);

            return IsOnCurve(x, y);
        }

        private static readonly BigInteger Q =
            BigInteger.Parse("57896044618658097711785492504343953926634992332820282019728792003956564819949");

        private static readonly BigInteger Qm2 =
            BigInteger.Parse("57896044618658097711785492504343953926634992332820282019728792003956564819947");

        private static readonly BigInteger Qp3 =
            BigInteger.Parse("57896044618658097711785492504343953926634992332820282019728792003956564819952");

        private static readonly BigInteger D =
            BigInteger.Parse("-4513249062541557337682894930092624173785641285191125241628941591882900924598840740");

        private static readonly BigInteger I =
            BigInteger.Parse("19681161376707505956807079304988542015446066515923890162744021073123829784752");

        private static readonly BigInteger Un =
            BigInteger.Parse("57896044618658097711785492504343953926634992332820282019728792003956564819967");

        private static readonly BigInteger Two = new(2);
        private static readonly BigInteger Eight = new(8);
    }

    internal static class BigIntegerHelpers
    {
        internal static BigInteger Mod(this BigInteger num, BigInteger modulo)
        {
            var result = num % modulo;
            return result < 0 ? result + modulo : result;
        }
    }
}