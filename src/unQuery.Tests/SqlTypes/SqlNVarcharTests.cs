using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlNVarcharTests : TestFixture
	{
		[Test]
		public void Casting()
		{
			var col = (SqlNVarchar)"Test";
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.NVarChar, 4, "Test");
		}

		[Test]
		public void Constructor()
		{
			var col = new SqlNVarchar("Test");
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.NVarChar, 4, "Test");
		}

		[Test]
		public void ExplicitSize()
		{
			var col = new SqlNVarchar("Test", 10);
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.NVarChar, 10, "Test");
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(SqlNVarchar.GetParameter("Test"), SqlDbType.NVarChar, 4, "Test");
			TestHelper.AssertSqlParameter(SqlNVarchar.GetParameter("Test", 10), SqlDbType.NVarChar, 10, "Test");
			TestHelper.AssertSqlParameter(SqlNVarchar.GetParameter(null), SqlDbType.NVarChar, null, DBNull.Value);
		}
	}
}