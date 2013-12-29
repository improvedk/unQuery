using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlTextTests : TestFixture
	{
		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlText.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlText.GetTypeHandler().CreateParamFromValue(null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlText.GetTypeHandler().CreateMetaData(null));

			ITypeHandler col = new SqlText("Test");
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.Text, meta.SqlDbType);
			Assert.AreEqual(-1, meta.MaxLength);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			ISqlType type = new SqlText("Test");
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Text, "Test", size: -1);

			type = new SqlText(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Text, DBNull.Value, size: -1);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlText("Test");
			Assert.AreEqual("Test", type.GetRawValue());

			type = new SqlText(null);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlText>(Col.Text("Test"));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfTexts", new[] {
					new { A = Col.Text("Test") },
					new { A = Col.Text(null) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(string), rows[0].A.GetType());
			Assert.AreEqual("Test", rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlText)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.Text]);
		}
	}
}