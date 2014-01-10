using System.Data.SqlClient;
using Microsoft.SqlServer.Server;
using NUnit.Framework;
using System;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class TestType : SqlType
	{
		public new object GetDBNullableValue(object value)
		{
			return base.GetDBNullableValue(value);
		}

		public new int GetAppropriateSizeFromLength(int length)
		{
			return base.GetAppropriateSizeFromLength(length);
		}

		internal override SqlParameter GetParameter() { throw new NotImplementedException(); }
		internal override object GetRawValue() { throw new NotImplementedException(); }
		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal) { throw new NotImplementedException(); }
		internal override SqlParameter CreateParamFromValue(string name, object value) { throw new NotImplementedException(); }
		internal override SqlMetaData CreateMetaData(string name) { throw new NotImplementedException(); }
	}

	[TestFixture]
	public class SqlTypeTests
	{
		[Test]
		public void GetDBNullableValue()
		{
			var testType = new TestType();

			Assert.AreEqual(5, testType.GetDBNullableValue((int?)5));
			Assert.AreEqual("Test", testType.GetDBNullableValue("Test"));
			Assert.AreEqual(true, testType.GetDBNullableValue(true));
			Assert.AreEqual(false, testType.GetDBNullableValue(false));

			Assert.AreEqual(DBNull.Value, testType.GetDBNullableValue(null));
		}

		[Test]
		public void GetAppropriateSizeFromLength()
		{
			var testType = new TestType();

			Assert.AreEqual(64, testType.GetAppropriateSizeFromLength(0));
			Assert.AreEqual(64, testType.GetAppropriateSizeFromLength(1));
			Assert.AreEqual(64, testType.GetAppropriateSizeFromLength(64));
			Assert.AreEqual(256, testType.GetAppropriateSizeFromLength(65));
			Assert.AreEqual(256, testType.GetAppropriateSizeFromLength(256));
			Assert.AreEqual(1024, testType.GetAppropriateSizeFromLength(257));
			Assert.AreEqual(1024, testType.GetAppropriateSizeFromLength(1024));
			Assert.AreEqual(4096, testType.GetAppropriateSizeFromLength(1025));
			Assert.AreEqual(4096, testType.GetAppropriateSizeFromLength(4096));
			Assert.AreEqual(8000, testType.GetAppropriateSizeFromLength(4097));
			Assert.AreEqual(8000, testType.GetAppropriateSizeFromLength(8000));
			Assert.AreEqual(8000, testType.GetAppropriateSizeFromLength(8001));
		}
	}
}