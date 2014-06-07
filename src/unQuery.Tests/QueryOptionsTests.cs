using NUnit.Framework;
using System.Data.SqlClient;

namespace unQuery.Tests
{
	public class QueryOptionsTests : TestFixture
	{
		[Test]
		public void CommandTimeout()
		{
			DB.Execute("WAITFOR DELAY '00:00:02'");

			Assert.Throws<SqlException>(() => DB.Execute("WAITFOR DELAY '00:00:02'", options: new QueryOptions {
				CommandTimeout = 1
			}));
		}

		[Test]
		public void CommandType()
		{
			int rowCount = DB.GetRows("sp_server_info", new { Attribute_ID = 1 }).Count;
			Assert.Greater(rowCount, 0);

			rowCount = DB.GetRows("sp_server_info", new { Attribute_ID = 1 }, new QueryOptions {
				CommandType = System.Data.CommandType.StoredProcedure
			}).Count;
			Assert.AreEqual(1, rowCount);
		}
	}
}