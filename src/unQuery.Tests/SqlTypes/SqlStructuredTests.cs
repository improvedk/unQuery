using Microsoft.SqlServer.Server;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlStructuredTests : TestFixture
	{
		private readonly dynamic[] value = new dynamic[] {
			new { A = 5, B = true },
			new { A = 10, B = false }
		};

		[Test]
		public void Constructor()
		{
			var col = new SqlStructured("MyType", value);
			var param = col.GetParameter();

			Assert.AreEqual("MyType", param.TypeName);
			Assert.AreEqual(SqlDbType.Structured, param.SqlDbType);
			Assert.AreEqual(typeof(StructuredDynamicYielder), param.Value.GetType());
			
			var sdr = (IEnumerable<SqlDataRecord>)param.Value;

			Assert.AreEqual(2, sdr.Count());
		}

		[Test]
		public void GetParameter()
		{
			TestHelper.AssertSqlParameter(SqlStructured.GetParameter("MyType", null), SqlDbType.Structured, null, DBNull.Value);

			var param = SqlStructured.GetParameter("MyType", value);
			Assert.AreEqual(SqlDbType.Structured, param.SqlDbType);
			Assert.AreEqual("MyType", param.TypeName);
			Assert.AreEqual(typeof(StructuredDynamicYielder), param.Value.GetType());
			Assert.AreEqual(2, ((IEnumerable<SqlDataRecord>)param.Value).Count());
		}

		[Test]
		public void StructuredParameter()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("MyType", value)
			});

			Assert.AreEqual(2, rows.Count);

			Assert.AreEqual(typeof(int), rows[0].A.GetType());
			Assert.AreEqual(5, rows[0].A);
			Assert.AreEqual(typeof(bool), rows[0].B.GetType());
			Assert.AreEqual(true, rows[0].B);

			Assert.AreEqual(typeof(int), rows[1].A.GetType());
			Assert.AreEqual(10, rows[1].A);
			Assert.AreEqual(typeof(bool), rows[1].B.GetType());
			Assert.AreEqual(false, rows[1].B);
		}

		[Test]
		public void GetRawValue()
		{
			Assert.Throws<InvalidOperationException>(() => new SqlStructured("A", null).GetRawValue());
		}

		[Test]
		public void GetSqlDbType()
		{
			Assert.AreEqual(SqlDbType.Structured, new SqlStructured("Test", null).GetSqlDbType());
		}
	}
}