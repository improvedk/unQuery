using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlBinaryTests : TestFixture
	{
		private readonly byte[] data = new byte[] { 0x0A, 0xA0, 0xAA };

		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlBinary.GetTypeHandler());
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

			ITypeHandler col = new SqlBinary(data, 10);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.Binary, meta.SqlDbType);
			Assert.AreEqual(10, meta.MaxLength);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			ISqlType type = new SqlBinary(data, 10);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Binary, data, size: 10);

			type = new SqlBinary(null, 10);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Binary, DBNull.Value, size: 10);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlBinary(data, 10);
			Assert.AreEqual(data, type.GetRawValue());

			type = new SqlBinary(null, 10);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlBinary>(Col.Binary(data, 10));
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
		public void TypeMaps()
		{
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlBinary)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.Binary]);
		}
	}
}