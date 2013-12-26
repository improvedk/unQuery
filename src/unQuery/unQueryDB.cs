using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using unQuery.SqlTypes;

namespace unQuery
{
	public class unQueryDB
	{
		private readonly string connectionString;

		/// <summary>
		/// Instantiates an unQuery instance that connects using the provided connection string.
		/// </summary>
		/// <param name="connectionString">The database connection string.</param>
		public unQueryDB(string connectionString)
		{
			this.connectionString = connectionString;
		}

		/// <summary>
		/// Executes the command and returns all rows from the single result set.
		/// </summary>
		/// <param name="cmd">The SqlCommand to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		public IList<dynamic> GetRows(SqlCommand cmd, dynamic parameters = null)
		{
			using (var conn = getConnection())
			{
				cmd.Connection = conn;

				if (parameters != null)
					AddParametersToCommand(cmd, parameters);

				var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);

				return MapReaderRowsToObject(reader).ToList();
			}
		}

		/// <summary>
		/// Executes the batch and returns all rows from the single result set.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		public IList<dynamic> GetRows(string sql, dynamic parameters = null)
		{
			using (var cmd = new SqlCommand(sql))
				return GetRows(cmd, parameters);
		}

		/// <summary>
		/// Executes the command and returns a single row of data. If more than one row is is returned from the database,
		/// all but the first will be discarded.
		/// </summary>
		/// <param name="cmd">The SqlCommandstatement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		public dynamic GetRow(SqlCommand cmd, dynamic parameters = null)
		{
			using (var conn = getConnection())
			{
				cmd.Connection = conn;

				if (parameters != null)
					AddParametersToCommand(cmd, parameters);

				var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow);

				return MapReaderRowsToObject(reader).SingleOrDefault();
			}
		}

		/// <summary>
		/// Executes the batch and returns a single row of data. If more than one row is is returned from the database,
		/// all but the first will be discarded.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		public dynamic GetRow(string sql, dynamic parameters = null)
		{
			using (var cmd = new SqlCommand(sql))
				return GetRow(cmd, parameters);
		}

		/// <summary>
		/// Executes the command and returns the first column of the first row of the first result set returned by the query.
		/// </summary>
		/// <param name="cmd">The SqlCommand to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		/// <exception cref="NoRowsException" />
		public T GetScalar<T>(SqlCommand cmd, dynamic parameters = null)
		{
			using (var conn = getConnection())
			{
				cmd.Connection = conn;

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
		/// Executes the batch, and returns the first column of the first row of the first result set returned by the query.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		/// <exception cref="NoRowsException" />
		public T GetScalar<T>(string sql, dynamic parameters = null)
		{
			using (var cmd = new SqlCommand(sql))
				return GetScalar<T>(cmd, parameters);
		}

		/// <summary>
		/// Executes the command and returns the number of rows affected.
		/// </summary>
		/// <param name="cmd">The SqlCommand to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		public int Execute(SqlCommand cmd, dynamic parameters = null)
		{
			using (var conn = getConnection())
			{
				cmd.Connection = conn;

				if (parameters != null)
					AddParametersToCommand(cmd, parameters);

				return cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Executes a batch and returns the number of rows affected.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		public int Execute(string sql, dynamic parameters = null)
		{
			using (var cmd = new SqlCommand(sql))
				return Execute(cmd, parameters);
		}

		/// <summary>
		/// Maps any number of rows to dynamic objects. This method is optimized for returning many rows.
		/// </summary>
		/// <param name="reader">The SqlDataReader from which the schema & values should be read.</param>
		internal static IEnumerable<dynamic> MapReaderRowsToObject(SqlDataReader reader)
		{
			int visibleFieldCount = reader.VisibleFieldCount;
			var fieldMap = new Dictionary<string, int>(visibleFieldCount);

			// First loop through each column and create map between the field name and storage array index
			for (int i = 0; i < visibleFieldCount; i++)
			{
				string fieldName = reader.GetName(i);

				if (string.IsNullOrWhiteSpace(fieldName))
					throw new UnnamedColumnException(i);

				try
				{
					fieldMap.Add(reader.GetName(i), i);
				}
				catch (ArgumentException)
				{
					throw new DuplicateColumnException(fieldName);
				}
			}

			while (reader.Read())
			{
				var obj = new object[visibleFieldCount];
				reader.GetValues(obj);

				for (int i = 0; i < visibleFieldCount; i++)
					if (obj[i] is DBNull)
						obj[i] = null;
				
				yield return new DynamicRow(obj, fieldMap);
			}
		}

		/// <summary>
		/// Map between core CLR & ISqlTypes and their corresponding ITypeHandlers
		/// </summary>
		internal static Dictionary<Type, ITypeHandler> ClrTypeHandlers = new Dictionary<Type, ITypeHandler> {
			// Core CLR types
			{ typeof(byte), SqlTinyInt.GetTypeHandler() },
			{ typeof(byte?), SqlTinyInt.GetTypeHandler() },
			{ typeof(short), SqlSmallInt.GetTypeHandler() },
			{ typeof(short?), SqlSmallInt.GetTypeHandler() },
			{ typeof(int), SqlInt.GetTypeHandler() },
			{ typeof(int?), SqlInt.GetTypeHandler() },
			{ typeof(long), SqlBigInt.GetTypeHandler() },
			{ typeof(long?), SqlBigInt.GetTypeHandler() },
			{ typeof(bool), SqlBit.GetTypeHandler() },
			{ typeof(bool?), SqlBit.GetTypeHandler() },
			{ typeof(Guid), SqlUniqueIdentifier.GetTypeHandler() },
			{ typeof(Guid?), SqlUniqueIdentifier.GetTypeHandler() },

			// ISqlTypes
			{ typeof(SqlBigInt), SqlBigInt.GetTypeHandler() },
			{ typeof(SqlBinary), SqlBinary.GetTypeHandler() },
			{ typeof(SqlBit), SqlBit.GetTypeHandler() },
			{ typeof(SqlInt), SqlInt.GetTypeHandler() },
			{ typeof(SqlSmallInt), SqlSmallInt.GetTypeHandler() },
			{ typeof(SqlTinyInt), SqlTinyInt.GetTypeHandler() },
			{ typeof(SqlUniqueIdentifier), SqlUniqueIdentifier.GetTypeHandler() },
			{ typeof(SqlVarChar), SqlVarChar.GetTypeHandler() },
			{ typeof(SqlNVarChar), SqlNVarChar.GetTypeHandler() }
		};

		/// <summary>
		/// Map between SqlDbType and their corresponding ITypeHandlers
		/// </summary>
		internal static Dictionary<SqlDbType, ITypeHandler> SqlDbTypeHandlers = new Dictionary<SqlDbType, ITypeHandler> {	
			{ SqlDbType.BigInt, SqlBigInt.GetTypeHandler() },  
			{ SqlDbType.Binary, SqlBinary.GetTypeHandler() },  
			{ SqlDbType.Bit, SqlBit.GetTypeHandler() },		   
			{ SqlDbType.Int, SqlInt.GetTypeHandler() },
			{ SqlDbType.NVarChar, SqlNVarChar.GetTypeHandler() },
			{ SqlDbType.SmallInt, SqlSmallInt.GetTypeHandler() },
			{ SqlDbType.TinyInt, SqlTinyInt.GetTypeHandler() },
			{ SqlDbType.UniqueIdentifier, SqlUniqueIdentifier.GetTypeHandler() },
			{ SqlDbType.VarChar, SqlVarChar.GetTypeHandler() }
		};
		
		/// <summary>
		/// Loops through each property on the parameters object and adds it as a parameter to the SqlCommand.
		/// </summary>
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
						param = ClrTypeHandlers[prop.PropertyType].CreateParamFromValue(propValue);
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

				// To avoid nasty surprises, throw in case parameter already has parameter
				// by the same name.
				if (cmd.Parameters.Contains(prop.Name) || cmd.Parameters.Contains(param.ParameterName))
					throw new DuplicateParameterException(param.ParameterName);

				cmd.Parameters.Add(param);
			}
		}

		/// <summary>
		/// Returns a raw and open SqlConnection for manual use.
		/// </summary>
		public SqlConnection GetOpenConnection()
		{
			return getConnection();
		}

		/// <summary>
		/// Creates and returns an open SqlConnection.
		/// </summary>
		private SqlConnection getConnection()
		{
			var conn = new SqlConnection(connectionString);

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