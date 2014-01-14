﻿using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

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
		public static IList<dynamic> GetRows(string sql, object parameters = null)
		{
			return db.GetRows(sql, parameters);
		}

		/// <summary>
		/// Executes the batch and returns a single row of data. If more than one row is is returned from the database,
		/// all but the first will be discarded.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		public static dynamic GetRow(string sql, object parameters = null)
		{
			return db.GetRow(sql, parameters);
		}

		/// <summary>
		/// Executes the batch, and returns the first column of the first row of the first result set returned by the query.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		/// <exception cref="NoRowsException" />
		public static T GetScalar<T>(string sql, object parameters = null)
		{
			return db.GetScalar<T>(sql, parameters);
		}

		/// <summary>
		/// Executes a batch and returns the number of rows affected.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		public static int Execute(string sql, object parameters = null)
		{
			return db.Execute(sql, parameters);
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