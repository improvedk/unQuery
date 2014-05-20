using NUnit.Framework;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.PerformanceTests
{
	public class GetScalerTests : TestFixture
	{
		[Test]
		public void GetScalar_NoParameters()
		{
			RunTest(1,
				() =>
				{
					using (var conn = GetOpenConnection())
					using (var cmd = new SqlCommand("SELECT 5", conn))
					{
						var x = (int) cmd.ExecuteScalar();
					}
				},
				() =>
				{
					var x = DB.GetScalar<int>("SELECT 5");
				}
			);
		}

		[Test]
		public void GetScalar_OneParameter()
		{
			RunTest(2.5,
				() =>
				{
					using (var conn = GetOpenConnection())
					using (var cmd = new SqlCommand("SELECT @Param", conn))
					{
						cmd.Parameters.Add("@Param", SqlDbType.Int).Value = 5;

						var x = (int)cmd.ExecuteScalar();
					}
				},
				() =>
				{
					var x = DB.GetScalar<int>("SELECT @Param", new { Param = 5 });
				}
			);
		}

		[Test]
		public void GetScalar_FiveParameters()
		{
			RunTest(2.5,
				() =>
				{
					using (var conn = GetOpenConnection())
					using (var cmd = new SqlCommand("SELECT @Param", conn))
					{
						cmd.Parameters.Add("@Param", SqlDbType.Int).Value = 5;
						cmd.Parameters.Add("@ParamB", SqlDbType.Int).Value = 6;
						cmd.Parameters.Add("@ParamC", SqlDbType.Int).Value = 7;
						cmd.Parameters.Add("@ParamD", SqlDbType.Int).Value = 8;
						cmd.Parameters.Add("@ParamE", SqlDbType.Int).Value = 9;

						var x = (int)cmd.ExecuteScalar();
					}
				},
				() =>
				{
					var x = DB.GetScalar<int>("SELECT @Param", new {
						Param = 5,
						ParamB = 6,
						ParamC = 7,
						ParamD = 8,
						ParamE = 9,
					});
				}
			);
		}
	}
}