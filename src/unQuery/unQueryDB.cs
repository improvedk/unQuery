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

		public unQueryDB(string connectionString)
		{
			this.connectionString = connectionString;
		}

		/// <summary>
		/// Executes the batch and returns all rows from the single result set.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		public IList<dynamic> GetRows(string sql, dynamic parameters = null)
		{
			using (var conn = getConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				if (parameters != null)
					AddParametersToCommand(cmd, parameters);

				var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
				
				return MapReaderRowsToObject(reader).ToList();
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
			using (var conn = getConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				if (parameters != null)
					AddParametersToCommand(cmd, parameters);

				var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow);

				if (!reader.Read())
					return null;

				return MapReaderRowToObject(reader, reader.VisibleFieldCount);
			}
		}

		/// <summary>
		/// Executes the batch, and returns the first column of the first row of the first result set returned by the query.
		/// Additional visibleFieldCount or rows are ignored.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		/// <exception cref="NoRowsException" />
		public T GetScalar<T>(string sql, dynamic parameters = null)
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
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		public int Execute(string sql, dynamic parameters = null)
		{
			using (var conn = getConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				if (parameters != null)
					AddParametersToCommand(cmd, parameters);

				return cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Maps a single row from a SqlDataReader into a dynamic object. This method is optimized for just a single row.
		/// </summary>
		/// <param name="reader">The SqlDataReader from which the schema & values should be read.</param>
		/// <param name="visibleFieldCount">The number of visible columns in the datareader.</param>
		internal static dynamic MapReaderRowToObject(SqlDataReader reader, int visibleFieldCount)
		{
			var obj = new Dictionary<string, object>(visibleFieldCount);

			for (int i = 0; i < visibleFieldCount; i++)
			{
				object value = reader[i];
				string fieldName = reader.GetName(i);

				if (string.IsNullOrWhiteSpace(fieldName))
					throw new UnnamedColumnException(i);

				try
				{
					obj.Add(reader.GetName(i), value is DBNull ? null : value);
				}
				catch (ArgumentException)
				{
					throw new DuplicateColumnException(fieldName);
				}
			}

			return new DynamicRow(obj);
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
				
				yield return new DynamicFieldMapRow(obj, fieldMap);
			}
		}

		/// <summary>
		/// These are the default mappings between C# datatypes and their equivalent database types. By default, only the
		/// safe types are mapped, e.g. types that are non-ambiguously translated between C# and SQL Server.
		/// </summary>
		private static readonly Dictionary<Type, Func<object, SqlParameter>> typeHandlers = new Dictionary<Type, Func<object, SqlParameter>> {
			{ typeof(short), x => SqlSmallInt.GetParameter((short)x) },
			{ typeof(short?), x => SqlSmallInt.GetParameter((short?)x) },
			{ typeof(byte), x => SqlTinyInt.GetParameter((byte)x) },
			{ typeof(byte?), x => SqlTinyInt.GetParameter((byte?)x) },
			{ typeof(int), x => SqlInt.GetParameter((int)x) },
			{ typeof(int?), x => SqlInt.GetParameter((int?)x) },
			{ typeof(long), x => SqlBigInt.GetParameter((long)x) },
			{ typeof(long?), x => SqlBigInt.GetParameter((long?)x) },
			{ typeof(bool), x => SqlBit.GetParameter((bool)x) },
			{ typeof(bool?), x => SqlBit.GetParameter((bool?)x) },
			{ typeof(Guid), x => SqlUniqueIdentifier.GetParameter((Guid)x) },
			{ typeof(Guid?), x => SqlUniqueIdentifier.GetParameter((Guid?)x) }
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
				Type propertyType = propValue != null ? propValue.GetType() : prop.PropertyType;
				var sqlType = propValue as ISqlType;
			
				try
				{
					// If it's a SqlType value, let it create the parameter for us. Otherwise, for native CLR types we'll
					// let the type handlers take care of creating the parameter.
					if (sqlType != null)
						param = sqlType.GetParameter();
					else
						param = typeHandlers[propertyType](propValue);
				}
				catch (KeyNotFoundException)
				{
					throw new TypeNotSupportedException(propertyType);
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