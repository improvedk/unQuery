using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlIntTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(SqlInt.GetTypeHandler());
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
			SqlType type = new SqlInt(5, ParameterDirection.Input);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Int, 5);

			type = new SqlInt(null, ParameterDirection.Input);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Int, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlInt(5, ParameterDirection.Input);
			Assert.AreEqual(5, type.GetRawValue());

			type = new SqlInt(null, ParameterDirection.Input);
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
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = 1,
				B = (int?)2,
				C = Col.Int(3),
				D = (int?)null,
				E = Col.Int(null)
			}}).First();

			Assert.AreEqual(5, result.FieldCount);
			Assert.AreEqual(typeof(int), result.GetValue(0).GetType());
			Assert.AreEqual(1, result.GetValue(0));
			Assert.AreEqual(typeof(int), result.GetValue(1).GetType());
			Assert.AreEqual(2, result.GetValue(1));
			Assert.AreEqual(typeof(int), result.GetValue(2).GetType());
			Assert.AreEqual(3, result.GetValue(2));
			Assert.AreEqual(DBNull.Value, result.GetValue(3));
			Assert.AreEqual(DBNull.Value, result.GetValue(4));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(int)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(int?)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlInt)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.Int]);
		}
	}
}