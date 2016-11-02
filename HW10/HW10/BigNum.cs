using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace CS422
{
    public class BigNum
    {
        BigInteger m_num, m_power;
        bool isUndefined;

        /// <summary>
        /// Instantiates a BigNum from a real number string.
        /// </summary>
        public BigNum(string number)
        {
            Regex startsWith = new Regex(@"^([0 - 9] | - | .){ 1 }");
            if (!startsWith.IsMatch(number) || number.Any(char.IsWhiteSpace) || string.IsNullOrWhiteSpace(number))
            {
                throw new ArgumentException();
            }

            BigInteger numBig;
            BigInteger.TryParse(number, out numBig);
            var binary = numBig.ToByteArray();
        }

        public BigNum(double value, bool useDoubleToString)
        {
            if (Double.IsNaN(value) || Double.IsPositiveInfinity(value) || Double.IsNegativeInfinity(value))
                isUndefined = true;

            if (useDoubleToString)
            {
                string valueString = value.ToString();
                Regex startsWith = new Regex(@"^([0 - 9] | -|.){ 1 }");
                if (!startsWith.IsMatch(valueString) || valueString.Any(char.IsWhiteSpace) || string.IsNullOrWhiteSpace(valueString)
                    || valueString.Substring(1).Contains('.') || valueString.Substring(1).Contains('-'))
                {
                    throw new ArgumentException();
                }

                BigInteger.TryParse(valueString, out m_num);
            }
            else
            {
                throw new NotImplementedException();
            }

        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public bool IsUndefined { get { return isUndefined; } }

        public static BigNum operator +(BigNum lhs, BigNum rhs)
        {
            throw new NotImplementedException();
        }

        public static BigNum operator -(BigNum lhs, BigNum rhs)
        {
            throw new NotImplementedException();
        }

        public static BigNum operator *(BigNum lhs, BigNum rhs)
        {
            throw new NotImplementedException();
        }

        public static BigNum operator /(BigNum lhs, BigNum rhs)
        {
            throw new NotImplementedException();
        }

        public static bool operator >(BigNum lhs, BigNum rhs)
        {
            return lhs > rhs;
        }

        public static bool operator >=(BigNum lhs, BigNum rhs)
        {
            //return lhs >= rhs;
            throw new NotImplementedException();
        }

        public static bool operator <(BigNum lhs, BigNum rhs)
        {
            return lhs < rhs;
        }

        public static bool operator <=(BigNum lhs, BigNum rhs)
        {
            //return lhs >= rhs;
            throw new NotImplementedException();
        }

        public static bool IsToStringCorrect(double value)
        {
            throw new NotImplementedException();
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ulongDoubleUnion
        {
            [FieldOffset(0)]
            public double D;

            [FieldOffset(0)]
            public ulong U;
        }
    }
}
