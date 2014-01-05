using NUnit.Framework;
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Reflection;
using unQuery.SqlTypes;

namespace unQuery.Tests
{
	[TestFixture]
	public class StructuredDynamicYielderTests
	{
		[Test]
		public void RawListOfImplicitTypes()
		{
			var values = new[] { 1, 2, (int?)null }.Cast<object>();
			var yielder = new StructuredDynamicYielder(values);

			int counter = 0;
			foreach (var value in yielder)
			{
				counter++;

				if (counter == 3)
				{
					Assert.AreEqual(SqlDbType.Int, value.GetSqlMetaData(0).SqlDbType);
					Assert.AreEqual(DBNull.Value, value.GetValue(0));
				}
				else
				{
					Assert.AreEqual(SqlDbType.Int, value.GetSqlMetaData(0).SqlDbType);
					Assert.AreEqual(typeof(int), value.GetValue(0).GetType());
					Assert.AreEqual(counter, value.GetValue(0));
				}
			}

			Assert.AreEqual(3, counter);
		}

		[Test]
		public void RawListOfUnspecifiedExplicitTypes()
		{	
			var values = new[] { Col.Decimal(5.27m) };
			var yielder = new StructuredDynamicYielder(values);

			Assert.Throws<TypePropertiesMustBeSetExplicitlyException>(() => yielder.ToList());
		}

		[Test]
		public void RawListOfDifferentImplicitTypes()
		{
			var yielder = new StructuredDynamicYielder(new object[] { 5, (short)2 });
			Assert.Throws<StructuredTypeMismatchException>(() => yielder.ToList());

			yielder = new StructuredDynamicYielder(new object[] { (short)2, 5 });
			Assert.Throws<StructuredTypeMismatchException>(() => yielder.ToList());
		}

		[Test]
		public void RawListOfMixedImplicitAndSimilarExplicit()
		{
			var yielder = new StructuredDynamicYielder(new object[] { 5, Col.Int(2) });
			Assert.Throws<StructuredTypeMismatchException>(() => yielder.ToList());

			yielder = new StructuredDynamicYielder(new object[] { Col.Int(2), 5 });
			Assert.Throws<StructuredTypeMismatchException>(() => yielder.ToList());
		}

		[Test]
		public void RawListOfMixedImplicitAndDifferentExplicit()
		{
			var yielder = new StructuredDynamicYielder(new object[] { 5, Col.SmallInt(2) });
			Assert.Throws<StructuredTypeMismatchException>(() => yielder.ToList());

			yielder = new StructuredDynamicYielder(new object[] { Col.SmallInt(2), 5 });
			Assert.Throws<StructuredTypeMismatchException>(() => yielder.ToList());
		}

		[Test]
		public void RawListOfDifferentExplicitTypes()
		{
			var values = new object[] { Col.Decimal(5.27m, 5, 2), Col.SmallMoney(5.27m) };
			var yielder = new StructuredDynamicYielder(values);

			Assert.Throws<StructuredTypeMismatchException>(() => yielder.ToList());
		}

		[Test]
		public void RawListOfDifferingSpecification()
		{
			// While this is bad pracice, the first definition will win
			var values = new[] { Col.Decimal(5.27m, 5, 2), Col.Decimal(5.27m, 6, 1) };
			var yielder = new StructuredDynamicYielder(values);

			Assert.DoesNotThrow(() => yielder.ToList());
		}

		[Test]
		public void RawListOfExplicitTypes()
		{
			var values = new[] { Col.Decimal(5.27m, 5, 2), Col.Decimal(null) };
			var yielder = new StructuredDynamicYielder(values);

			int counter = 0;
			foreach (var value in yielder)
			{
				counter++;

				if (counter == 1)
				{
					Assert.AreEqual(SqlDbType.Decimal, value.GetSqlMetaData(0).SqlDbType);
					Assert.AreEqual(5, value.GetSqlMetaData(0).Precision);
					Assert.AreEqual(2, value.GetSqlMetaData(0).Scale);
					Assert.AreEqual(typeof(decimal), value.GetValue(0).GetType());
					Assert.AreEqual(5.27m, value.GetValue(0));
				}
				else
				{
					Assert.AreEqual(SqlDbType.Decimal, value.GetSqlMetaData(0).SqlDbType);
					Assert.AreEqual(5, value.GetSqlMetaData(0).Precision);
					Assert.AreEqual(2, value.GetSqlMetaData(0).Scale);
					Assert.AreEqual(DBNull.Value, value.GetValue(0));
				}
			}

			Assert.AreEqual(2, counter);
		}

		[Test]
		public void Constructor_Null()
		{
			Assert.Throws<ArgumentException>(() => new StructuredDynamicYielder(null));
		}

		[Test]
		public void TypeReaderCaching()
		{
			StructuredDynamicYielder.ResetCache();

			var type = typeof(StructuredDynamicYielder);
			var field = type.GetField("typeHandlers", BindingFlags.NonPublic | BindingFlags.Static);
			var value = field.GetValue(null);

			Assert.AreEqual(0, ((IDictionary)value).Count);

			new StructuredDynamicYielder(new[] { new { A = 25 } }).GetEnumerator().MoveNext();

			Assert.AreEqual(1, ((IDictionary)value).Count);

			new StructuredDynamicYielder(new[] { new { A = 25 } }).GetEnumerator().MoveNext();

			Assert.AreEqual(1, ((IDictionary)value).Count);

			new StructuredDynamicYielder(new[] { new { A = 25, B = 32 } }).GetEnumerator().MoveNext();

			Assert.AreEqual(2, ((IDictionary)value).Count);
		}

		[Test]
		public void SubsequentIdenticalType()
		{
			var yielder = new StructuredDynamicYielder(new[] {
				new { A = 25, B = false },
				new { A = 17, B = true }
			});
			var rows = yielder.GetEnumerator();

			rows.MoveNext();
			var row = rows.Current;
			Assert.AreEqual(25, row["A"]);
			Assert.AreEqual(false, row["B"]);

			rows.MoveNext();
			row = rows.Current;
			Assert.AreEqual(17, row["A"]);
			Assert.AreEqual(true, row["B"]);

			yielder = new StructuredDynamicYielder(new[] {
				new { A = 11, B = true }
			});
			var value = yielder.Single();

			Assert.AreEqual(11, value["A"]);
			Assert.AreEqual(true, value["B"]);
		}

		[Test]
		public void MixAndMatchTypes()
		{
			var values = new[] { new {
				A = (byte?)null,
				B = 5,
				C = Col.Money(5.27m),
				D = 8L,
				E = Col.Float(2.17d),
				F = Col.Float(null),
				G = (int?)null,
				H = (long?)8,
				I = true,
				J = (byte)2,
				K = Col.SmallInt(null),
				L = Col.SmallInt(null),
				M = Col.SmallInt(2),
				N = (bool?)true,
				O = (bool?)true
			}};

			var yielder = new StructuredDynamicYielder(values);
			var record = yielder.Single();

			Assert.AreEqual(DBNull.Value, record["A"]);
			Assert.AreEqual(typeof(int), record["B"].GetType());
			Assert.AreEqual(5, record["B"]);
			Assert.AreEqual(typeof(decimal), record["C"].GetType());
			Assert.AreEqual(5.27m, record["C"]);
			Assert.AreEqual(typeof(long), record["D"].GetType());
			Assert.AreEqual(8L, record["D"]);
			Assert.AreEqual(typeof(double), record["E"].GetType());
			Assert.AreEqual(2.17d, record["E"]);
			Assert.AreEqual(DBNull.Value, record["F"]);
			Assert.AreEqual(DBNull.Value, record["G"]);
			Assert.AreEqual(typeof(long), record["H"].GetType());
			Assert.AreEqual(8L, record["H"]);
			Assert.AreEqual(typeof(bool), record["I"].GetType());
			Assert.AreEqual(true, record["I"]);
			Assert.AreEqual(typeof(byte), record["J"].GetType());
			Assert.AreEqual(2, record["J"]);
			Assert.AreEqual(DBNull.Value, record["K"]);
			Assert.AreEqual(DBNull.Value, record["L"]);
			Assert.AreEqual(typeof(short), record["M"].GetType());
			Assert.AreEqual(2, record["M"]);
			Assert.AreEqual(typeof(bool), record["N"].GetType());
			Assert.AreEqual(true, record["N"]);
			Assert.AreEqual(typeof(bool), record["O"].GetType());
			Assert.AreEqual(true, record["O"]);
		}

		[Test]
		public void ImplicitNativeTypes()
		{
			var guid = Guid.NewGuid();

			var values = new[] { new {
				ByteA = (byte)1,
				ByteB = (byte?)2,
				ByteC = (byte?)null,
				ShortA = (short)3,
				ShortB = (short?)4,
				ShortC = (short?)null,
				IntA = (int)5,
				IntB = (int?)6,
				IntC = (int?)null,
				LongA = (long)7,
				LongB = (long?)8,
				LongC = (long?)null,
				BoolA = true,
				BoolB = (bool?)false,
				BoolC = (bool?)null,
				GuidA = guid,
				GuidB = (Guid?)guid,
				GuidC = (Guid?)null
			} };

			var yielder = new StructuredDynamicYielder(values);
			var record = yielder.Single();

			Assert.AreEqual(1, record["ByteA"]);
			Assert.AreEqual(2, record["ByteB"]);
			Assert.AreEqual(DBNull.Value, record["ByteC"]);
			Assert.AreEqual(3, record["ShortA"]);
			Assert.AreEqual(4, record["ShortB"]);
			Assert.AreEqual(DBNull.Value, record["ShortC"]);
			Assert.AreEqual(5, record["IntA"]);
			Assert.AreEqual(6, record["IntB"]);
			Assert.AreEqual(DBNull.Value, record["IntC"]);
			Assert.AreEqual(7, record["LongA"]);
			Assert.AreEqual(8, record["LongB"]);
			Assert.AreEqual(DBNull.Value, record["LongC"]);
			Assert.AreEqual(true, record["BoolA"]);
			Assert.AreEqual(false, record["BoolB"]);
			Assert.AreEqual(DBNull.Value, record["BoolC"]);
			Assert.AreEqual(guid, record["GuidA"]);
			Assert.AreEqual(guid, record["GuidB"]);
			Assert.AreEqual(DBNull.Value, record["GuidC"]);
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
	}
}