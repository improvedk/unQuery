using Microsoft.SqlServer.Server;
using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlVarCharTests : TestFixture
	{
		[Test]
		public void Casting()
		{
			var col = (SqlVarChar)"Test";
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.VarChar, 4, "Test");
		}

		[Test]
		public void Constructor()
		{
			var col = new SqlVarChar("Test");
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.VarChar, 4, "Test");
		}

		[Test]
		public void ExplicitSize()
		{
			var col = new SqlVarChar("Test", 10);
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.VarChar, 10, "Test");
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(SqlVarChar.GetParameter("Test"), SqlDbType.VarChar, 4, "Test");
			TestHelper.AssertSqlParameter(SqlVarChar.GetParameter("Test", 10), SqlDbType.VarChar, 10, "Test");
			TestHelper.AssertSqlParameter(SqlVarChar.GetParameter(null), SqlDbType.VarChar, null, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			Assert.AreEqual("Test", new SqlVarChar("Test").GetRawValue());
		}

		[Test]
		public void TypeHandler_GetInstance()
		{
			Assert.NotNull(SqlVarCharTypeHandler.GetInstance());
		}

		[Test]
		public void TypeHandler_CreateParamFromValue()
		{
			var instance = SqlVarCharTypeHandler.GetInstance();
			TestHelper.AssertSqlParameter(instance.CreateParamFromValue("Test"), SqlDbType.VarChar, null, "Test");
			TestHelper.AssertSqlParameter(instance.CreateParamFromValue(null), SqlDbType.VarChar, null, DBNull.Value);
		}

		[Test]
		public void TypeHandler_GetSqlDbType()
		{
			var instance = SqlVarCharTypeHandler.GetInstance();
			Assert.AreEqual(SqlDbType.VarChar, instance.GetSqlDbType());
		}

		[Test]
		public void TypeHandler_CreateSqlMetaData()
		{
			var instance = SqlVarCharTypeHandler.GetInstance();

			var metaData = instance.CreateSqlMetaData("Test");
			Assert.AreEqual("Test", metaData.Name);
			Assert.AreEqual(SqlDbType.VarChar, metaData.SqlDbType);
		}

		[Test]
		public void TypeHandler_SetDataRecordValue()
		{
			var instance = SqlVarCharTypeHandler.GetInstance();

			var record = new SqlDataRecord(new SqlMetaData("A", SqlDbType.VarChar, -1));
			instance.SetDataRecordValue(0, record, "Test");
			Assert.AreEqual("Test", record.GetValue(0));
		}
	}
}