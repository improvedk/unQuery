using NUnit.Framework;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using unQuery.SqlTypes;

namespace unQuery.Tests
{
	public class ExecuteTests : TestFixture
	{
		[Test]
		public void StoredProcedure()
		{
			DB.Execute("CREATE TABLE XYZ (A int)");

			DB.Execute("sp_rename", new {
				objname = Col.NVarChar("XYZ"),
				newname = Col.NVarChar("ABC")
			}, new QueryOptions {
				CommandType = CommandType.StoredProcedure
			});
			
			DB.Execute("DROP TABLE ABC");
		}
		
		[Test]
		public void EmptySql()
		{
			Assert.Throws<InvalidOperationException>(() => DB.Execute(""));

			string x = null;
			Assert.Throws<InvalidOperationException>(() => DB.Execute(x));
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
				Name = Col.NVarChar("Daniel Gallagher", 128)
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