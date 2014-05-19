using NUnit.Framework;
using System;
using System.Data.SqlClient;

namespace unQuery.PerformanceTests
{
	public class GetRowTests : TestFixture
	{
		[Test]
		public void GetRow_Typed_NoParameters()
		{
			RunTest(6,
				() =>
				{
					using (var conn = GetOpenConnection())
					using (var cmd = new SqlCommand("SELECT name, create_date, object_id, schema_id, type FROM sys.all_objects WHERE name = 'sysrowsets'", conn))
					{
						var reader = cmd.ExecuteReader();
						reader.Read();

						var obj = new SysAllObject {
							name = (string)reader["name"],
							create_date = (DateTime)reader["create_date"],
							object_id = (int)reader["object_id"],
							schema_id = (int)reader["schema_id"],
							type = (string)reader["type"]
						};
					}
				},
				() => {
					var obj = DB.GetRow<SysAllObject>("SELECT name, create_date, object_id, schema_id, type FROM sys.all_objects WHERE name = 'sysrowsets'");
				}
			);
		}

		[Test]
		public void GetRow_Dynamic_NoParameters()
		{
			RunTest(2,
				() =>
				{
					using (var conn = GetOpenConnection())
					using (var cmd = new SqlCommand("SELECT name, create_date, object_id, schema_id, type FROM sys.all_objects WHERE name = 'sysrowsets'", conn))
					{
						var reader = cmd.ExecuteReader();
						reader.Read();

						dynamic obj = new {
							name = (string)reader["name"],
							create_date = (DateTime)reader["create_date"],
							object_id = (int)reader["object_id"],
							schema_id = (int)reader["schema_id"],
							type = (string)reader["type"]
						};
					}
				},
				() => {
					var obj = DB.GetRow("SELECT name, create_date, object_id, schema_id, type FROM sys.all_objects WHERE name = 'sysrowsets'");
				}
			);
		}

		[Test]
		public void GetRow_Dynamic_Parameters()
		{
			RunTest(1,
				() =>
				{
					using (var conn = GetOpenConnection())
					using (var cmd = new SqlCommand("SELECT 'name' AS name, getDate() AS create_date, 25 AS object_id, 27 AS schema_id, 'type' AS type", conn))
					{
						var reader = cmd.ExecuteReader();
						reader.Read();

						dynamic obj = new {
							name = (string)reader["name"],
							create_date = (DateTime)reader["create_date"],
							object_id = (int)reader["object_id"],
							schema_id = (int)reader["schema_id"],
							type = (string)reader["type"]
						};
					}
				},
				() => {
					var obj = DB.GetRow("SELECT 'name' AS name, getDate() AS create_date, 25 AS object_id, 27 AS schema_id, 'type' AS type");
				}
			);
		}
	}
}