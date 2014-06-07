using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests
{
	public class GetRowsTests : TestFixture
	{
		private class Typed_Person
		{
			public int PersonID { get; set; }
			public string Name { get; set; }
			public byte Age { get; set; }
			public string Sex { get; set; }
			public DateTime? SignedUp { get; set; }
		}

		[Test]
		public void StoredProcedure()
		{
			DB.Execute("CREATE PROCEDURE usp_Test @A int AS SELECT @A AS A UNION ALL SELECT 37 AS A");

			var rows = DB.GetRows("usp_Test", new {
				A = 25
			}, new QueryOptions {
				CommandType = CommandType.StoredProcedure
			});

			Assert.AreEqual(2, rows.Count);
			Assert.AreEqual(25, rows[0].A);
			Assert.AreEqual(37, rows[1].A);
		}

		[Test]
		public void Typed_Persons()
		{
			var persons = DB.GetRows<Typed_Person>("SELECT * FROM Persons WHERE PersonID IN (2, 5) ORDER BY PersonID ASC");

			var lee = persons[0];
			Assert.AreEqual(2, lee.PersonID);
			Assert.AreEqual("Lee Buckley", lee.Name);
			Assert.AreEqual(37, lee.Age);
			Assert.AreEqual("M", lee.Sex);
			Assert.AreEqual(null, lee.SignedUp);
			
			var annie = persons[1];
			Assert.AreEqual(5, annie.PersonID);
			Assert.AreEqual("Annie Brennan", annie.Name);
			Assert.AreEqual(23, annie.Age);
			Assert.AreEqual("M", annie.Sex);
			Assert.AreEqual(new DateTime(1984, 01, 07, 13, 24, 42, 110), annie.SignedUp);
		}

		[Test]
		public void NoResults()
		{
			var result = DB.GetRows("SELECT * FROM Persons WHERE 1 = 0");

			Assert.AreEqual(0, result.Count());
		}

		[Test]
		public void CaseInsensitive()
		{
			var result = DB.GetRows("SELECT Age FROM Persons WHERE Name = @Name", new { Name = Col.NVarChar("Stefanie Alexander", 128) }).First();

			Assert.AreEqual(55, result.Age);
			Assert.AreEqual(55, result.age);
			Assert.AreEqual(55, result.AGE);
		}

		[Test]
		public void SingleRow()
		{
			var result = DB.GetRows("SELECT Age, Name FROM Persons WHERE Name = @Name", new { Name = Col.NVarChar("Stefanie Alexander", 128) });

			Assert.AreEqual(1, result.Count());

			var row = result.First();
			Assert.AreEqual(55, row.Age);
			Assert.AreEqual("Stefanie Alexander", row.Name);
			Assert.AreEqual(2, ((Dictionary<string, object>)row).Count);
		}

		[Test]
		public void MultipleRows()
		{
			var result = DB.GetRows("SELECT * FROM Persons WHERE PersonID IN (2, 3)");

			Assert.AreEqual(2, result.Count());

			var row = result.First();
			Assert.AreEqual(2, row.PersonID);
			Assert.AreEqual("Lee Buckley", row.Name);
			Assert.AreEqual(37, row.Age);
			Assert.AreEqual("M", row.Sex);
			Assert.AreEqual(null, row.SignedUp);
			Assert.AreEqual(5, ((Dictionary<string, object>)row).Count);

			row = result.Skip(1).First();
			Assert.AreEqual(3, row.PersonID);
			Assert.AreEqual("Daniel Gallagher", row.Name);
			Assert.AreEqual(25, row.Age);
			Assert.AreEqual("M", row.Sex);
			Assert.AreEqual(Convert.ToDateTime("1997-11-15 21:03:54.000"), row.SignedUp);
			Assert.AreEqual(5, ((Dictionary<string, object>)row).Count);
		}

		[Test]
		public void SimpleList_Int()
		{
			var rows = DB.GetRows<int>(@"
				SELECT 2 AS A UNION ALL
				SELECT 7 AS A UNION ALL
				SELECT 5 AS A");

			Assert.AreEqual(3, rows.Count);
			Assert.AreEqual(2, rows[0]);
			Assert.AreEqual(7, rows[1]);
			Assert.AreEqual(5, rows[2]);
		}

		[Test]
		public void SimpleList_MultipleColumns()
		{
			Assert.Throws<MoreThanOneColumnException>(() => DB.GetRows<int>("SELECT 2 AS A, 3 AS B"));
		}

		[Test]
		public void SimpleList_NoRows()
		{
			var rows = DB.GetRows<int>("DECLARE @A int");

			Assert.AreEqual(0, rows.Count);
		}

		[Test]
		public void SimpleList_UnnamedInt()
		{
			var rows = DB.GetRows<int>("SELECT 5");

			Assert.AreEqual(1, rows.Count);
			Assert.AreEqual(5, rows[0]);
		}

		[Test]
		public void SimpleList_Empty()
		{
			var rows = DB.GetRows<int>("SELECT NULL WHERE 0=1");

			Assert.AreEqual(0, rows.Count);
		}

		[Test]
		public void SimpleList_NullableGuid()
		{
			var rows = DB.GetRows<Guid?>(@"
				SELECT CAST('9162D161-50C9-4DF2-A0DF-464A14CE9012' AS uniqueidentifier) AS A UNION ALL
				SELECT CAST('1180322C-7794-400D-9141-FCD1F2D16A14' AS uniqueidentifier) AS A UNION ALL
				SELECT CAST(NULL AS uniqueidentifier) AS A");

			Assert.AreEqual(3, rows.Count);
			Assert.AreEqual(new Guid("9162D161-50C9-4DF2-A0DF-464A14CE9012"), rows[0]);
			Assert.AreEqual(new Guid("1180322C-7794-400D-9141-FCD1F2D16A14"), rows[1]);
			Assert.AreEqual(null, rows[2]);
		}
		
		[Test]
		public void SimpleList_String()
		{
			var rows = DB.GetRows<string>(@"
				SELECT 'asd' AS A UNION ALL
				SELECT 'xyz' AS A UNION ALL
				SELECT 'Foo Bar' AS A");

			Assert.AreEqual(3, rows.Count);
			Assert.AreEqual("asd", rows[0]);
			Assert.AreEqual("xyz", rows[1]);
			Assert.AreEqual("Foo Bar", rows[2]);
		}
		
		[Test]
		public void SimpleList_NullableByte()
		{
			var rows = DB.GetRows<byte?>(@"
				SELECT CAST(NULL AS tinyint) AS A UNION ALL
				SELECT CAST(7 AS tinyint) AS A UNION ALL
				SELECT CAST(5 AS tinyint) AS A");

			Assert.AreEqual(3, rows.Count);
			Assert.AreEqual(null, rows[0]);
			Assert.AreEqual(7, rows[1]);
			Assert.AreEqual(5, rows[2]);
		}
	}
}