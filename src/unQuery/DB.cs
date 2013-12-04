using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace unQuery
{
	/// <summary>
	/// Provides easy static access to to the database referenced by the first available connection string.
	/// </summary>
	public static class DB
	{
		private static unQuery db;

		static DB()
		{
			if (ConfigurationManager.ConnectionStrings.Count == 0)
				throw new MissingConnectionStringException();

			db = new unQuery(ConfigurationManager.ConnectionStrings[0].ConnectionString);
		}

		public static IList<dynamic> GetRows(string sql)
		{
			return db.GetRows(sql);
		}

		public static IList<dynamic> GetRows(string sql, dynamic parameters)
		{
			return db.GetRows(sql, parameters);
		}

		public static dynamic GetRow(string sql)
		{
			return db.GetRow(sql);
		}

		public static dynamic GetRow(string sql, dynamic parameters)
		{
			return db.GetRow(sql, parameters);
		}

		public static T GetScalar<T>(string sql)
		{
			return db.GetScalar<T>(sql);
		}

		public static T GetScalar<T>(string sql, dynamic parameters)
		{
			return db.GetScalar<T>(sql, parameters);
		}

		public static int Execute(string sql)
		{
			return db.Execute(sql);
		}

		public static int Execute(string sql, dynamic parameters)
		{
			return db.Execute(sql, parameters);
		}

		public static SqlConnection GetOpenConnection()
		{
			return db.GetOpenConnection();
		}
	}
}