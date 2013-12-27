using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlTinyIntTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlTinyInt.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			TestHelper.AssertParameterFromValue((byte)5, SqlDbType.TinyInt, (byte)5);
			TestHelper.AssertParameterFromValue((byte?)null, SqlDbType.TinyInt, DBNull.Value);
		}

		[Test]
		public void CreateMetaData()
		{
			var meta = SqlTinyInt.GetTypeHandler().CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.TinyInt, meta.SqlDbType);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			ISqlType type = new SqlTinyInt(5);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.TinyInt, (byte)5);

			type = new SqlTinyInt(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.TinyInt, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlTinyInt(5);
			Assert.AreEqual((byte)5, type.GetRawValue());

			type = new SqlTinyInt(null);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlTinyInt>(Col.TinyInt(5));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfTinyInts", new[] {
					new { A = (byte?)1 },
					new { A = (byte?)null }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(byte), rows[0].A.GetType());
			Assert.AreEqual(1, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(byte)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(byte?)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlTinyInt)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.TinyInt]);
		}
	}
}