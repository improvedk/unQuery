using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlMoneyTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlMoney.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlMoney.GetTypeHandler().CreateParamFromValue(null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlMoney.GetTypeHandler().CreateMetaData(null));

			ITypeHandler col = new SqlMoney(5.27m);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.Money, meta.SqlDbType);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			ISqlType type = new SqlMoney(5.27m);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Money, 5.27m);

			type = new SqlMoney(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Money, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlMoney(5.27m);
			Assert.AreEqual(5.27m, type.GetRawValue());

			type = new SqlMoney(null);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlMoney>(Col.Money(5.27m));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfMoneys", new[] {
					new { A = Col.Money(5.27m) },
					new { A = Col.Money(null) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(decimal), rows[0].A.GetType());
			Assert.AreEqual(5.27m, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlMoney)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.Money]);
		}
	}
}