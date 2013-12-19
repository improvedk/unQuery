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
			var map = new Dictionary<string, int> {
				{ "A", 0 },
				{ "B", 1 }
			};
			var obj = new object[] { "Xyz", 25 };
			dynamic row = new DynamicRow(obj, map);

			Assert.Throws<InvalidOperationException>(() => row.B = "abc");
		}

		[Test]
		public void CanGetValues()
		{
			var map = new Dictionary<string, int> {
				{ "A", 0 },
				{ "B", 1 }
			};
			var obj = new object[] { "Xyz", 25 };
			dynamic row = new DynamicRow(obj, map);

			Assert.AreEqual("Xyz", row.A);
			Assert.AreEqual(25, row.B);
		}

		[Test]
		public void ColumnDoesNotExist()
		{
			var map = new Dictionary<string, int> {
				{ "A", 0 }
			};
			var obj = new object[] { "Xyz" };
			dynamic row = new DynamicRow(obj, map);

			Assert.AreEqual("Xyz", row.A);

			object dummy;
			Assert.AreEqual("Xyz", row.A);
			Assert.Throws<ColumnDoesNotExistException>(() => dummy = row.B);
		}
	}
}