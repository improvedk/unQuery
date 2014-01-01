using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlNVarCharTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlNVarChar.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlNVarChar.GetTypeHandler().CreateParamFromValue(null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlNVarChar.GetTypeHandler().CreateMetaData("Test"));

			ITypeHandler col = new SqlNVarChar("Test");
			Assert.Throws<TypePropertiesMustBeSetExplicitlyException>(() => col.CreateMetaData("Test"));

			col = new SqlNVarChar("Test", 10);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.NVarChar, meta.SqlDbType);
			Assert.AreEqual(10, meta.MaxLength);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(((ISqlType)new SqlNVarChar("Hello рæøåсски", 20)).GetParameter(), SqlDbType.NVarChar, "Hello рæøåсски", size: 20);
			TestHelper.AssertSqlParameter(((ISqlType)new SqlNVarChar(null, 10)).GetParameter(), SqlDbType.NVarChar, DBNull.Value, size: 10);
			TestHelper.AssertSqlParameter(((ISqlType)new SqlNVarChar("Hello рæøåсски")).GetParameter(), SqlDbType.NVarChar, "Hello рæøåсски", size: 14);
			TestHelper.AssertSqlParameter(((ISqlType)new SqlNVarChar(null)).GetParameter(), SqlDbType.NVarChar, DBNull.Value, size: 0);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlNVarChar("Hello ру́сский", 15);
			Assert.AreEqual("Hello ру́сский", type.GetRawValue());

			type = new SqlNVarChar(null, 15);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlNVarChar>(Col.NVarChar("Test", 10));
			Assert.IsInstanceOf<SqlNVarChar>(Col.NVarChar("Test"));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfNVarChars", new[] {
					new { A = Col.NVarChar("слово", 256) },
					new { A = Col.NVarChar(null, 256) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(string), rows[0].A.GetType());
			Assert.AreEqual("слово", rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = Col.NVarChar("A", 10),
				B = Col.NVarChar(null, 10)
			}}).First();

			Assert.AreEqual(2, result.FieldCount);
			Assert.AreEqual(typeof(string), result.GetValue(0).GetType());
			Assert.AreEqual("A", result.GetValue(0));
			Assert.AreEqual(DBNull.Value, result.GetValue(1));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlNVarChar)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.NVarChar]);
		}
	}
}