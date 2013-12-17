using Microsoft.SqlServer.Server;
using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlTinyIntTests : TestFixture
	{
		[Test]
		public void Casting()
		{
			var col = (SqlTinyInt)(byte)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.TinyInt, null, (byte)5);

			col = (SqlTinyInt)(short)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.TinyInt, null, (byte)5);

			col = (SqlTinyInt)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.TinyInt, null, (byte)5);

			col = (SqlTinyInt)5L;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.TinyInt, null, (byte)5);

		}

		[Test]
		public void Constructor()
		{
			var col = new SqlTinyInt(5);
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.TinyInt, null, (byte)5);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(SqlTinyInt.GetParameter(5), SqlDbType.TinyInt, null, (byte)5);
			TestHelper.AssertSqlParameter(SqlTinyInt.GetParameter(null), SqlDbType.TinyInt, null, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			Assert.AreEqual(1, new SqlTinyInt(1).GetRawValue());
		}

		[Test]
		public void TypeHandler_GetInstance()
		{
			Assert.NotNull(SqlTinyIntTypeHandler.GetInstance());
		}

		[Test]
		public void TypeHandler_CreateParamFromValue()
		{
			var instance = SqlTinyIntTypeHandler.GetInstance();
			TestHelper.AssertSqlParameter(instance.CreateParamFromValue((byte)5), SqlDbType.TinyInt, null, (byte)5);
			TestHelper.AssertSqlParameter(instance.CreateParamFromValue(null), SqlDbType.TinyInt, null, DBNull.Value);
		}

		[Test]
		public void TypeHandler_GetSqlDbType()
		{
			var instance = SqlTinyIntTypeHandler.GetInstance();
			Assert.AreEqual(SqlDbType.TinyInt, instance.GetSqlDbType());
		}

		[Test]
		public void TypeHandler_CreateSqlMetaData()
		{
			var instance = SqlTinyIntTypeHandler.GetInstance();

			var metaData = instance.CreateSqlMetaData("Test");
			Assert.AreEqual("Test", metaData.Name);
			Assert.AreEqual(SqlDbType.TinyInt, metaData.SqlDbType);
		}

		[Test]
		public void TypeHandler_SetDataRecordValue()
		{
			var instance = SqlTinyIntTypeHandler.GetInstance();

			var record = new SqlDataRecord(new SqlMetaData("A", SqlDbType.TinyInt));
			instance.SetDataRecordValue(0, record, (byte)5);
			Assert.AreEqual((byte)5, record.GetValue(0));
		}
	}
}