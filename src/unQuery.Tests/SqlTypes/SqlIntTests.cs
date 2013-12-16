using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlIntTests : TestFixture
	{
		[Test]
		public void Casting()
		{
			var col = (SqlInt)(byte)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.Int, null, 5);

			col = (SqlInt)(short)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.Int, null, 5);

			col = (SqlInt)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.Int, null, 5);

			col = (SqlInt)5L;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.Int, null, 5);
		}

		[Test]
		public void Constructor()
		{
			var col = new SqlInt(5);
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.Int, null, 5);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(SqlInt.GetParameter(5), SqlDbType.Int, null, 5);
			TestHelper.AssertSqlParameter(SqlInt.GetParameter(null), SqlDbType.Int, null, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			Assert.AreEqual(1, new SqlInt(1).GetRawValue());
		}
	}
}