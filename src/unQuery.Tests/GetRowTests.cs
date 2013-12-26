using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using unQuery.SqlTypes;

namespace unQuery.Tests
{
	public class GetRowTests : TestFixture
	{
		[Test]
		public void SqlCommand()
		{
			var cmd = new SqlCommand("SELECT TOP 1 * FROM Persons WHERE PersonID = 1");

			Assert.AreEqual("Stefanie Alexander", DB.GetRow(cmd).Name);
		}

		[Test]
		public void SqlCommand_WithParameters()
		{
			var cmd = new SqlCommand("SELECT TOP 1 * FROM Persons WHERE PersonID = @PersonID");
			cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = 1;

			Assert.AreEqual("Stefanie Alexander", DB.GetRow(cmd).Name);
		}

		[Test]
		public void SqlCommand_WithMixedParameters()
		{
			var cmd = new SqlCommand("SELECT TOP 1 * FROM Persons WHERE PersonID = @PersonID AND 2 = @One");
			cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = 1;

			Assert.IsNull(DB.GetRow(cmd, new { One = 1 }));
		}

		[Test]
		public void GetRow_NoResults()
		{
			var result = DB.GetRow("SELECT * FROM Persons WHERE 1 = 0");

			Assert.IsNull(result);
		}

		[Test]
		public void GetRow_CaseSensitive()
		{
			var result = DB.GetRow("SELECT Age FROM Persons WHERE Name = @Name", new { Name = Col.NVarChar("Stefanie Alexander", 128) });

			Assert.AreEqual(55, result.Age);
			
			object dummy;
			Assert.Throws<ColumnDoesNotExistException>(() => dummy = result.age);
			Assert.Throws<ColumnDoesNotExistException>(() => dummy = result.AGE);
		}
	
		[Test]
		public void GetRow_SingleColumn()
		{
			var result = DB.GetRow("SELECT Age FROM Persons WHERE Name = @Name", new { Name = Col.NVarChar("Stefanie Alexander", 128) });

			Assert.AreEqual(55, result.Age);
			Assert.AreEqual(1, ((Dictionary<string, object>)result).Count);
		}

		[Test]
		public void GetRow_MultipleColumns()
		{
			var result = DB.GetRow("SELECT Age, Sex FROM Persons WHERE Name = @Name", new { Name = Col.NVarChar("Daniel Gallagher", 128) });

			Assert.AreEqual(25, result.Age);
			Assert.AreEqual("M", result.Sex);
			Assert.AreEqual(2, ((Dictionary<string, object>)result).Count);
		}

		[Test]
		public void GetRow_AllColumns()
		{
			var result = DB.GetRow("SELECT * FROM Persons WHERE Name = @Name", new { Name = Col.NVarChar("Annie Brennan", 128) });

			Assert.AreEqual(5, result.PersonID);
			Assert.AreEqual("Annie Brennan", result.Name);
			Assert.AreEqual(23, result.Age);
			Assert.AreEqual("M", result.Sex);
			Assert.AreEqual(Convert.ToDateTime("1984-01-07 13:24:42.110"), result.SignedUp);
			Assert.AreEqual(5, ((Dictionary<string, object>)result).Count);
		}

		[Test]
		public void GetRow_MultipleResultsPossible()
		{
			var result = DB.GetRow("SELECT * FROM Persons ORDER BY PersonID");

			Assert.AreEqual(1, result.PersonID);
		}
	}
}