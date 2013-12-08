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
		private static unQueryDB db;

		static DB()
		{
			if (ConfigurationManager.ConnectionStrings.Count == 0)
				throw new MissingConnectionStringException();

			db = new unQueryDB(ConfigurationManager.ConnectionStrings[0].ConnectionString);
		}

		public static IList<dynamic> GetRows(string sql, dynamic parameters = null)
		{
			return db.GetRows(sql, parameters);
		}

		public static dynamic GetRow(string sql, dynamic parameters = null)
		{
			return db.GetRow(sql, parameters);
		}

		public static T GetScalar<T>(SqlCommand cmd, dynamic parameters = null)
		{
			return db.GetScalar<T>(cmd, parameters);
		}

		public static T GetScalar<T>(string sql, dynamic parameters = null)
		{
			return db.GetScalar<T>(sql, parameters);
		}

		public static int Execute(string sql, dynamic parameters = null)
		{
			return db.Execute(sql, parameters);
		}

		public static SqlConnection GetOpenConnection()
		{
			return db.GetOpenConnection();
		}
	}
}