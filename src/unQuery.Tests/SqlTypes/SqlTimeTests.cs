using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlTimeTests : TestFixture
	{
		private readonly TimeSpan testValue = new TimeSpan(0, 22, 17, 11, 352);

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
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlTime.GetTypeHandler().CreateMetaData("Test"));

			ITypeHandler col = new SqlTime(testValue);
			Assert.Throws<TypePropertiesMustBeSetExplicitlyException>(() => col.CreateMetaData("Test"));

			col = new SqlTime(testValue, 6);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.Time, meta.SqlDbType);
			Assert.AreEqual(6, meta.Scale);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(((ISqlType)new SqlTime(testValue, 2)).GetParameter(), SqlDbType.Time, testValue, scale: 2);
			TestHelper.AssertSqlParameter(((ISqlType)new SqlTime(null, 5)).GetParameter(), SqlDbType.Time, DBNull.Value, scale: 5);
			TestHelper.AssertSqlParameter(((ISqlType)new SqlTime(testValue)).GetParameter(), SqlDbType.Time, testValue, scale: 0);
			TestHelper.AssertSqlParameter(((ISqlType)new SqlTime(null)).GetParameter(), SqlDbType.Time, DBNull.Value, size: 0);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlTime(testValue, 2);
			Assert.AreEqual(testValue, type.GetRawValue());

			type = new SqlTime(null, 1);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlTime>(Col.Time(testValue, 4));
			Assert.IsInstanceOf<SqlTime>(Col.Time(testValue));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfTimes", new[] {
					new { A = Col.Time(testValue, 5) },
					new { A = Col.Time(null, 5) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(TimeSpan), rows[0].A.GetType());
			Assert.AreEqual(testValue, rows[0].A);
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