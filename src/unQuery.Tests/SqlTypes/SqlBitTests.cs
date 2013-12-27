using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlBitTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlBit.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			TestHelper.AssertParameterFromValue(true, SqlDbType.Bit, true);
			TestHelper.AssertParameterFromValue((bool?)null, SqlDbType.Bit, DBNull.Value);
		}

		[Test]
		public void CreateMetaData()
		{
			var meta = SqlBit.GetTypeHandler().CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.Bit, meta.SqlDbType);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			ISqlType type = new SqlBit(true);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Bit, true);

			type = new SqlBit(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Bit, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlBit(false);
			Assert.AreEqual(false, type.GetRawValue());

			type = new SqlBit(null);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlBit>(Col.Bit(true));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfBits", new[] {
					new { A = (bool?)true },
					new { A = (bool?)false },
					new { A = (bool?)null }
				})
			});

			Assert.AreEqual(3, rows.Count);
			Assert.AreEqual(typeof(bool), rows[0].A.GetType());
			Assert.AreEqual(true, rows[0].A);
			Assert.AreEqual(false, rows[1].A);
			Assert.AreEqual(null, rows[2].A);
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(bool)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(bool?)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlBit)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.Bit]);
		}
	}
}