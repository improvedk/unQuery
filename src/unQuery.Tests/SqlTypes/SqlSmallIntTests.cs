using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlSmallIntTests : TestFixture
	{
		[Test]
		public void Casting()
		{
			var col = (SqlSmallInt)(short)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.SmallInt, null, 5);

			col = (SqlSmallInt)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.SmallInt, null, 5);
		}

		[Test]
		public void Constructor()
		{
			var col = new SqlSmallInt(5);
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.SmallInt, null, 5);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(SqlSmallInt.GetParameter(5), SqlDbType.SmallInt, null, 5L);
			TestHelper.AssertSqlParameter(SqlSmallInt.GetParameter(null), SqlDbType.SmallInt, null, DBNull.Value);
		}
	}
}