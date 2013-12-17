using NUnit.Framework;
using System;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	[TestFixture]
	public class SqlTypeTests
	{
		[Test]
		public void GetDBNullableValue()
		{
			Assert.AreEqual(5, SqlType.GetDBNullableValue((int?)5));
			Assert.AreEqual("Test", SqlType.GetDBNullableValue("Test"));
			Assert.AreEqual(true, SqlType.GetDBNullableValue(true));
			Assert.AreEqual(false, SqlType.GetDBNullableValue(false));

			Assert.AreEqual(DBNull.Value, SqlType.GetDBNullableValue(null));
		}
	}
}