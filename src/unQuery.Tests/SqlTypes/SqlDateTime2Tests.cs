using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlDateTime2Tests : TestFixture
	{
		private readonly DateTime testDateTime = new DateTime(2013, 05, 12, 11, 22, 33, 3);

		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(SqlDateTime2.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlDateTime2.GetTypeHandler().CreateParamFromValue("Test", null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlDateTime2.GetTypeHandler().CreateMetaData("Test"));

			SqlTypeHandler col = new SqlDateTime2(testDateTime);
			Assert.Throws<TypePropertiesMustBeSetExplicitlyException>(() => col.CreateMetaData("Test"));

			col = new SqlDateTime2(testDateTime, 6);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.DateTime2, meta.SqlDbType);
			Assert.AreEqual(6, meta.Scale);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter((new SqlDateTime2(testDateTime, 6)).GetParameter(), SqlDbType.DateTime2, testDateTime, scale: 6);
			TestHelper.AssertSqlParameter((new SqlDateTime2(null, 4)).GetParameter(), SqlDbType.DateTime2, DBNull.Value, scale: 4);
			TestHelper.AssertSqlParameter((new SqlDateTime2(testDateTime)).GetParameter(), SqlDbType.DateTime2, testDateTime, scale: 0);
			TestHelper.AssertSqlParameter((new SqlDateTime2(null)).GetParameter(), SqlDbType.DateTime2, DBNull.Value, scale: 0);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlDateTime2(testDateTime, 5);
			Assert.AreEqual(testDateTime, type.GetRawValue());

			type = new SqlDateTime2(null, 3);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlDateTime2>(Col.DateTime2(testDateTime, 4));
			Assert.IsInstanceOf<SqlDateTime2>(Col.DateTime2(testDateTime));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfDateTime2s", new[] {
					new { A = Col.DateTime2(testDateTime, 5) },
					new { A = Col.DateTime2(null, 5) }
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
				A = Col.DateTime2(testDateTime, 5),
				B = Col.DateTime2(null, 5)
			}}).First();

			Assert.AreEqual(2, result.FieldCount);
			Assert.AreEqual(typeof(DateTime), result.GetValue(0).GetType());
			Assert.AreEqual(testDateTime, result.GetValue(0));
			Assert.AreEqual(DBNull.Value, result.GetValue(1));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlDateTime2)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.DateTime2]);
		}
	}
}