using Microsoft.SqlServer.Server;
using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlNVarCharTests : TestFixture
	{
		[Test]
		public void Casting()
		{
			var col = (SqlNVarChar)"Test";
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.NVarChar, 4, "Test");
		}

		[Test]
		public void Constructor()
		{
			var col = new SqlNVarChar("Test");
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.NVarChar, 4, "Test");
		}

		[Test]
		public void ExplicitSize()
		{
			var col = new SqlNVarChar("Test", 10);
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.NVarChar, 10, "Test");
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(SqlNVarChar.GetParameter("Test"), SqlDbType.NVarChar, 4, "Test");
			TestHelper.AssertSqlParameter(SqlNVarChar.GetParameter("Test", 10), SqlDbType.NVarChar, 10, "Test");
			TestHelper.AssertSqlParameter(SqlNVarChar.GetParameter(null), SqlDbType.NVarChar, null, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			Assert.AreEqual("Test", new SqlNVarChar("Test").GetRawValue());
		}

		[Test]
		public void TypeHandler_GetInstance()
		{
			Assert.NotNull(SqlNVarCharTypeHandler.GetInstance());
		}

		[Test]
		public void TypeHandler_CreateParamFromValue()
		{
			var instance = SqlNVarCharTypeHandler.GetInstance();
			TestHelper.AssertSqlParameter(instance.CreateParamFromValue("язы́к"), SqlDbType.NVarChar, null, "язы́к");
			TestHelper.AssertSqlParameter(instance.CreateParamFromValue(null), SqlDbType.NVarChar, null, DBNull.Value);
		}

		[Test]
		public void TypeHandler_GetSqlDbType()
		{
			var instance = SqlNVarCharTypeHandler.GetInstance();
			Assert.AreEqual(SqlDbType.NVarChar, instance.GetSqlDbType());
		}

		[Test]
		public void TypeHandler_CreateSqlMetaData()
		{
			var instance = SqlNVarCharTypeHandler.GetInstance();

			var metaData = instance.CreateSqlMetaData("Test");
			Assert.AreEqual("Test", metaData.Name);
			Assert.AreEqual(SqlDbType.NVarChar, metaData.SqlDbType);
		}
	}
}