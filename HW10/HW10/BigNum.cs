using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CS422
{
    public class BigNum
    {
        BigInteger m_num, m_power;
        string m_number;
        bool isUndefined;
        bool isNegative;

        /// <summary>
        /// Instantiates a BigNum from a real number string.
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

        public BigNum(double value, bool useDoubleToString)
        {
            if (Double.IsNaN(value) || Double.IsPositiveInfinity(value) || Double.IsNegativeInfinity(value))
                isUndefined = true;

            if (useDoubleToString)
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
                DoubleToBigNum(value);
            }

        }

        private void StringtoBigNum (string number)
        {
            //if (number[0] == '-')
            //{
            //    isNegative = true;
            //    number = number.TrimStart('-');
            //}

            int decIndex = number.IndexOf('.');
            m_power = decIndex == -1 ? 0 : decIndex - number.Length + 1;
            BigInteger.TryParse(String.Join("", number.Split('.')), out m_num);
        }

        private void DoubleToBigNum(double value)
        {
            BitArray bitArray = new BitArray(BitConverter.GetBytes(value));
            bool[] bits = new bool[64];

            for (int i = 0; i < 64; i++)        // Bits are backwards, reverse
                bits[i] = bitArray[63 - i];

            isNegative = bitArray[0];

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
            //GetFrac(frac);
        }

        private void GetFrac (BitArray array)
        {
            List<long> eachBit = new List<long>();
            for (int i = 0; i < array.Length; i++)
            {
                long num = (long)0;
                if (array[i])
                {
                    long mask = (long)(1 << i);
                    num |= mask;
                }
                num = num * (long)Math.Pow(2, (int) 6);
                eachBit.Add(num);
            }
            m_num = eachBit.Sum();
        }

        //private void ReverseBitArray (BitArray array)
        //{
        //    int length = array.Length;
        //    int mid = (length / 2);

        //    for (int i = 0; i < mid; i++)
        //    {
        //        bool bit = array[i];
        //        array[i] = array[length - i - 1];
        //        array[length - i - 1] = bit;
        //    }
        //}

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
            return numString.Insert(numString.Length - power , ".");
        }

        public bool IsUndefined { get { return isUndefined; } }

        public static BigNum operator +(BigNum lhs, BigNum rhs)
        {
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
            BigInteger b = (lhs.m_num * rhs.m_num) * BigInteger.Pow(10, (int)(lhs.m_power + rhs.m_power));
            return new BigNum(b.ToString());
        }

        public static BigNum operator /(BigNum lhs, BigNum rhs)
        {
            throw new NotImplementedException();
        }

        public static bool operator >(BigNum lhs, BigNum rhs)
        {
            if (String.Compare(lhs.ToString(), rhs.ToString()) == 1)
                return true;
            return false;
        }

        public static bool operator >=(BigNum lhs, BigNum rhs)
        {
            if (String.Compare(lhs.ToString(), rhs.ToString()) == 1 || String.Compare(lhs.ToString(), rhs.ToString()) == 0)
                return true;
            return false;
        }

        public static bool operator <(BigNum lhs, BigNum rhs)
        {
            if (String.Compare(lhs.ToString(), rhs.ToString()) == -1)
                return true;
            return false;
        }

        public static bool operator <=(BigNum lhs, BigNum rhs)
        {
            if (String.Compare(lhs.ToString(), rhs.ToString()) == -1 || String.Compare(lhs.ToString(), rhs.ToString()) == 0)
                return true;
            return false;
        }

        public static bool operator ==(BigNum lhs, BigNum rhs)
        {
            return lhs.ToString() == rhs.ToString();
        }

        public static bool operator !=(BigNum lhs, BigNum rhs)
        {
            return lhs.ToString() != rhs.ToString();
        }

        public static bool IsToStringCorrect(double value)
        {
            return true;//value.ToString() == 
        }
    }
}
