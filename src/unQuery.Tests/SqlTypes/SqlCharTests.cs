using System.Linq;
using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlCharTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(SqlChar.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlChar.GetTypeHandler().CreateParamFromValue(null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlChar.GetTypeHandler().CreateMetaData("Test"));

			SqlTypeHandler col = new SqlChar("Test");
			Assert.Throws<TypePropertiesMustBeSetExplicitlyException>(() => col.CreateMetaData("Test"));

			col = new SqlChar("Test", 10);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.Char, meta.SqlDbType);
			Assert.AreEqual(10, meta.MaxLength);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(((SqlType)new SqlChar("Test", 10)).GetParameter(), SqlDbType.Char, "Test", size: 10);
			TestHelper.AssertSqlParameter(((SqlType)new SqlChar(null, 10)).GetParameter(), SqlDbType.Char, DBNull.Value, size: 10);
			TestHelper.AssertSqlParameter(((SqlType)new SqlChar("Test")).GetParameter(), SqlDbType.Char, "Test", size: 4);
			TestHelper.AssertSqlParameter(((SqlType)new SqlChar(null)).GetParameter(), SqlDbType.Char, DBNull.Value, size: 0);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlChar("Test", 10);
			Assert.AreEqual("Test", type.GetRawValue());

			type = new SqlChar(null, 10);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlChar>(Col.Char("Test", 10));
			Assert.IsInstanceOf<SqlChar>(Col.Char("Test"));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfChars", new[] {
					new { A = Col.Char("Test", 10) },
					new { A = Col.Char(null, 10) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(string), rows[0].A.GetType());
			Assert.AreEqual("Test      ", rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = Col.Char("Test", 10),
				B = Col.Char(null, 10)
			}}).First();

			Assert.AreEqual(2, result.FieldCount);
			Assert.AreEqual(typeof(string), result.GetValue(0).GetType());
			Assert.AreEqual("Test", result.GetValue(0));
			Assert.AreEqual(DBNull.Value, result.GetValue(1));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlChar)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.Char]);
		}
	}
}