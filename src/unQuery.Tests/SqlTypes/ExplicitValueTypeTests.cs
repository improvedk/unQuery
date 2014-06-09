using NUnit.Framework;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class ExplicitValueTypeTests : TestFixture
	{
		[Test]
		public void ParameterValue()
		{
			var param = Col.Float(5);

			double? x;
			Assert.Throws<CannotAccessParameterValueBeforeExecutingQuery>(() => x = param.Value);

			DB.Execute("SELECT @A", new { A = param });

			Assert.AreEqual(5, param.Value);
		}
	}
}