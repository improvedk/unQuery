using NUnit.Framework;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class ExplicitMaxLengthTypeTests : TestFixture
	{
		[Test]
		public void ParameterValue()
		{
			var param = Col.VarChar("Hello", 10);

			string x;
			Assert.Throws<CannotAccessParameterValueBeforeExecutingQuery>(() => x = param.Value);

			DB.Execute("SELECT @A", new { A = param });

			Assert.AreEqual("Hello", param.Value);
		}
	}
}