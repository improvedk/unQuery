using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlUniqueIdentifierTests : TestFixture
	{
		private Guid guid = Guid.NewGuid();

		[Test]
		public void Casting()
		{
			
			var col = (SqlUniqueIdentifier)guid;
			TestHelper.AssertSqlParameter(col.GetParameter(), SqlDbType.UniqueIdentifier, null, guid);
		}

		[Test]
		public void Constructor()
		{
			var col = new SqlUniqueIdentifier(guid);
			var param = col.GetParameter();

			TestHelper.AssertSqlParameter(param, SqlDbType.UniqueIdentifier, null, guid);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(SqlUniqueIdentifier.GetParameter(guid), SqlDbType.UniqueIdentifier, null, guid);
			TestHelper.AssertSqlParameter(SqlUniqueIdentifier.GetParameter(null), SqlDbType.UniqueIdentifier, null, DBNull.Value);
		}
	}
}