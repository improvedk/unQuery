using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace unQuery.PerformanceTests
{
	public class Program
	{
		static void Main()
		{
			// Ensure our output percentages are formatted in a sane way, no matter the OS settings
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

			// Ensure we've got above-normal priority
			Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;

			// When running the tests in console mode, we don't want to perform assertions
			TestFixture.AssertionsEnabled = false;

			// Execute
			var execute = new ExecuteTests();
			output("Execute - No Parameters", execute.NoParameters());
			output("Execute - 1 Parameter", execute.OneParameter());
			output("Execute - 5 Parameters", execute.FiveParameters());

			// GetScalar
			var scalar = new GetScalerTests();
			output("GetScalar - No Parameters", scalar.NoParameters());
			output("GetScalar - 1 Parameter", scalar.OneParameter());
			output("GetScalar - 5 Parameters", scalar.FiveParameters());

			// GetRow
			var getRow = new GetRowTests();
			output("GetRow - Typed - No Parameters", getRow.Typed_NoParameters());
			output("GetRow - Typed - 5 Parameters", getRow.Typed_FiveParameters());
			output("GetRow - Dynamic - No Parameters", getRow.Dynamic_NoParameters());
			output("GetRow - Dynamic - 5 Parameters", getRow.Dynamic_FiveParameters());

			// GetRows
			var getRows = new GetRowsTests();
			output("GetRows - Typed - No Results", getRows.Typed_NoResults());
			output("GetRows - Typed - 1 Result", getRows.Typed_1Result());
			output("GetRows - Typed - 10 Results", getRows.Typed_10Results());
			output("GetRows - Dynamic - No Results", getRows.Dynamic_NoResults());
			output("GetRows - Dynamic - 1 Result", getRows.Dynamic_1Result());
			output("GetRows - Dynamic - 10 Results", getRows.Dynamic_10Results());

			Console.WriteLine("Done...");
			Console.ReadLine();
		}

		private static void output(string name, TestResult result)
		{
			Console.Write(name + ": ");

			if (result.unQueryOverheadInPercent > result.MaximumUnQueryOverheadInPercent)
				Console.ForegroundColor = ConsoleColor.Red;
			else
				Console.ForegroundColor = ConsoleColor.Green;

			Console.WriteLine(result.unQueryOverheadInPercent.ToString("N") + "%  (" + result.MaximumUnQueryOverheadInPercent.ToString("N") + "%)");

			Console.ResetColor();
		}
	}
}