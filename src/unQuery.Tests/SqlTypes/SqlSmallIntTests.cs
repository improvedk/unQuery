using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlSmallIntTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlSmallInt.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			TestHelper.AssertParameterFromValue((short)5, SqlDbType.SmallInt, (short)5);
			TestHelper.AssertParameterFromValue((short?)null, SqlDbType.SmallInt, DBNull.Value);
		}

		[Test]
		public void CreateMetaData()
		{
			var meta = SqlSmallInt.GetTypeHandler().CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.SmallInt, meta.SqlDbType);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			ISqlType type = new SqlSmallInt(5);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.SmallInt, (short)5);

			type = new SqlSmallInt(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.SmallInt, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlSmallInt(5);
			Assert.AreEqual((short)5, type.GetRawValue());

			type = new SqlSmallInt(null);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlSmallInt>(Col.SmallInt(5));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfSmallInts", new[] {
					new { A = (short?)1 },
					new { A = (short?)null }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(short), rows[0].A.GetType());
			Assert.AreEqual(1, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}
	}
}