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
			SqlType col = new SqlStructured("MyType", null);
			var param = col.GetParameter();
			TestHelper.AssertSqlParameter(param, SqlDbType.Structured, DBNull.Value);

			col = new SqlStructured("MyType", value);
			param = col.GetParameter();
			Assert.AreEqual(SqlDbType.Structured, param.SqlDbType);
			Assert.AreEqual("MyType", param.TypeName);
			Assert.AreEqual(typeof(StructuredDynamicYielder), param.Value.GetType());
			Assert.AreEqual(2, ((IEnumerable<SqlDataRecord>)param.Value).Count());
		}

		[Test]
		public void NoPropertiesObject()
		{
			Assert.Throws<ObjectHasNoPropertiesException>(() => DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfTinyInts", new object[] { new {} })
			}));
		}

		public class MyPersonType
		{
			public SqlNVarChar Name { get; set; }
			public short Age { get; set; }
			public bool? Active { get; set; }

			public MyPersonType(string name, short age, bool? active)
			{
				Name = Col.NVarChar(name, 50);
				Age = age;
				Active = active;
			}
		}

		[Test]
		public void AnonymousValues()
		{
			var persons = DB.GetRows("SELECT * FROM @Persons", new {
				Persons = Col.Structured("MyPersonType", new[] {
					new { Name = Col.NVarChar("ABC", 50), Age = (short)25, Active = (bool?)true },
					new { Name = Col.NVarChar("XYZ", 50), Age = (short)2, Active = (bool?)false },
					new { Name = Col.NVarChar("IJK", 50), Age = (short)17, Active = (bool?)null }
				})
			});

			Assert.AreEqual(3, persons.Count);

			Assert.AreEqual("ABC", persons[0].Name);
			Assert.AreEqual(25, persons[0].Age);
			Assert.AreEqual(true, persons[0].Active);

			Assert.AreEqual("XYZ", persons[1].Name);
			Assert.AreEqual(2, persons[1].Age);
			Assert.AreEqual(false, persons[1].Active);

			Assert.AreEqual("IJK", persons[2].Name);
			Assert.AreEqual(17, persons[2].Age);
			Assert.AreEqual(null, persons[2].Active);
		}

		[Test]
		public void IEnumerableOfValueTypes()
		{
			var persons = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfInts", new [] { 1, 2, 3 })
			});

			Assert.AreEqual(3, persons.Count);
			Assert.AreEqual(1, persons[0].A);
			Assert.AreEqual(2, persons[1].A);
			Assert.AreEqual(3, persons[2].A);
		}

		[Test]
		public void IEnumerableOfSqlTypes()
		{
			var persons = DB.GetRows("SELECT * FROM @Input", new {
				Input = Col.Structured("ListOfInts", new [] {
					Col.Int(1),
					Col.Int(2),
					Col.Int(3)
				})
			});

			Assert.AreEqual(3, persons.Count);
			Assert.AreEqual(1, persons[0].A);
			Assert.AreEqual(2, persons[1].A);
			Assert.AreEqual(3, persons[2].A);
		}

		[Test]
		public void NoInputRows()
		{
			int personCount = DB.GetScalar<int>("SELECT COUNT(*) FROM @Persons", new {
				Persons = Col.Structured("MyPersonType", new MyPersonType[0])
			});

			Assert.AreEqual(0, personCount);
		}

		[Test]
		public void NullInput()
		{
			Assert.Throws<NotSupportedException>(() => DB.GetScalar<int>("SELECT COUNT(*) FROM @Persons", new {
				Persons = Col.Structured("MyPersonType", null)
			}));
		}

		[Test]
		public void StronglyTypedValues()
		{
			var persons = DB.GetRows("SELECT * FROM @Persons", new {
				Persons = Col.Structured("MyPersonType", new [] {
					new MyPersonType("ABC", 25, true),
					new MyPersonType("XYZ", 2, false),
					new MyPersonType("IJK", 17, null)
				})
			});

			Assert.AreEqual(3, persons.Count);

			Assert.AreEqual("ABC", persons[0].Name);
			Assert.AreEqual(25, persons[0].Age);
			Assert.AreEqual(true, persons[0].Active);

			Assert.AreEqual("XYZ", persons[1].Name);
			Assert.AreEqual(2, persons[1].Age);
			Assert.AreEqual(false, persons[1].Active);

			Assert.AreEqual("IJK", persons[2].Name);
			Assert.AreEqual(17, persons[2].Age);
			Assert.AreEqual(null, persons[2].Active);
		}

		[Test]
		public void Factory()
		{
			SqlType col = Col.Structured("MyType", new dynamic[] { new { A = 5, B = true } });
			var param = col.GetParameter();

			Assert.AreEqual("MyType", param.TypeName);
			Assert.AreEqual(SqlDbType.Structured, param.SqlDbType);
			Assert.AreEqual(typeof(StructuredDynamicYielder), param.Value.GetType());
		}

		[Test]
		public void GetRawValue()
		{
			SqlType col = new SqlStructured("A", null);

			Assert.Throws<InvalidOperationException>(() => col.GetRawValue());
		}
	}
}