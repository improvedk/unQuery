using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlBigIntTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlBigInt.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			TestHelper.AssertParameterFromValue((long)5, SqlDbType.BigInt, (long)5);
			TestHelper.AssertParameterFromValue((long?)null, SqlDbType.BigInt, DBNull.Value);
		}

		[Test]
		public void CreateMetaData()
		{
			var meta = SqlBigInt.GetTypeHandler().CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.BigInt, meta.SqlDbType);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			ISqlType type = new SqlBigInt(5);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.BigInt, null, (long)5);

			type = new SqlBigInt(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.BigInt, null, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlBigInt(5);
			Assert.AreEqual((long)5, type.GetRawValue());

			type = new SqlBigInt(null);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlBigInt>(Col.BigInt(5));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfBigInts", new[] {
					new { A = (long?)1 },
					new { A = (long?)null }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(long), rows[0].A.GetType());
			Assert.AreEqual(1, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}
	}
}