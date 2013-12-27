using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlDateTimeOffsetTests : TestFixture
	{
		private readonly DateTimeOffset testValue = new DateTimeOffset(new DateTime(2012, 11, 10, 1, 2, 3, 003), TimeSpan.FromHours(2));

		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlDateTimeOffset.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlDateTimeOffset.GetTypeHandler().CreateParamFromValue(null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlDateTimeOffset.GetTypeHandler().CreateMetaData(null));

			ITypeHandler col = new SqlDateTimeOffset(testValue, 5);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.DateTimeOffset, meta.SqlDbType);
			Assert.AreEqual(5, meta.Scale);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			ISqlType type = new SqlDateTimeOffset(testValue, 1);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.DateTimeOffset, testValue, scale: 1);

			type = new SqlDateTimeOffset(null, 7);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.DateTimeOffset, DBNull.Value, scale: 7);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlDateTimeOffset(testValue, 2);
			Assert.AreEqual(testValue, type.GetRawValue());

			type = new SqlDateTimeOffset(null, 0);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlDateTimeOffset>(Col.DateTimeOffset(testValue, 7));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfDateTimeOffsets", new[] {
					new { A = Col.DateTimeOffset(testValue, 4) },
					new { A = Col.DateTimeOffset(null, 4) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(DateTimeOffset), rows[0].A.GetType());
			Assert.AreEqual(testValue, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlDateTimeOffset)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.DateTimeOffset]);
		}
	}
}