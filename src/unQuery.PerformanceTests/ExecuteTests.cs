using NUnit.Framework;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using unQuery.SqlTypes;

namespace unQuery.PerformanceTests
{
	public class ExecuteTests : TestFixture
	{
		[Test]
		public TestResult NoParameters()
		{
			return RunTest(1,
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
		public TestResult OneParameter()
		{
			return RunTest(1.5,
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
		public TestResult FiveParameters()
		{
			return RunTest(2.75,
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