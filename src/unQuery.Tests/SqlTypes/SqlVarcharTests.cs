using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlVarCharTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlVarChar.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlVarChar.GetTypeHandler().CreateParamFromValue(null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlVarChar.GetTypeHandler().CreateMetaData("Test"));

			ITypeHandler col = new SqlVarChar("Test");
			Assert.Throws<TypePropertiesMustBeSetExplicitlyException>(() => col.CreateMetaData("Test"));

			col = new SqlVarChar("Test", 10);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.VarChar, meta.SqlDbType);
			Assert.AreEqual(10, meta.MaxLength);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(((ISqlType)new SqlVarChar("Hello", 10)).GetParameter(), SqlDbType.VarChar, "Hello", size: 10);
			TestHelper.AssertSqlParameter(((ISqlType)new SqlVarChar(null, 5)).GetParameter(), SqlDbType.VarChar, DBNull.Value, size: 5);
			TestHelper.AssertSqlParameter(((ISqlType)new SqlVarChar("Hello")).GetParameter(), SqlDbType.VarChar, "Hello", size: 64);
			TestHelper.AssertSqlParameter(((ISqlType)new SqlVarChar("Hello".PadRight(200, ' '))).GetParameter(), SqlDbType.VarChar, "Hello".PadRight(200, ' '), size: 256);
			TestHelper.AssertSqlParameter(((ISqlType)new SqlVarChar(null)).GetParameter(), SqlDbType.VarChar, DBNull.Value, size: 64);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlVarChar("Hello", 10);
			Assert.AreEqual("Hello", type.GetRawValue());

			type = new SqlVarChar(null, 10);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlVarChar>(Col.VarChar("Test", 10));
			Assert.IsInstanceOf<SqlVarChar>(Col.VarChar("Test"));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfVarChars", new[] {
					new { A = Col.VarChar("ABC", 256) },
					new { A = Col.VarChar(null, 256) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(string), rows[0].A.GetType());
			Assert.AreEqual("ABC", rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = Col.VarChar("A", 10),
				B = Col.VarChar(null, 10)
			}}).First();

			Assert.AreEqual(2, result.FieldCount);
			Assert.AreEqual(typeof(string), result.GetValue(0).GetType());
			Assert.AreEqual("A", result.GetValue(0));
			Assert.AreEqual(DBNull.Value, result.GetValue(1));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlVarChar)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.VarChar]);
		}
	}
}