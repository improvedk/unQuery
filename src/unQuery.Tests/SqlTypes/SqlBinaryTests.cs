using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlBinaryTests : TestFixture
	{
		private readonly byte[] data = new byte[] { 0x0A, 0xA0, 0xAA };

		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(SqlBinary.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlBinary.GetTypeHandler().CreateParamFromValue(null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlBinary.GetTypeHandler().CreateMetaData(null));

			SqlTypeHandler col = new SqlBinary(data);
			Assert.Throws<TypePropertiesMustBeSetExplicitlyException>(() => col.CreateMetaData("Test"));

			col = new SqlBinary(data, 10);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.Binary, meta.SqlDbType);
			Assert.AreEqual(10, meta.MaxLength);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(((SqlType)new SqlBinary(data, 10)).GetParameter(), SqlDbType.Binary, data, size: 10);
			TestHelper.AssertSqlParameter(((SqlType)new SqlBinary(null, 10)).GetParameter(), SqlDbType.Binary, DBNull.Value, size: 10);
			TestHelper.AssertSqlParameter(((SqlType)new SqlBinary(data)).GetParameter(), SqlDbType.Binary, data, size: data.Length);
			TestHelper.AssertSqlParameter(((SqlType)new SqlBinary(null)).GetParameter(), SqlDbType.Binary, DBNull.Value, size: 0);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlBinary(data, 10);
			Assert.AreEqual(data, type.GetRawValue());

			type = new SqlBinary(null, 10);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlBinary>(Col.Binary(data, 10));
			Assert.IsInstanceOf<SqlBinary>(Col.Binary(data));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfBinary", new[] {
					new { A = Col.Binary(new byte[] { 0xAA, 0xBB }, 2) },
					new { A = Col.Binary(null, 2) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(byte[]), rows[0].A.GetType());
			Assert.AreEqual(new byte[] { 0xAA, 0xBB }, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = Col.Binary(new byte[] { 0xAA, 0xBB }, 2),
				B = Col.Binary(null, 2)
			}}).First();

			Assert.AreEqual(2, result.FieldCount);
			Assert.AreEqual(typeof(byte[]), result.GetValue(0).GetType());
			Assert.AreEqual(new byte[] { 0xAA, 0xBB }, result.GetValue(0));
			Assert.AreEqual(DBNull.Value, result.GetValue(1));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlBinary)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.Binary]);
		}
	}
}