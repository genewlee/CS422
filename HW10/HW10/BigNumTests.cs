using NUnit.Framework;
using CS422;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS422.Tests
{
  [TestFixture()]
  public class BigNumTests
  {

    [Test()]
    public void ToStringTest()
    {
      BigNum num1 = new BigNum("1.25");
      Assert.That(num1.ToString() == "1.25");
      num1 = new BigNum(".00025");
      Assert.That(num1.ToString() == ".00025");
      num1 = new BigNum("-.00025");
      Assert.That(num1.ToString() == "-.00025");
      num1 = new BigNum("345734");
      Assert.That(num1.ToString() == "345734");
    }

    [Test()]
    public void Add()
    {
      BigNum num1 = new BigNum("1.25");
      BigNum num2 = new BigNum(".05");
      Assert.That((num1 + num2).ToString() == "1.3");

      num1 = new BigNum("50000");
      num2 = new BigNum(".02345");
      Assert.That((num1 + num2).ToString() == "50000.02345");
    }

    [Test()]
    public void Subtract()
    {
      BigNum num1 = new BigNum("1.25");
      BigNum num2 = new BigNum(".05");
      Assert.That((num1 - num2).ToString() == "1.2");

      num1 = new BigNum("50001");
      num2 = new BigNum(".01");
      Assert.That((num1 - num2).ToString() == "50000.99");
    }

    [Test()]
    public void Multiply()
    {
      BigNum num1 = new BigNum("13");
      BigNum num2 = new BigNum("4");
      Assert.That((num1 * num2).ToString() == "52");

      num1 = new BigNum("500");
      num2 = new BigNum(".01");
      Assert.That((num1 * num2).ToString() == "5");
    }

    [Test()]
    public void Divide()
    {
      BigNum num1 = new BigNum("1.254353");
      BigNum num2 = new BigNum(".0532112");

      Assert.That((num1 / num2).ToString() == "23.57310115163724930089905884475");
    }

    [Test()]
    public void quotent()
    {
      BigNum myBigNum = new BigNum("3.1415");
      BigNum bn = new BigNum("3.004837");
      BigNum quotient = myBigNum / bn;
      string s = quotient.ToString();
      Assert.AreEqual(s, "1.0454810027964911241441715474");
    }
  }
}