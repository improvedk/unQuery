using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace unQuery.Tests
{
	[TestFixture]
	public class DynamicRowTests
	{
		[Test]
		public void CantSetValuesExplicitly()
		{
			var dict = new Dictionary<string, object>();
			dynamic row = new DynamicRow(dict);

			Assert.Throws<InvalidOperationException>(() => row.Test = "abc");
		}
	}
}