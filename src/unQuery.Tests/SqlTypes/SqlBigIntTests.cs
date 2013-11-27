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

		public void Constructor()
		{
			var col = new SqlBigInt(5);
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.BigInt, null, 5L);
		}

		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(SqlBigInt.GetParameter(5), SqlDbType.BigInt, null, 5L);
			TestHelper.AssertSqlParameter(SqlBigInt.GetParameter(null), SqlDbType.BigInt, null, DBNull.Value);
		}

		[Test]
		public void ParameterType()
		{
			TestHelper.AssertParameterFromValue(Col.BigInt(5), SqlDbType.BigInt, 5L);
			TestHelper.AssertParameterFromValue(Col.BigInt(null), SqlDbType.BigInt, DBNull.Value);
		}
	}
}