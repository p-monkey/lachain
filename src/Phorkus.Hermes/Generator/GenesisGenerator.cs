﻿using System;
using Org.BouncyCastle.Math;
using Phorkus.Hermes.Math;

namespace Phorkus.Hermes.Generator
{
    public static class GenesisGenerator
    {
        /** Generates the parameters for n parties, t of which can be corrupted without breaking the security.
         * @param k the bitlength of p and q, such that N has a security of 2k
         * @param n The number of parties
         * @param t The maximum number of parties an adversary can corrupt without breaking the security of the protocol. Must be < <code>n/2</code>
         * @param random a randomness generator
         * @return the protocol parameters structure
         */
        public static GenesisPrivateParams GeneratePrivate(int k, int n, int t, Random random)
        {
            if (k < 16)
                throw new ArgumentException("k should be at least 16");
            if (n < 3)
                throw new ArgumentException("n should be at least 3");
            if (t > n / 2)
                throw new ArgumentException("t should be smaller than n/2");
            if (random == null)
                throw new ArgumentException("random cannot be null");

            var fact = k < 512 ? 4 : 1;
            BigInteger np = BigInteger.ValueOf(n);
            BigInteger three = BigInteger.ValueOf(3);
            BigInteger
                maxN = BigInteger.ValueOf(2)
                    .Pow(fact * k - 1); //TODO: the formula for min Pp given in the paper does not work. Add a factor on the bitlen is a quick, dirty fix
            BigInteger minPp = np.Multiply(three.Multiply(maxN)).Pow(2);
            BigInteger maxPp = BigInteger.ValueOf(2).Pow(minPp.BitLength + 1);

            BigInteger Pp = IntegersUtils.PickPrimeInRange(minPp, maxPp, random);
            return new GenesisPrivateParams(Pp, t, k, 1000, n);
        }
    }
}