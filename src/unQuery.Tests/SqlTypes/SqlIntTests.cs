using Microsoft.SqlServer.Server;
using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlIntTests : TestFixture
	{
		[Test]
		public void Casting()
		{
			var col = (SqlInt)(byte)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.Int, null, 5);

			col = (SqlInt)(short)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.Int, null, 5);

			col = (SqlInt)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.Int, null, 5);

			col = (SqlInt)5L;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.Int, null, 5);
		}

		[Test]
		public void Constructor()
		{
			var col = new SqlInt(5);
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.Int, null, 5);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(SqlInt.GetParameter(5), SqlDbType.Int, null, 5);
			TestHelper.AssertSqlParameter(SqlInt.GetParameter(null), SqlDbType.Int, null, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			Assert.AreEqual(1, new SqlInt(1).GetRawValue());
		}

		[Test]
		public void TypeHandler_GetInstance()
		{
			Assert.NotNull(SqlIntTypeHandler.GetInstance());
		}

		[Test]
		public void TypeHandler_CreateParamFromValue()
		{
			var instance = SqlIntTypeHandler.GetInstance();
			TestHelper.AssertSqlParameter(instance.CreateParamFromValue(5), SqlDbType.Int, null, 5);
			TestHelper.AssertSqlParameter(instance.CreateParamFromValue(null), SqlDbType.Int, null, DBNull.Value);
		}

		[Test]
		public void TypeHandler_GetSqlDbType()
		{
			var instance = SqlIntTypeHandler.GetInstance();
			Assert.AreEqual(SqlDbType.Int, instance.GetSqlDbType());
		}

		[Test]
		public void TypeHandler_CreateSqlMetaData()
		{
			var instance = SqlIntTypeHandler.GetInstance();

			var metaData = instance.CreateSqlMetaData("Test");
			Assert.AreEqual("Test", metaData.Name);
			Assert.AreEqual(SqlDbType.Int, metaData.SqlDbType);
		}

		[Test]
		public void TypeHandler_SetDataRecordValue()
		{
			var instance = SqlIntTypeHandler.GetInstance();

			var record = new SqlDataRecord(new SqlMetaData("A", SqlDbType.Int));
			instance.SetDataRecordValue(0, record, 5);
			Assert.AreEqual(5, record.GetValue(0));
		}
	}
}