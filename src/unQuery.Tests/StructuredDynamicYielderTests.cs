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
		public void GetEnumerator_Types()
		{
			var values = new[] {
				new {
					A = (byte)1,
					B = (byte?)2,
					C = Col.TinyInt(3),
					D = (byte?)null,
					E = Col.TinyInt(null)
				}
			};

			var yielder = new StructuredDynamicYielder(values);
			var result = yielder.First();

			Assert.AreEqual(5, result.FieldCount);
			Assert.AreEqual(1, result.GetValue(0));
			Assert.AreEqual(2, result.GetValue(1));
			Assert.AreEqual(3, result.GetValue(2));
			Assert.AreEqual(DBNull.Value, result.GetValue(3));
			Assert.AreEqual(DBNull.Value, result.GetValue(4));
		}
	}
}