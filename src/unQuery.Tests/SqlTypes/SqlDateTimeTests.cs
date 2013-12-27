using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlDateTimeTests : TestFixture
	{
		private readonly DateTime testDateTime = new DateTime(2013, 05, 12, 11, 22, 33, 3);

		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlDateTime.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlDateTime.GetTypeHandler().CreateParamFromValue(null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlDateTime.GetTypeHandler().CreateMetaData(null));

			ITypeHandler col = new SqlDateTime(testDateTime);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.DateTime, meta.SqlDbType);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			ISqlType type = new SqlDateTime(testDateTime);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.DateTime, testDateTime);

			type = new SqlDateTime(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.DateTime, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlDateTime(testDateTime);
			Assert.AreEqual(testDateTime, type.GetRawValue());

			type = new SqlDateTime(null);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlDateTime>(Col.DateTime(testDateTime));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfDateTimes", new[] {
					new { A = Col.DateTime(testDateTime) },
					new { A = Col.DateTime(null) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(DateTime), rows[0].A.GetType());
			Assert.AreEqual(testDateTime, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlDateTime)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.DateTime]);
		}
	}
}