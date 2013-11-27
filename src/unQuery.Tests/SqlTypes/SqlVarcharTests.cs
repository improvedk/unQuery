using NUnit.Framework;
using System.Data;
using System.Data.SqlClient;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlVarcharTests : TestFixture
	{
		[Test]
		public void Casting()
		{
			var col = (SqlVarchar)"Test";
			var param = col.GetParameter();

			assertParameter(param, 4, "Test");
		}

		[Test]
		public void Constructor()
		{
			var col = new SqlVarchar("Test");
			var param = col.GetParameter();

			assertParameter(param, 4, "Test");
		}

		[Test]
		public void ExplicitSize()
		{
			var col = new SqlVarchar("Test", 10);
			var param = col.GetParameter();

			assertParameter(param, 10, "Test");
		}

		[Test]
		public void GetParameter()
		{
			assertParameter(SqlVarchar.GetParameter("Test"), 4, "Test");
			assertParameter(SqlVarchar.GetParameter("Test", 10), 10, "Test");
		}

		private void assertParameter(SqlParameter param, int size, string value)
		{
			Assert.That(param.SqlDbType == SqlDbType.VarChar);
			Assert.That(param.Size == size);
			Assert.That(param.Value.ToString() == value);
		}
	}
}