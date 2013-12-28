using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlRealTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlReal.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlReal.GetTypeHandler().CreateParamFromValue(null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlReal.GetTypeHandler().CreateMetaData(null));

			ITypeHandler col = new SqlReal(5.27f);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.Real, meta.SqlDbType);
			Assert.AreEqual(24, meta.Precision);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			ISqlType type = new SqlReal(5.27f);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Real, 5.27f);

			type = new SqlReal(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Real, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlReal(5.27f);
			Assert.AreEqual(5.27f, type.GetRawValue());

			type = new SqlReal(null);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlReal>(Col.Real(5.27f));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfReals", new[] {
					new { A = Col.Real(5.27f) },
					new { A = Col.Real(null) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(float), rows[0].A.GetType());
			Assert.AreEqual(5.27f, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlReal)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.Real]);
		}
	}
}