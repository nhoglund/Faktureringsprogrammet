using System;
using NUnit.Framework;
using Fakturering;

namespace FaktureringTest
{
	[TestFixture()]
	public class SpecTest
	{
		[Test()]
		public void TestCurrency1()
		{
			Assert.AreEqual("1.00", Spec.Currency(1));
		}

		[Test()]
		public void TestCurrency2()
		{
			Assert.AreEqual("0.00", Spec.Currency(0));
		}
		
		[Test()]
		public void TestCurrency3()
		{
			Assert.AreEqual("0.33", Spec.Currency(0.33333333));
		}

		[Test()]
		public void TestRealVal1()
		{
			Assert.AreEqual(123.45, Spec.RealVal("123.45"));
		}

		[Test()]
		public void TestRealVal()
		{
			Assert.AreEqual(123.45, Spec.RealVal("123,45"));
		}

		static public void Main()
		{
		}

	}
}
