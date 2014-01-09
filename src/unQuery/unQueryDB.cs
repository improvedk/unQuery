using System;
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
		public IList<dynamic> GetRows(string sql, dynamic parameters = null)
		{
			using (var conn = getConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				if (parameters != null)
					AddParametersToCommand(cmd, parameters);

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
		public dynamic GetRow(string sql, dynamic parameters = null)
		{
			using (var conn = getConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				if (parameters != null)
					AddParametersToCommand(cmd, parameters);

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
		private static ConcurrentDictionary<Type, Action<SqlCommand, object>> parameterAdderCache = new ConcurrentDictionary<Type, Action<SqlCommand, object>>();
		internal void AddParametersToCommand(SqlCommand cmd, object parameters)
		{
			// We can't mix unQuery parameters with existing parameters
			if (cmd.Parameters.Count > 0)
				throw new SqlCommandAlreadyHasParametersException();

			// Attempt to get parameter adder from cache; otherwise create it
			var paramType = parameters.GetType();
			Action<SqlCommand, object> parameterAdder;

			if (!parameterAdderCache.TryGetValue(paramType, out parameterAdder))
			{
				PropertyInfo[] properties = paramType.GetProperties();

				// If no properties are found on the provided parameter object, then there's no schema & value to read
				if (properties.Length == 0)
					throw new ObjectHasNoPropertiesException("For an object to be used as a value for a Structured parameter, its properties need to match the SQL Server type. The provided object has no properties.");

				var dm = new DynamicMethod("AddParameters", typeof(void), new[] { typeof(SqlCommand), typeof(object) }, true);
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

					// The name of the parameter we're about to create
					string paramName = "@" + prop.Name;

					if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
					{
						int paramLocIndex = localIndex++;

						// It's a nullable type
						il.Emit(OpCodes.Call, ClrTypeHandlers[prop.PropertyType].GetType().GetMethod("GetTypeHandler", BindingFlags.NonPublic | BindingFlags.Static)); // Get the type handler [typeHandler]
						il.Emit(OpCodes.Ldloc_0); // Load the object [typeHandler, object]
						il.Emit(OpCodes.Callvirt, prop.GetMethod); // Get the property value [typeHandler, value]
						il.Emit(OpCodes.Box, prop.PropertyType); // Box the value [typeHandler, boxedValue]
						il.Emit(OpCodes.Callvirt, typeof(SqlTypeHandler).GetMethod("CreateParamFromValue", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(object) }, null)); // Let the type handler create the param [param]
						il.DeclareLocal(typeof(SqlParameter));
						il.Emit(OpCodes.Stloc, paramLocIndex); // Store the parameter as a variable []
						il.Emit(OpCodes.Ldloc, paramLocIndex); // Load the parameter again [param]
						il.Emit(OpCodes.Ldstr, paramName); // Load the parameter name [param, paramName]
						il.Emit(OpCodes.Callvirt, typeof(SqlParameter).GetMethod("set_ParameterName")); // Set the parameter name []
						il.Emit(OpCodes.Ldarg_0); // Load the command [cmd]
						il.Emit(OpCodes.Call, typeof(SqlCommand).GetMethod("get_Parameters")); // Load the parameter collection [paramCollection]
						il.Emit(OpCodes.Ldloc, paramLocIndex); // Load the parameter [paramCollection, param]
						il.Emit(OpCodes.Call, typeof(SqlParameterCollection).GetMethod("Add", new[] { typeof(SqlParameter) })); // Add the parameter to the collection [param]
						il.Emit(OpCodes.Pop); // Get rid of the added parameter, as returned by SqlParameterCollection.Add []
					}
					else if (typeof(SqlType).IsAssignableFrom(prop.PropertyType))
					{
						int paramLocIndex = localIndex++;

						// It's a SqlType
						il.Emit(OpCodes.Ldloc_0); // Load the object [object]
						il.Emit(OpCodes.Callvirt, prop.GetMethod); // Get the property value [value]
						il.Emit(OpCodes.Callvirt, prop.PropertyType.GetMethod("GetParameter", BindingFlags.NonPublic | BindingFlags.Instance)); // Get the parameter from the SqlType value [param]
						il.DeclareLocal(typeof(SqlParameter));
						il.Emit(OpCodes.Stloc, paramLocIndex); // Store the parameter as a var []
						il.Emit(OpCodes.Ldloc, paramLocIndex); // Load the parameter again [param]
						il.Emit(OpCodes.Ldstr, paramName); // Load the parameter name [param, name]
						il.Emit(OpCodes.Call, typeof(SqlParameter).GetMethod("set_ParameterName")); // Set the parameter name []
						il.Emit(OpCodes.Ldarg_0); // Load the command [cmd]
						il.Emit(OpCodes.Call, typeof(SqlCommand).GetMethod("get_Parameters")); // Load the parameter collection [paramCollection]
						il.Emit(OpCodes.Ldloc, paramLocIndex); // Load the parameter [paramCollection, param]
						il.Emit(OpCodes.Call, typeof(SqlParameterCollection).GetMethod("Add", new[] { typeof(SqlParameter) })); // Add the parameter to the collection [param]
						il.Emit(OpCodes.Pop); // Get rid of the added parameter, as returned by SqlParameterCollection.Add []
					}
					else
					{
						int valueTypeLocIndex = localIndex++;
						int typeHandlerLocIndex = localIndex++;
						int paramLocIndex = localIndex++;

						// It's a non-nullable value type
						il.Emit(OpCodes.Ldloc_0); // Load the object [object]
						il.Emit(OpCodes.Callvirt, prop.GetMethod); // Get the property value [value]
						il.Emit(OpCodes.Box, prop.PropertyType); // Box the value [boxedValue]
						il.Emit(OpCodes.Call, typeof(object).GetMethod("GetType")); // Get the type of the value [valueType]
						il.DeclareLocal(typeof(Type));
						il.Emit(OpCodes.Stloc, valueTypeLocIndex); // Store the type as a variable []
						il.Emit(OpCodes.Ldsfld, typeof(unQueryDB).GetField("ClrTypeHandlers", BindingFlags.NonPublic | BindingFlags.Static)); // Load the ClrTypeHandler map [typeHandlers]
						il.Emit(OpCodes.Ldloc, valueTypeLocIndex); // Load the value type [typeHandlers, valueType]
						il.Emit(OpCodes.Callvirt, typeof(Dictionary<Type, SqlTypeHandler>).GetMethod("get_Item")); // Get the value type handler [typeHandler]
						il.DeclareLocal(typeof(SqlTypeHandler));
						il.Emit(OpCodes.Stloc, typeHandlerLocIndex); // Store the type handler as a variable []
						il.Emit(OpCodes.Ldloc, typeHandlerLocIndex); // Load the type handler again [typeHandler]
						il.Emit(OpCodes.Ldloc_0); // Load the object [typeHandler, object]
						il.Emit(OpCodes.Callvirt, prop.GetMethod); // Get the property value [typeHandler, value]
						il.Emit(OpCodes.Box, prop.PropertyType); // Box the value [typeHandler, boxedValue]
						il.Emit(OpCodes.Callvirt, typeof(SqlTypeHandler).GetMethod("CreateParamFromValue", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(object) }, null)); // Let the type handler create a param based upon the value [param]
						il.DeclareLocal(typeof(SqlParameter));
						il.Emit(OpCodes.Stloc, paramLocIndex); // Store the parameter as a variable []
						il.Emit(OpCodes.Ldloc, paramLocIndex); // Load the parameter again [param]
						il.Emit(OpCodes.Ldstr, paramName); // Load the parameter name [param, paramName]
						il.Emit(OpCodes.Callvirt, typeof(SqlParameter).GetMethod("set_ParameterName")); // Set the parameter name []
						il.Emit(OpCodes.Ldarg_0); // Load the command [cmd]
						il.Emit(OpCodes.Call, typeof(SqlCommand).GetMethod("get_Parameters")); // Load the parameter collection [paramCollection]
						il.Emit(OpCodes.Ldloc, paramLocIndex); // Load the parameter [paramCollection, param]
						il.Emit(OpCodes.Call, typeof(SqlParameterCollection).GetMethod("Add", new[] { typeof(SqlParameter) })); // Add the parameter to the collection [param]
						il.Emit(OpCodes.Pop); // Get rid of the added parameter, as returned by SqlParameterCollection.Add []
					}
				}

				// Return
				il.Emit(OpCodes.Ret);
				
				// Add it to the cache
				parameterAdder = (Action<SqlCommand, object>)dm.CreateDelegate(typeof(Action<SqlCommand, object>));
				parameterAdderCache.AddOrUpdate(paramType, parameterAdder, (k, v) => v);
			}

			// Run the cached parameter adder
			parameterAdder(cmd, parameters);
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