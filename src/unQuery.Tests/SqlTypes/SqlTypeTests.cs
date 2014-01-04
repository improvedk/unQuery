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

		[Test]
		public void GetAppropriateSizeFromLength()
		{
			Assert.AreEqual(64, SqlType.GetAppropriateSizeFromLength(0));
			Assert.AreEqual(64, SqlType.GetAppropriateSizeFromLength(1));
			Assert.AreEqual(64, SqlType.GetAppropriateSizeFromLength(64));
			Assert.AreEqual(256, SqlType.GetAppropriateSizeFromLength(65));
			Assert.AreEqual(256, SqlType.GetAppropriateSizeFromLength(256));
			Assert.AreEqual(1024, SqlType.GetAppropriateSizeFromLength(257));
			Assert.AreEqual(1024, SqlType.GetAppropriateSizeFromLength(1024));
			Assert.AreEqual(4096, SqlType.GetAppropriateSizeFromLength(1025));
			Assert.AreEqual(4096, SqlType.GetAppropriateSizeFromLength(4096));
			Assert.AreEqual(8000, SqlType.GetAppropriateSizeFromLength(4097));
			Assert.AreEqual(8000, SqlType.GetAppropriateSizeFromLength(8000));
			Assert.AreEqual(8000, SqlType.GetAppropriateSizeFromLength(8001));
		}
	}
}