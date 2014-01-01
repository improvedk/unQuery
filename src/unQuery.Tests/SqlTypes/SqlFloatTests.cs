using System.Linq;
using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlFloatTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlFloat.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlFloat.GetTypeHandler().CreateParamFromValue(null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlFloat.GetTypeHandler().CreateMetaData(null));

			ITypeHandler col = new SqlFloat(5.27d);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.Float, meta.SqlDbType);
			Assert.AreEqual(53, meta.Precision);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			ISqlType type = new SqlFloat(5.27d);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Float, 5.27d);

			type = new SqlFloat(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Float, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlFloat(5.27d);
			Assert.AreEqual(5.27d, type.GetRawValue());

			type = new SqlFloat(null);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlFloat>(Col.Float(5.27d));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfFloats", new[] {
					new { A = Col.Float(5.27d) },
					new { A = Col.Float(null) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(double), rows[0].A.GetType());
			Assert.AreEqual(5.27d, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = Col.Float(5.27d),
				B = Col.Float(null)
			}}).First();

			Assert.AreEqual(2, result.FieldCount);
			Assert.AreEqual(typeof(double), result.GetValue(0).GetType());
			Assert.AreEqual(5.27d, result.GetValue(0));
			Assert.AreEqual(DBNull.Value, result.GetValue(1));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlFloat)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.Float]);
		}
	}
}