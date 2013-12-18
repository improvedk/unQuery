using Microsoft.SqlServer.Server;
using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlUniqueIdentifierTests : TestFixture
	{
		private Guid guid = Guid.NewGuid();

		[Test]
		public void Casting()
		{
			
			var col = (SqlUniqueIdentifier)guid;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.UniqueIdentifier, null, guid);
		}

		[Test]
		public void Constructor()
		{
			var col = new SqlUniqueIdentifier(guid);
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.UniqueIdentifier, null, guid);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(SqlUniqueIdentifier.GetParameter(guid), SqlDbType.UniqueIdentifier, null, guid);
			TestHelper.AssertSqlParameter(SqlUniqueIdentifier.GetParameter(null), SqlDbType.UniqueIdentifier, null, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			Assert.AreEqual(guid, new SqlUniqueIdentifier(guid).GetRawValue());
		}

		[Test]
		public void TypeHandler_GetInstance()
		{
			Assert.NotNull(SqlUniqueIdentifierTypeHandler.GetInstance());
		}

		[Test]
		public void TypeHandler_CreateParamFromValue()
		{
			var instance = SqlUniqueIdentifierTypeHandler.GetInstance();
			TestHelper.AssertSqlParameter(instance.CreateParamFromValue(guid), SqlDbType.UniqueIdentifier, null, guid);
			TestHelper.AssertSqlParameter(instance.CreateParamFromValue(null), SqlDbType.UniqueIdentifier, null, DBNull.Value);
		}

		[Test]
		public void TypeHandler_GetSqlDbType()
		{
			var instance = SqlUniqueIdentifierTypeHandler.GetInstance();
			Assert.AreEqual(SqlDbType.UniqueIdentifier, instance.GetSqlDbType());
		}

		[Test]
		public void TypeHandler_CreateSqlMetaData()
		{
			var instance = SqlUniqueIdentifierTypeHandler.GetInstance();

			var metaData = instance.CreateSqlMetaData("Test");
			Assert.AreEqual("Test", metaData.Name);
			Assert.AreEqual(SqlDbType.UniqueIdentifier, metaData.SqlDbType);
		}
	}
}