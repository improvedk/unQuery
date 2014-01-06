using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlUniqueIdentifierTests : TestFixture
	{
		private readonly Guid guid = Guid.NewGuid();

		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(SqlUniqueIdentifier.GetTypeHandler());
		}

		[Test]
		public void CreateParamFromValue()
		{
			TestHelper.AssertParameterFromValue(guid, SqlDbType.UniqueIdentifier, guid);
			TestHelper.AssertParameterFromValue((Guid?)null, SqlDbType.UniqueIdentifier, DBNull.Value);
		}

		[Test]
		public void CreateMetaData()
		{
			var meta = SqlUniqueIdentifier.GetTypeHandler().CreateMetaData("Test");
			Assert.AreEqual(SqlDbType.UniqueIdentifier, meta.SqlDbType);
			Assert.AreEqual("Test", meta.Name);
		}

		[Test]
		public void GetParameter()
		{
			SqlType type = new SqlUniqueIdentifier(guid);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.UniqueIdentifier, guid);

			type = new SqlUniqueIdentifier(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.UniqueIdentifier, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			SqlType type = new SqlUniqueIdentifier(guid);
			Assert.AreEqual(guid, type.GetRawValue());

			type = new SqlUniqueIdentifier(null);
			Assert.Null(type.GetRawValue());
		}

		[Test]
		public void Factory()
		{
			Assert.IsInstanceOf<SqlUniqueIdentifier>(Col.UniqueIdentifier(guid));
		}

		[Test]
		public void Structured()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfUniqueIdentifiers", new[] {
					new { A = (Guid?)guid },
					new { A = (Guid?)null }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(Guid), rows[0].A.GetType());
			Assert.AreEqual(guid, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredDynamicYielder()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = guid,
				B = (Guid?)guid,
				C = Col.UniqueIdentifier(guid),
				D = (Guid?)null,
				E = Col.UniqueIdentifier(null)
			}}).First();

			Assert.AreEqual(5, result.FieldCount);
			Assert.AreEqual(typeof(Guid), result.GetValue(0).GetType());
			Assert.AreEqual(guid, result.GetValue(0));
			Assert.AreEqual(typeof(Guid), result.GetValue(1).GetType());
			Assert.AreEqual(guid, result.GetValue(1));
			Assert.AreEqual(typeof(Guid), result.GetValue(2).GetType());
			Assert.AreEqual(guid, result.GetValue(2));
			Assert.AreEqual(DBNull.Value, result.GetValue(3));
			Assert.AreEqual(DBNull.Value, result.GetValue(4));
		}

		[Test]
		public void TypeMaps()
		{
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(Guid)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(Guid?)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.ClrTypeHandlers[typeof(SqlUniqueIdentifier)]);
			Assert.IsInstanceOf<SqlTypeHandler>(unQueryDB.SqlDbTypeHandlers[SqlDbType.UniqueIdentifier]);
		}
	}
}