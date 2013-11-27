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
	}
}