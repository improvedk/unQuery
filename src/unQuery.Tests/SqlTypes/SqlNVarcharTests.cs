﻿using NUnit.Framework;
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
			Assert.IsInstanceOf<SqlTypeHandler>(SqlNVarChar.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlNVarChar.GetTypeHandler().CreateParamFromValue("Test", null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlNVarChar.GetTypeHandler().CreateMetaData("Test"));

			SqlTypeHandler col = new SqlNVarChar("Test", null, ParameterDirection.Input);
			Assert.Throws<TypePropertiesMustBeSetExplicitlyException>(() => col.CreateMetaData("Test"));

			col = new SqlNVarChar("Test", 10, ParameterDirection.Input);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.NVarChar, meta.SqlDbType);
			Assert.AreEqual(10, meta.MaxLength);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter((new SqlNVarChar("Hello рæøåсски", 20, ParameterDirection.Input)).GetParameter(), SqlDbType.NVarChar, "Hello рæøåсски", size: 20);
			TestHelper.AssertSqlParameter((new SqlNVarChar(null, 10, ParameterDirection.Input)).GetParameter(), SqlDbType.NVarChar, DBNull.Value, size: 10);
			TestHelper.AssertSqlParameter((new SqlNVarChar("Hello рæøåсски", null, ParameterDirection.Input)).GetParameter(), SqlDbType.NVarChar, "Hello рæøåсски", size: 64);
			TestHelper.AssertSqlParameter((new SqlNVarChar("Hello рæøåсски".PadRight(200, ' '), null, ParameterDirection.Input)).GetParameter(), SqlDbType.NVarChar, "Hello рæøåсски".PadRight(200, ' '), size: 256);
			TestHelper.AssertSqlParameter((new SqlNVarChar(null, null, ParameterDirection.Input)).GetParameter(), SqlDbType.NVarChar, DBNull.Value, size: 64);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlNVarChar("Hello ру́сский", 15, ParameterDirection.Input);
			Assert.AreEqual("Hello ру́сский", type.GetRawValue());

			type = new SqlNVarChar(null, 15, ParameterDirection.Input);
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
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlNVarChar)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.NVarChar]);
		}
	}
}