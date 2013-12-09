using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests
{
	public class GetRowsTests : TestFixture
	{
		[Test]
		public void SqlCommand()
		{
			var cmd = new SqlCommand("SELECT TOP 2 * FROM Persons WHERE PersonID IN (1, 2) ORDER BY PersonID");
			
			var result = DB.GetRows(cmd);

			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("Stefanie Alexander", result.First().Name);
			Assert.AreEqual("Lee Buckley", result.Skip(1).Take(1).First().Name);
		}

		[Test]
		public void SqlCommand_WithParameters()
		{
			var cmd = new SqlCommand("SELECT TOP 2 * FROM Persons WHERE PersonID IN (@One, @Two) ORDER BY PersonID");
			cmd.Parameters.Add("@One", SqlDbType.Int).Value = 1;
			cmd.Parameters.Add("@Two", SqlDbType.Int).Value = 2;

			var result = DB.GetRows(cmd);

			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("Stefanie Alexander", result.First().Name);
			Assert.AreEqual("Lee Buckley", result.Skip(1).Take(1).First().Name);
		}

		[Test]
		public void SqlCommand_WithMixedParameters()
		{
			var cmd = new SqlCommand("SELECT TOP 2 * FROM Persons WHERE PersonID IN (@One, @Two) ORDER BY PersonID");
			cmd.Parameters.Add("@One", SqlDbType.Int).Value = 1;

			var result = DB.GetRows(cmd, new { Two = 2 });

			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("Stefanie Alexander", result.First().Name);
			Assert.AreEqual("Lee Buckley", result.Skip(1).Take(1).First().Name);
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
			var result = DB.GetRows("SELECT Age FROM Persons WHERE Name = @Name", new { Name = Col.NVarChar("Stefanie Alexander") }).First();

			Assert.AreEqual(55, result.Age);

			object dummy;
			Assert.Throws<ColumnDoesNotExistException>(() => dummy = result.age);
			Assert.Throws<ColumnDoesNotExistException>(() => dummy = result.AGE);
		}

		[Test]
		public void SingleRow()
		{
			var result = DB.GetRows("SELECT Age, Name FROM Persons WHERE Name = @Name", new { Name = Col.NVarChar("Stefanie Alexander") });

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