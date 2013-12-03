using NUnit.Framework;
using System.Linq;

namespace unQuery.Tests
{
	public class MapReaderRowsToObjectTests : TestFixture
	{
		[Test]
		public void MapReaderRowsToObjectTests_Simple()
		{
			var rows = DB.GetRows(@"
				SELECT 25 AS TestInt, N'abc' AS TestNVarChar
				UNION ALL
				SELECT 57 AS TestInt, N'xyz' AS TestNVarChar");

			Assert.AreEqual(2, rows.Count);

			var firstRow = rows.First();
			Assert.AreEqual(25, firstRow.TestInt);
			Assert.AreEqual("abc", firstRow.TestNVarChar);

			var secondRow = rows.Skip(1).Single();
			Assert.AreEqual(57, secondRow.TestInt);
			Assert.AreEqual("xyz", secondRow.TestNVarChar);
		}
	}
}