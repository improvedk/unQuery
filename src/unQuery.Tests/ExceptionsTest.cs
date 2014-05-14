using NUnit.Framework;
using System.Collections.Generic;

namespace unQuery.Tests
{
	[TestFixture]
	public class ExceptionsTest
	{
		[Test]
		public void TypeRecommendation_UnknownType()
		{
			var ex = new ParameterTypeNotSupportedException("Xyz", typeof(List<int>));
			Assert.True(ex.Message.Contains("Xyz"));
			Assert.False(ex.Message.Contains("Consider"));
		}

		[Test]
		public void TypeRecommendation_KnownType()
		{
			var ex = new ParameterTypeNotSupportedException("ABC", typeof(string));
			Assert.True(ex.Message.Contains("ABC"));
			Assert.True(ex.Message.Contains("Consider"));
		}
	}
}