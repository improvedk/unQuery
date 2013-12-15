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
			var col = (SqlSmallInt)(byte)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.SmallInt, null, (short)5);

			col = (SqlSmallInt)(short)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.SmallInt, null, (short)5);

			col = (SqlSmallInt)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.SmallInt, null, (short)5);

			col = (SqlSmallInt)5L;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.SmallInt, null, (short)5);
		}

		[Test]
		public void Constructor()
		{
			var col = new SqlSmallInt(5);
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.SmallInt, null, (short)5);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(SqlSmallInt.GetParameter(5), SqlDbType.SmallInt, null, (short)5);
			TestHelper.AssertSqlParameter(SqlSmallInt.GetParameter(null), SqlDbType.SmallInt, null, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			Assert.AreEqual(1, new SqlSmallInt(1).GetRawValue());
		}

		[Test]
		public void GetSqlDbType()
		{
			Assert.AreEqual(SqlDbType.SmallInt, new SqlSmallInt(1).GetSqlDbType());
		}
	}
}