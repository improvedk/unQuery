using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlIntTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlInt.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			TestHelper.AssertParameterFromValue(5, SqlDbType.Int, 5);
			TestHelper.AssertParameterFromValue((int?)null, SqlDbType.Int, DBNull.Value);
		}

		[Test]
		public void CreateMetaData()
		{
			var meta = SqlInt.GetTypeHandler().CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.Int, meta.SqlDbType);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			ISqlType type = new SqlInt(5);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Int, 5);

			type = new SqlInt(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Int, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlInt(5);
			Assert.AreEqual(5, type.GetRawValue());

			type = new SqlInt(null);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlInt>(Col.Int(5));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfInts", new[] {
					new { A = (int?)1 },
					new { A = (int?)null }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(int), rows[0].A.GetType());
			Assert.AreEqual(1, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(int)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(int?)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlInt)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.Int]);
		}
	}
}