using System;

namespace Special_functions
{
    public class alglibexception : System.Exception
    {
        public string msg;
        public alglibexception(string s)
        {
            msg = s;
        }

    }

    public struct complex
    {
        public double x;
        public double y;

        public complex(double _x)
        {
            x = _x;
            y = 0;
        }
        public complex(double _x, double _y)
        {
            x = _x;
            y = _y;
        }
        public static implicit operator complex(double _x)
        {
            return new complex(_x);
        }
        public static bool operator ==(complex lhs, complex rhs)
        {
            return ((double)lhs.x == (double)rhs.x) & ((double)lhs.y == (double)rhs.y);
        }
        public static bool operator !=(complex lhs, complex rhs)
        {
            return ((double)lhs.x != (double)rhs.x) | ((double)lhs.y != (double)rhs.y);
        }
        public static complex operator +(complex lhs)
        {
            return lhs;
        }
        public static complex operator -(complex lhs)
        {
            return new complex(-lhs.x, -lhs.y);
        }
        public static complex operator +(complex lhs, complex rhs)
        {
            return new complex(lhs.x + rhs.x, lhs.y + rhs.y);
        }
        public static complex operator -(complex lhs, complex rhs)
        {
            return new complex(lhs.x - rhs.x, lhs.y - rhs.y);
        }
        public static complex operator *(complex lhs, complex rhs)
        {
            return new complex(lhs.x * rhs.x - lhs.y * rhs.y, lhs.x * rhs.y + lhs.y * rhs.x);
        }
        public static complex operator /(complex lhs, complex rhs)
        {
            complex result;
            double e;
            double f;
            if (System.Math.Abs(rhs.y) < System.Math.Abs(rhs.x))
            {
                e = rhs.y / rhs.x;
                f = rhs.x + rhs.y * e;
                result.x = (lhs.x + lhs.y * e) / f;
                result.y = (lhs.y - lhs.x * e) / f;
            }
            else
            {
                e = rhs.x / rhs.y;
                f = rhs.y + rhs.x * e;
                result.x = (lhs.y + lhs.x * e) / f;
                result.y = (-lhs.x + lhs.y * e) / f;
            }
            return result;
        }
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj is byte)
                return Equals(new complex((byte)obj));
            if (obj is sbyte)
                return Equals(new complex((sbyte)obj));
            if (obj is short)
                return Equals(new complex((short)obj));
            if (obj is ushort)
                return Equals(new complex((ushort)obj));
            if (obj is int)
                return Equals(new complex((int)obj));
            if (obj is uint)
                return Equals(new complex((uint)obj));
            if (obj is long)
                return Equals(new complex((long)obj));
            if (obj is ulong)
                return Equals(new complex((ulong)obj));
            if (obj is float)
                return Equals(new complex((float)obj));
            if (obj is double)
                return Equals(new complex((double)obj));
            if (obj is decimal)
                return Equals(new complex((double)(decimal)obj));
            return base.Equals(obj);
        }
    }

    public class math
    {
        //public static System.Random RndObject = new System.Random(System.DateTime.Now.Millisecond);
        public static System.Random rndobject = new System.Random(System.DateTime.Now.Millisecond + 1000 * System.DateTime.Now.Second + 60 * 1000 * System.DateTime.Now.Minute);

        public const double machineepsilon = 5E-16;
        public const double maxrealnumber = 1E300;
        public const double minrealnumber = 1E-300;

        public static bool isfinite(double d)
        {
            return !System.Double.IsNaN(d) && !System.Double.IsInfinity(d);
        }

        public static double randomreal()
        {
            double r = 0;
            lock (rndobject) { r = rndobject.NextDouble(); }
            return r;
        }
        public static int randominteger(int N)
        {
            int r = 0;
            lock (rndobject) { r = rndobject.Next(N); }
            return r;
        }
        public static double sqr(double X)
        {
            return X * X;
        }
        public static double abscomplex(complex z)
        {
            double w;
            double xabs;
            double yabs;
            double v;

            xabs = System.Math.Abs(z.x);
            yabs = System.Math.Abs(z.y);
            w = xabs > yabs ? xabs : yabs;
            v = xabs < yabs ? xabs : yabs;
            if (v == 0)
                return w;
            else
            {
                double t = v / w;
                return w * System.Math.Sqrt(1 + t * t);
            }
        }
        public static complex conj(complex z)
        {
            return new complex(z.x, -z.y);
        }
        public static complex csqr(complex z)
        {
            return new complex(z.x * z.x - z.y * z.y, 2 * z.x * z.y);
        }

    }

    public class gammafunc
    {
        /*************************************************************************
        Gamma function

        Input parameters:
            X   -   argument

        Domain:
            0 < X < 171.6
            -170 < X < 0, X is not an integer.

        Relative error:
         arithmetic   domain     # trials      peak         rms
            IEEE    -170,-33      20000       2.3e-15     3.3e-16
            IEEE     -33,  33     20000       9.4e-16     2.2e-16
            IEEE      33, 171.6   20000       2.3e-15     3.2e-16

        Cephes Math Library Release 2.8:  June, 2000
        Original copyright 1984, 1987, 1989, 1992, 2000 by Stephen L. Moshier
        Translated to AlgoPascal by Bochkanov Sergey (2005, 2006, 2007).
        *************************************************************************/
        public static double gammafunction(double x)
        {
            double result = 0;
            double p = 0;
            double pp = 0;
            double q = 0;
            double qq = 0;
            double z = 0;
            int i = 0;
            double sgngam = 0;

            sgngam = 1;
            q = Math.Abs(x);
            if ((double)(q) > (double)(33.0))
            {
                if ((double)(x) < (double)(0.0))
                {
                    p = (int)Math.Floor(q);
                    i = (int)Math.Round(p);
                    if (i % 2 == 0)
                    {
                        sgngam = -1;
                    }
                    z = q - p;
                    if ((double)(z) > (double)(0.5))
                    {
                        p = p + 1;
                        z = q - p;
                    }
                    z = q * Math.Sin(Math.PI * z);
                    z = Math.Abs(z);
                    z = Math.PI / (z * gammastirf(q));
                }
                else
                {
                    z = gammastirf(x);
                }
                result = sgngam * z;
                return result;
            }
            z = 1;
            while ((double)(x) >= (double)(3))
            {
                x = x - 1;
                z = z * x;
            }
            while ((double)(x) < (double)(0))
            {
                if ((double)(x) > (double)(-0.000000001))
                {
                    result = z / ((1 + 0.5772156649015329 * x) * x);
                    return result;
                }
                z = z / x;
                x = x + 1;
            }
            while ((double)(x) < (double)(2))
            {
                if ((double)(x) < (double)(0.000000001))
                {
                    result = z / ((1 + 0.5772156649015329 * x) * x);
                    return result;
                }
                z = z / x;
                x = x + 1.0;
            }
            if ((double)(x) == (double)(2))
            {
                result = z;
                return result;
            }
            x = x - 2.0;
            pp = 1.60119522476751861407E-4;
            pp = 1.19135147006586384913E-3 + x * pp;
            pp = 1.04213797561761569935E-2 + x * pp;
            pp = 4.76367800457137231464E-2 + x * pp;
            pp = 2.07448227648435975150E-1 + x * pp;
            pp = 4.94214826801497100753E-1 + x * pp;
            pp = 9.99999999999999996796E-1 + x * pp;
            qq = -2.31581873324120129819E-5;
            qq = 5.39605580493303397842E-4 + x * qq;
            qq = -4.45641913851797240494E-3 + x * qq;
            qq = 1.18139785222060435552E-2 + x * qq;
            qq = 3.58236398605498653373E-2 + x * qq;
            qq = -2.34591795718243348568E-1 + x * qq;
            qq = 7.14304917030273074085E-2 + x * qq;
            qq = 1.00000000000000000320 + x * qq;
            result = z * pp / qq;
            return result;
        }


        /*************************************************************************
        Natural logarithm of gamma function

        Input parameters:
            X       -   argument

        Result:
            logarithm of the absolute value of the Gamma(X).

        Output parameters:
            SgnGam  -   sign(Gamma(X))

        Domain:
            0 < X < 2.55e305
            -2.55e305 < X < 0, X is not an integer.

        ACCURACY:
        arithmetic      domain        # trials     peak         rms
           IEEE    0, 3                 28000     5.4e-16     1.1e-16
           IEEE    2.718, 2.556e305     40000     3.5e-16     8.3e-17
        The error criterion was relative when the function magnitude
        was greater than one but absolute when it was less than one.

        The following test used the relative error criterion, though
        at certain points the relative error could be much higher than
        indicated.
           IEEE    -200, -4             10000     4.8e-16     1.3e-16

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 1989, 1992, 2000 by Stephen L. Moshier
        Translated to AlgoPascal by Bochkanov Sergey (2005, 2006, 2007).
        *************************************************************************/
        public static double lngamma(double x,
            ref double sgngam)
        {
            double result = 0;
            double a = 0;
            double b = 0;
            double c = 0;
            double p = 0;
            double q = 0;
            double u = 0;
            double w = 0;
            double z = 0;
            int i = 0;
            double logpi = 0;
            double ls2pi = 0;
            double tmp = 0;

            sgngam = 0;

            sgngam = 1;
            logpi = 1.14472988584940017414;
            ls2pi = 0.91893853320467274178;
            if ((double)(x) < (double)(-34.0))
            {
                q = -x;
                w = lngamma(q, ref tmp);
                p = (int)Math.Floor(q);
                i = (int)Math.Round(p);
                if (i % 2 == 0)
                {
                    sgngam = -1;
                }
                else
                {
                    sgngam = 1;
                }
                z = q - p;
                if ((double)(z) > (double)(0.5))
                {
                    p = p + 1;
                    z = p - q;
                }
                z = q * Math.Sin(Math.PI * z);
                result = logpi - Math.Log(z) - w;
                return result;
            }
            if ((double)(x) < (double)(13))
            {
                z = 1;
                p = 0;
                u = x;
                while ((double)(u) >= (double)(3))
                {
                    p = p - 1;
                    u = x + p;
                    z = z * u;
                }
                while ((double)(u) < (double)(2))
                {
                    z = z / u;
                    p = p + 1;
                    u = x + p;
                }
                if ((double)(z) < (double)(0))
                {
                    sgngam = -1;
                    z = -z;
                }
                else
                {
                    sgngam = 1;
                }
                if ((double)(u) == (double)(2))
                {
                    result = Math.Log(z);
                    return result;
                }
                p = p - 2;
                x = x + p;
                b = -1378.25152569120859100;
                b = -38801.6315134637840924 + x * b;
                b = -331612.992738871184744 + x * b;
                b = -1162370.97492762307383 + x * b;
                b = -1721737.00820839662146 + x * b;
                b = -853555.664245765465627 + x * b;
                c = 1;
                c = -351.815701436523470549 + x * c;
                c = -17064.2106651881159223 + x * c;
                c = -220528.590553854454839 + x * c;
                c = -1139334.44367982507207 + x * c;
                c = -2532523.07177582951285 + x * c;
                c = -2018891.41433532773231 + x * c;
                p = x * b / c;
                result = Math.Log(z) + p;
                return result;
            }
            q = (x - 0.5) * Math.Log(x) - x + ls2pi;
            if ((double)(x) > (double)(100000000))
            {
                result = q;
                return result;
            }
            p = 1 / (x * x);
            if ((double)(x) >= (double)(1000.0))
            {
                q = q + ((7.9365079365079365079365 * 0.0001 * p - 2.7777777777777777777778 * 0.001) * p + 0.0833333333333333333333) / x;
            }
            else
            {
                a = 8.11614167470508450300 * 0.0001;
                a = -(5.95061904284301438324 * 0.0001) + p * a;
                a = 7.93650340457716943945 * 0.0001 + p * a;
                a = -(2.77777777730099687205 * 0.001) + p * a;
                a = 8.33333333333331927722 * 0.01 + p * a;
                q = q + a / x;
            }
            result = q;
            return result;
        }


        private static double gammastirf(double x)
        {
            double result = 0;
            double y = 0;
            double w = 0;
            double v = 0;
            double stir = 0;

            w = 1 / x;
            stir = 7.87311395793093628397E-4;
            stir = -2.29549961613378126380E-4 + w * stir;
            stir = -2.68132617805781232825E-3 + w * stir;
            stir = 3.47222221605458667310E-3 + w * stir;
            stir = 8.33333333333482257126E-2 + w * stir;
            w = 1 + w * stir;
            y = Math.Exp(x);
            if ((double)(x) > (double)(143.01608))
            {
                v = Math.Pow(x, 0.5 * x - 0.25);
                y = v * (v / y);
            }
            else
            {
                y = Math.Pow(x, x - 0.5) / y;
            }
            result = 2.50662827463100050242 * y * w;
            return result;
        }


    }

    public class normaldistr
    {
        /*************************************************************************
        Error function

        The integral is

                                  x
                                   -
                        2         | |          2
          erf(x)  =  --------     |    exp( - t  ) dt.
                     sqrt(pi)   | |
                                 -
                                  0

        For 0 <= |x| < 1, erf(x) = x * P4(x**2)/Q5(x**2); otherwise
        erf(x) = 1 - erfc(x).


        ACCURACY:

                             Relative error:
        arithmetic   domain     # trials      peak         rms
           IEEE      0,1         30000       3.7e-16     1.0e-16

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double errorfunction(double x)
        {
            double result = 0;
            double xsq = 0;
            double s = 0;
            double p = 0;
            double q = 0;

            s = Math.Sign(x);
            x = Math.Abs(x);
            if ((double)(x) < (double)(0.5))
            {
                xsq = x * x;
                p = 0.007547728033418631287834;
                p = -0.288805137207594084924010 + xsq * p;
                p = 14.3383842191748205576712 + xsq * p;
                p = 38.0140318123903008244444 + xsq * p;
                p = 3017.82788536507577809226 + xsq * p;
                p = 7404.07142710151470082064 + xsq * p;
                p = 80437.3630960840172832162 + xsq * p;
                q = 0.0;
                q = 1.00000000000000000000000 + xsq * q;
                q = 38.0190713951939403753468 + xsq * q;
                q = 658.070155459240506326937 + xsq * q;
                q = 6379.60017324428279487120 + xsq * q;
                q = 34216.5257924628539769006 + xsq * q;
                q = 80437.3630960840172826266 + xsq * q;
                result = s * 1.1283791670955125738961589031 * x * p / q;
                return result;
            }
            if ((double)(x) >= (double)(10))
            {
                result = s;
                return result;
            }
            result = s * (1 - errorfunctionc(x));
            return result;
        }


        /*************************************************************************
        Complementary error function

         1 - erf(x) =

                                  inf.
                                    -
                         2         | |          2
          erfc(x)  =  --------     |    exp( - t  ) dt
                      sqrt(pi)   | |
                                  -
                                   x


        For small x, erfc(x) = 1 - erf(x); otherwise rational
        approximations are computed.


        ACCURACY:

                             Relative error:
        arithmetic   domain     # trials      peak         rms
           IEEE      0,26.6417   30000       5.7e-14     1.5e-14

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double errorfunctionc(double x)
        {
            double result = 0;
            double p = 0;
            double q = 0;

            if ((double)(x) < (double)(0))
            {
                result = 2 - errorfunctionc(-x);
                return result;
            }
            if ((double)(x) < (double)(0.5))
            {
                result = 1.0 - errorfunction(x);
                return result;
            }
            if ((double)(x) >= (double)(10))
            {
                result = 0;
                return result;
            }
            p = 0.0;
            p = 0.5641877825507397413087057563 + x * p;
            p = 9.675807882987265400604202961 + x * p;
            p = 77.08161730368428609781633646 + x * p;
            p = 368.5196154710010637133875746 + x * p;
            p = 1143.262070703886173606073338 + x * p;
            p = 2320.439590251635247384768711 + x * p;
            p = 2898.0293292167655611275846 + x * p;
            p = 1826.3348842295112592168999 + x * p;
            q = 1.0;
            q = 17.14980943627607849376131193 + x * q;
            q = 137.1255960500622202878443578 + x * q;
            q = 661.7361207107653469211984771 + x * q;
            q = 2094.384367789539593790281779 + x * q;
            q = 4429.612803883682726711528526 + x * q;
            q = 6089.5424232724435504633068 + x * q;
            q = 4958.82756472114071495438422 + x * q;
            q = 1826.3348842295112595576438 + x * q;
            result = Math.Exp(-math.sqr(x)) * p / q;
            return result;
        }


        /*************************************************************************
        Normal distribution function

        Returns the area under the Gaussian probability density
        function, integrated from minus infinity to x:

                                   x
                                    -
                          1        | |          2
           ndtr(x)  = ---------    |    exp( - t /2 ) dt
                      sqrt(2pi)  | |
                                  -
                                 -inf.

                    =  ( 1 + erf(z) ) / 2
                    =  erfc(z) / 2

        where z = x/sqrt(2). Computation is via the functions
        erf and erfc.


        ACCURACY:

                             Relative error:
        arithmetic   domain     # trials      peak         rms
           IEEE     -13,0        30000       3.4e-14     6.7e-15

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double normaldistribution(double x)
        {
            double result = 0;

            result = 0.5 * (errorfunction(x / 1.41421356237309504880) + 1);
            return result;
        }


        /*************************************************************************
        Inverse of the error function

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double inverf(double e)
        {
            double result = 0;

            result = invnormaldistribution(0.5 * (e + 1)) / Math.Sqrt(2);
            return result;
        }


        /*************************************************************************
        Inverse of Normal distribution function

        Returns the argument, x, for which the area under the
        Gaussian probability density function (integrated from
        minus infinity to x) is equal to y.


        For small arguments 0 < y < exp(-2), the program computes
        z = sqrt( -2.0 * log(y) );  then the approximation is
        x = z - log(z)/z  - (1/z) P(1/z) / Q(1/z).
        There are two rational functions P/Q, one for 0 < y < exp(-32)
        and the other for y up to exp(-2).  For larger arguments,
        w = y - 0.5, and  x/sqrt(2pi) = w + w**3 R(w**2)/S(w**2)).

        ACCURACY:

                             Relative error:
        arithmetic   domain        # trials      peak         rms
           IEEE     0.125, 1        20000       7.2e-16     1.3e-16
           IEEE     3e-308, 0.135   50000       4.6e-16     9.8e-17

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double invnormaldistribution(double y0)
        {
            double result = 0;
            double expm2 = 0;
            double s2pi = 0;
            double x = 0;
            double y = 0;
            double z = 0;
            double y2 = 0;
            double x0 = 0;
            double x1 = 0;
            int code = 0;
            double p0 = 0;
            double q0 = 0;
            double p1 = 0;
            double q1 = 0;
            double p2 = 0;
            double q2 = 0;

            expm2 = 0.13533528323661269189;
            s2pi = 2.50662827463100050242;
            if ((double)(y0) <= (double)(0))
            {
                result = -math.maxrealnumber;
                return result;
            }
            if ((double)(y0) >= (double)(1))
            {
                result = math.maxrealnumber;
                return result;
            }
            code = 1;
            y = y0;
            if ((double)(y) > (double)(1.0 - expm2))
            {
                y = 1.0 - y;
                code = 0;
            }
            if ((double)(y) > (double)(expm2))
            {
                y = y - 0.5;
                y2 = y * y;
                p0 = -59.9633501014107895267;
                p0 = 98.0010754185999661536 + y2 * p0;
                p0 = -56.6762857469070293439 + y2 * p0;
                p0 = 13.9312609387279679503 + y2 * p0;
                p0 = -1.23916583867381258016 + y2 * p0;
                q0 = 1;
                q0 = 1.95448858338141759834 + y2 * q0;
                q0 = 4.67627912898881538453 + y2 * q0;
                q0 = 86.3602421390890590575 + y2 * q0;
                q0 = -225.462687854119370527 + y2 * q0;
                q0 = 200.260212380060660359 + y2 * q0;
                q0 = -82.0372256168333339912 + y2 * q0;
                q0 = 15.9056225126211695515 + y2 * q0;
                q0 = -1.18331621121330003142 + y2 * q0;
                x = y + y * y2 * p0 / q0;
                x = x * s2pi;
                result = x;
                return result;
            }
            x = Math.Sqrt(-(2.0 * Math.Log(y)));
            x0 = x - Math.Log(x) / x;
            z = 1.0 / x;
            if ((double)(x) < (double)(8.0))
            {
                p1 = 4.05544892305962419923;
                p1 = 31.5251094599893866154 + z * p1;
                p1 = 57.1628192246421288162 + z * p1;
                p1 = 44.0805073893200834700 + z * p1;
                p1 = 14.6849561928858024014 + z * p1;
                p1 = 2.18663306850790267539 + z * p1;
                p1 = -(1.40256079171354495875 * 0.1) + z * p1;
                p1 = -(3.50424626827848203418 * 0.01) + z * p1;
                p1 = -(8.57456785154685413611 * 0.0001) + z * p1;
                q1 = 1;
                q1 = 15.7799883256466749731 + z * q1;
                q1 = 45.3907635128879210584 + z * q1;
                q1 = 41.3172038254672030440 + z * q1;
                q1 = 15.0425385692907503408 + z * q1;
                q1 = 2.50464946208309415979 + z * q1;
                q1 = -(1.42182922854787788574 * 0.1) + z * q1;
                q1 = -(3.80806407691578277194 * 0.01) + z * q1;
                q1 = -(9.33259480895457427372 * 0.0001) + z * q1;
                x1 = z * p1 / q1;
            }
            else
            {
                p2 = 3.23774891776946035970;
                p2 = 6.91522889068984211695 + z * p2;
                p2 = 3.93881025292474443415 + z * p2;
                p2 = 1.33303460815807542389 + z * p2;
                p2 = 2.01485389549179081538 * 0.1 + z * p2;
                p2 = 1.23716634817820021358 * 0.01 + z * p2;
                p2 = 3.01581553508235416007 * 0.0001 + z * p2;
                p2 = 2.65806974686737550832 * 0.000001 + z * p2;
                p2 = 6.23974539184983293730 * 0.000000001 + z * p2;
                q2 = 1;
                q2 = 6.02427039364742014255 + z * q2;
                q2 = 3.67983563856160859403 + z * q2;
                q2 = 1.37702099489081330271 + z * q2;
                q2 = 2.16236993594496635890 * 0.1 + z * q2;
                q2 = 1.34204006088543189037 * 0.01 + z * q2;
                q2 = 3.28014464682127739104 * 0.0001 + z * q2;
                q2 = 2.89247864745380683936 * 0.000001 + z * q2;
                q2 = 6.79019408009981274425 * 0.000000001 + z * q2;
                x1 = z * p2 / q2;
            }
            x = x0 - x1;
            if (code != 0)
            {
                x = -x;
            }
            result = x;
            return result;
        }
    }

    public class ap
    {
        public static int len<T>(T[] a)
        { return a.Length; }
        public static int rows<T>(T[,] a)
        { return a.GetLength(0); }
        public static int cols<T>(T[,] a)
        { return a.GetLength(1); }
        public static void swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }

        public static void assert(bool cond, string s)
        {
            if (!cond)
                throw new alglibexception(s);
        }

        public static void assert(bool cond)
        {
            assert(cond, "ALGLIB: assertion failed");
        }

        /****************************************************************
        returns dps (digits-of-precision) value corresponding to threshold.
        dps(0.9)  = dps(0.5)  = dps(0.1) = 0
        dps(0.09) = dps(0.05) = dps(0.01) = 1
        and so on
        ****************************************************************/
        public static int threshold2dps(double threshold)
        {
            int result = 0;
            double t;
            for (result = 0, t = 1; t / 10 > threshold * (1 + 1E-10); result++, t /= 10) ;
            return result;
        }

        /****************************************************************
        prints formatted complex
        ****************************************************************/
        public static string format(complex a, int _dps)
        {
            int dps = Math.Abs(_dps);
            string fmt = _dps >= 0 ? "F" : "E";
            string fmtx = String.Format("{{0:" + fmt + "{0}}}", dps);
            string fmty = String.Format("{{0:" + fmt + "{0}}}", dps);
            string result = String.Format(fmtx, a.x) + (a.y >= 0 ? "+" : "-") + String.Format(fmty, Math.Abs(a.y)) + "i";
            result = result.Replace(',', '.');
            return result;
        }

        /****************************************************************
        prints formatted array
        ****************************************************************/
        public static string format(bool[] a)
        {
            string[] result = new string[len(a)];
            int i;
            for (i = 0; i < len(a); i++)
                if (a[i])
                    result[i] = "true";
                else
                    result[i] = "false";
            return "{" + String.Join(",", result) + "}";
        }

        /****************************************************************
        prints formatted array
        ****************************************************************/
        public static string format(int[] a)
        {
            string[] result = new string[len(a)];
            int i;
            for (i = 0; i < len(a); i++)
                result[i] = a[i].ToString();
            return "{" + String.Join(",", result) + "}";
        }

        /****************************************************************
        prints formatted array
        ****************************************************************/
        public static string format(double[] a, int _dps)
        {
            int dps = Math.Abs(_dps);
            string sfmt = _dps >= 0 ? "F" : "E";
            string fmt = String.Format("{{0:" + sfmt + "{0}}}", dps);
            string[] result = new string[len(a)];
            int i;
            for (i = 0; i < len(a); i++)
            {
                result[i] = String.Format(fmt, a[i]);
                result[i] = result[i].Replace(',', '.');
            }
            return "{" + String.Join(",", result) + "}";
        }

        /****************************************************************
        prints formatted array
        ****************************************************************/
        public static string format(complex[] a, int _dps)
        {
            int dps = Math.Abs(_dps);
            string fmt = _dps >= 0 ? "F" : "E";
            string fmtx = String.Format("{{0:" + fmt + "{0}}}", dps);
            string fmty = String.Format("{{0:" + fmt + "{0}}}", dps);
            string[] result = new string[len(a)];
            int i;
            for (i = 0; i < len(a); i++)
            {
                result[i] = String.Format(fmtx, a[i].x) + (a[i].y >= 0 ? "+" : "-") + String.Format(fmty, Math.Abs(a[i].y)) + "i";
                result[i] = result[i].Replace(',', '.');
            }
            return "{" + String.Join(",", result) + "}";
        }

        /****************************************************************
        prints formatted matrix
        ****************************************************************/
        public static string format(bool[,] a)
        {
            int i, j, m, n;
            n = cols(a);
            m = rows(a);
            bool[] line = new bool[n];
            string[] result = new string[m];
            for (i = 0; i < m; i++)
            {
                for (j = 0; j < n; j++)
                    line[j] = a[i, j];
                result[i] = format(line);
            }
            return "{" + String.Join(",", result) + "}";
        }

        /****************************************************************
        prints formatted matrix
        ****************************************************************/
        public static string format(int[,] a)
        {
            int i, j, m, n;
            n = cols(a);
            m = rows(a);
            int[] line = new int[n];
            string[] result = new string[m];
            for (i = 0; i < m; i++)
            {
                for (j = 0; j < n; j++)
                    line[j] = a[i, j];
                result[i] = format(line);
            }
            return "{" + String.Join(",", result) + "}";
        }

        /****************************************************************
        prints formatted matrix
        ****************************************************************/
        public static string format(double[,] a, int dps)
        {
            int i, j, m, n;
            n = cols(a);
            m = rows(a);
            double[] line = new double[n];
            string[] result = new string[m];
            for (i = 0; i < m; i++)
            {
                for (j = 0; j < n; j++)
                    line[j] = a[i, j];
                result[i] = format(line, dps);
            }
            return "{" + String.Join(",", result) + "}";
        }

        /****************************************************************
        prints formatted matrix
        ****************************************************************/
        public static string format(complex[,] a, int dps)
        {
            int i, j, m, n;
            n = cols(a);
            m = rows(a);
            complex[] line = new complex[n];
            string[] result = new string[m];
            for (i = 0; i < m; i++)
            {
                for (j = 0; j < n; j++)
                    line[j] = a[i, j];
                result[i] = format(line, dps);
            }
            return "{" + String.Join(",", result) + "}";
        }

        /****************************************************************
        checks that matrix is symmetric.
        max|A-A^T| is calculated; if it is within 1.0E-14 of max|A|,
        matrix is considered symmetric
        ****************************************************************/
        public static bool issymmetric(double[,] a)
        {
            int i, j, n;
            double err, mx, v1, v2;
            if (rows(a) != cols(a))
                return false;
            n = rows(a);
            if (n == 0)
                return true;
            mx = 0;
            err = 0;
            for (i = 0; i < n; i++)
            {
                for (j = i + 1; j < n; j++)
                {
                    v1 = a[i, j];
                    v2 = a[j, i];
                    if (!math.isfinite(v1))
                        return false;
                    if (!math.isfinite(v2))
                        return false;
                    err = Math.Max(err, Math.Abs(v1 - v2));
                    mx = Math.Max(mx, Math.Abs(v1));
                    mx = Math.Max(mx, Math.Abs(v2));
                }
                v1 = a[i, i];
                if (!math.isfinite(v1))
                    return false;
                mx = Math.Max(mx, Math.Abs(v1));
            }
            if (mx == 0)
                return true;
            return err / mx <= 1.0E-14;
        }

        /****************************************************************
        checks that matrix is Hermitian.
        max|A-A^H| is calculated; if it is within 1.0E-14 of max|A|,
        matrix is considered Hermitian
        ****************************************************************/
        public static bool ishermitian(complex[,] a)
        {
            int i, j, n;
            double err, mx;
            complex v1, v2, vt;
            if (rows(a) != cols(a))
                return false;
            n = rows(a);
            if (n == 0)
                return true;
            mx = 0;
            err = 0;
            for (i = 0; i < n; i++)
            {
                for (j = i + 1; j < n; j++)
                {
                    v1 = a[i, j];
                    v2 = a[j, i];
                    if (!math.isfinite(v1.x))
                        return false;
                    if (!math.isfinite(v1.y))
                        return false;
                    if (!math.isfinite(v2.x))
                        return false;
                    if (!math.isfinite(v2.y))
                        return false;
                    vt.x = v1.x - v2.x;
                    vt.y = v1.y + v2.y;
                    err = Math.Max(err, math.abscomplex(vt));
                    mx = Math.Max(mx, math.abscomplex(v1));
                    mx = Math.Max(mx, math.abscomplex(v2));
                }
                v1 = a[i, i];
                if (!math.isfinite(v1.x))
                    return false;
                if (!math.isfinite(v1.y))
                    return false;
                err = Math.Max(err, Math.Abs(v1.y));
                mx = Math.Max(mx, math.abscomplex(v1));
            }
            if (mx == 0)
                return true;
            return err / mx <= 1.0E-14;
        }


        /****************************************************************
        Forces symmetricity by copying upper half of A to the lower one
        ****************************************************************/
        public static bool forcesymmetric(double[,] a)
        {
            int i, j, n;
            if (rows(a) != cols(a))
                return false;
            n = rows(a);
            if (n == 0)
                return true;
            for (i = 0; i < n; i++)
                for (j = i + 1; j < n; j++)
                    a[i, j] = a[j, i];
            return true;
        }

        /****************************************************************
        Forces Hermiticity by copying upper half of A to the lower one
        ****************************************************************/
        public static bool forcehermitian(complex[,] a)
        {
            int i, j, n;
            complex v;
            if (rows(a) != cols(a))
                return false;
            n = rows(a);
            if (n == 0)
                return true;
            for (i = 0; i < n; i++)
                for (j = i + 1; j < n; j++)
                {
                    v = a[j, i];
                    a[i, j].x = v.x;
                    a[i, j].y = -v.y;
                }
            return true;
        }
    };

    public class ibetaf
    {
        /*************************************************************************
        Incomplete beta integral

        Returns incomplete beta integral of the arguments, evaluated
        from zero to x.  The function is defined as

                         x
            -            -
           | (a+b)      | |  a-1     b-1
         -----------    |   t   (1-t)   dt.
          -     -     | |
         | (a) | (b)   -
                        0

        The domain of definition is 0 <= x <= 1.  In this
        implementation a and b are restricted to positive values.
        The integral from x to 1 may be obtained by the symmetry
        relation

           1 - incbet( a, b, x )  =  incbet( b, a, 1-x ).

        The integral is evaluated by a continued fraction expansion
        or, when b*x is small, by a power series.

        ACCURACY:

        Tested at uniformly distributed random points (a,b,x) with a and b
        in "domain" and x between 0 and 1.
                                               Relative error
        arithmetic   domain     # trials      peak         rms
           IEEE      0,5         10000       6.9e-15     4.5e-16
           IEEE      0,85       250000       2.2e-13     1.7e-14
           IEEE      0,1000      30000       5.3e-12     6.3e-13
           IEEE      0,10000    250000       9.3e-11     7.1e-12
           IEEE      0,100000    10000       8.7e-10     4.8e-11
        Outputs smaller than the IEEE gradual underflow threshold
        were excluded from these statistics.

        Cephes Math Library, Release 2.8:  June, 2000
        Copyright 1984, 1995, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double incompletebeta(double a,
            double b,
            double x)
        {
            double result = 0;
            double t = 0;
            double xc = 0;
            double w = 0;
            double y = 0;
            int flag = 0;
            double sg = 0;
            double big = 0;
            double biginv = 0;
            double maxgam = 0;
            double minlog = 0;
            double maxlog = 0;

            big = 4.503599627370496e15;
            biginv = 2.22044604925031308085e-16;
            maxgam = 171.624376956302725;
            minlog = Math.Log(math.minrealnumber);
            maxlog = Math.Log(math.maxrealnumber);
            ap.assert((double)(a) > (double)(0) && (double)(b) > (double)(0), "Domain error in IncompleteBeta");
            ap.assert((double)(x) >= (double)(0) && (double)(x) <= (double)(1), "Domain error in IncompleteBeta");
            if ((double)(x) == (double)(0))
            {
                result = 0;
                return result;
            }
            if ((double)(x) == (double)(1))
            {
                result = 1;
                return result;
            }
            flag = 0;
            if ((double)(b * x) <= (double)(1.0) && (double)(x) <= (double)(0.95))
            {
                result = incompletebetaps(a, b, x, maxgam);
                return result;
            }
            w = 1.0 - x;
            if ((double)(x) > (double)(a / (a + b)))
            {
                flag = 1;
                t = a;
                a = b;
                b = t;
                xc = x;
                x = w;
            }
            else
            {
                xc = w;
            }
            if ((flag == 1 && (double)(b * x) <= (double)(1.0)) && (double)(x) <= (double)(0.95))
            {
                t = incompletebetaps(a, b, x, maxgam);
                if ((double)(t) <= (double)(math.machineepsilon))
                {
                    result = 1.0 - math.machineepsilon;
                }
                else
                {
                    result = 1.0 - t;
                }
                return result;
            }
            y = x * (a + b - 2.0) - (a - 1.0);
            if ((double)(y) < (double)(0.0))
            {
                w = incompletebetafe(a, b, x, big, biginv);
            }
            else
            {
                w = incompletebetafe2(a, b, x, big, biginv) / xc;
            }
            y = a * Math.Log(x);
            t = b * Math.Log(xc);
            if (((double)(a + b) < (double)(maxgam) && (double)(Math.Abs(y)) < (double)(maxlog)) && (double)(Math.Abs(t)) < (double)(maxlog))
            {
                t = Math.Pow(xc, b);
                t = t * Math.Pow(x, a);
                t = t / a;
                t = t * w;
                t = t * (gammafunc.gammafunction(a + b) / (gammafunc.gammafunction(a) * gammafunc.gammafunction(b)));
                if (flag == 1)
                {
                    if ((double)(t) <= (double)(math.machineepsilon))
                    {
                        result = 1.0 - math.machineepsilon;
                    }
                    else
                    {
                        result = 1.0 - t;
                    }
                }
                else
                {
                    result = t;
                }
                return result;
            }
            y = y + t + gammafunc.lngamma(a + b, ref sg) - gammafunc.lngamma(a, ref sg) - gammafunc.lngamma(b, ref sg);
            y = y + Math.Log(w / a);
            if ((double)(y) < (double)(minlog))
            {
                t = 0.0;
            }
            else
            {
                t = Math.Exp(y);
            }
            if (flag == 1)
            {
                if ((double)(t) <= (double)(math.machineepsilon))
                {
                    t = 1.0 - math.machineepsilon;
                }
                else
                {
                    t = 1.0 - t;
                }
            }
            result = t;
            return result;
        }


        /*************************************************************************
        Inverse of imcomplete beta integral

        Given y, the function finds x such that

         incbet( a, b, x ) = y .

        The routine performs interval halving or Newton iterations to find the
        root of incbet(a,b,x) - y = 0.


        ACCURACY:

                             Relative error:
                       x     a,b
        arithmetic   domain  domain  # trials    peak       rms
           IEEE      0,1    .5,10000   50000    5.8e-12   1.3e-13
           IEEE      0,1   .25,100    100000    1.8e-13   3.9e-15
           IEEE      0,1     0,5       50000    1.1e-12   5.5e-15
        With a and b constrained to half-integer or integer values:
           IEEE      0,1    .5,10000   50000    5.8e-12   1.1e-13
           IEEE      0,1    .5,100    100000    1.7e-14   7.9e-16
        With a = .5, b constrained to half-integer or integer values:
           IEEE      0,1    .5,10000   10000    8.3e-11   1.0e-11

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1996, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double invincompletebeta(double a,
            double b,
            double y)
        {
            double result = 0;
            double aaa = 0;
            double bbb = 0;
            double y0 = 0;
            double d = 0;
            double yyy = 0;
            double x = 0;
            double x0 = 0;
            double x1 = 0;
            double lgm = 0;
            double yp = 0;
            double di = 0;
            double dithresh = 0;
            double yl = 0;
            double yh = 0;
            double xt = 0;
            int i = 0;
            int rflg = 0;
            int dir = 0;
            int nflg = 0;
            double s = 0;
            int mainlooppos = 0;
            int ihalve = 0;
            int ihalvecycle = 0;
            int newt = 0;
            int newtcycle = 0;
            int breaknewtcycle = 0;
            int breakihalvecycle = 0;

            i = 0;
            ap.assert((double)(y) >= (double)(0) && (double)(y) <= (double)(1), "Domain error in InvIncompleteBeta");

            //
            // special cases
            //
            if ((double)(y) == (double)(0))
            {
                result = 0;
                return result;
            }
            if ((double)(y) == (double)(1.0))
            {
                result = 1;
                return result;
            }

            //
            // these initializations are not really necessary,
            // but without them compiler complains about 'possibly uninitialized variables'.
            //
            dithresh = 0;
            rflg = 0;
            aaa = 0;
            bbb = 0;
            y0 = 0;
            x = 0;
            yyy = 0;
            lgm = 0;
            dir = 0;
            di = 0;

            //
            // normal initializations
            //
            x0 = 0.0;
            yl = 0.0;
            x1 = 1.0;
            yh = 1.0;
            nflg = 0;
            mainlooppos = 0;
            ihalve = 1;
            ihalvecycle = 2;
            newt = 3;
            newtcycle = 4;
            breaknewtcycle = 5;
            breakihalvecycle = 6;

            //
            // main loop
            //
            while (true)
            {

                //
                // start
                //
                if (mainlooppos == 0)
                {
                    if ((double)(a) <= (double)(1.0) || (double)(b) <= (double)(1.0))
                    {
                        dithresh = 1.0e-6;
                        rflg = 0;
                        aaa = a;
                        bbb = b;
                        y0 = y;
                        x = aaa / (aaa + bbb);
                        yyy = incompletebeta(aaa, bbb, x);
                        mainlooppos = ihalve;
                        continue;
                    }
                    else
                    {
                        dithresh = 1.0e-4;
                    }
                    yp = -normaldistr.invnormaldistribution(y);
                    if ((double)(y) > (double)(0.5))
                    {
                        rflg = 1;
                        aaa = b;
                        bbb = a;
                        y0 = 1.0 - y;
                        yp = -yp;
                    }
                    else
                    {
                        rflg = 0;
                        aaa = a;
                        bbb = b;
                        y0 = y;
                    }
                    lgm = (yp * yp - 3.0) / 6.0;
                    x = 2.0 / (1.0 / (2.0 * aaa - 1.0) + 1.0 / (2.0 * bbb - 1.0));
                    d = yp * Math.Sqrt(x + lgm) / x - (1.0 / (2.0 * bbb - 1.0) - 1.0 / (2.0 * aaa - 1.0)) * (lgm + 5.0 / 6.0 - 2.0 / (3.0 * x));
                    d = 2.0 * d;
                    if ((double)(d) < (double)(Math.Log(math.minrealnumber)))
                    {
                        x = 0;
                        break;
                    }
                    x = aaa / (aaa + bbb * Math.Exp(d));
                    yyy = incompletebeta(aaa, bbb, x);
                    yp = (yyy - y0) / y0;
                    if ((double)(Math.Abs(yp)) < (double)(0.2))
                    {
                        mainlooppos = newt;
                        continue;
                    }
                    mainlooppos = ihalve;
                    continue;
                }

                //
                // ihalve
                //
                if (mainlooppos == ihalve)
                {
                    dir = 0;
                    di = 0.5;
                    i = 0;
                    mainlooppos = ihalvecycle;
                    continue;
                }

                //
                // ihalvecycle
                //
                if (mainlooppos == ihalvecycle)
                {
                    if (i <= 99)
                    {
                        if (i != 0)
                        {
                            x = x0 + di * (x1 - x0);
                            if ((double)(x) == (double)(1.0))
                            {
                                x = 1.0 - math.machineepsilon;
                            }
                            if ((double)(x) == (double)(0.0))
                            {
                                di = 0.5;
                                x = x0 + di * (x1 - x0);
                                if ((double)(x) == (double)(0.0))
                                {
                                    break;
                                }
                            }
                            yyy = incompletebeta(aaa, bbb, x);
                            yp = (x1 - x0) / (x1 + x0);
                            if ((double)(Math.Abs(yp)) < (double)(dithresh))
                            {
                                mainlooppos = newt;
                                continue;
                            }
                            yp = (yyy - y0) / y0;
                            if ((double)(Math.Abs(yp)) < (double)(dithresh))
                            {
                                mainlooppos = newt;
                                continue;
                            }
                        }
                        if ((double)(yyy) < (double)(y0))
                        {
                            x0 = x;
                            yl = yyy;
                            if (dir < 0)
                            {
                                dir = 0;
                                di = 0.5;
                            }
                            else
                            {
                                if (dir > 3)
                                {
                                    di = 1.0 - (1.0 - di) * (1.0 - di);
                                }
                                else
                                {
                                    if (dir > 1)
                                    {
                                        di = 0.5 * di + 0.5;
                                    }
                                    else
                                    {
                                        di = (y0 - yyy) / (yh - yl);
                                    }
                                }
                            }
                            dir = dir + 1;
                            if ((double)(x0) > (double)(0.75))
                            {
                                if (rflg == 1)
                                {
                                    rflg = 0;
                                    aaa = a;
                                    bbb = b;
                                    y0 = y;
                                }
                                else
                                {
                                    rflg = 1;
                                    aaa = b;
                                    bbb = a;
                                    y0 = 1.0 - y;
                                }
                                x = 1.0 - x;
                                yyy = incompletebeta(aaa, bbb, x);
                                x0 = 0.0;
                                yl = 0.0;
                                x1 = 1.0;
                                yh = 1.0;
                                mainlooppos = ihalve;
                                continue;
                            }
                        }
                        else
                        {
                            x1 = x;
                            if (rflg == 1 && (double)(x1) < (double)(math.machineepsilon))
                            {
                                x = 0.0;
                                break;
                            }
                            yh = yyy;
                            if (dir > 0)
                            {
                                dir = 0;
                                di = 0.5;
                            }
                            else
                            {
                                if (dir < -3)
                                {
                                    di = di * di;
                                }
                                else
                                {
                                    if (dir < -1)
                                    {
                                        di = 0.5 * di;
                                    }
                                    else
                                    {
                                        di = (yyy - y0) / (yh - yl);
                                    }
                                }
                            }
                            dir = dir - 1;
                        }
                        i = i + 1;
                        mainlooppos = ihalvecycle;
                        continue;
                    }
                    else
                    {
                        mainlooppos = breakihalvecycle;
                        continue;
                    }
                }

                //
                // breakihalvecycle
                //
                if (mainlooppos == breakihalvecycle)
                {
                    if ((double)(x0) >= (double)(1.0))
                    {
                        x = 1.0 - math.machineepsilon;
                        break;
                    }
                    if ((double)(x) <= (double)(0.0))
                    {
                        x = 0.0;
                        break;
                    }
                    mainlooppos = newt;
                    continue;
                }

                //
                // newt
                //
                if (mainlooppos == newt)
                {
                    if (nflg != 0)
                    {
                        break;
                    }
                    nflg = 1;
                    lgm = gammafunc.lngamma(aaa + bbb, ref s) - gammafunc.lngamma(aaa, ref s) - gammafunc.lngamma(bbb, ref s);
                    i = 0;
                    mainlooppos = newtcycle;
                    continue;
                }

                //
                // newtcycle
                //
                if (mainlooppos == newtcycle)
                {
                    if (i <= 7)
                    {
                        if (i != 0)
                        {
                            yyy = incompletebeta(aaa, bbb, x);
                        }
                        if ((double)(yyy) < (double)(yl))
                        {
                            x = x0;
                            yyy = yl;
                        }
                        else
                        {
                            if ((double)(yyy) > (double)(yh))
                            {
                                x = x1;
                                yyy = yh;
                            }
                            else
                            {
                                if ((double)(yyy) < (double)(y0))
                                {
                                    x0 = x;
                                    yl = yyy;
                                }
                                else
                                {
                                    x1 = x;
                                    yh = yyy;
                                }
                            }
                        }
                        if ((double)(x) == (double)(1.0) || (double)(x) == (double)(0.0))
                        {
                            mainlooppos = breaknewtcycle;
                            continue;
                        }
                        d = (aaa - 1.0) * Math.Log(x) + (bbb - 1.0) * Math.Log(1.0 - x) + lgm;
                        if ((double)(d) < (double)(Math.Log(math.minrealnumber)))
                        {
                            break;
                        }
                        if ((double)(d) > (double)(Math.Log(math.maxrealnumber)))
                        {
                            mainlooppos = breaknewtcycle;
                            continue;
                        }
                        d = Math.Exp(d);
                        d = (yyy - y0) / d;
                        xt = x - d;
                        if ((double)(xt) <= (double)(x0))
                        {
                            yyy = (x - x0) / (x1 - x0);
                            xt = x0 + 0.5 * yyy * (x - x0);
                            if ((double)(xt) <= (double)(0.0))
                            {
                                mainlooppos = breaknewtcycle;
                                continue;
                            }
                        }
                        if ((double)(xt) >= (double)(x1))
                        {
                            yyy = (x1 - x) / (x1 - x0);
                            xt = x1 - 0.5 * yyy * (x1 - x);
                            if ((double)(xt) >= (double)(1.0))
                            {
                                mainlooppos = breaknewtcycle;
                                continue;
                            }
                        }
                        x = xt;
                        if ((double)(Math.Abs(d / x)) < (double)(128.0 * math.machineepsilon))
                        {
                            break;
                        }
                        i = i + 1;
                        mainlooppos = newtcycle;
                        continue;
                    }
                    else
                    {
                        mainlooppos = breaknewtcycle;
                        continue;
                    }
                }

                //
                // breaknewtcycle
                //
                if (mainlooppos == breaknewtcycle)
                {
                    dithresh = 256.0 * math.machineepsilon;
                    mainlooppos = ihalve;
                    continue;
                }
            }

            //
            // done
            //
            if (rflg != 0)
            {
                if ((double)(x) <= (double)(math.machineepsilon))
                {
                    x = 1.0 - math.machineepsilon;
                }
                else
                {
                    x = 1.0 - x;
                }
            }
            result = x;
            return result;
        }


        /*************************************************************************
        Continued fraction expansion #1 for incomplete beta integral

        Cephes Math Library, Release 2.8:  June, 2000
        Copyright 1984, 1995, 2000 by Stephen L. Moshier
        *************************************************************************/
        private static double incompletebetafe(double a,
            double b,
            double x,
            double big,
            double biginv)
        {
            double result = 0;
            double xk = 0;
            double pk = 0;
            double pkm1 = 0;
            double pkm2 = 0;
            double qk = 0;
            double qkm1 = 0;
            double qkm2 = 0;
            double k1 = 0;
            double k2 = 0;
            double k3 = 0;
            double k4 = 0;
            double k5 = 0;
            double k6 = 0;
            double k7 = 0;
            double k8 = 0;
            double r = 0;
            double t = 0;
            double ans = 0;
            double thresh = 0;
            int n = 0;

            k1 = a;
            k2 = a + b;
            k3 = a;
            k4 = a + 1.0;
            k5 = 1.0;
            k6 = b - 1.0;
            k7 = k4;
            k8 = a + 2.0;
            pkm2 = 0.0;
            qkm2 = 1.0;
            pkm1 = 1.0;
            qkm1 = 1.0;
            ans = 1.0;
            r = 1.0;
            n = 0;
            thresh = 3.0 * math.machineepsilon;
            do
            {
                xk = -(x * k1 * k2 / (k3 * k4));
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                xk = x * k5 * k6 / (k7 * k8);
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                if ((double)(qk) != (double)(0))
                {
                    r = pk / qk;
                }
                if ((double)(r) != (double)(0))
                {
                    t = Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                {
                    t = 1.0;
                }
                if ((double)(t) < (double)(thresh))
                {
                    break;
                }
                k1 = k1 + 1.0;
                k2 = k2 + 1.0;
                k3 = k3 + 2.0;
                k4 = k4 + 2.0;
                k5 = k5 + 1.0;
                k6 = k6 - 1.0;
                k7 = k7 + 2.0;
                k8 = k8 + 2.0;
                if ((double)(Math.Abs(qk) + Math.Abs(pk)) > (double)(big))
                {
                    pkm2 = pkm2 * biginv;
                    pkm1 = pkm1 * biginv;
                    qkm2 = qkm2 * biginv;
                    qkm1 = qkm1 * biginv;
                }
                if ((double)(Math.Abs(qk)) < (double)(biginv) || (double)(Math.Abs(pk)) < (double)(biginv))
                {
                    pkm2 = pkm2 * big;
                    pkm1 = pkm1 * big;
                    qkm2 = qkm2 * big;
                    qkm1 = qkm1 * big;
                }
                n = n + 1;
            }
            while (n != 300);
            result = ans;
            return result;
        }


        /*************************************************************************
        Continued fraction expansion #2
        for incomplete beta integral

        Cephes Math Library, Release 2.8:  June, 2000
        Copyright 1984, 1995, 2000 by Stephen L. Moshier
        *************************************************************************/
        private static double incompletebetafe2(double a,
            double b,
            double x,
            double big,
            double biginv)
        {
            double result = 0;
            double xk = 0;
            double pk = 0;
            double pkm1 = 0;
            double pkm2 = 0;
            double qk = 0;
            double qkm1 = 0;
            double qkm2 = 0;
            double k1 = 0;
            double k2 = 0;
            double k3 = 0;
            double k4 = 0;
            double k5 = 0;
            double k6 = 0;
            double k7 = 0;
            double k8 = 0;
            double r = 0;
            double t = 0;
            double ans = 0;
            double z = 0;
            double thresh = 0;
            int n = 0;

            k1 = a;
            k2 = b - 1.0;
            k3 = a;
            k4 = a + 1.0;
            k5 = 1.0;
            k6 = a + b;
            k7 = a + 1.0;
            k8 = a + 2.0;
            pkm2 = 0.0;
            qkm2 = 1.0;
            pkm1 = 1.0;
            qkm1 = 1.0;
            z = x / (1.0 - x);
            ans = 1.0;
            r = 1.0;
            n = 0;
            thresh = 3.0 * math.machineepsilon;
            do
            {
                xk = -(z * k1 * k2 / (k3 * k4));
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                xk = z * k5 * k6 / (k7 * k8);
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                if ((double)(qk) != (double)(0))
                {
                    r = pk / qk;
                }
                if ((double)(r) != (double)(0))
                {
                    t = Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                {
                    t = 1.0;
                }
                if ((double)(t) < (double)(thresh))
                {
                    break;
                }
                k1 = k1 + 1.0;
                k2 = k2 - 1.0;
                k3 = k3 + 2.0;
                k4 = k4 + 2.0;
                k5 = k5 + 1.0;
                k6 = k6 + 1.0;
                k7 = k7 + 2.0;
                k8 = k8 + 2.0;
                if ((double)(Math.Abs(qk) + Math.Abs(pk)) > (double)(big))
                {
                    pkm2 = pkm2 * biginv;
                    pkm1 = pkm1 * biginv;
                    qkm2 = qkm2 * biginv;
                    qkm1 = qkm1 * biginv;
                }
                if ((double)(Math.Abs(qk)) < (double)(biginv) || (double)(Math.Abs(pk)) < (double)(biginv))
                {
                    pkm2 = pkm2 * big;
                    pkm1 = pkm1 * big;
                    qkm2 = qkm2 * big;
                    qkm1 = qkm1 * big;
                }
                n = n + 1;
            }
            while (n != 300);
            result = ans;
            return result;
        }


        private static double incompletebetaps(double a,
             double b,
             double x,
             double maxgam)
        {
            double result = 0;
            double s = 0;
            double t = 0;
            double u = 0;
            double v = 0;
            double n = 0;
            double t1 = 0;
            double z = 0;
            double ai = 0;
            double sg = 0;

            ai = 1.0 / a;
            u = (1.0 - b) * x;
            v = u / (a + 1.0);
            t1 = v;
            t = u;
            n = 2.0;
            s = 0.0;
            z = math.machineepsilon * ai;
            while ((double)(Math.Abs(v)) > (double)(z))
            {
                u = (n - b) * x / n;
                t = t * u;
                v = t / (a + n);
                s = s + v;
                n = n + 1.0;
            }
            s = s + t1;
            s = s + ai;
            u = a * Math.Log(x);
            if ((double)(a + b) < (double)(maxgam) && (double)(Math.Abs(u)) < (double)(Math.Log(math.maxrealnumber)))
            {
                t = gammafunc.gammafunction(a + b) / (gammafunc.gammafunction(a) * gammafunc.gammafunction(b));
                s = s * t * Math.Pow(x, a);
            }
            else
            {
                t = gammafunc.lngamma(a + b, ref sg) - gammafunc.lngamma(a, ref sg) - gammafunc.lngamma(b, ref sg) + u + Math.Log(s);
                if ((double)(t) < (double)(Math.Log(math.minrealnumber)))
                {
                    s = 0.0;
                }
                else
                {
                    s = Math.Exp(t);
                }
            }
            result = s;
            return result;
        }


    }

    public class studenttdistr
    {
        /*************************************************************************
        Student's t distribution

        Computes the integral from minus infinity to t of the Student
        t distribution with integer k > 0 degrees of freedom:

                                             t
                                             -
                                            | |
                     -                      |         2   -(k+1)/2
                    | ( (k+1)/2 )           |  (     x   )
              ----------------------        |  ( 1 + --- )        dx
                            -               |  (      k  )
              sqrt( k pi ) | ( k/2 )        |
                                          | |
                                           -
                                          -inf.

        Relation to incomplete beta integral:

               1 - stdtr(k,t) = 0.5 * incbet( k/2, 1/2, z )
        where
               z = k/(k + t**2).

        For t < -2, this is the method of computation.  For higher t,
        a direct method is derived from integration by parts.
        Since the function is symmetric about t=0, the area under the
        right tail of the density is found by calling the function
        with -t instead of t.

        ACCURACY:

        Tested at random 1 <= k <= 25.  The "domain" refers to t.
                             Relative error:
        arithmetic   domain     # trials      peak         rms
           IEEE     -100,-2      50000       5.9e-15     1.4e-15
           IEEE     -2,100      500000       2.7e-15     4.9e-17

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double studenttdistribution(int k,
            double t)
        {
            double result = 0;
            double x = 0;
            double rk = 0;
            double z = 0;
            double f = 0;
            double tz = 0;
            double p = 0;
            double xsqk = 0;
            int j = 0;

            ap.assert(k > 0, "Domain error in StudentTDistribution");
            if ((double)(t) == (double)(0))
            {
                result = 0.5;
                return result;
            }
            if ((double)(t) < (double)(-2.0))
            {
                rk = k;
                z = rk / (rk + t * t);
                result = 0.5 * ibetaf.incompletebeta(0.5 * rk, 0.5, z);
                return result;
            }
            if ((double)(t) < (double)(0))
            {
                x = -t;
            }
            else
            {
                x = t;
            }
            rk = k;
            z = 1.0 + x * x / rk;
            if (k % 2 != 0)
            {
                xsqk = x / Math.Sqrt(rk);
                p = Math.Atan(xsqk);
                if (k > 1)
                {
                    f = 1.0;
                    tz = 1.0;
                    j = 3;
                    while (j <= k - 2 && (double)(tz / f) > (double)(math.machineepsilon))
                    {
                        tz = tz * ((j - 1) / (z * j));
                        f = f + tz;
                        j = j + 2;
                    }
                    p = p + f * xsqk / z;
                }
                p = p * 2.0 / Math.PI;
            }
            else
            {
                f = 1.0;
                tz = 1.0;
                j = 2;
                while (j <= k - 2 && (double)(tz / f) > (double)(math.machineepsilon))
                {
                    tz = tz * ((j - 1) / (z * j));
                    f = f + tz;
                    j = j + 2;
                }
                p = f * x / Math.Sqrt(z * rk);
            }
            if ((double)(t) < (double)(0))
            {
                p = -p;
            }
            result = 0.5 + 0.5 * p;
            return result;
        }


        /*************************************************************************
        Functional inverse of Student's t distribution

        Given probability p, finds the argument t such that stdtr(k,t)
        is equal to p.

        ACCURACY:

        Tested at random 1 <= k <= 100.  The "domain" refers to p:
                             Relative error:
        arithmetic   domain     # trials      peak         rms
           IEEE    .001,.999     25000       5.7e-15     8.0e-16
           IEEE    10^-6,.001    25000       2.0e-12     2.9e-14

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double invstudenttdistribution(int k,
            double p)
        {
            double result = 0;
            double t = 0;
            double rk = 0;
            double z = 0;
            int rflg = 0;

            ap.assert((k > 0 && (double)(p) > (double)(0)) && (double)(p) < (double)(1), "Domain error in InvStudentTDistribution");
            rk = k;
            if ((double)(p) > (double)(0.25) && (double)(p) < (double)(0.75))
            {
                if ((double)(p) == (double)(0.5))
                {
                    result = 0;
                    return result;
                }
                z = 1.0 - 2.0 * p;
                z = ibetaf.invincompletebeta(0.5, 0.5 * rk, Math.Abs(z));
                t = Math.Sqrt(rk * z / (1.0 - z));
                if ((double)(p) < (double)(0.5))
                {
                    t = -t;
                }
                result = t;
                return result;
            }
            rflg = -1;
            if ((double)(p) >= (double)(0.5))
            {
                p = 1.0 - p;
                rflg = 1;
            }
            z = ibetaf.invincompletebeta(0.5 * rk, 0.5, 2.0 * p);
            if ((double)(math.maxrealnumber * z) < (double)(rk))
            {
                result = rflg * math.maxrealnumber;
                return result;
            }
            t = Math.Sqrt(rk / z - rk);
            result = rflg * t;
            return result;
        }
    }

    public class studentttests
    {
        /*************************************************************************
        One-sample t-test

        This test checks three hypotheses about the mean of the given sample.  The
        following tests are performed:
            * two-tailed test (null hypothesis - the mean is equal  to  the  given
              value)
            * left-tailed test (null hypothesis - the  mean  is  greater  than  or
              equal to the given value)
            * right-tailed test (null hypothesis - the mean is less than or  equal
              to the given value).

        The test is based on the assumption that  a  given  sample  has  a  normal
        distribution and  an  unknown  dispersion.  If  the  distribution  sharply
        differs from normal, the test will work incorrectly.

        Input parameters:
            X       -   sample. Array whose index goes from 0 to N-1.
            N       -   size of sample.
            Mean    -   assumed value of the mean.

        Output parameters:
            BothTails   -   p-value for two-tailed test.
                            If BothTails is less than the given significance level
                            the null hypothesis is rejected.
            LeftTail    -   p-value for left-tailed test.
                            If LeftTail is less than the given significance level,
                            the null hypothesis is rejected.
            RightTail   -   p-value for right-tailed test.
                            If RightTail is less than the given significance level
                            the null hypothesis is rejected.

          -- ALGLIB --
             Copyright 08.09.2006 by Bochkanov Sergey
        *************************************************************************/
        public static void studentttest1(double[] x,
            int n,
            double mean,
            out double bothtails,
            out double lefttail,
            out double righttail)
        {
            int i = 0;
            double xmean = 0;
            double xvariance = 0;
            double xstddev = 0;
            double v1 = 0;
            double v2 = 0;
            double stat = 0;
            double s = 0;

            bothtails = 0;
            lefttail = 0;
            righttail = 0;

            if (n <= 1)
            {
                bothtails = 1.0;
                lefttail = 1.0;
                righttail = 1.0;
                return;
            }

            //
            // Mean
            //
            xmean = 0;
            for (i = 0; i <= n - 1; i++)
            {
                xmean = xmean + x[i];
            }
            xmean = xmean / n;

            //
            // Variance (using corrected two-pass algorithm)
            //
            xvariance = 0;
            xstddev = 0;
            if (n != 1)
            {
                v1 = 0;
                for (i = 0; i <= n - 1; i++)
                {
                    v1 = v1 + math.sqr(x[i] - xmean);
                }
                v2 = 0;
                for (i = 0; i <= n - 1; i++)
                {
                    v2 = v2 + (x[i] - xmean);
                }
                v2 = math.sqr(v2) / n;
                xvariance = (v1 - v2) / (n - 1);
                if ((double)(xvariance) < (double)(0))
                {
                    xvariance = 0;
                }
                xstddev = Math.Sqrt(xvariance);
            }
            if ((double)(xstddev) == (double)(0))
            {
                if ((double)(xmean) == (double)(mean))
                {
                    bothtails = 1.0;
                }
                else
                {
                    bothtails = 0.0;
                }
                if ((double)(xmean) >= (double)(mean))
                {
                    lefttail = 1.0;
                }
                else
                {
                    lefttail = 0.0;
                }
                if ((double)(xmean) <= (double)(mean))
                {
                    righttail = 1.0;
                }
                else
                {
                    righttail = 0.0;
                }
                return;
            }

            //
            // Statistic
            //
            stat = (xmean - mean) / (xstddev / Math.Sqrt(n));
            s = studenttdistr.studenttdistribution(n - 1, stat);
            bothtails = 2 * Math.Min(s, 1 - s);
            lefttail = s;
            righttail = 1 - s;
        }

        public static void studentttest1(float[] x, int n, float mean, out double bothtails, out double leftTail, out double righttail)
        {
            int x_length = x.Length;
            double[] x_double = new double[x_length];
            for (int indexX = 0; indexX < x_length; indexX++)
            {
                x_double[indexX] = (double)x[indexX];
            }
            studentttest1(x_double, n, (double)mean, out bothtails, out leftTail, out righttail);
        }
        /*************************************************************************
        Two-sample pooled test

        This test checks three hypotheses about the mean of the given samples. The
        following tests are performed:
            * two-tailed test (null hypothesis - the means are equal)
            * left-tailed test (null hypothesis - the mean of the first sample  is
              greater than or equal to the mean of the second sample)
            * right-tailed test (null hypothesis - the mean of the first sample is
              less than or equal to the mean of the second sample).

        Test is based on the following assumptions:
            * given samples have normal distributions
            * dispersions are equal
            * samples are independent.

        Input parameters:
            X       -   sample 1. Array whose index goes from 0 to N-1.
            N       -   size of sample.
            Y       -   sample 2. Array whose index goes from 0 to M-1.
            M       -   size of sample.

        Output parameters:
            BothTails   -   p-value for two-tailed test.
                            If BothTails is less than the given significance level
                            the null hypothesis is rejected.
            LeftTail    -   p-value for left-tailed test.
                            If LeftTail is less than the given significance level,
                            the null hypothesis is rejected.
            RightTail   -   p-value for right-tailed test.
                            If RightTail is less than the given significance level
                            the null hypothesis is rejected.

          -- ALGLIB --
             Copyright 18.09.2006 by Bochkanov Sergey
        *************************************************************************/
        public static void studentttest2(double[] x, double[] y, out double bothtails, out double lefttail, out double righttail)
        {
            //
            // S
            //
            double xmean = 0;
            double ymean = 0;
            double s = 0;
            int n = x.Length;
            int m = y.Length;

            //
            // Mean
            //
            xmean = 0;
            for (int i = 0; i <= n - 1; i++)
            {
                xmean = xmean + x[i];
            }
            xmean = xmean / n;
            ymean = 0;
            for (int i = 0; i <= m - 1; i++)
            {
                ymean = ymean + y[i];
            }
            ymean = ymean / m;

            //
            // Variance
            //

            for (int i = 0; i <= n - 1; i++)
            {
                s = s + math.sqr(x[i] - xmean);
            }
            for (int i = 0; i <= m - 1; i++)
            {
                s = s + math.sqr(y[i] - ymean);
            }
            s = Math.Sqrt(s * ((double)1 / (double)n + (double)1 / (double)m) / (n + m - 2));
            studentttest2(x, y, s, out bothtails, out lefttail, out righttail);
        }

        public static void studentttest2_paired_jens(double[] x, double[] y, out double bothtails, out double lefttail, out double righttail)
        {
            if (x.Length != y.Length) { throw new Exception(); }
            //
            // S
            //
            double difference_mean = 0;
            double s = 0;
            int n = x.Length;
            double xmean;
            double ymean;

            if (n <= 1)
            {
                bothtails = 1.0;
                lefttail = 1.0;
                righttail = 1.0;
                return;
            }

            //
            // Mean
            //
            xmean = 0;
            ymean = 0;
            difference_mean = 0;
            for (int i = 0; i < n; i++)
            {
                xmean = xmean + x[i];
                ymean = ymean + y[i];
                difference_mean = difference_mean + (x[i] - y[i]);
            }
            difference_mean = difference_mean / n;
            xmean = xmean / n;
            ymean = ymean / n;

            //
            // Variance
            //
            double sum_of_squares = 0;
            for (int i = 0; i < n; i++)
            {
                sum_of_squares += math.sqr(x[i] - y[i] - 0);
            }
            s = Math.Sqrt((sum_of_squares / n - math.sqr(difference_mean)) * n / (float)(n - 1));
            //s = s / Math.Sqrt(n);

            //s = Math.Sqrt((math.sqr(difference_mean) - (s/n)) / (n-1));

            double stat = (difference_mean - 0) / (s / Math.Sqrt(n));
            double p = studenttdistr.studenttdistribution(n - 1, stat);
            bothtails = 2 * Math.Min(p, 1 - p);
            lefttail = p;
            righttail = 1 - p;

            //stat = (xmean - mean) / (xstddev / Math.Sqrt(n));
            //s = studenttdistr.studenttdistribution(n - 1, stat);
            //bothtails = 2 * Math.Min(s, 1 - s);
            //lefttail = s;
            //righttail = 1 - s;


        }

        public static void studentttest2(float[] x, float[] y, out double bothtails, out double lefttail, out double righttail)
        {
            int x_length = x.Length;
            int y_length = y.Length;
            double[] x_double = new double[x_length];
            for (int indexX = 0; indexX < x_length; indexX++)
            {
                x_double[indexX] = x[indexX];
            }
            double[] y_double = new double[y_length];
            for (int indexY = 0; indexY < y_length; indexY++)
            {
                y_double[indexY] = y[indexY];
            }
            studentttest2(x_double, y_double, out bothtails, out lefttail, out righttail);
        }

        public static void studentttest2(float[] x, float[] y, double variance, out double bothtails, out double lefttail, out double righttail)
        {
            int x_length = x.Length;
            int y_length = y.Length;
            double[] x_double = new double[x_length];
            for (int indexX = 0; indexX < x_length; indexX++)
            {
                x_double[indexX] = x[indexX];
            }
            double[] y_double = new double[y_length];
            for (int indexY = 0; indexY < y_length; indexY++)
            {
                y_double[indexY] = y[indexY];
            }
            studentttest2(x_double, y_double, variance, out bothtails, out lefttail, out righttail);
        }

        public static void studentttest2(double[] x, double[] y, double variance, out double bothtails, out double lefttail, out double righttail)
        {
            int i = 0;
            double xmean = 0;
            double ymean = 0;
            double stat = 0;
            double s = variance;
            double p = 0;
            int n = x.Length;
            int m = y.Length;

            bothtails = 0;
            lefttail = 0;
            righttail = 0;

            if (n <= 1 || m <= 1)
            {
                bothtails = 1.0;
                lefttail = 1.0;
                righttail = 1.0;
                return;
            }

            //
            // Mean
            //
            xmean = 0;
            for (i = 0; i <= n - 1; i++)
            {
                xmean = xmean + x[i];
            }
            xmean = xmean / n;
            ymean = 0;
            for (i = 0; i <= m - 1; i++)
            {
                ymean = ymean + y[i];
            }
            ymean = ymean / m;

            if ((double)(s) == (double)(0))
            {
                if ((double)(xmean) == (double)(ymean))
                {
                    bothtails = 1.0;
                }
                else
                {
                    bothtails = 0.0;
                }
                if ((double)(xmean) >= (double)(ymean))
                {
                    lefttail = 1.0;
                }
                else
                {
                    lefttail = 0.0;
                }
                if ((double)(xmean) <= (double)(ymean))
                {
                    righttail = 1.0;
                }
                else
                {
                    righttail = 0.0;
                }
                return;
            }

            //
            // Statistic
            //
            stat = (xmean - ymean) / s;
            p = studenttdistr.studenttdistribution(n + m - 2, stat);
            bothtails = 2 * Math.Min(p, 1 - p);
            lefttail = p;
            righttail = 1 - p;
        }

        /*************************************************************************
        Two-sample unpooled test

        This test checks three hypotheses about the mean of the given samples. The
        following tests are performed:
            * two-tailed test (null hypothesis - the means are equal)
            * left-tailed test (null hypothesis - the mean of the first sample  is
              greater than or equal to the mean of the second sample)
            * right-tailed test (null hypothesis - the mean of the first sample is
              less than or equal to the mean of the second sample).

        Test is based on the following assumptions:
            * given samples have normal distributions
            * samples are independent.
        Dispersion equality is not required

        Input parameters:
            X - sample 1. Array whose index goes from 0 to N-1.
            N - size of the sample.
            Y - sample 2. Array whose index goes from 0 to M-1.
            M - size of the sample.

        Output parameters:
            BothTails   -   p-value for two-tailed test.
                            If BothTails is less than the given significance level
                            the null hypothesis is rejected.
            LeftTail    -   p-value for left-tailed test.
                            If LeftTail is less than the given significance level,
                            the null hypothesis is rejected.
            RightTail   -   p-value for right-tailed test.
                            If RightTail is less than the given significance level
                            the null hypothesis is rejected.

          -- ALGLIB --
             Copyright 18.09.2006 by Bochkanov Sergey
        *************************************************************************/
        public static void unequalvariancettest(double[] x,
            int n,
            double[] y,
            int m,
            out double bothtails,
            out double lefttail,
            out double righttail)
        {
            int i = 0;
            double xmean = 0;
            double ymean = 0;
            double xvar = 0;
            double yvar = 0;
            double df = 0;
            double p = 0;
            double stat = 0;
            double c = 0;

            bothtails = 0;
            lefttail = 0;
            righttail = 0;

            if (n <= 1 || m <= 1)
            {
                bothtails = 1.0;
                lefttail = 1.0;
                righttail = 1.0;
                return;
            }

            //
            // Mean
            //
            xmean = 0;
            for (i = 0; i <= n - 1; i++)
            {
                xmean = xmean + x[i];
            }
            xmean = xmean / n;
            ymean = 0;
            for (i = 0; i <= m - 1; i++)
            {
                ymean = ymean + y[i];
            }
            ymean = ymean / m;

            //
            // Variance (using corrected two-pass algorithm)
            //
            xvar = 0;
            for (i = 0; i <= n - 1; i++)
            {
                xvar = xvar + math.sqr(x[i] - xmean);
            }
            xvar = xvar / (n - 1);
            yvar = 0;
            for (i = 0; i <= m - 1; i++)
            {
                yvar = yvar + math.sqr(y[i] - ymean);
            }
            yvar = yvar / (m - 1);
            if ((double)(xvar) == (double)(0) || (double)(yvar) == (double)(0))
            {
                bothtails = 1.0;
                lefttail = 1.0;
                righttail = 1.0;
                return;
            }

            //
            // Statistic
            //
            stat = (xmean - ymean) / Math.Sqrt(xvar / n + yvar / m);
            c = xvar / n / (xvar / n + yvar / m);
            df = (n - 1) * (m - 1) / ((m - 1) * math.sqr(c) + (n - 1) * math.sqr(1 - c));
            if ((double)(stat) > (double)(0))
            {
                p = 1 - 0.5 * ibetaf.incompletebeta(df / 2, 0.5, df / (df + math.sqr(stat)));
            }
            else
            {
                p = 0.5 * ibetaf.incompletebeta(df / 2, 0.5, df / (df + math.sqr(stat)));
            }
            bothtails = 2 * Math.Min(p, 1 - p);
            lefttail = p;
            righttail = 1 - p;
        }



    }
}
