using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlSmallDateTimeTests : TestFixture
	{
		private readonly DateTime testSmallDateTime = new DateTime(2013, 05, 12, 11, 22, 0);

		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(SqlSmallDateTime.GetTypeHandler());
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

			SqlTypeHandler col = new SqlSmallDateTime(testSmallDateTime);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.SmallDateTime, meta.SqlDbType);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			SqlType type = new SqlSmallDateTime(testSmallDateTime);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.SmallDateTime, testSmallDateTime);

			type = new SqlSmallDateTime(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.SmallDateTime, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlSmallDateTime(testSmallDateTime);
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
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = Col.SmallDateTime(testSmallDateTime),
				B = Col.SmallDateTime(null)
			}}).First();

			Assert.AreEqual(2, result.FieldCount);
			Assert.AreEqual(typeof(DateTime), result.GetValue(0).GetType());
			Assert.AreEqual(testSmallDateTime, result.GetValue(0));
			Assert.AreEqual(DBNull.Value, result.GetValue(1));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlSmallDateTime)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.SmallDateTime]);
		}
	}
}