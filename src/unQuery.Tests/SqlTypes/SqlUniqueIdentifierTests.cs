using NUnit.Framework;
using System;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlUniqueIdentifierTests : TestFixture
	{
		private readonly Guid guid = Guid.NewGuid();

		[Test]
		public void GetTypeHandler()
		{
			Assert.IsInstanceOf<ITypeHandler>(SqlUniqueIdentifier.GetTypeHandler());
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
			ISqlType type = new SqlUniqueIdentifier(guid);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.UniqueIdentifier, guid);

			type = new SqlUniqueIdentifier(null);
			TestHelper.AssertSqlParameter(type.GetParameter(), SqlDbType.UniqueIdentifier, DBNull.Value);
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType type = new SqlUniqueIdentifier(guid);
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
	}
}