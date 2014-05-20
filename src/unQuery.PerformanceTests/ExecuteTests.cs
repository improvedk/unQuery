using NUnit.Framework;
using System;
using System.Data;
using System.Data.SqlClient;
using unQuery.SqlTypes;

namespace unQuery.PerformanceTests
{
	public class ExecuteTests : TestFixture
	{
		[Test]
		public void Execute_NoParameters()
		{
			RunTest(1,
				() =>
				{
					using (var conn = GetOpenConnection())
					using (var cmd = new SqlCommand("DECLARE @A int", conn))
						cmd.ExecuteNonQuery();
				},
				() => DB.Execute("DECLARE @A int")
			);
		}

		[Test]
		public void Execute_OneParameter()
		{
			RunTest(1.5,
				() =>
				{
					using (var conn = GetOpenConnection())
					using (var cmd = new SqlCommand("DECLARE @A int", conn))
					{
						cmd.Parameters.Add("@B", SqlDbType.Int).Value = 25;
						cmd.ExecuteNonQuery();
					}
				},
				() => DB.Execute("DECLARE @A int", new { B = 25 })
			);
		}

		[Test]
		public void Execute_FiveParameters()
		{
			RunTest(2.5,
				() =>
				{
					using (var conn = GetOpenConnection())
					using (var cmd = new SqlCommand("DECLARE @A int", conn))
					{
						cmd.Parameters.Add("@B", SqlDbType.Int).Value = 25;
						cmd.Parameters.Add("@C", SqlDbType.DateTime).Value = DateTime.Now;
						cmd.Parameters.Add("@D", SqlDbType.Bit).Value = true;
						cmd.Parameters.Add("@E", SqlDbType.VarChar, 500).Value = "Hello world!";
						cmd.Parameters.Add("@F", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
						cmd.ExecuteNonQuery();
					}
				},
				() => DB.Execute("DECLARE @A int", new {
					B = 25,
					C = Col.DateTime(DateTime.Now),
					D = true,
					E = Col.VarChar("Hello world!", 500),
					F = Guid.NewGuid()
				})
			);
		}
	}
}