using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlBigIntTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(SqlBigInt.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			TestHelper.AssertParameterFromValue((long)5, SqlDbType.BigInt, (long)5);
			TestHelper.AssertParameterFromValue((long?)null, SqlDbType.BigInt, DBNull.Value);
		}

		[Test]
		public void CreateMetaData()
		{
			var meta = SqlBigInt.GetTypeHandler().CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.BigInt, meta.SqlDbType);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			SqlType type = new SqlBigInt(5, ParameterDirection.Input);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.BigInt, (long)5);

			type = new SqlBigInt(null, ParameterDirection.Input);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.BigInt, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlBigInt(5, ParameterDirection.Input);
			Assert.AreEqual((long)5, type.GetRawValue());

			type = new SqlBigInt(null, ParameterDirection.Input);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlBigInt>(Col.BigInt(5));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfBigInts", new[] {
					new { A = (long?)1 },
					new { A = (long?)null }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(long), rows[0].A.GetType());
			Assert.AreEqual(1, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = (long)1,
				B = (long?)2,
				C = Col.BigInt(3),
				D = (long?)null,
				E = Col.BigInt(null)
			}}).First();

			Assert.AreEqual(5, result.FieldCount);
			Assert.AreEqual(typeof(long), result.GetValue(0).GetType());
			Assert.AreEqual(1, result.GetValue(0));
			Assert.AreEqual(typeof(long), result.GetValue(1).GetType());
			Assert.AreEqual(2, result.GetValue(1));
			Assert.AreEqual(typeof(long), result.GetValue(2).GetType());
			Assert.AreEqual(3, result.GetValue(2));
			Assert.AreEqual(DBNull.Value, result.GetValue(3));
			Assert.AreEqual(DBNull.Value, result.GetValue(4));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(long)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(long?)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlBigInt)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.BigInt]);
		}
	}
}