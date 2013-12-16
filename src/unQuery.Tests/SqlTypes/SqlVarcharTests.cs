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
	}
}