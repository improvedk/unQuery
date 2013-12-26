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
		public void GetParameter()
		{
			ISqlType col = new SqlStructured("MyType", null);
			var param = col.GetParameter();
			TestHelper.AssertSqlParameter(param, SqlDbType.Structured, null, DBNull.Value);

			col = new SqlStructured("MyType", value);
			param = col.GetParameter();
			Assert.AreEqual(SqlDbType.Structured, param.SqlDbType);
			Assert.AreEqual("MyType", param.TypeName);
			Assert.AreEqual(typeof(StructuredDynamicYielder), param.Value.GetType());
			Assert.AreEqual(2, ((IEnumerable<SqlDataRecord>)param.Value).Count());
		}

		[Test]
		public void StructuredParameter_NoPropertiesObject()
		{
			Assert.Throws<ObjectHasNoPropertiesException>(() => DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfTinyInts", new object[] { (byte)1 })
			}));
		}

		[Test]
		public void Factory()
		{
			ISqlType col = Col.Structured("MyType", new dynamic[] { new { A = 5, B = true } });
			var param = col.GetParameter();

			Assert.AreEqual("MyType", param.TypeName);
			Assert.AreEqual(SqlDbType.Structured, param.SqlDbType);
			Assert.AreEqual(typeof(StructuredDynamicYielder), param.Value.GetType());
		}

		[Test]
		public void GetRawValue()
		{
			ISqlType col = new SqlStructured("A", null);

			Assert.Throws<InvalidOperationException>(() => col.GetRawValue());
		}
	}
}