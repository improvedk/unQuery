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
	 *  - Test SqlUniqueidentifier
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