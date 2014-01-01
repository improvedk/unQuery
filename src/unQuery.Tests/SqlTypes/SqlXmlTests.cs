using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlXmlTests : TestFixture
	{
		private string testXml = "<root>Test</root>";

		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlXml.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlXml.GetTypeHandler().CreateParamFromValue(null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlXml.GetTypeHandler().CreateMetaData(null));

			ITypeHandler col = new SqlXml(testXml);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.Xml, meta.SqlDbType);
			Assert.AreEqual(-1, meta.MaxLength);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			ISqlType type = new SqlXml(testXml);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Xml, testXml);

			type = new SqlXml(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Xml, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlXml(testXml);
			Assert.AreEqual(testXml, type.GetRawValue());

			type = new SqlXml(null);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlXml>(Col.Xml(testXml));
		}

		[Test]
		public void ValueType_String()
		{
			string xml = DB.GetScalar<string>("SELECT @Xml", new {
				Xml = Col.Xml("<root>Test</root>")
			});
			Assert.AreEqual(xml, "<root>Test</root>");
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfXmls", new[] {
					new { A = Col.Xml(testXml) },
					new { A = Col.Xml(null) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(string), rows[0].A.GetType());
			Assert.AreEqual(testXml, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = Col.Xml(testXml),
				B = Col.Xml(null)
			}}).First();

			Assert.AreEqual(2, result.FieldCount);
			Assert.AreEqual(typeof(string), result.GetValue(0).GetType());
			Assert.AreEqual(testXml, result.GetValue(0));
			Assert.AreEqual(DBNull.Value, result.GetValue(1));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlXml)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.Xml]);
		}
	}
}