﻿using System;

namespace Lachain.Crypto.MCL.BLS12_381
{
    public static class Mcl
    {
        private static bool _initCalled;

        public static void Init()
        {
            if (_initCalled) return; // TODO: make singleton or whatsoever
            const int curveBls12381 = 5;
            const int compileTimeVar = 46;
            var error = MclImports.mclBn_init(curveBls12381, compileTimeVar);
            if (error != 0)
            {
                throw new InvalidOperationException("mclBn_init returned error " + error);
            }

            _initCalled = true;
        }

        public static G2 LagrangeInterpolateG2(Fr[] xs, G2[] ys)
        {
            if (xs.Length != ys.Length) throw new ArgumentException("arrays are unequal length");
            var res = new G2();
            if (MclImports.mclBn_G2LagrangeInterpolation(ref res, xs, ys, xs.Length) != 0)
                throw new Exception("Lagrange interpolation failed");
            return res;
        }

        public static G1 LagrangeInterpolateG1(Fr[] xs, G1[] ys)
        {
            if (xs.Length != ys.Length) throw new ArgumentException("arrays are unequal length");
            var res = new G1();
            if (MclImports.mclBn_G1LagrangeInterpolation(ref res, xs, ys, xs.Length) != 0)
                throw new Exception("Lagrange interpolation failed");
            return res;
        }

        public static Fr LagrangeInterpolateFr(Fr[] xs, Fr[] ys)
        {
            if (xs.Length != ys.Length) throw new ArgumentException("arrays are unequal length");
            var res = new Fr();
            if (MclImports.mclBn_FrLagrangeInterpolation(ref res, xs, ys, xs.Length) != 0)
                throw new Exception("Lagrange interpolation failed");
            return res;
        }

        public static GT Pairing(G1 x, G2 y)
        {
            var res = new GT();
            MclImports.mclBn_pairing(ref res, ref x, ref y);
            return res;
        }

        public static Fr GetValue(Fr[] poly, Fr at)
        {
            var res = Fr.Zero;
            if (MclImports.mclBn_FrEvaluatePolynomial(ref res, poly, poly.Length, ref at) != 0)
                throw new Exception("Polynomial evaluation failed");
            return res;
        }

        public static G1 GetValue(G1[] poly, Fr at)
        {
            var res = G1.Zero;
            if (MclImports.mclBn_G1EvaluatePolynomial(ref res, poly, poly.Length, ref at) != 0)
                throw new Exception("Polynomial evaluation failed");
            return res;
        }

        public static G2 GetValue(G2[] poly, Fr at)
        {
            var res = G2.Zero;
            if (MclImports.mclBn_G2EvaluatePolynomial(ref res, poly, poly.Length, ref at) != 0)
                throw new Exception("Polynomial evaluation failed");
            return res;
        }

        public static Fr[] Powers(Fr x, int n)
        {
            var result = new Fr[n];
            result[0] = Fr.One;
            for (var i = 1; i < n; ++i) result[i] = result[i - 1] * x;
            return result;
        }
    }
}