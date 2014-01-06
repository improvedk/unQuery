using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlTinyIntTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(SqlTinyInt.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			TestHelper.AssertParameterFromValue((byte)5, SqlDbType.TinyInt, (byte)5);
			TestHelper.AssertParameterFromValue((byte?)null, SqlDbType.TinyInt, DBNull.Value);
		}

		[Test]
		public void CreateMetaData()
		{
			var meta = SqlTinyInt.GetTypeHandler().CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.TinyInt, meta.SqlDbType);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			SqlType type = new SqlTinyInt(5);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.TinyInt, (byte)5);

			type = new SqlTinyInt(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.TinyInt, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlTinyInt(5);
			Assert.AreEqual((byte)5, type.GetRawValue());

			type = new SqlTinyInt(null);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlTinyInt>(Col.TinyInt(5));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfTinyInts", new[] {
					new { A = (byte?)1 },
					new { A = (byte?)null }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(byte), rows[0].A.GetType());
			Assert.AreEqual(1, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = (byte)1,
				B = (byte?)2,
				C = Col.TinyInt(3),
				D = (byte?)null,
				E = Col.TinyInt(null)
			}}).First();

			Assert.AreEqual(5, result.FieldCount);
			Assert.AreEqual(typeof(byte), result.GetValue(0).GetType());
			Assert.AreEqual(1, result.GetValue(0));
			Assert.AreEqual(typeof(byte), result.GetValue(1).GetType());
			Assert.AreEqual(2, result.GetValue(1));
			Assert.AreEqual(typeof(byte), result.GetValue(2).GetType());
			Assert.AreEqual(3, result.GetValue(2));
			Assert.AreEqual(DBNull.Value, result.GetValue(3));
			Assert.AreEqual(DBNull.Value, result.GetValue(4));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(byte)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(byte?)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlTinyInt)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.TinyInt]);
		}
	}
}