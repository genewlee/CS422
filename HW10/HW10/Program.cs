using System;
using System.Text.RegularExpressions;
using System.Numerics;

namespace CS422
{
    public class Program
    {
        static void Main()
        {
            //var num = "127.6948";
            //var blah = new BigNum(num);

            //var value = 127.6948;
            //var foo = new BigNum(value, false);

            var one = "-10.894";
            var two = "4.6828";
            var bone = new BigNum(one);
            var btwo = new BigNum(two);
            BigNum added = bone + btwo;

            var ne = ".00000278";
            var wo = ".00011";
            var bne = new BigNum(ne);
            var bwo = new BigNum(wo);
            BigNum dded = bne - bwo;

            var maxx = long.MaxValue;
            var maxxx = "9223372036854775806.5";
            var bmaxx = new BigNum(maxx.ToString());
            var bmaxxx = new BigNum(maxxx);
            BigNum b = bmaxx + bmaxxx;

            //bool h = added > dded;

            string s = dded.ToString();
            Regex criteria = new Regex(@"^-?\d*.?\d*$");
            if (!criteria.IsMatch(s) || string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentException();
            }
            s = added.ToString();
            if (!criteria.IsMatch(s) || string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentException();
            }
        }
    }
}
