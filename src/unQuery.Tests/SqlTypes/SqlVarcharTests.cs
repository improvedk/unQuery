using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlVarcharTests : TestFixture
	{
		[Test]
		public void Casting()
		{
			var col = (SqlVarchar)"Test";
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.VarChar, 4, "Test");
		}

		[Test]
		public void Constructor()
		{
			var col = new SqlVarchar("Test");
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.VarChar, 4, "Test");
		}

		[Test]
		public void ExplicitSize()
		{
			var col = new SqlVarchar("Test", 10);
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.VarChar, 10, "Test");
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(SqlVarchar.GetParameter("Test"), SqlDbType.VarChar, 4, "Test");
			TestHelper.AssertSqlParameter(SqlVarchar.GetParameter("Test", 10), SqlDbType.VarChar, 10, "Test");
			TestHelper.AssertSqlParameter(SqlVarchar.GetParameter(null), SqlDbType.VarChar, null, DBNull.Value);
		}

		[Test]
		public void ParameterType()
		{
			TestHelper.AssertParameterFromValue(Col.Varchar("Test"), SqlDbType.VarChar, "Test", 4);
			TestHelper.AssertParameterFromValue(Col.Varchar("Test", 10), SqlDbType.VarChar, "Test", 10);
			TestHelper.AssertParameterFromValue(Col.Varchar(null), SqlDbType.VarChar, DBNull.Value);
		}
	}
}