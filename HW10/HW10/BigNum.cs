using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Numerics;
using System.Collections;
using System.ComponentModel;

namespace CS422
{
    public class BigNum
    {
        BigInteger m_num, m_power;
        string m_number;
        bool isUndefined;
        bool isNegative;

        /// <summary>
        /// First Constructor - Instantiates a BigNum from a real number string.
        /// </summary>
        public BigNum(string number)
        {
            Regex criteria = new Regex(@"^-?\d*.?\d*$");
            if (!criteria.IsMatch(number) || string.IsNullOrWhiteSpace(number))
            {
                throw new ArgumentException();
            }

            m_number = number;
            StringtoBigNum(number);
        }

        /// <summary>
        /// Second Constructor
        /// </summary>
        public BigNum(double value, bool useDoubleToString)
        {
            if (double.IsNaN(value) || double.IsPositiveInfinity(value) || double.IsNegativeInfinity(value))
                isUndefined = true;

            else if (useDoubleToString)
            {
                string valueString = value.ToString();
                Regex criteria = new Regex(@"^-?\d*.?\d*$");
                if (!criteria.IsMatch(valueString) || string.IsNullOrWhiteSpace(valueString))
                {
                    throw new ArgumentException();
                }

                m_number = value.ToString();
                StringtoBigNum(value.ToString());
            }
            else // value is a real number and useDoubleToString is false
            {
                NotUseDoubleToString(value);
            }
        }

        /// <summary>
        /// Private helper constructor
        /// </summary>
        private BigNum (BigInteger num, BigInteger exp)
        {
            m_num = num;
            m_power = exp;
        }

        /// <summary>
        /// Takes a string and assigns the member variables of a BigNum
        /// </summary>
        private void StringtoBigNum (string number)
        {
            if (number[0] == '-')
            {
                isNegative = true;
            }

            if (number.Contains('.'))
                number = number.TrimEnd('0');

            int decIndex = number.IndexOf('.');
            m_power = decIndex == -1 ? 0 : decIndex - number.Length + 1;    // set exponent
            BigInteger.TryParse(String.Join("", number.Split('.')), out m_num); // set m_num
        }

        /// <summary>
        /// Called from Second constuctor to construct the number from bit array
        /// </summary>
        private void NotUseDoubleToString(double value)
        {
            BitArray bitArray = new BitArray(BitConverter.GetBytes(value));
            bool[] bits = new bool[64];

            for (int i = 0; i < 64; i++)        // Bits are backwards, reverse
                bits[i] = bitArray[63 - i];

            isNegative = bits[0];

            bool[] exp = new bool[11];
            bool[] frac = new bool[52];

            for (int i = 0; i < 11; i++)
            {
                exp[i] = bits[i + 1];
            }

            for (int i = 0; i < 52; i++)
            {
                frac[i] = bits[i + 12];
            }

            Array.Reverse(exp);
            Array.Reverse(frac);

            m_power = new BigInteger(GetLongFromBitArray(exp) - 1023);
            GetFrac(frac);
        }

        /// <summary>
        /// Gets the frac portion by running the ieee double-precision equation 
        /// </summary>
        private void GetFrac (bool[] array)
        {
            double num = 0;
            BigNum number = new BigNum(num.ToString());    // double.tostring() here is guarunteed to be lossless

            for (int i = 0; i < 52; i++)
            {
                if (array[51 - i])
                {
                    double val = 1.0;

                    for (int j = 0; j < (i + 1); j++)
                    {
                        val *= 2;
                    }

                    val = 1.0 / val; // the negative exponent

                    number += new BigNum(val.ToString("." + new string('#', 100000)));
                }
            }
            //num = (1 + num) * Pow(2, (int)m_power);
            //number = number + new BigNum(num.ToString("0." + new string('#', 339)));
            BigNum onePlusSummation = new BigNum("1".ToString()) + number;
            BigNum two_exp = new BigNum(Pow(2, (int)m_power).ToString("." + new string('#', 100000)));
            number = onePlusSummation * two_exp;
            //number = (new BigNum("1".ToString()) + number) * (new BigNum(two_exp.ToString()));
            m_num = isNegative ? -number.m_num : number.m_num;
            m_power = number.m_power;
        }

        /// <summary>
        /// Pow the specified num and exp. Works for negative exponents as well.
        /// </summary>
        private double Pow (int num, int exp)
        {
            double val = 1.0;

            for (int j = 0; j < Math.Abs(exp); j++)
            {
                val *= num;
            }

            if (exp < 0)
                val = 1.0 / val;

            return val;
        }

        /// <summary>
        /// Gets the long from bit array. Basically for exponent portion
        /// </summary>
        private long GetLongFromBitArray (bool[] bits)
        {
            //var array = new byte[8];
            //bits.CopyTo(array, 0);
            //return BitConverter.ToInt64(array, 0);

            long num = (long)0;
            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i])
                {
                    long mask = (long)(1 << i);
                    num |= mask;
                }
            }
            return num;
        }

        public override string ToString()
        {
            if (isUndefined)
                return "undefined";

            if (m_num == 0)
                return "0";

            string numString = m_num.ToString();
            int power = Math.Abs((int)m_power);
            if (m_power != 0)
            {
                if (numString.Length < power)
                {
                    if (numString[0] == '-')
                    {
                        numString = numString.Substring(1).PadLeft(power, '0');
                        numString = '-' + numString;
                    }
                    else
                        numString = numString.PadLeft(power, '0');
                }
            }
            numString = numString.Insert(numString.Length - power, ".").TrimEnd('0'); // insert the decimal point and trim any trailing zeros
            if (numString[numString.Length - 1] == '.')
                return numString.TrimEnd('.');
            return numString;
        }

        public bool IsUndefined { get { return isUndefined; } }

        public static BigNum operator +(BigNum lhs, BigNum rhs)
        {
            if (lhs.isUndefined || rhs.isUndefined)
                return new BigNum(double.NaN, true);

            int power = (int)(lhs.m_power > rhs.m_power ? rhs.m_power : lhs.m_power);
            BigInteger diffPower = BigInteger.Abs(lhs.m_power - rhs.m_power);
            BigInteger lhs_num = lhs.m_num;
            BigInteger rhs_num = rhs.m_num;
            if (lhs.m_power > power) // lhs power is less than
            {
                lhs_num *= BigInteger.Pow(10, (int)diffPower);
            }
            else
            {
                rhs_num *= BigInteger.Pow(10, (int)diffPower);
            }
            BigInteger b = lhs_num + rhs_num;

            power = Math.Abs(power);
            string bstring = b.ToString();

            if (bstring.Length < power)
            {
                if (bstring[0] == '-')
                {
                    bstring = bstring.Substring(1).PadLeft(power, '0');
                    bstring = '-' + bstring;
                }
                else
                    bstring = bstring.PadLeft(power, '0');
            }

            bstring = power != 0 ? bstring.Insert(bstring.Length - power, ".") : bstring;
            return new BigNum(bstring);
        }

        public static BigNum operator -(BigNum lhs, BigNum rhs)
        {
            if(lhs.isUndefined || rhs.isUndefined)
                return new BigNum(double.NaN, true);

            int power = (int)(lhs.m_power > rhs.m_power ? rhs.m_power : lhs.m_power);
            BigInteger diffPower = BigInteger.Abs(lhs.m_power - rhs.m_power);
            BigInteger lhs_num = lhs.m_num;
            BigInteger rhs_num = rhs.m_num;
            if (lhs.m_power > power) // lhs power is less than
            {
                lhs_num *= BigInteger.Pow(10, (int)diffPower);
            }
            else
            {
                rhs_num *= BigInteger.Pow(10, (int)diffPower);
            }
            BigInteger b = lhs_num - rhs_num;

            power = Math.Abs(power);
            string bstring = b.ToString();

            if (bstring.Length < power)
            {
                if (bstring[0] == '-')
                {
                    bstring = bstring.Substring(1).PadLeft(power, '0');
                    bstring = '-' + bstring;
                }
                else 
                    bstring = bstring.PadLeft(power, '0');
            }

            bstring = power != 0 ? bstring.Insert(bstring.Length - power, ".") : bstring;
            return new BigNum(bstring);
        }

        public static BigNum operator *(BigNum lhs, BigNum rhs)
        {
            if(lhs.isUndefined || rhs.isUndefined)
                return new BigNum(double.NaN, true);

            return new BigNum(lhs.m_num * rhs.m_num, lhs.m_power + rhs.m_power);
        }

        public static BigNum operator /(BigNum lhs, BigNum rhs)
        {
            if (lhs.isUndefined || rhs.isUndefined)
                return new BigNum(double.NaN, true);

            BigInteger numerator = lhs.m_num * BigInteger.Pow(10, 30);
            BigInteger numeratorExp = lhs.m_power - 30;

            BigInteger division = numerator / rhs.m_num;
            BigInteger newExp = numeratorExp - rhs.m_power;

            return new BigNum(division, newExp);
        }

        public static bool operator >(BigNum lhs, BigNum rhs)
        {
            BigNum b = rhs - lhs;
            return b.isNegative;
        }

        public static bool operator >=(BigNum lhs, BigNum rhs)
        {
            if (lhs.m_num == rhs.m_num)
            {
                if (lhs.m_power == rhs.m_power)
                {
                    return true;
                }
            }
            return lhs > rhs;
        }

        public static bool operator <(BigNum lhs, BigNum rhs)
        {
            BigNum b = lhs - rhs;
            return b.isNegative;
        }

        public static bool operator <=(BigNum lhs, BigNum rhs)
        {
            if (lhs.m_num == rhs.m_num)
            {
                if (lhs.m_power == rhs.m_power)
                {
                    return true;
                }
            }
            return lhs < rhs;
        }

        public static bool operator ==(BigNum lhs, BigNum rhs)
        {
            if (lhs.m_num == rhs.m_num)
            {
                if (lhs.m_power == rhs.m_power)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool operator !=(BigNum lhs, BigNum rhs)
        {
            if(lhs.m_num == rhs.m_num)
            {
                if (lhs.m_power == rhs.m_power)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Utility function that determines whether or not ToString for the specified value 
        /// generates an exact representation of the stored value.
        /// </summary>
        public static bool IsToStringCorrect(double value)
        {
            BigNum bn = new BigNum(value, false);
            string valueString = value.ToString();
            string bnString = bn.ToString();
            return valueString == bnString;
        }
    }
}
