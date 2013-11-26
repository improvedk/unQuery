using NUnit.Framework;
using System.Data;
using System.Data.SqlClient;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class SqlNVarcharTests : TestFixture
	{
		[Test]
		public void Casting()
		{
			var col = (SqlNVarchar)"Test";
			var param = col.GetParameter();

			assertParameter(param, 4, "Test");
		}

		[Test]
		public void Constructor()
		{
			var col = new SqlNVarchar("Test");
			var param = col.GetParameter();

			assertParameter(param, 4, "Test");
		}

		[Test]
		public void ExplicitSize()
		{
			var col = new SqlNVarchar("Test", 10);
			var param = col.GetParameter();

			assertParameter(param, 10, "Test");
		}

		private void assertParameter(SqlParameter param, int size, string value)
		{
			Assert.That(param.SqlDbType == SqlDbType.NVarChar);
			Assert.That(param.Size == size);
			Assert.That(param.Value.ToString() == value);
		}
	}
}