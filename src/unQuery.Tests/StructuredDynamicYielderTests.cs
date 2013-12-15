using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests
{
	[TestFixture]
	public class StructuredDynamicYielderTests
	{
		[Test]
		public void Constructor_Null()
		{
			Assert.Throws<ArgumentException>(() => new StructuredDynamicYielder(null));
		}

		[Test]
		public void GetEnumerator_SchemaAndValues()
		{
			var values = new[] {
				new { A = 5, B = true },
				new { A = 10, B = false }
			};

			var yielder = new StructuredDynamicYielder(values);

			int counter = 0;
			foreach (var value in yielder)
			{
				counter++;

				if (counter == 1)
				{
					Assert.AreEqual(2, value.FieldCount);

					Assert.AreEqual("A", value.GetName(0));
					Assert.AreEqual(SqlDbType.Int, value.GetSqlMetaData(0).SqlDbType);
					Assert.AreEqual(5, value.GetInt32(0));

					Assert.AreEqual("B", value.GetName(1));
					Assert.AreEqual(SqlDbType.Bit, value.GetSqlMetaData(1).SqlDbType);
					Assert.AreEqual(true, value.GetBoolean(1));
				}
				else
				{
					Assert.AreEqual(2, value.FieldCount);

					Assert.AreEqual("A", value.GetName(0));
					Assert.AreEqual(SqlDbType.Int, value.GetSqlMetaData(0).SqlDbType);
					Assert.AreEqual(10, value.GetInt32(0));

					Assert.AreEqual("B", value.GetName(1));
					Assert.AreEqual(SqlDbType.Bit, value.GetSqlMetaData(1).SqlDbType);
					Assert.AreEqual(false, value.GetBoolean(1));
				}
			}

			Assert.AreEqual(2, counter);
		}

		[Test]
		public void GetEnumerator_TinyInt()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = (byte)1,
				B = (byte?)2,
				C = Col.TinyInt(3),
				D = (byte?)null,
				E = Col.TinyInt(null)
			}}).First();
			
			Assert.AreEqual(5, result.FieldCount);
			Assert.AreEqual(typeof(byte), result.GetValue(0).GetType());
			Assert.AreEqual(1, result.GetValue(0));
			Assert.AreEqual(typeof(byte), result.GetValue(1).GetType());
			Assert.AreEqual(2, result.GetValue(1));
			Assert.AreEqual(typeof(byte), result.GetValue(2).GetType());
			Assert.AreEqual(3, result.GetValue(2));
			Assert.AreEqual(DBNull.Value, result.GetValue(3));
			Assert.AreEqual(DBNull.Value, result.GetValue(4));
		}

		[Test]
		public void GetEnumerator_SmallInt()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = (short)1,
				B = (short?)2,
				C = Col.SmallInt(3),
				D = (short?)null,
				E = Col.SmallInt(null)
			}}).First();
			
			Assert.AreEqual(5, result.FieldCount);
			Assert.AreEqual(typeof(short), result.GetValue(0).GetType());
			Assert.AreEqual(1, result.GetValue(0));
			Assert.AreEqual(typeof(short), result.GetValue(1).GetType());
			Assert.AreEqual(2, result.GetValue(1));
			Assert.AreEqual(typeof(short), result.GetValue(2).GetType());
			Assert.AreEqual(3, result.GetValue(2));
			Assert.AreEqual(DBNull.Value, result.GetValue(3));
			Assert.AreEqual(DBNull.Value, result.GetValue(4));
		}

		[Test]
		public void GetEnumerator_Int()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = 1,
				B = (int?)2,
				C = Col.Int(3),
				D = (int?)null,
				E = Col.Int(null)
			}}).First();
			
			Assert.AreEqual(5, result.FieldCount);
			Assert.AreEqual(typeof(int), result.GetValue(0).GetType());
			Assert.AreEqual(1, result.GetValue(0));
			Assert.AreEqual(typeof(int), result.GetValue(1).GetType());
			Assert.AreEqual(2, result.GetValue(1));
			Assert.AreEqual(typeof(int), result.GetValue(2).GetType());
			Assert.AreEqual(3, result.GetValue(2));
			Assert.AreEqual(DBNull.Value, result.GetValue(3));
			Assert.AreEqual(DBNull.Value, result.GetValue(4));
		}

		[Test]
		public void GetEnumerator_BigInt()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = (long)1,
				B = (long?)2,
				C = Col.BigInt(3),
				D = (long?)null,
				E = Col.BigInt(null)
			}}).First();
			
			Assert.AreEqual(5, result.FieldCount);
			Assert.AreEqual(typeof(long), result.GetValue(0).GetType());
			Assert.AreEqual(1, result.GetValue(0));
			Assert.AreEqual(typeof(long), result.GetValue(1).GetType());
			Assert.AreEqual(2, result.GetValue(1));
			Assert.AreEqual(typeof(long), result.GetValue(2).GetType());
			Assert.AreEqual(3, result.GetValue(2));
			Assert.AreEqual(DBNull.Value, result.GetValue(3));
			Assert.AreEqual(DBNull.Value, result.GetValue(4));
		}

		[Test]
		public void GetEnumerator_Bit()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = true,
				B = (bool?)false,
				C = Col.Bit(true),
				D = (bool?)null,
				E = Col.Bit(null)
			}}).First();
			
			Assert.AreEqual(5, result.FieldCount);
			Assert.AreEqual(typeof(bool), result.GetValue(0).GetType());
			Assert.AreEqual(true, result.GetValue(0));
			Assert.AreEqual(typeof(bool), result.GetValue(1).GetType());
			Assert.AreEqual(false, result.GetValue(1));
			Assert.AreEqual(typeof(bool), result.GetValue(2).GetType());
			Assert.AreEqual(true, result.GetValue(2));
			Assert.AreEqual(DBNull.Value, result.GetValue(3));
			Assert.AreEqual(DBNull.Value, result.GetValue(4));
		}

		[Test]
		public void GetEnumerator_UniqueIdentifier()
		{
			var guid = Guid.NewGuid();

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
		public void GetEnumerator_NVarChar()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = Col.NVarChar("A"),
				B = Col.NVarChar(null)
			}}).First();
			
			Assert.AreEqual(2, result.FieldCount);
			Assert.AreEqual(typeof(string), result.GetValue(0).GetType());
			Assert.AreEqual("A", result.GetValue(0));
			Assert.AreEqual(DBNull.Value, result.GetValue(1));
		}

		[Test]
		public void GetEnumerator_VarChar()
		{
			var result = new StructuredDynamicYielder(new[] { new {
				A = Col.NVarChar("A"),
				B = Col.NVarChar(null)
			}}).First();

			Assert.AreEqual(2, result.FieldCount);
			Assert.AreEqual(typeof(string), result.GetValue(0).GetType());
			Assert.AreEqual("A", result.GetValue(0));
			Assert.AreEqual(DBNull.Value, result.GetValue(1));
		}
	}
}