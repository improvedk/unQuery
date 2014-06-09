using System.Linq;
using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlDecimalTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(SqlDecimal.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlDecimal.GetTypeHandler().CreateParamFromValue("Test", null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlDecimal.GetTypeHandler().CreateMetaData("Test"));

			SqlTypeHandler col = new SqlDecimal(5.27m, null, null, ParameterDirection.Input);
			Assert.Throws<TypePropertiesMustBeSetExplicitlyException>(() => col.CreateMetaData("Test"));

			col = new SqlDecimal(5.27m, 10, 5, ParameterDirection.Input);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.Decimal, meta.SqlDbType);
			Assert.AreEqual(10, meta.Precision);
			Assert.AreEqual(5, meta.Scale);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter((new SqlDecimal(5.27m, 10, 5, ParameterDirection.Input)).GetParameter(), SqlDbType.Decimal, 5.27m, precision: 10, scale: 5);
			TestHelper.AssertSqlParameter((new SqlDecimal(null, 10, 5, ParameterDirection.Input)).GetParameter(), SqlDbType.Decimal, DBNull.Value, precision: 10, scale: 5);
			TestHelper.AssertSqlParameter((new SqlDecimal(5.27m, null, null, ParameterDirection.Input)).GetParameter(), SqlDbType.Decimal, 5.27m, precision: 3, scale: 2);
			TestHelper.AssertSqlParameter((new SqlDecimal(null, null, null, ParameterDirection.Input)).GetParameter(), SqlDbType.Decimal, DBNull.Value, precision: 0, scale: 0);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlDecimal(5.27m, 10, 5, ParameterDirection.Input);
			Assert.AreEqual(5.27m, type.GetRawValue());

			type = new SqlDecimal(null, 10, 5, ParameterDirection.Input);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlDecimal>(Col.Decimal(5.27m, 10, 5));
			Assert.IsInstanceOf<SqlDecimal>(Col.Decimal(5.27m));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfDecimals", new[] {
					new { A = Col.Decimal(5.27m, 10, 5) },
					new { A = Col.Decimal(null, 10, 5) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(decimal), rows[0].A.GetType());
			Assert.AreEqual(5.27m, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = Col.Decimal(5.27m, 10, 5),
				B = Col.Decimal(null, 10, 5)
			}}).First();

			Assert.AreEqual(2, result.FieldCount);
			Assert.AreEqual(typeof(decimal), result.GetValue(0).GetType());
			Assert.AreEqual(5.27m, result.GetValue(0));
			Assert.AreEqual(DBNull.Value, result.GetValue(1));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlDecimal)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.Decimal]);
		}
	}
}