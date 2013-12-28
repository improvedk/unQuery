using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlImageTests : TestFixture
	{
		private readonly byte[] data = new byte[] { 0x0A, 0xA0, 0xAA };

		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlImage.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlImage.GetTypeHandler().CreateParamFromValue(null));
		}

		[Test]
		public void CreateMetaData()
		{
			Assert.Throws<TypeCannotBeUsedAsAClrTypeException>(() => SqlImage.GetTypeHandler().CreateMetaData(null));

			ITypeHandler col = new SqlImage(data);
			var meta = col.CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.Image, meta.SqlDbType);
			Assert.AreEqual(-1, meta.MaxLength);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			ISqlType type = new SqlImage(data);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Image, data, size: -1);

			type = new SqlImage(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.Image, DBNull.Value, size: -1);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlImage(data);
			Assert.AreEqual(data, type.GetRawValue());

			type = new SqlImage(null);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlImage>(Col.Image(data));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfImages", new[] {
					new { A = Col.Image(new byte[] { 0xAA, 0xBB }) },
					new { A = Col.Image(null) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(byte[]), rows[0].A.GetType());
			Assert.AreEqual(new byte[] { 0xAA, 0xBB }, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlImage)]);
			Assert.IsInstanceOf<ITypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.Image]);
		}
	}
}