using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlTinyIntTests : TestFixture
	{
		[Test]
		public void Casting()
		{
			var col = (SqlTinyInt)(byte)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.TinyInt, null, (byte)5);

			col = (SqlTinyInt)(short)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.TinyInt, null, (byte)5);

			col = (SqlTinyInt)5;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.TinyInt, null, (byte)5);

			col = (SqlTinyInt)5L;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.TinyInt, null, (byte)5);

		}

		[Test]
		public void Constructor()
		{
			var col = new SqlTinyInt(5);
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.TinyInt, null, (byte)5);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(SqlTinyInt.GetParameter(5), SqlDbType.TinyInt, null, (byte)5);
			TestHelper.AssertSqlParameter(SqlTinyInt.GetParameter(null), SqlDbType.TinyInt, null, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			Assert.AreEqual(1, new SqlTinyInt(1).GetRawValue());
		}

		[Test]
		public void GetSqlDbType()
		{
			Assert.AreEqual(SqlDbType.TinyInt, new SqlTinyInt(1).GetSqlDbType());
		}
	}
}