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