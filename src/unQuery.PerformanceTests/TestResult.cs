
namespace unQuery.PerformanceTests
{
	public class TestResult
	{
		public int Iterations { get; set; }
		public double HandcodedAvgRuntimeInMs { get; set; }
		public double unQueryAvgRuntimeInMs { get; set; }
		public double unQueryOverheadInPercent { get; set; }
		public double MaximumUnQueryOverheadInPercent { get; set; }
	}
}