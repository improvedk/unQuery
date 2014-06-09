using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlBitTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(SqlBit.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			TestHelper.AssertParameterFromValue(true, SqlDbType.Bit, true);
			TestHelper.AssertParameterFromValue((bool?)null, SqlDbType.Bit, DBNull.Value);
		}

		[Test]
		public void CreateMetaData()
		{
			var meta = SqlBit.GetTypeHandler().CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.Bit, meta.SqlDbType);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			SqlType type = new SqlBit(true, ParameterDirection.Input);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Bit, true);

			type = new SqlBit(null, ParameterDirection.Input);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Bit, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlBit(false, ParameterDirection.Input);
			Assert.AreEqual(false, type.GetRawValue());

			type = new SqlBit(null, ParameterDirection.Input);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlBit>(Col.Bit(true));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfBits", new[] {
					new { A = (bool?)true },
					new { A = (bool?)false },
					new { A = (bool?)null }
				})
			});

			Assert.AreEqual(3, rows.Count);
			Assert.AreEqual(typeof(bool), rows[0].A.GetType());
			Assert.AreEqual(true, rows[0].A);
			Assert.AreEqual(false, rows[1].A);
			Assert.AreEqual(null, rows[2].A);
		}

		[Test]
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = true,
				B = (bool?)false,
				C = Col.Bit(true),
				D = (bool?)null,
				E = Col.Bit(null)
			}}).First();

			Assert.AreEqual(5, result.FieldCount);
			Assert.AreEqual(typeof(bool), result.GetValue(0).GetType());
			Assert.AreEqual(true, result.GetValue(0));
			Assert.AreEqual(typeof(bool), result.GetValue(1).GetType());
			Assert.AreEqual(false, result.GetValue(1));
			Assert.AreEqual(typeof(bool), result.GetValue(2).GetType());
			Assert.AreEqual(true, result.GetValue(2));
			Assert.AreEqual(DBNull.Value, result.GetValue(3));
			Assert.AreEqual(DBNull.Value, result.GetValue(4));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(bool)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(bool?)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlBit)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.Bit]);
		}
	}
}