using NUnit.Framework;
using System;
using System.Data;
using System.Data.SqlClient;
using unQuery.SqlTypes;

namespace unQuery.PerformanceTests
{
	public class GetRowTests : TestFixture
	{
		[Test]
		public void GetRow_Typed_NoParameters()
		{
			RunTest(5.5,
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
		public void GetRow_Typed_Parameters()
		{
			RunTest(10,
				() =>
				{
					using (var conn = GetOpenConnection())
					using (var cmd = new SqlCommand("SELECT @Name AS name, @CreateDate AS create_date, @ObjectID AS object_id, @SchemaID AS schema_id, @Type AS type", conn))
					{
						cmd.Parameters.Add("@Name", SqlDbType.VarChar, 100).Value = "name";
						cmd.Parameters.Add("@CreateDate", SqlDbType.DateTime).Value = DateTime.Now;
						cmd.Parameters.Add("@ObjectID", SqlDbType.Int).Value = 25;
						cmd.Parameters.Add("@SchemaID", SqlDbType.Int).Value = 27;
						cmd.Parameters.Add("@Type", SqlDbType.VarChar, 20).Value = "type";

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
				() =>
				{
					var obj = DB.GetRow<SysAllObject>("SELECT @Name AS name, @CreateDate AS create_date, @ObjectID AS object_id, @SchemaID AS schema_id, @Type AS type", new {
						Name = Col.VarChar("name", 100),
						CreateDate = Col.DateTime(DateTime.Now),
						ObjectID = 25,
						SchemaID = 27,
						Type = Col.VarChar("type", 20)
					});
				}
			);
		}

		[Test]
		public void GetRow_Dynamic_NoParameters()
		{
			RunTest(1,
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
			RunTest(4,
				() =>
				{
					using (var conn = GetOpenConnection())
					using (var cmd = new SqlCommand("SELECT @Name AS name, @CreateDate AS create_date, @ObjectID AS object_id, @SchemaID AS schema_id, @Type AS type", conn))
					{
						cmd.Parameters.Add("@Name", SqlDbType.VarChar, 100).Value = "name";
						cmd.Parameters.Add("@CreateDate", SqlDbType.DateTime).Value = DateTime.Now;
						cmd.Parameters.Add("@ObjectID", SqlDbType.Int).Value = 25;
						cmd.Parameters.Add("@SchemaID", SqlDbType.Int).Value = 27;
						cmd.Parameters.Add("@Type", SqlDbType.VarChar, 20).Value = "type";

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
					var obj = DB.GetRow("SELECT @Name AS name, @CreateDate AS create_date, @ObjectID AS object_id, @SchemaID AS schema_id, @Type AS type", new {
						Name = Col.VarChar("name", 100),
						CreateDate = Col.DateTime(DateTime.Now),
						ObjectID = 25,
						SchemaID = 27,
						Type = Col.VarChar("type", 20)
					});
				}
			);
		}
	}
}