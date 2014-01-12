using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using unQuery.SqlTypes;

namespace unQuery.Tests
{
	public class ExecuteManyTests : TestFixture
	{
		[Test]
		public void NullParameterRows()
		{
			Assert.Throws<ArgumentNullException>(() => DB.ExecuteMany("SELECT 1", null));
		}
		
		[Test]
		public void NoParameterRows()
		{
			IEnumerable<dynamic> paramRows = Enumerable.Range(0, 0).Select(x => new { A = 25 });

			Assert.AreEqual(0, DB.ExecuteMany("SELECT @A AS A INTO #TMP", paramRows));
		}

		[Test]
		public void SingleParameterRow()
		{
			IEnumerable<dynamic> paramRows = Enumerable.Range(0, 1).Select(x => new { A = 25 });

			Assert.AreEqual(1, DB.ExecuteMany("SELECT @A AS A INTO #TMP", paramRows));
		}

		[Test]
		public void MultipleInsertsInOneRow()
		{
			IEnumerable<dynamic> paramRows = Enumerable.Range(0, 1).Select(x => new { A = 25 });

			Assert.AreEqual(2, DB.ExecuteMany("SELECT @A AS A INTO #TMP; SELECT @A AS A INTO #TMP2", paramRows));
		}

		[Test]
		public void MultipleParameterRows()
		{
			IEnumerable<dynamic> paramRows = Enumerable.Range(0, 5).Select(x => new { A = 25 });

			Assert.AreEqual(5, DB.ExecuteMany("SELECT @A AS A INTO #TMP", paramRows));
		}

		[Test]
		public void DifferentParameterRowTypes()
		{
			var rows = new object[] {
				new { A = 25 },
				new { A = 17, B = true }
			};

			Assert.AreEqual(2, DB.ExecuteMany("SELECT @A AS A INTO #TMP", rows));
		}

		[Test]
		public void ParameterRowsWithNoProperties()
		{
			IEnumerable<dynamic> paramRows = Enumerable.Range(0, 1).Select(x => new { });

			Assert.Throws<SqlException>(() => DB.ExecuteMany("SELECT @A AS A INTO #TMP", paramRows));
		}

		[Test]
		public void MissingParameterProperties()
		{
			IEnumerable<dynamic> paramRows = Enumerable.Range(0, 1).Select(x => new { B = 25 });

			Assert.Throws<SqlException>(() => DB.ExecuteMany("SELECT @A AS A INTO #TMP", paramRows));
		}

		[Test]
		public void ExtraProperties()
		{
			IEnumerable<dynamic> paramRows = Enumerable.Range(0, 1).Select(x => new { A = Col.Int(5), B = 25 });

			Assert.AreEqual(1, DB.ExecuteMany("SELECT @A AS A INTO #TMP", paramRows));
		}
	}
}