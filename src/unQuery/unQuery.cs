using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using unQuery.SqlTypes;

namespace unQuery
{
	/*
	 * Access methods
	 *  - Get scalar value
	 *  - Get single row
	 *  - Get multiple rows
	 *  - Get multiple result sets with multiple rows
	 *  - Execute without result
	 *  
	 * Query methods
	 *	- Stored procedure
	 *	- Text
	 *	
	 * TODO
	 *  - Way to override standard CLR type handlers
	 *  - Make sure DBNull.Value is used for null values
	 */

	public abstract class unQuery
	{
		protected abstract string ConnectionString { get; }

		/// <summary>
		/// Executes the batch, and returns the first column of the first row of the first result set returned by the query.
		/// Additional columns or rows are ignored.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <exception cref="NoRowsException" />
		public T GetScalar<T>(string sql)
		{
			return GetScalar<T>(sql, null);
		}

		/// <summary>
		/// Executes the batch, and returns the first column of the first row of the first result set returned by the query.
		/// Additional columns or rows are ignored.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		/// <exception cref="NoRowsException" />
		public T GetScalar<T>(string sql, dynamic parameters)
		{
			using (var conn = getConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				if (parameters != null)
					AddParametersToCommand(cmd, parameters);

				object result = cmd.ExecuteScalar();
				
				if (result == null)
					throw new NoRowsException();

				if (result is DBNull)
					return default(T);

				return (T)result;
			}
		}

		/// <summary>
		/// Executes a batch and returns the number of rows affected.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		public int Execute(string sql)
		{
			return Execute(sql, null);
		}

		/// <summary>
		/// Executes a batch and returns the number of rows affected.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		public int Execute(string sql, dynamic parameters)
		{
			using (var conn = getConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				if (parameters != null)
					AddParametersToCommand(cmd, parameters);

				return cmd.ExecuteNonQuery();
			}
		}

		private static readonly Dictionary<Type, Func<object, SqlParameter>> typeHandlers = new Dictionary<Type, Func<object, SqlParameter>> {
			{ typeof(short), SqlSmallInt.GetParameter },
			{ typeof(short?), SqlSmallInt.GetParameter },
			{ typeof(byte), SqlTinyInt.GetParameter },
			{ typeof(byte?), SqlTinyInt.GetParameter },
			{ typeof(int), SqlInt.GetParameter },
			{ typeof(int?), SqlInt.GetParameter },
			{ typeof(long), SqlBigInt.GetParameter },
			{ typeof(long?), SqlBigInt.GetParameter },
			{ typeof(bool), SqlBit.GetParameter },
			{ typeof(bool?), SqlBit.GetParameter },
			{ typeof(Guid), SqlUniqueIdentifier.GetParameter },
			{ typeof(Guid?), SqlUniqueIdentifier.GetParameter }

			// TODO: char, decimal, double, enum, float, sbyte, struct, uint, ulong, ushort, guid, datetime
			// TODO: object, string
		};

		internal void AddParametersToCommand(SqlCommand cmd, object parameters)
		{
			// For each property in the dynamic parameters object, create a SqlParameter and add it to the SqlCommand
			foreach (PropertyInfo prop in parameters.GetType().GetProperties())
			{
				SqlParameter param;
				object propValue = prop.GetValue(parameters);
				var sqlType = propValue as ISqlType;
			
				try
				{
					// If it's a SqlType value, let it create the parameter for us. Otherwise, for native CLR types we'll
					// let the type handlers take care of creating the parameter.
					if (sqlType != null)
						param = sqlType.GetParameter();
					else
						param = typeHandlers[prop.PropertyType](propValue);
				}
				catch (KeyNotFoundException)
				{
					throw new TypeNotSupportedException(prop.PropertyType);
				}

				// Set parameter name
				param.ParameterName = "@" + prop.Name;

				// If it's a null value, convert it to DBNull.value
				if (param.Value == null)
					param.Value = DBNull.Value;

				cmd.Parameters.Add(param);
			}
		}

		/// <summary>
		/// Creates and returns an open SqlConnection.
		/// </summary>
		private SqlConnection getConnection()
		{
			var conn = new SqlConnection(ConnectionString);

			try
			{
				conn.Open();
			}
			catch (SqlException)
			{
				// If we fail, clearing the pooled connection may help
				SqlConnection.ClearPool(conn);
				conn.Open();
			}

			return conn;
		}
	}
}




/*
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;

namespace iPaper.Framework.Database
{
	public class SqlDB
	{
		public static T GetScalar<T>(SqlCommand cmd, IDBSettings dbSettings)
		{
			return DBConvert.To<T>(GetScalar(cmd, dbSettings), true);
		}

		public static T GetScalarOrDefault<T>(SqlCommand cmd, IDBSettings dbSettings)
		{
			object result = GetScalar(cmd, dbSettings);

			if (result is DBNull)
				return default(T);
			else
				return (T)result;
		}

		public static DataTable GetDT(SqlCommand cmd, IDBSettings dbSettings)
		{
			var da = new SqlDataAdapter();
			var dt = new DataTable();
			
			using (var conn = getConnection(dbSettings))
			{
				da.SelectCommand = cmd;
				da.SelectCommand.Connection = conn;
				da.Fill(dt);
			}

			return dt;
		}
		public static DataTable GetDT(string sql, IDBSettings dbSettings)
		{
			return GetDT(new SqlCommand(sql), dbSettings);
		}
		public static EnumerableRowCollection<DataRow> GetRows(SqlCommand cmd, IDBSettings dbSettings)
		{
			return GetDT(cmd, dbSettings).AsEnumerable();
		}

		public static T GetRow<T>(SqlCommand cmd, IDBSettings dbSettings, Func<IDataReader, T> projection)
		{
			using (var conn = getConnection(dbSettings))
			{
				cmd.Connection = conn;
				
				using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
				{
					if (reader.Read())
						return projection(reader);
					
					return default(T);
				}
			}
		}

		public static DataRow GetDR(SqlCommand cmd, IDBSettings dbSettings)
		{
			DataTable dt = GetDT(cmd, dbSettings);

			if (dt.Rows.Count > 0)
				return dt.Rows[0];
			
			return null;
		}
		public static DataRow GetDR(string sql, IDBSettings dbSettings)
		{
			return GetDR(new SqlCommand(sql), dbSettings);
		}

		public static int Execute(SqlCommand cmd, IDBSettings dbSettings)
		{
			using (cmd.Connection = getConnection(dbSettings))
				return cmd.ExecuteNonQuery();
		}

		public static int Execute(string sql, IDBSettings dbSettings)
		{
			return Execute(new SqlCommand(sql), dbSettings);
		}

		public static IList<T> GetList<T>(SqlCommand cmd, IDBSettings dbSettings) where T: struct
		{
			return GetDT(cmd, dbSettings)
				.AsEnumerable()
				.Select(x => x.Field<T>(0))
				.ToList();
		}

		public static Table<TEntity> GetTable<TEntity>(DataContextWrapper dataContext) where TEntity: class
		{
			return dataContext.DataContext.GetTable<TEntity>();
		}

		public static Table<TEntity> GetTable<TEntity>(IDBSettings dbSettings) where TEntity : class
		{
			return dbSettings.CreateDataContext().DataContext.GetTable<TEntity>();
		}

		public static IList<TEntity> GetEntitiesByQuery<TEntity>(SqlCommand cmd, IDBSettings dbSettings) where TEntity: class
		{
			using (SqlConnection conn = getConnection(dbSettings))
			{
				cmd.Connection = conn;

				using (var dataContext = dbSettings.CreateDataContext())
					return dataContext.DataContext.Translate<TEntity>(cmd.ExecuteReader()).ToList();
			}
		}
	}
}
*/


/*

	internal static class DBConvert
	{
		public static T To<T>(object value)
		{
			// For nullable value types, return the default value
			if (value is DBNull && typeof(T).IsValueType && typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
				return default(T);

			return changeType<T>(value);
		}

		private static T changeType<T>(object value)
		{
			Type conversionType = typeof(T);

			if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
			{
				if (value is DBNull || value == null)
					return default(T);

				conversionType = Nullable.GetUnderlyingType(conversionType);

				if (value is string && conversionType != typeof(string) && string.IsNullOrEmpty((string)value))
					return default(T);
			}
			else if (conversionType.IsEnum)
			{
				try
				{
					return (T)Convert.ChangeType(value, Enum.GetUnderlyingType(conversionType));
				}
				catch
				{
					return (T)Enum.Parse(conversionType, value.ToString());
				}
			}

			if (value is string && conversionType == typeof(Guid))
				return (T)(object)new Guid((string)value);

			return (T)Convert.ChangeType(value, conversionType);
		}
	}
*/