using NUnit.Framework;
using System.Data.SqlClient;

namespace unQuery.Tests
{
	public partial class unQueryTests
	{
		[Test]
		public void Execute_WithRowsModified()
		{
			var result = db.Execute("UPDATE Persons SET Name = Name");

			Assert.AreEqual(5, result);
		}

		[Test]
		public void Execute_WithNoRowsModified()
		{
			var result = db.Execute("UPDATE Persons SET Name = Name WHERE 1=0");

			Assert.AreEqual(0, result);
		}

		[Test]
		public void Execute_ThrowsSqlException()
		{
			Assert.Throws<SqlException>(() => db.Execute("x"));
		}
	}
}