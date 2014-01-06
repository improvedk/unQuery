using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlDateTimeTests : TestFixture
	{
		private readonly DateTime testDateTime = new DateTime(2013, 05, 12, 11, 22, 33, 3);

		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(SqlDateTime.GetTypeHandler());
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

			SqlTypeHandler col = new SqlDateTime(testDateTime);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.DateTime, meta.SqlDbType);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			SqlType type = new SqlDateTime(testDateTime);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.DateTime, testDateTime);

			type = new SqlDateTime(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.DateTime, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlDateTime(testDateTime);
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
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = Col.DateTime(testDateTime),
				B = Col.DateTime(null)
			}}).First();

			Assert.AreEqual(2, result.FieldCount);
			Assert.AreEqual(typeof(DateTime), result.GetValue(0).GetType());
			Assert.AreEqual(testDateTime, result.GetValue(0));
			Assert.AreEqual(DBNull.Value, result.GetValue(1));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlDateTime)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.DateTime]);
		}
	}
}