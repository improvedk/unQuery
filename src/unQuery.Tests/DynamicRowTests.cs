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

		[Test]
		public void ColumnDoesNotExist()
		{
			var dict = new Dictionary<string, object>();
			dict["Test"] = "abc";

			dynamic row = new DynamicRow(dict);

			object dummy;
			Assert.AreEqual("abc", row.Test);
			Assert.Throws<ColumnDoesNotExistException>(() => dummy = row.DoesNotExist);
		}
	}
}