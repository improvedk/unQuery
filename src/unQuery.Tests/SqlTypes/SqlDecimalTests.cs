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
			Assert.IsInstanceOf<ITypeHandler>(SqlDecimal.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlDecimal.GetTypeHandler().CreateParamFromValue(null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlDecimal.GetTypeHandler().CreateMetaData(null));

			ITypeHandler col = new SqlDecimal(5.27m, 10, 5);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.Decimal, meta.SqlDbType);
			Assert.AreEqual(10, meta.Precision);
			Assert.AreEqual(5, meta.Scale);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			ISqlType type = new SqlDecimal(5.27m, 10, 5);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Decimal, 5.27m, precision: 10, scale: 5);

			type = new SqlDecimal(null, 10, 5);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Decimal, DBNull.Value, precision: 10, scale: 5);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlDecimal(5.27m, 10, 5);
			Assert.AreEqual(5.27m, type.GetRawValue());

			type = new SqlDecimal(null, 10, 5);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlDecimal>(Col.Decimal(5.27m, 10, 5));
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
		public void TypeMaps()
		{
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlDecimal)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.Decimal]);
		}
	}
}