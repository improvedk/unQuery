using NUnit.Framework;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;

namespace unQuery.PerformanceTests
{
	[TestFixture]
	public abstract class TestFixture
	{
		private int testIterations = 5 * 5000;

		private string connectionString = ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString;

		protected SqlConnection GetOpenConnection()
		{
			var conn = new SqlConnection(connectionString);
			conn.Open();

			return conn;
		}

		protected void RunTest(double maxDiff, Action handCoded, Action unQuery)
		{
			// Ensure test runner has top priority to reduce fluctuation
			Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;

			var swHandCoded = new Stopwatch();
			var swUnQuery = new Stopwatch();

			// Reset & warmup
			for (int i = 0; i < testIterations / 5000; i++)
			{
				// Run handcoded
				for (int k = 0; k < 100; k++)
					handCoded();
				swHandCoded.Start();
				for (int j = 0; j < 5000; j++)
					handCoded();
				swHandCoded.Stop();

				// Run unQuery
				for (int k = 0; k < 100; k++)
					unQuery();
				swUnQuery.Start();
				for (int j = 0; j < 5000; j++)
					unQuery();
				swUnQuery.Stop();
			}
			
			Console.WriteLine("Hand coded: " + swHandCoded.ElapsedMilliseconds + "ms");
			Console.WriteLine("unQuery: " + swUnQuery.ElapsedMilliseconds + "ms");

			double diff = swUnQuery.ElapsedMilliseconds - swHandCoded.ElapsedMilliseconds;
			var diffPercentage = diff / swHandCoded.ElapsedMilliseconds * 100;

			Console.WriteLine("unQuery diff: " + diffPercentage.ToString("N") + "% (Max: " + maxDiff.ToString("N") + "%)");

			Assert.Less(diffPercentage, maxDiff, "unQuery difference should be below " + maxDiff.ToString("N") + "%");
		}
	}
}