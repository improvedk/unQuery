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
			var col = (SqlBigInt)5;

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
			TestHelper.AssertSqlParameter(SqlBigInt.GetParameter(5), SqlDbType.BigInt, null, 5L);
			TestHelper.AssertSqlParameter(SqlBigInt.GetParameter(null), SqlDbType.BigInt, null, DBNull.Value);
		}
	}
}