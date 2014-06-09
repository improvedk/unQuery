using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlVarBinaryTests : TestFixture
	{
		private readonly byte[] data = new byte[] { 0x0A, 0xA0, 0xAA };

		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(SqlVarBinary.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlVarBinary.GetTypeHandler().CreateParamFromValue("Test", null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlVarBinary.GetTypeHandler().CreateMetaData("Test"));

			SqlTypeHandler col = new SqlVarBinary(data, null, ParameterDirection.Input);
			Assert.Throws<TypePropertiesMustBeSetExplicitlyException>(() => col.CreateMetaData("Test"));

			col = new SqlVarBinary(data, 10, ParameterDirection.Input);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.VarBinary, meta.SqlDbType);
			Assert.AreEqual(10, meta.MaxLength);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(((SqlType)new SqlVarBinary(data, 10, ParameterDirection.Input)).GetParameter(), SqlDbType.VarBinary, data, size: 10);
			TestHelper.AssertSqlParameter(((SqlType)new SqlVarBinary(null, 5, ParameterDirection.Input)).GetParameter(), SqlDbType.VarBinary, DBNull.Value, size: 5);
			TestHelper.AssertSqlParameter(((SqlType)new SqlVarBinary(data, null, ParameterDirection.Input)).GetParameter(), SqlDbType.VarBinary, data, size: 64);
			TestHelper.AssertSqlParameter(((SqlType)new SqlVarBinary(null, null, ParameterDirection.Input)).GetParameter(), SqlDbType.VarBinary, DBNull.Value, size: 64);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlVarBinary(data, 10, ParameterDirection.Input);
			Assert.AreEqual(data, type.GetRawValue());

			type = new SqlVarBinary(null, 10, ParameterDirection.Input);
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
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = Col.VarBinary(data, 10),
				B = Col.VarBinary(null, 10)
			}}).First();

			Assert.AreEqual(2, result.FieldCount);
			Assert.AreEqual(typeof(byte[]), result.GetValue(0).GetType());
			Assert.AreEqual(data, result.GetValue(0));
			Assert.AreEqual(DBNull.Value, result.GetValue(1));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlVarBinary)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.VarBinary]);
		}
	}
}