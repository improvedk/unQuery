using Microsoft.SqlServer.Server;
using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlBigIntTests : TestFixture
	{
		[Test]
		public void Casting()
		{
			var col = (SqlBigInt)(byte)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.BigInt, null, 5L);

			col = (SqlBigInt)(short)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.BigInt, null, 5L);

			col = (SqlBigInt)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.BigInt, null, 5L);

			col = (SqlBigInt)5L;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.BigInt, null, 5L);
		}

		[Test]
		public void Constructor()
		{
			var col = new SqlBigInt(5);
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.BigInt, null, 5L);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(SqlBigInt.GetParameter(5L), SqlDbType.BigInt, null, 5L);
			TestHelper.AssertSqlParameter(SqlBigInt.GetParameter(null), SqlDbType.BigInt, null, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			Assert.AreEqual(1, new SqlBigInt(1).GetRawValue());
		}

		[Test]
		public void TypeHandler_GetInstance()
		{
			Assert.NotNull(SqlBigIntTypeHandler.GetInstance());
		}

		[Test]
		public void TypeHandler_CreateParamFromValue()
		{
			var instance = SqlBigIntTypeHandler.GetInstance();
			TestHelper.AssertSqlParameter(instance.CreateParamFromValue(5L), SqlDbType.BigInt, null, 5L);
			TestHelper.AssertSqlParameter(instance.CreateParamFromValue(null), SqlDbType.BigInt, null, DBNull.Value);
		}

		[Test]
		public void TypeHandler_GetSqlDbType()
		{
			var instance = SqlBigIntTypeHandler.GetInstance();
			Assert.AreEqual(SqlDbType.BigInt, instance.GetSqlDbType());
		}

		[Test]
		public void TypeHandler_CreateSqlMetaData()
		{
			var instance = SqlBigIntTypeHandler.GetInstance();

			var metaData = instance.CreateSqlMetaData("Test");
			Assert.AreEqual("Test", metaData.Name);
			Assert.AreEqual(SqlDbType.BigInt, metaData.SqlDbType);
		}

		[Test]
		public void TypeHandler_SetDataRecordValue()
		{
			var instance = SqlBigIntTypeHandler.GetInstance();

			var record = new SqlDataRecord(new SqlMetaData("A", SqlDbType.BigInt));
			instance.SetDataRecordValue(0, record, 5L);
			Assert.AreEqual(5L, record.GetValue(0));
		}
	}
}