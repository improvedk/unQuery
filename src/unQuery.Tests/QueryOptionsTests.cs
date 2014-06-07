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
	}
}