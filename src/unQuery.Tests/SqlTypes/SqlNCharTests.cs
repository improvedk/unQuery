using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlNCharTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlNChar.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlNChar.GetTypeHandler().CreateParamFromValue(null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlNChar.GetTypeHandler().CreateMetaData(null));

			ITypeHandler col = new SqlNChar("ру́сский", 10);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.NChar, meta.SqlDbType);
			Assert.AreEqual(10, meta.MaxLength);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			ISqlType type = new SqlNChar("ру́сский", 10);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.NChar, "ру́сский", size: 10);

			type = new SqlNChar(null, 10);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.NChar, DBNull.Value, size: 10);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlNChar("ру́сский", 10);
			Assert.AreEqual("ру́сский", type.GetRawValue());

			type = new SqlNChar(null, 10);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlNChar>(Col.NChar("ру́сский", 10));
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
		public void TypeMaps()
		{
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlNChar)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.NChar]);
		}
	}
}