using NUnit.Framework;
using System.Data;
using Microsoft.Data.SqlClient;

namespace unQuery.Tests
{
	public class GetOpenConnectionTests : TestFixture
	{
		[Test]
		public void GetOpenConnection()
		{
			using (var conn = DB.GetOpenConnection())
			{
				Assert.AreEqual(ConnectionState.Open, conn.State);

				var cmd = new SqlCommand("SELECT COUNT(*) FROM Persons WHERE PersonID = 1", conn);

				Assert.AreEqual(1, (int)cmd.ExecuteScalar());
			}
		}
	}
}