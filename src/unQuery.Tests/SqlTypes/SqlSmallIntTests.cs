using Microsoft.SqlServer.Server;
using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlSmallIntTests : TestFixture
	{
		[Test]
		public void Casting()
		{
			var col = (SqlSmallInt)(byte)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.SmallInt, null, (short)5);

			col = (SqlSmallInt)(short)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.SmallInt, null, (short)5);

			col = (SqlSmallInt)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.SmallInt, null, (short)5);

			col = (SqlSmallInt)5L;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.SmallInt, null, (short)5);
		}

		[Test]
		public void Constructor()
		{
			var col = new SqlSmallInt(5);
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.SmallInt, null, (short)5);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(SqlSmallInt.GetParameter(5), SqlDbType.SmallInt, null, (short)5);
			TestHelper.AssertSqlParameter(SqlSmallInt.GetParameter(null), SqlDbType.SmallInt, null, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			Assert.AreEqual(1, new SqlSmallInt(1).GetRawValue());
		}

		[Test]
		public void TypeHandler_GetInstance()
		{
			Assert.NotNull(SqlSmallIntTypeHandler.GetInstance());
		}

		[Test]
		public void TypeHandler_CreateParamFromValue()
		{
			var instance = SqlSmallIntTypeHandler.GetInstance();
			TestHelper.AssertSqlParameter(instance.CreateParamFromValue((short)5), SqlDbType.SmallInt, null, (short)5);
			TestHelper.AssertSqlParameter(instance.CreateParamFromValue(null), SqlDbType.SmallInt, null, DBNull.Value);
		}

		[Test]
		public void TypeHandler_GetSqlDbType()
		{
			var instance = SqlSmallIntTypeHandler.GetInstance();
			Assert.AreEqual(SqlDbType.SmallInt, instance.GetSqlDbType());
		}

		[Test]
		public void TypeHandler_CreateSqlMetaData()
		{
			var instance = SqlSmallIntTypeHandler.GetInstance();

			var metaData = instance.CreateSqlMetaData("Test");
			Assert.AreEqual("Test", metaData.Name);
			Assert.AreEqual(SqlDbType.SmallInt, metaData.SqlDbType);
		}

		[Test]
		public void TypeHandler_SetDataRecordValue()
		{
			var instance = SqlSmallIntTypeHandler.GetInstance();

			var record = new SqlDataRecord(new SqlMetaData("A", SqlDbType.SmallInt));
			instance.SetDataRecordValue(0, record, (short)5);
			Assert.AreEqual((short)5, record.GetValue(0));
		}
	}
}