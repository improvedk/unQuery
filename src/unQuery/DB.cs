using System.Collections.Generic;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace unQuery
{
	/// <summary>
	/// Provides easy static access to to the database referenced by the first available connection string.
	/// </summary>
	public static class DB
	{
		private static readonly unQueryDB db;

		static DB()
		{
			if (ConfigurationManager.ConnectionStrings.Count == 0)
				throw new MissingConnectionStringException();

			db = new unQueryDB(ConfigurationManager.ConnectionStrings[0].ConnectionString);
		}

		/// <summary>
		/// Returns a BatchExecutioner that can be used to efficiently batch up several execution commands in one go.
		/// </summary>
		public static BatchExecutor Batch()
		{
			return db.Batch();
		}

		/// <summary>
		/// Executes the batch and returns all rows from the single result set.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		/// <param name="options">An optional set of query options</param>
		public static IList<dynamic> GetRows(string sql, object parameters = null, QueryOptions options = null)
		{
			return db.GetRows(sql, parameters, options);
		}

		/// <summary>
		/// Executes the batch and returns all rows from the single result set.
		/// The rows will be mapped to the properties of the generic type.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		/// <param name="options">An optional set of query options</param>
		public static IList<T> GetRows<T>(string sql, object parameters = null, QueryOptions options = null)
		{
			return db.GetRows<T>(sql, parameters, options);
		}

		/// <summary>
		/// Executes the batch and returns a single row of data. If more than one row is is returned from the database,
		/// all but the first will be discarded.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		/// <param name="options">An optional set of query options</param>
		public static dynamic GetRow(string sql, object parameters = null, QueryOptions options = null)
		{
			return db.GetRow(sql, parameters, options);
		}

		/// <summary>
		/// Executes the batch and returns a single row of data. If more than one row is is returned from the database,
		/// all but the first will be discarded. The row will be mapped to the properties of the generic type.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		/// <param name="options">An optional set of query options</param>
		public static T GetRow<T>(string sql, object parameters = null, QueryOptions options = null) where T : new()
		{
			return db.GetRow<T>(sql, parameters, options);
		}

		/// <summary>
		/// Executes the batch, and returns the first column of the first row of the first result set returned by the query.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		/// <param name="options">An optional set of query options</param>
		/// <exception cref="NoRowsException" />
		public static T GetScalar<T>(string sql, object parameters = null, QueryOptions options = null)
		{
			return db.GetScalar<T>(sql, parameters, options);
		}

		/// <summary>
		/// Executes a batch and returns the number of rows affected.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		/// <param name="options">An optional set of query options</param>
		public static int Execute(string sql, object parameters = null, QueryOptions options = null)
		{
			return db.Execute(sql, parameters, options);
		}

		/// <summary>
		/// Returns a raw and open SqlConnection for manual use.
		/// </summary>
		public static SqlConnection GetOpenConnection()
		{
			return db.GetOpenConnection();
		}
	}
}