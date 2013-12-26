using NUnit.Framework;
using System;
using System.Data;
using System.Data.SqlClient;
using unQuery.SqlTypes;

namespace unQuery.Tests
{
	public class ExecuteTests : TestFixture
	{
		[Test]
		public void SqlCommand()
		{
			var cmd = new SqlCommand("UPDATE Persons SET Name = Name");

			Assert.AreEqual(5, DB.Execute(cmd));
		}

		[Test]
		public void SqlCommand_WithParameters()
		{
			var cmd = new SqlCommand("UPDATE Persons SET Name = Name WHERE Age = @Age");
			cmd.Parameters.Add("@Age", SqlDbType.TinyInt).Value = (byte)55;

			Assert.AreEqual(1, DB.Execute(cmd));
		}

		[Test]
		public void SqlCommand_WithMixedParameters()
		{
			var cmd = new SqlCommand("UPDATE Persons SET Name = Name WHERE Age = @Age AND 2 = @One");
			cmd.Parameters.Add("@Age", SqlDbType.TinyInt).Value = (byte)55;

			int result = DB.Execute(cmd, new { One = 1 });

			Assert.AreEqual(0, result);
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