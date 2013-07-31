using System.Data.SqlClient;
using NUnit.Framework;

namespace unQuery.Tests
{
	public class ExecuteTests : TestFixture
	{
		[Test]
		public void Execute_WithRowsModified()
		{
			var result = DB.Execute("UPDATE Persons SET Name = Name");

			Assert.AreEqual(5, result);
		}

		[Test]
		public void Execute_WithNoRowsModified()
		{
			var result = DB.Execute("UPDATE Persons SET Name = Name WHERE 1=0");

			Assert.AreEqual(0, result);
		}

		[Test]
		public void Execute_ThrowsSqlException()
		{
			Assert.Throws<SqlException>(() => DB.Execute("x"));
		}
	}
}