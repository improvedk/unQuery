using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using unQuery.SqlTypes;

namespace unQuery.PerformanceTests
{
	public class GetRowsTests : TestFixture
	{
		[Test]
		public TestResult Typed_NoResults()
		{
			return RunTest(9,
				() =>
				{
					using (var conn = GetOpenConnection())
					using (var cmd = new SqlCommand("SELECT name, create_date, object_id, schema_id, type FROM sys.all_objects WHERE 1=0", conn))
					{
						cmd.Parameters.Add("@DummyA", SqlDbType.VarChar, 100).Value = "asd";
						cmd.Parameters.Add("@DummyB", SqlDbType.Int).Value = 25;
						cmd.Parameters.Add("@DummyC", SqlDbType.Bit).Value = DBNull.Value;

						var reader = cmd.ExecuteReader();

						var results = new List<SysAllObject>();
						while (reader.Read())
							throw new Exception("Shouldn't be here!");
					}
				},
				() => {
					var obj = DB.GetRows<SysAllObject>("SELECT name, create_date, object_id, schema_id, type FROM sys.all_objects WHERE 1=0", new {
						DummyA = Col.VarChar("asd", 100),
						DummyB = 25,
						DummyC = (bool?)null
					});
				}
			);
		}

		[Test]
		public TestResult Typed_1Result()
		{
			return RunTest(7.5,
				() =>
				{
					using (var conn = GetOpenConnection())
					using (var cmd = new SqlCommand("SELECT TOP 1 name, create_date, object_id, schema_id, type FROM sys.all_objects ORDER BY name", conn))
					{
						cmd.Parameters.Add("@DummyA", SqlDbType.VarChar, 100).Value = "asd";
						cmd.Parameters.Add("@DummyB", SqlDbType.Int).Value = 25;
						cmd.Parameters.Add("@DummyC", SqlDbType.Bit).Value = DBNull.Value;

						var reader = cmd.ExecuteReader();

						var results = new List<SysAllObject>();
						while (reader.Read())
						{
							results.Add(new SysAllObject {
								name = (string) reader["name"],
								create_date = (DateTime) reader["create_date"],
								object_id = (int) reader["object_id"],
								schema_id = (int) reader["schema_id"],
								type = (string) reader["type"]
							});
						}
					}
				},
				() => {
					var obj = DB.GetRows<SysAllObject>("SELECT TOP 1 name, create_date, object_id, schema_id, type FROM sys.all_objects ORDER BY name", new {
						DummyA = Col.VarChar("asd", 100),
						DummyB = 25,
						DummyC = (bool?)null
					});
				}
			);
		}

		[Test]
		public TestResult Typed_10Results()
		{
			return RunTest(5.25,
				() =>
				{
					using (var conn = GetOpenConnection())
					using (var cmd = new SqlCommand("SELECT TOP 10 name, create_date, object_id, schema_id, type FROM sys.all_objects ORDER BY name", conn))
					{
						cmd.Parameters.Add("@DummyA", SqlDbType.VarChar, 100).Value = "asd";
						cmd.Parameters.Add("@DummyB", SqlDbType.Int).Value = 25;
						cmd.Parameters.Add("@DummyC", SqlDbType.Bit).Value = DBNull.Value;

						var reader = cmd.ExecuteReader();

						var results = new List<SysAllObject>();
						while (reader.Read())
						{
							results.Add(new SysAllObject {
								name = (string) reader["name"],
								create_date = (DateTime) reader["create_date"],
								object_id = (int) reader["object_id"],
								schema_id = (int) reader["schema_id"],
								type = (string) reader["type"]
							});
						}
					}
				},
				() => {
					var obj = DB.GetRows<SysAllObject>("SELECT TOP 10 name, create_date, object_id, schema_id, type FROM sys.all_objects ORDER BY name", new {
						DummyA = Col.VarChar("asd", 100),
						DummyB = 25,
						DummyC = (bool?)null
					});
				}
			);
		}

		[Test]
		public TestResult Dynamic_NoResults()
		{
			return RunTest(4.5,
				() =>
				{
					using (var conn = GetOpenConnection())
					using (var cmd = new SqlCommand("SELECT name, create_date, object_id, schema_id, type FROM sys.all_objects WHERE 1=0", conn))
					{
						cmd.Parameters.Add("@DummyA", SqlDbType.VarChar, 100).Value = "asd";
						cmd.Parameters.Add("@DummyB", SqlDbType.Int).Value = 25;
						cmd.Parameters.Add("@DummyC", SqlDbType.Bit).Value = DBNull.Value;

						var reader = cmd.ExecuteReader();

						var results = new List<SysAllObject>();
						while (reader.Read())
							throw new Exception("Shouldn't be here!");
					}
				},
				() => {
					var obj = DB.GetRows("SELECT name, create_date, object_id, schema_id, type FROM sys.all_objects WHERE 1=0", new {
						DummyA = Col.VarChar("asd", 100),
						DummyB = 25,
						DummyC = (bool?)null
					});
				}
			);
		}

		[Test]
		public TestResult Dynamic_1Result()
		{
			return RunTest(7.5,
				() =>
				{
					using (var conn = GetOpenConnection())
					using (var cmd = new SqlCommand("SELECT TOP 1 name, create_date, object_id, schema_id, type FROM sys.all_objects ORDER BY name", conn))
					{
						cmd.Parameters.Add("@DummyA", SqlDbType.VarChar, 100).Value = "asd";
						cmd.Parameters.Add("@DummyB", SqlDbType.Int).Value = 25;
						cmd.Parameters.Add("@DummyC", SqlDbType.Bit).Value = DBNull.Value;

						var reader = cmd.ExecuteReader();

						var results = new List<dynamic>();
						while (reader.Read())
						{
							results.Add(new {
								name = (string) reader["name"],
								create_date = (DateTime) reader["create_date"],
								object_id = (int) reader["object_id"],
								schema_id = (int) reader["schema_id"],
								type = (string) reader["type"]
							});
						}
					}
				},
				() => {
					var obj = DB.GetRows<SysAllObject>("SELECT TOP 1 name, create_date, object_id, schema_id, type FROM sys.all_objects ORDER BY name", new {
						DummyA = Col.VarChar("asd", 100),
						DummyB = 25,
						DummyC = (bool?)null
					});
				}
			);
		}

		[Test]
		public TestResult Dynamic_10Results()
		{
			return RunTest(-0.5,
				() =>
				{
					using (var conn = GetOpenConnection())
					using (var cmd = new SqlCommand("SELECT TOP 10 name, create_date, object_id, schema_id, type FROM sys.all_objects ORDER BY name", conn))
					{
						cmd.Parameters.Add("@DummyA", SqlDbType.VarChar, 100).Value = "asd";
						cmd.Parameters.Add("@DummyB", SqlDbType.Int).Value = 25;
						cmd.Parameters.Add("@DummyC", SqlDbType.Bit).Value = DBNull.Value;

						var reader = cmd.ExecuteReader();

						var results = new List<dynamic>();
						while (reader.Read())
						{
							results.Add(new {
								name = (string) reader["name"],
								create_date = (DateTime) reader["create_date"],
								object_id = (int) reader["object_id"],
								schema_id = (int) reader["schema_id"],
								type = (string) reader["type"]
							});
						}
					}
				},
				() => {
					var obj = DB.GetRows("SELECT TOP 10 name, create_date, object_id, schema_id, type FROM sys.all_objects ORDER BY name", new {
						DummyA = Col.VarChar("asd", 100),
						DummyB = 25,
						DummyC = (bool?)null
					});
				}
			);
		}
	}
}