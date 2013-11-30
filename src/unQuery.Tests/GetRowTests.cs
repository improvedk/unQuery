using Microsoft.CSharp.RuntimeBinder;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using unQuery.SqlTypes;

namespace unQuery.Tests
{
	public class GetRowTests : TestFixture
	{
		[Test]
		public void GetRow_NoResults()
		{
			var result = DB.GetRow("SELECT * FROM Persons WHERE 1 = 0");

			Assert.IsNull(result);
		}

		[Test]
		public void GetRow_CaseSensitive()
		{
			var result = DB.GetRow("SELECT Age FROM Persons WHERE Name = @Name", new { Name = Col.NVarChar("Stefanie Alexander") });

			Assert.AreEqual(55, result.Age);
			
			object dummy;
			Assert.Throws<RuntimeBinderException>(() => dummy = result.age);
			Assert.Throws<RuntimeBinderException>(() => dummy = result.AGE);
		}
	
		[Test]
		public void GetRow_SingleColumn()
		{
			var result = DB.GetRow("SELECT Age FROM Persons WHERE Name = @Name", new { Name = Col.NVarChar("Stefanie Alexander") });

			Assert.AreEqual(55, result.Age);
			Assert.AreEqual(1, ((Dictionary<string, object>)result).Count);
		}

		[Test]
		public void GetRow_MultipleColumns()
		{
			var result = DB.GetRow("SELECT Age, Sex FROM Persons WHERE Name = @Name", new { Name = Col.NVarChar("Daniel Gallagher") });

			Assert.AreEqual(25, result.Age);
			Assert.AreEqual("M", result.Sex);
			Assert.AreEqual(2, ((Dictionary<string, object>)result).Count);
		}

		[Test]
		public void GetRow_AllColumns()
		{
			var result = DB.GetRow("SELECT * FROM Persons WHERE Name = @Name", new { Name = Col.NVarChar("Annie Brennan") });

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