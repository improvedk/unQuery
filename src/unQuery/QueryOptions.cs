using System.Data;

namespace unQuery
{
	public class QueryOptions
	{
		/// <summary>
		/// The type of command to execute.
		/// </summary>
		public CommandType? CommandType { get; set; }

		/// <summary>
		/// The execution timeout of the query, in seconds.
		/// </summary>
		public int? CommandTimeout { get; set; }
	}
}