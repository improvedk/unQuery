using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlSmallIntTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(SqlSmallInt.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			TestHelper.AssertParameterFromValue((short)5, SqlDbType.SmallInt, (short)5);
			TestHelper.AssertParameterFromValue((short?)null, SqlDbType.SmallInt, DBNull.Value);
		}

		[Test]
		public void CreateMetaData()
		{
			var meta = SqlSmallInt.GetTypeHandler().CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.SmallInt, meta.SqlDbType);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			SqlType type = new SqlSmallInt(5, ParameterDirection.Input);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.SmallInt, (short)5);

			type = new SqlSmallInt(null, ParameterDirection.Input);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.SmallInt, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlSmallInt(5, ParameterDirection.Input);
			Assert.AreEqual((short)5, type.GetRawValue());

			type = new SqlSmallInt(null, ParameterDirection.Input);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlSmallInt>(Col.SmallInt(5));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfSmallInts", new[] {
					new { A = (short?)1 },
					new { A = (short?)null }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(short), rows[0].A.GetType());
			Assert.AreEqual(1, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = (short)1,
				B = (short?)2,
				C = Col.SmallInt(3),
				D = (short?)null,
				E = Col.SmallInt(null)
			}}).First();

			Assert.AreEqual(5, result.FieldCount);
			Assert.AreEqual(typeof(short), result.GetValue(0).GetType());
			Assert.AreEqual(1, result.GetValue(0));
			Assert.AreEqual(typeof(short), result.GetValue(1).GetType());
			Assert.AreEqual(2, result.GetValue(1));
			Assert.AreEqual(typeof(short), result.GetValue(2).GetType());
			Assert.AreEqual(3, result.GetValue(2));
			Assert.AreEqual(DBNull.Value, result.GetValue(3));
			Assert.AreEqual(DBNull.Value, result.GetValue(4));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(short)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(short?)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlSmallInt)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.SmallInt]);
		}
	}
}