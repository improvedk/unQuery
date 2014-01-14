﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using unQuery.SqlTypes;

namespace unQuery
{
	public class unQueryDB
	{
		private readonly string connectionString;
		private List<SqlCommand> batchCommands;

		/// <summary>
		/// Instantiates an unQuery instance that connects using the provided connection string.
		/// </summary>
		/// <param name="connectionString">The database connection string.</param>
		public unQueryDB(string connectionString)
		{
			this.connectionString = connectionString;
		}

		/// <summary>
		/// Executes the batch and returns all rows from the single result set.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		public IList<dynamic> GetRows(string sql, object parameters = null)
		{
			using (var conn = getConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				if (parameters != null)
					AddParametersToCommand(cmd.Parameters, parameters);

				var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);

				return mapReaderRowsToObject(reader).ToList();
			}
		}

		/// <summary>
		/// Executes the batch and returns a single row of data. If more than one row is is returned from the database,
		/// all but the first will be discarded.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		public dynamic GetRow(string sql, object parameters = null)
		{
			using (var conn = getConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				if (parameters != null)
					AddParametersToCommand(cmd.Parameters, parameters);

				var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow);

				return mapReaderRowsToObject(reader).SingleOrDefault();
			}
		}

		/// <summary>
		/// Executes the batch, and returns the first column of the first row of the first result set returned by the query.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		/// <exception cref="NoRowsException" />
		public T GetScalar<T>(string sql, object parameters = null)
		{
			using (var conn = getConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				if (parameters != null)
					AddParametersToCommand(cmd.Parameters, parameters);

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
		public int Execute(string sql, object parameters = null)
		{
			using (var conn = getConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				if (parameters != null)
					AddParametersToCommand(cmd.Parameters, parameters);

				return cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Returns a BatchExecutioner that can be used to efficiently batch up several execution commands in one go.
		/// </summary>
		public BatchExecutor Batch()
		{
			return new BatchExecutor(this);
		}
		
		/// <summary>
		/// Maps any number of rows to dynamic objects. This method is optimized for returning many rows.
		/// </summary>
		/// <param name="reader">The SqlDataReader from which the schema & values should be read.</param>
		private IEnumerable<dynamic> mapReaderRowsToObject(SqlDataReader reader)
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
		internal static Dictionary<Type, SqlTypeHandler> ClrTypeHandlers = new Dictionary<Type, SqlTypeHandler> {
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
			{ typeof(SqlChar), SqlChar.GetTypeHandler() },
			{ typeof(SqlDate), SqlDate.GetTypeHandler() },
			{ typeof(SqlDateTime), SqlDateTime.GetTypeHandler() },	 
			{ typeof(SqlDateTime2), SqlDateTime2.GetTypeHandler() },	 
			{ typeof(SqlDateTimeOffset), SqlDateTimeOffset.GetTypeHandler() },	 
			{ typeof(SqlDecimal), SqlDecimal.GetTypeHandler() },
			{ typeof(SqlFloat), SqlFloat.GetTypeHandler() },
			{ typeof(SqlImage), SqlImage.GetTypeHandler() },
			{ typeof(SqlInt), SqlInt.GetTypeHandler() },		
			{ typeof(SqlMoney), SqlMoney.GetTypeHandler() },		
			{ typeof(SqlNChar), SqlNChar.GetTypeHandler() },	
			{ typeof(SqlNText), SqlNText.GetTypeHandler() },	
			{ typeof(SqlNVarChar), SqlNVarChar.GetTypeHandler() },
			{ typeof(SqlReal), SqlReal.GetTypeHandler() },
			{ typeof(SqlSmallDateTime), SqlSmallDateTime.GetTypeHandler() },
			{ typeof(SqlSmallInt), SqlSmallInt.GetTypeHandler() },
			{ typeof(SqlSmallMoney), SqlSmallMoney.GetTypeHandler() },
			{ typeof(SqlText), SqlText.GetTypeHandler() },
			{ typeof(SqlTime), SqlTime.GetTypeHandler() },
			{ typeof(SqlTinyInt), SqlTinyInt.GetTypeHandler() },
			{ typeof(SqlUniqueIdentifier), SqlUniqueIdentifier.GetTypeHandler() },
			{ typeof(SqlVarBinary), SqlVarBinary.GetTypeHandler() },
			{ typeof(SqlVarChar), SqlVarChar.GetTypeHandler() },
			{ typeof(SqlXml), SqlXml.GetTypeHandler() }
		};

		/// <summary>
		/// Map between SqlDbType and their corresponding ITypeHandlers
		/// </summary>
		internal static Dictionary<SqlDbType, SqlTypeHandler> SqlDbTypeHandlers = new Dictionary<SqlDbType, SqlTypeHandler> {
			{ SqlDbType.BigInt, SqlBigInt.GetTypeHandler() },  
			{ SqlDbType.Binary, SqlBinary.GetTypeHandler() },
			{ SqlDbType.Bit, SqlBit.GetTypeHandler() },
			{ SqlDbType.Char, SqlChar.GetTypeHandler() },
			{ SqlDbType.Date, SqlDate.GetTypeHandler() },
			{ SqlDbType.DateTime, SqlDateTime.GetTypeHandler() },
			{ SqlDbType.DateTime2, SqlDateTime2.GetTypeHandler() },
			{ SqlDbType.DateTimeOffset, SqlDateTimeOffset.GetTypeHandler() },
 			{ SqlDbType.Decimal, SqlDecimal.GetTypeHandler() },
 			{ SqlDbType.Float, SqlFloat.GetTypeHandler() },
 			{ SqlDbType.Image, SqlImage.GetTypeHandler() },
			{ SqlDbType.Int, SqlInt.GetTypeHandler() },
			{ SqlDbType.Money, SqlMoney.GetTypeHandler() },
			{ SqlDbType.NChar, SqlNChar.GetTypeHandler() },
			{ SqlDbType.NText, SqlNText.GetTypeHandler() },
			{ SqlDbType.NVarChar, SqlNVarChar.GetTypeHandler() },
			{ SqlDbType.Real, SqlReal.GetTypeHandler() },
			{ SqlDbType.SmallDateTime, SqlSmallDateTime.GetTypeHandler() },
			{ SqlDbType.SmallInt, SqlSmallInt.GetTypeHandler() },
			{ SqlDbType.SmallMoney, SqlSmallMoney.GetTypeHandler() },
			{ SqlDbType.Text, SqlText.GetTypeHandler() },
			{ SqlDbType.Time, SqlTime.GetTypeHandler() },
			{ SqlDbType.TinyInt, SqlTinyInt.GetTypeHandler() },
			{ SqlDbType.UniqueIdentifier, SqlUniqueIdentifier.GetTypeHandler() },
			{ SqlDbType.VarBinary, SqlVarBinary.GetTypeHandler() },
			{ SqlDbType.VarChar, SqlVarChar.GetTypeHandler() },
			{ SqlDbType.Xml, SqlXml.GetTypeHandler() }
		};

		/// <summary>
		/// Loops through each property on the parameters object and adds it as a parameter to the SqlCommand.
		/// </summary>
		private static readonly ConcurrentDictionary<Type, Action<SqlParameterCollection, object>> parameterAdderCache = new ConcurrentDictionary<Type, Action<SqlParameterCollection, object>>();
		internal void AddParametersToCommand(SqlParameterCollection paramCollection, object parameters)
		{
			// We can't mix unQuery parameters with existing parameters
			if (paramCollection.Count > 0)
				throw new SqlCommandAlreadyHasParametersException();

			// Attempt to get parameter adder from cache; otherwise create it
			var paramType = parameters.GetType();
			Action<SqlParameterCollection, object> parameterAdder;

			if (!parameterAdderCache.TryGetValue(paramType, out parameterAdder))
			{
				PropertyInfo[] properties = paramType.GetProperties();

				// If no properties are found on the provided parameter object, then there's no schema & value to read
				if (properties.Length == 0)
					parameterAdderCache.AddOrUpdate(paramType, (Action<SqlParameterCollection, object>)null, (k, v) => v);
				else
				{
					var dm = new DynamicMethod("AddParameters", typeof(void), new[] {typeof(SqlParameterCollection), typeof(object)}, true);
					var il = dm.GetILGenerator();

					// First we'll want to cast the object value into the type of the actual value and store it as a local variable
					il.DeclareLocal(paramType); // []
					il.Emit(OpCodes.Ldarg_1); // [object]
					il.Emit(OpCodes.Castclass, paramType); // [object]
					il.Emit(OpCodes.Stloc_0); // []

					// Loop object properties
					int localIndex = 1;
					foreach (var prop in properties)
					{
						// Is this property type supported?
						if (!ClrTypeHandlers.ContainsKey(prop.PropertyType) && prop.PropertyType != typeof(SqlStructured))
							throw new TypeNotSupportedException(prop.PropertyType);

						if (typeof(SqlType).IsAssignableFrom(prop.PropertyType))
						{
							int paramLocIndex = localIndex++;

							// It's a SqlType
							il.Emit(OpCodes.Ldloc_0); // Load the object [object]
							il.Emit(OpCodes.Callvirt, prop.GetGetMethod()); // Get the property value [value]
							il.Emit(OpCodes.Callvirt, prop.PropertyType.GetMethod("GetParameter", BindingFlags.NonPublic | BindingFlags.Instance)); // Get the parameter from the SqlType value [param]
							il.DeclareLocal(typeof(SqlParameter));
							il.Emit(OpCodes.Stloc, paramLocIndex); // Store the parameter as a var []
							il.Emit(OpCodes.Ldloc, paramLocIndex); // Load the parameter again [param]
							il.Emit(OpCodes.Ldstr, "@" + prop.Name); // Load the parameter name [param, name]
							il.Emit(OpCodes.Call, typeof(SqlParameter).GetMethod("set_ParameterName")); // Set the parameter name []
							il.Emit(OpCodes.Ldarg_0); // Load the param collection [paramCollection]
							il.Emit(OpCodes.Ldloc, paramLocIndex); // Load the parameter [paramCollection, param]
							il.Emit(OpCodes.Call, typeof(SqlParameterCollection).GetMethod("Add", new[] {typeof(SqlParameter)})); // Add the parameter to the collection [param]
							il.Emit(OpCodes.Pop); // Get rid of the added parameter, as returned by SqlParameterCollection.Add []
						}
						else
						{
							il.Emit(OpCodes.Ldarg_0); // Load the parameter collection [paramCollection]
							il.Emit(OpCodes.Call, ClrTypeHandlers[prop.PropertyType].GetType().GetMethod("GetTypeHandler", BindingFlags.NonPublic | BindingFlags.Static)); // Get the type handler [paramCollection, typeHandler]
							il.Emit(OpCodes.Ldstr, prop.Name); // Load the parameter name [paramCollection, typeHandler, paramName]
							il.Emit(OpCodes.Ldloc_0); // Load the object [paramCollection, typeHandler, paramName, object]
							il.Emit(OpCodes.Call, prop.GetGetMethod()); // Get the property value [paramCollection, typeHandler, paramName, value]
							il.Emit(OpCodes.Box, prop.PropertyType); // Box the value [paramCollection, typeHandler, paramName, boxedValue]
							il.Emit(OpCodes.Callvirt, typeof(SqlTypeHandler).GetMethod("CreateParamFromValue", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] {typeof(string), typeof(object)}, null)); // Let the type handler create the param [paramCollection, param]
							il.Emit(OpCodes.Call, typeof(SqlParameterCollection).GetMethod("Add", new[] {typeof(SqlParameter)})); // Add the parameter to the collection [param]
							il.Emit(OpCodes.Pop); // Get rid of the param as we don't need it anymore []*/
						}
					}

					// Return
					il.Emit(OpCodes.Ret);

					// Add it to the cache
					parameterAdder = (Action<SqlParameterCollection, object>) dm.CreateDelegate(typeof(Action<SqlParameterCollection, object>));
					parameterAdderCache.AddOrUpdate(paramType, parameterAdder, (k, v) => v);
				}
			}

			// Run the cached parameter adder
			if (parameterAdder != null)
				parameterAdder(paramCollection, parameters);
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

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}