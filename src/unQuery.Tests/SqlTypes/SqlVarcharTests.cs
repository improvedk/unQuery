using NUnit.Framework;
using System.Data;
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

			Assert.That(param.SqlDbType == SqlDbType.VarChar);
			Assert.That(param.Size == 4);
			Assert.That(param.Value.ToString() == "Test");
		}
	}
}