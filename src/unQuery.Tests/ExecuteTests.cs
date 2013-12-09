using NUnit.Framework;
using System;
using System.Data.SqlClient;
using unQuery.SqlTypes;

namespace unQuery.Tests
{
	public class ExecuteTests : TestFixture
	{
		[Test]
		public void EmptySql()
		{
			Assert.Throws<InvalidOperationException>(() => DB.Execute(""));
			Assert.Throws<InvalidOperationException>(() => DB.Execute(null));
		}

		[Test]
		public void WithRowsModified()
		{
			var result = DB.Execute("UPDATE Persons SET Name = Name");

			Assert.AreEqual(5, result);
		}

		[Test]
		public void WithParameters()
		{
			var result = DB.Execute("UPDATE Persons SET Name = Name, Age = Age WHERE Name = @Name AND Age = @Age", new {
				Age = 25,
				Name = Col.NVarChar("Daniel Gallagher")
			});

			Assert.AreEqual(1, result);
		}

		[Test]
		public void WithNoRowsModified()
		{
			var result = DB.Execute("UPDATE Persons SET Name = Name WHERE 1=0");

			Assert.AreEqual(0, result);
		}

		[Test]
		public void ThrowsSqlException()
		{
			Assert.Throws<SqlException>(() => DB.Execute("x"));
		}
	}
}