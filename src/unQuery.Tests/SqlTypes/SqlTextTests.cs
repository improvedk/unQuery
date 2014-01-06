using System.Linq;
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
			Assert.IsInstanceOf<SqlTypeHandler>(SqlText.GetTypeHandler());
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

			SqlTypeHandler col = new SqlText("Test");
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.Text, meta.SqlDbType);
			Assert.AreEqual(-1, meta.MaxLength);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			SqlType type = new SqlText("Test");
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Text, "Test", size: -1);

			type = new SqlText(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Text, DBNull.Value, size: -1);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlText("Test");
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
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = Col.Text("Test"),
				B = Col.Text(null)
			}}).First();

			Assert.AreEqual(2, result.FieldCount);
			Assert.AreEqual(typeof(string), result.GetValue(0).GetType());
			Assert.AreEqual("Test", result.GetValue(0));
			Assert.AreEqual(DBNull.Value, result.GetValue(1));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlText)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.Text]);
		}
	}
}