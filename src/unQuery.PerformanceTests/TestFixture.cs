using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace unQuery.PerformanceTests
{
	[TestFixture]
	public abstract class TestFixture
	{
		internal static bool AssertionsEnabled = true;

		private TimeSpan testDuration = TimeSpan.FromMilliseconds(10000);
		private double testPercentile = 0.90d;
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

		protected TestResult RunTest(double maxDiff, Action handCoded, Action unQuery)
		{
			var sw = new Stopwatch();
			var handCodedRuntimes = new List<long>();
			var unQueryRuntimes = new List<long>();
			bool runTest = true;

			// Warmup
			handCoded();
			unQuery();
		
			// Testrunner thread
			var testThread = new Thread(() =>
			{
				int cnt = 0;
				while (runTest || cnt % 2 == 1)
				{
					if (cnt % 2 == 0)
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

					cnt++;
				}

			});
			testThread.Start();
			testThread.Join(testDuration);
			runTest = false;
			testThread.Join();

			// Sanity check that both code path were run an equal number of times
			if (AssertionsEnabled)
				Assert.AreEqual(handCodedRuntimes.Count, unQueryRuntimes.Count);

			// Extract the Nth percentile results to reduce variance
			int testIterations = handCodedRuntimes.Count;
			int percentileCount = Convert.ToInt32(testIterations * testPercentile);

			var avgHandCodedRuntime = handCodedRuntimes
				.OrderBy(x => x)
				.Skip((testIterations - percentileCount) / 2)
				.Take(percentileCount)
				.Average();

			var avgUnQueryRuntime = unQueryRuntimes
				.OrderBy(x => x)
				.Skip((testIterations - percentileCount) / 2)
				.Take(percentileCount)
				.Average();

			var diff = avgUnQueryRuntime - avgHandCodedRuntime;
			var diffPercentage = diff / avgHandCodedRuntime * 100;

			Trace.WriteLine("Iterations: " + testIterations);
			Trace.WriteLine("Hand coded: " + avgHandCodedRuntime.ToString("N") + " ticks");
			Trace.WriteLine("unQuery: " + avgUnQueryRuntime.ToString("N") + " ticks");
			Trace.WriteLine("unQuery diff: " + diffPercentage.ToString("N") + "% (Max: " + maxDiff.ToString("N") + "%)");
			
			if (AssertionsEnabled)
				Assert.Less(diffPercentage, maxDiff, "unQuery difference should be below " + maxDiff.ToString("N") + "%");

			return new TestResult {
				Iterations = testIterations,
				HandcodedAvgRuntimeInMs = avgHandCodedRuntime,
				unQueryAvgRuntimeInMs = avgUnQueryRuntime,
				unQueryOverheadInPercent = diffPercentage,
				MaximumUnQueryOverheadInPercent = maxDiff
			};
		}
	}
}