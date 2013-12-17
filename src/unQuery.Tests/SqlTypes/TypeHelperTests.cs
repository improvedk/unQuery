using NUnit.Framework;
using System;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	[TestFixture]
	public class TypeHelperTests
	{
		[Test]
		public void GetDBNullableValue()
		{
			Assert.AreEqual(5, TypeHelper.GetDBNullableValue((int?)5));
			Assert.AreEqual("Test", TypeHelper.GetDBNullableValue("Test"));
			Assert.AreEqual(true, TypeHelper.GetDBNullableValue(true));
			Assert.AreEqual(false, TypeHelper.GetDBNullableValue(false));

			Assert.AreEqual(DBNull.Value, TypeHelper.GetDBNullableValue(null));
		}
	}
}