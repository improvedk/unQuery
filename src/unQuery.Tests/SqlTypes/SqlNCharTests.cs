using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlNCharTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(SqlNChar.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlNChar.GetTypeHandler().CreateParamFromValue("Test", null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlNChar.GetTypeHandler().CreateMetaData("Test"));

			SqlTypeHandler col = new SqlNChar("ру́сский", null, ParameterDirection.Input);
			Assert.Throws<TypePropertiesMustBeSetExplicitlyException>(() => col.CreateMetaData("Test"));

			col = new SqlNChar("ру́сский", 10, ParameterDirection.Input);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.NChar, meta.SqlDbType);
			Assert.AreEqual(10, meta.MaxLength);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter((new SqlNChar("ру́сский", 10, ParameterDirection.Input)).GetParameter(), SqlDbType.NChar, "ру́сский", size: 10);
			TestHelper.AssertSqlParameter((new SqlNChar(null, 10, ParameterDirection.Input)).GetParameter(), SqlDbType.NChar, DBNull.Value, size: 10);
			TestHelper.AssertSqlParameter((new SqlNChar("рæøåсски", null, ParameterDirection.Input)).GetParameter(), SqlDbType.NChar, "рæøåсски", size: 8);
			TestHelper.AssertSqlParameter((new SqlNChar(null, null, ParameterDirection.Input)).GetParameter(), SqlDbType.NChar, DBNull.Value, size: 0);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlNChar("ру́сский", 10, ParameterDirection.Input);
			Assert.AreEqual("ру́сский", type.GetRawValue());

			type = new SqlNChar(null, 10, ParameterDirection.Input);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlNChar>(Col.NChar("ру́сский", 10));
			Assert.IsInstanceOf<SqlNChar>(Col.NChar("ру́сский"));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfNChars", new[] {
					new { A = Col.NChar("язк", 10) },
					new { A = Col.NChar(null, 10) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(string), rows[0].A.GetType());
			Assert.AreEqual("язк       ", rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = Col.NChar("язк", 10),
				B = Col.NChar(null, 10)
			}}).First();

			Assert.AreEqual(2, result.FieldCount);
			Assert.AreEqual(typeof(string), result.GetValue(0).GetType());
			Assert.AreEqual("язк", result.GetValue(0));
			Assert.AreEqual(DBNull.Value, result.GetValue(1));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlNChar)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.NChar]);
		}
	}
}