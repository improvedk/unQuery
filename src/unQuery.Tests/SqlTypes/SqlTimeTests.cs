using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlTimeTests : TestFixture
	{
		private readonly TimeSpan testDateTime = new TimeSpan(0, 22, 17, 11, 352);

		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlTime.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlTime.GetTypeHandler().CreateParamFromValue(null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlTime.GetTypeHandler().CreateMetaData(null));

			ITypeHandler col = new SqlTime(testDateTime, 6);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.Time, meta.SqlDbType);
			Assert.AreEqual(6, meta.Scale);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			ISqlType type = new SqlTime(testDateTime, 5);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Time, testDateTime, scale: 5);

			type = new SqlTime(null, 4);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Time, DBNull.Value, scale: 4);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlTime(testDateTime, 2);
			Assert.AreEqual(testDateTime, type.GetRawValue());

			type = new SqlTime(null, 1);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlTime>(Col.Time(testDateTime, 4));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfTimes", new[] {
					new { A = Col.Time(testDateTime, 5) },
					new { A = Col.Time(null, 5) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(TimeSpan), rows[0].A.GetType());
			Assert.AreEqual(testDateTime, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlTime)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.Time]);
		}
	}
}