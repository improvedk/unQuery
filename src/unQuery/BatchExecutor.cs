using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace unQuery
{
	/// <summary>
	/// This class enables you to add a series of commands to execute efficiently at the same time.
	/// </summary>
	public class BatchExecutor : IDisposable
	{
		private readonly unQueryDB db;
		private readonly List<SqlCommand> commands;

		internal BatchExecutor(unQueryDB db)
		{
			this.db = db;
			this.commands = new List<SqlCommand>();
		}

		/// <summary>
		/// Adds a statement to execute when Execute is called.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		public void Add(string sql, object parameters = null)
		{
			var cmd = new SqlCommand(sql);

			if (parameters != null)
				db.AddParametersToCommand(cmd.Parameters, parameters);

			commands.Add(cmd);
		}

		/// <summary>
		/// Executes all of the statements that have been added. This is non-transactional. Execution will continue even if
		/// some statements fail. If you need atomicity you should ensure there is an ambient transaction.
		/// </summary>
		/// <returns>The total number of rows modified by all statements.</returns>
		public int Execute()
		{
			if (commands.Count == 0)
				return 0;

			using (var conn = db.GetOpenConnection())
			using (var set = new PublicSqlCommandSet(conn))
			{
				commands.ForEach(set.Append);

				return set.ExecuteNonQuery();
			}
		}

		public void Dispose()
		{
			lock (commands)
				commands.ForEach(x => x.Dispose());
		}
	}
}