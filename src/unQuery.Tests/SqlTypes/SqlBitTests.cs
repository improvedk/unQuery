using Microsoft.SqlServer.Server;
using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlBitTests : TestFixture
	{
		[Test]
		public void Casting()
		{
			var col = (SqlBit)true;

			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.Bit, null, true);
		}

		[Test]
		public void Constructor()
		{
			var col = new SqlBit(true);
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.Bit, null, true);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(SqlBit.GetParameter(true), SqlDbType.Bit, null, true);
			TestHelper.AssertSqlParameter(SqlBit.GetParameter(false), SqlDbType.Bit, null, false);
			TestHelper.AssertSqlParameter(SqlBit.GetParameter(null), SqlDbType.Bit, null, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			Assert.AreEqual(true, new SqlBit(true).GetRawValue());
		}

		[Test]
		public void TypeHandler_GetInstance()
		{
			Assert.NotNull(SqlBitTypeHandler.GetInstance());
		}

		[Test]
		public void TypeHandler_CreateParamFromValue()
		{
			var instance = SqlBitTypeHandler.GetInstance();
			TestHelper.AssertSqlParameter(instance.CreateParamFromValue(true), SqlDbType.Bit, null, true);
			TestHelper.AssertSqlParameter(instance.CreateParamFromValue(null), SqlDbType.Bit, null, DBNull.Value);
		}

		[Test]
		public void TypeHandler_GetSqlDbType()
		{
			var instance = SqlBitTypeHandler.GetInstance();
			Assert.AreEqual(SqlDbType.Bit, instance.GetSqlDbType());
		}

		[Test]
		public void TypeHandler_CreateSqlMetaData()
		{
			var instance = SqlBitTypeHandler.GetInstance();

			var metaData = instance.CreateSqlMetaData("Test");
			Assert.AreEqual("Test", metaData.Name);
			Assert.AreEqual(SqlDbType.Bit, metaData.SqlDbType);
		}
	}
}