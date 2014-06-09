using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlTimeTests : TestFixture
	{
		private readonly TimeSpan testValue = new TimeSpan(0, 22, 17, 11, 352);

		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(SqlTime.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlTime.GetTypeHandler().CreateParamFromValue("Test", null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlTime.GetTypeHandler().CreateMetaData("Test"));

			SqlTypeHandler col = new SqlTime(testValue, null, ParameterDirection.Input);
			Assert.Throws<TypePropertiesMustBeSetExplicitlyException>(() => col.CreateMetaData("Test"));

			col = new SqlTime(testValue, 6, ParameterDirection.Input);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.Time, meta.SqlDbType);
			Assert.AreEqual(6, meta.Scale);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter((new SqlTime(testValue, 2, ParameterDirection.Input)).GetParameter(), SqlDbType.Time, testValue, scale: 2);
			TestHelper.AssertSqlParameter((new SqlTime(null, 5, ParameterDirection.Input)).GetParameter(), SqlDbType.Time, DBNull.Value, scale: 5);
			TestHelper.AssertSqlParameter((new SqlTime(testValue, null, ParameterDirection.Input)).GetParameter(), SqlDbType.Time, testValue, scale: 0);
			TestHelper.AssertSqlParameter((new SqlTime(null, null, ParameterDirection.Input)).GetParameter(), SqlDbType.Time, DBNull.Value, size: 0);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlTime(testValue, 2, ParameterDirection.Input);
			Assert.AreEqual(testValue, type.GetRawValue());

			type = new SqlTime(null, 1, ParameterDirection.Input);
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
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = Col.Time(testValue, 5),
				B = Col.Time(null, 5)
			}}).First();

			Assert.AreEqual(2, result.FieldCount);
			Assert.AreEqual(typeof(TimeSpan), result.GetValue(0).GetType());
			Assert.AreEqual(testValue, result.GetValue(0));
			Assert.AreEqual(DBNull.Value, result.GetValue(1));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlTime)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.Time]);
		}
	}
}