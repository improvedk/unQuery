using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlSmallMoneyTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(SqlSmallMoney.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlSmallMoney.GetTypeHandler().CreateParamFromValue("Test", null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlSmallMoney.GetTypeHandler().CreateMetaData(null));

			SqlTypeHandler col = new SqlSmallMoney(5.27m);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.SmallMoney, meta.SqlDbType);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			SqlType type = new SqlSmallMoney(5.27m);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.SmallMoney, 5.27m);

			type = new SqlSmallMoney(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.SmallMoney, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlSmallMoney(5.27m);
			Assert.AreEqual(5.27m, type.GetRawValue());

			type = new SqlSmallMoney(null);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlSmallMoney>(Col.SmallMoney(5.27m));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfSmallMoneys", new[] {
					new { A = Col.SmallMoney(5.27m) },
					new { A = Col.SmallMoney(null) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(decimal), rows[0].A.GetType());
			Assert.AreEqual(5.27m, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = Col.SmallMoney(5.27m),
				B = Col.SmallMoney(null)
			}}).First();

			Assert.AreEqual(2, result.FieldCount);
			Assert.AreEqual(typeof(decimal), result.GetValue(0).GetType());
			Assert.AreEqual(5.27m, result.GetValue(0));
			Assert.AreEqual(DBNull.Value, result.GetValue(1));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlSmallMoney)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.SmallMoney]);
		}
	}
}