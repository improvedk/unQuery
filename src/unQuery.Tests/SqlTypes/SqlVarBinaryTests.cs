using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlVarBinaryTests : TestFixture
	{
		private readonly byte[] data = new byte[] { 0x0A, 0xA0, 0xAA };

		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlVarBinary.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlVarBinary.GetTypeHandler().CreateParamFromValue(null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlVarBinary.GetTypeHandler().CreateMetaData("Test"));

			ITypeHandler col = new SqlVarBinary(data);
			Assert.Throws<TypePropertiesMustBeSetExplicitlyException>(() => col.CreateMetaData("Test"));

			col = new SqlVarBinary(data, 10);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.VarBinary, meta.SqlDbType);
			Assert.AreEqual(10, meta.MaxLength);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(((ISqlType)new SqlVarBinary(data, 10)).GetParameter(), SqlDbType.VarBinary, data, size: 10);
			TestHelper.AssertSqlParameter(((ISqlType)new SqlVarBinary(null, 5)).GetParameter(), SqlDbType.VarBinary, DBNull.Value, size: 5);
			TestHelper.AssertSqlParameter(((ISqlType)new SqlVarBinary(data)).GetParameter(), SqlDbType.VarBinary, data, size: data.Length);
			TestHelper.AssertSqlParameter(((ISqlType)new SqlVarBinary(null)).GetParameter(), SqlDbType.VarBinary, DBNull.Value, size: 0);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlVarBinary(data, 10);
			Assert.AreEqual(data, type.GetRawValue());

			type = new SqlVarBinary(null, 10);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlVarBinary>(Col.VarBinary(data, 10));
			Assert.IsInstanceOf<SqlVarBinary>(Col.VarBinary(data));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfVarBinaries", new[] {
					new { A = Col.VarBinary(new byte[] { 0xAA, 0xBB }, 2) },
					new { A = Col.VarBinary(null, 2) }
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
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlVarBinary)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.VarBinary]);
		}
	}
}