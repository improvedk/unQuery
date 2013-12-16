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
		public void StructuredParameter_NoPropertiesObject()
		{
			Assert.Throws<ObjectHasNoPropertiesException>(() => DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfTinyInts", new object[] { (byte)1 })
			}));
		}

		[Test]
		public void StructuredParameter_TinyInt()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfTinyInts", new[] {
					new { A = (byte?)1 },
					new { A = (byte?)null }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(byte), rows[0].A.GetType());
			Assert.AreEqual(1, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredParameter_SmallInt()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfSmallInts", new[] {
					new { A = (short?)1 },
					new { A = (short?)null }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(short), rows[0].A.GetType());
			Assert.AreEqual(1, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredParameter_Int()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfInts", new[] {
					new { A = (int?)1 },
					new { A = (int?)null }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(int), rows[0].A.GetType());
			Assert.AreEqual(1, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredParameter_BigInt()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfBigInts", new[] {
					new { A = (long?)1 },
					new { A = (long?)null }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(long), rows[0].A.GetType());
			Assert.AreEqual(1, rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredParameter_Bit()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfBits", new[] {
					new { A = (bool?)true },
					new { A = (bool?)false },
					new { A = (bool?)null }
				})
			});

			Assert.AreEqual(3, rows.Count);
			Assert.AreEqual(typeof(bool), rows[0].A.GetType());
			Assert.AreEqual(true, rows[0].A);
			Assert.AreEqual(false, rows[1].A);
			Assert.AreEqual(null, rows[2].A);
		}

		[Test]
		public void StructuredParameter_UniqueIdentifier()
		{
			var guid = Guid.NewGuid();

			var rows = DB.GetRows("SELECT * FROM @Input", new
			{
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
		public void StructuredParameter_NVarChar()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new
			{
				Input = Col.Structured("ListOfNVarChars", new[] {
					new { A = Col.NVarChar("слово") },
					new { A = Col.NVarChar(null) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(string), rows[0].A.GetType());
			Assert.AreEqual("слово", rows[0].A);
			Assert.AreEqual(null, rows[1].A);
		}

		[Test]
		public void StructuredParameter_VarChar()
		{
			var rows = DB.GetRows("SELECT * FROM @Input", new
			{
				Input = Col.Structured("ListOfVarChars", new[] {
					new { A = Col.VarChar("ABC") },
					new { A = Col.VarChar(null) }
				})
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(typeof(string), rows[0].A.GetType());
			Assert.AreEqual("ABC", rows[0].A);
			Assert.AreEqual(null, rows[1].A);
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