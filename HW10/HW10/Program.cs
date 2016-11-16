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

            //var one = "10.894";
            //var two = "4.6828";
            //var bone = new BigNum(one);
            //var btwo = new BigNum(two);
            //BigNum added = bone + btwo;
            //BigNum divided = bone / btwo;

            BigNum myBigNum = new BigNum("3.1415");
            BigNum bn = new BigNum("3.004837");
            BigNum quotent = myBigNum / bn;

            BigNum num1 = new BigNum("1.254353");
            BigNum num2 = new BigNum(".0532112");
            BigNum quotient = num1 / num2;
            //var ne = ".00000278";
            //var wo = ".00011";
            //var bne = new BigNum(ne);
            //var bwo = new BigNum(wo);
            //BigNum dded = bne - bwo;
            //BigNum dd = bne + bwo;

            //var n = "3.14";
            //var w = "3.145";
            //var bn = new BigNum(n);
            //var bw = new BigNum(w);
            //BigNum dde = bn - bw;

            //BigNum p = bn + bw;

            ////var maxx = long.MaxValue;
            ////var maxxx = "9223372036854775806.5";
            ////var bmaxx = new BigNum(maxx.ToString());
            ////var bmaxxx = new BigNum(maxxx);
            ////BigNum b = bmaxx + bmaxxx;

            //bool h = dde <= dd;

            //string s = dded.ToString();
            //Regex criteria = new Regex(@"^-?\d*.?\d*$");
            //if (!criteria.IsMatch(s) || string.IsNullOrWhiteSpace(s))
            //{
            //    throw new ArgumentException();
            //}
            //s = added.ToString();
            //if (!criteria.IsMatch(s) || string.IsNullOrWhiteSpace(s))
            //{
            //    throw new ArgumentException();
            //}
            var foo = new BigNum(5, false);
            var t = foo.ToString();
            var c = BigNum.IsToStringCorrect(.6899999);

            var q = new BigNum(double.MaxValue, false);
            var w = new BigNum(double.MaxValue, false);
            string r = q.ToString();
            var e = q + w;
        }
    }
}
