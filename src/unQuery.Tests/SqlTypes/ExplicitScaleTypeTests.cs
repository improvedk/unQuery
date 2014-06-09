using System;
using NUnit.Framework;
using unQuery.SqlTypes;

namespace unQuery.Tests.SqlTypes
{
	public class ExplicitScaleTypeTests : TestFixture
	{
		[Test]
		public void ParameterValue()
		{
			var param = Col.Time(TimeSpan.FromHours(1));

			TimeSpan? x;
			Assert.Throws<CannotAccessParameterValueBeforeExecutingQuery>(() => x = param.Value);

			DB.Execute("SELECT @A", new { A = param });

			Assert.AreEqual(TimeSpan.FromHours(1), param.Value);
		}
	}
}