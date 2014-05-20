using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace unQuery.PerformanceTests
{
	[TestFixture]
	public abstract class TestFixture
	{
		private int testIterations = 10000;
		private string connectionString = ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString;

		[TestFixtureSetUp]
		public void Init()
		{
			// Increase process priority since this is time sensitive code
			Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
		}

		protected SqlConnection GetOpenConnection()
		{
			var conn = new SqlConnection(connectionString);
			conn.Open();

			return conn;
		}

		protected void RunTest(double maxDiff, Action handCoded, Action unQuery)
		{
			var sw = new Stopwatch();
			var handCodedRuntimes = new List<long>(testIterations);
			var unQueryRuntimes = new List<long>(testIterations);

			handCoded();
			unQuery();
		
			for (int i = 0; i < testIterations  * 2; i++)
			{
				if (i%2 == 0)
				{
					sw.Restart();
					handCoded();
					sw.Stop();
					handCodedRuntimes.Add(sw.ElapsedTicks);
				}
				else
				{
					sw.Restart();
					unQuery();
					sw.Stop();
					unQueryRuntimes.Add(sw.ElapsedTicks);
				}
			}
			
			// Extract the 95th percentile results to reduce variance
			int percentileCount = Convert.ToInt32(testIterations * 0.95);

			var avgHandCodedRuntime = handCodedRuntimes
				.OrderBy(x => x)
				.Skip(testIterations - percentileCount)
				.Take(percentileCount)
				.Average();

			var avgUnQueryRuntime = unQueryRuntimes
				.OrderBy(x => x)
				.Skip(testIterations - percentileCount)
				.Take(percentileCount)
				.Average();

			var diff = avgUnQueryRuntime - avgHandCodedRuntime;
			var diffPercentage = diff / avgHandCodedRuntime * 100;

			Console.WriteLine("Hand coded: " + avgHandCodedRuntime + " ticks");
			Console.WriteLine("unQuery: " + avgUnQueryRuntime + " ticks");
			Console.WriteLine("unQuery diff: " + diffPercentage.ToString("N") + "% (Max: " + maxDiff.ToString("N") + "%)");

			Assert.Less(diffPercentage, maxDiff, "unQuery difference should be below " + maxDiff.ToString("N") + "%");
		}
	}
}