using NUnit.Framework;
using System;
using System.Collections.Generic;
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
		public void CaseSensitive()
		{
			var result = DB.GetRows("SELECT Age FROM Persons WHERE Name = @Name", new { Name = Col.NVarChar("Stefanie Alexander", 128) }).First();

			Assert.AreEqual(55, result.Age);

			object dummy;
			Assert.Throws<ColumnDoesNotExistException>(() => dummy = result.age);
			Assert.Throws<ColumnDoesNotExistException>(() => dummy = result.AGE);
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
	}
}