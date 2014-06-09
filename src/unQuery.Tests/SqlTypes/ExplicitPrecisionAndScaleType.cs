using NUnit.Framework;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class ExplicitPrecisionAndScaleTypeTests : TestFixture
	{
		[Test]
		public void ParameterValue()
		{
			var param = Col.Decimal(5.25m);

			decimal? x;
			Assert.Throws<CannotAccessParameterValueBeforeExecutingQuery>(() => x = param.Value);

			DB.Execute("SELECT @A", new { A = param });

			Assert.AreEqual(5.25m, param.Value);
		}
	}
}