using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlSmallDateTimeTests : TestFixture
	{
		private readonly DateTime testSmallDateTime = new DateTime(2013, 05, 12, 11, 22, 0);

		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlSmallDateTime.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlSmallDateTime.GetTypeHandler().CreateParamFromValue(null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlSmallDateTime.GetTypeHandler().CreateMetaData(null));

			ITypeHandler col = new SqlSmallDateTime(testSmallDateTime);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.SmallDateTime, meta.SqlDbType);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			ISqlType type = new SqlSmallDateTime(testSmallDateTime);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.SmallDateTime, testSmallDateTime);

			type = new SqlSmallDateTime(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.SmallDateTime, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlSmallDateTime(testSmallDateTime);
			Assert.AreEqual(testSmallDateTime, type.GetRawValue());

			type = new SqlSmallDateTime(null);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlSmallDateTime>(Col.SmallDateTime(testSmallDateTime));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfSmallDateTimes", new[] {
					new { A = Col.SmallDateTime(testSmallDateTime) },
					new { A = Col.SmallDateTime(null) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(DateTime), rows[0].A.GetType());
			Assert.AreEqual(testSmallDateTime, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlSmallDateTime)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.SmallDateTime]);
		}
	}
}