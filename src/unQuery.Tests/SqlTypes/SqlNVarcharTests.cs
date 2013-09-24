using NUnit.Framework;
using System.Data;
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

			Assert.That(param.SqlDbType == SqlDbType.NVarChar);
			Assert.That(param.Size == 4);
			Assert.That(param.Value.ToString() == "Test");
		}
	}
}