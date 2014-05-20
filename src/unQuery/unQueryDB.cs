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
	[Serializable]
	public class unQueryDB
	{
		[NonSerialized]
		private static ConcurrentDictionary<Type, Action<SqlParameterCollection, object>> parameterAdderCache = new ConcurrentDictionary<Type, Action<SqlParameterCollection, object>>();

		[NonSerialized]
		private static ConcurrentDictionary<string, Action<object, object[]>> typeWriterCache = new ConcurrentDictionary<string, Action<object, object[]>>();

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
		/// Resets the type caches. This should only be used for testing purposes.
		/// </summary>
		internal static void ResetCache()
		{
			typeWriterCache = new ConcurrentDictionary<string, Action<object, object[]>>();
			parameterAdderCache = new ConcurrentDictionary<Type, Action<SqlParameterCollection, object>>();
		}

		/// <summary>
		/// Executes the batch and returns all rows from the single result set.
		/// </summary>
		private IList<dynamic> getRows(string sql, CommandBehavior behavior, object parameters)
		{
			using (var conn = getConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				if (parameters != null)
					AddParametersToCommand(cmd.Parameters, parameters);

				using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | behavior))
					return mapReaderRowsToObject(reader).ToList();
			}
		}

		/// <summary>
		/// Executes the batch and returns all rows from the single result set. Each row is mapped into a new instance of T, mapping the columns
		/// to properties based on name matching.
		/// </summary>
		private IList<T> getRows<T>(string sql, CommandBehavior behavior, object parameters)
		{
			using (var conn = getConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				if (parameters != null)
					AddParametersToCommand(cmd.Parameters, parameters);

				using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | behavior))
				{
					if (typeof(T).IsValueType || typeof(T) == typeof(string))
						return mapReaderRowsToList<T>(reader);
					else
						return mapReaderRowsToType(reader, Activator.CreateInstance<T>).ToList();
				}
			}
		}

		/// <summary>
		/// Takes each row of a datareader and returns a list of the values in the first column.
		/// </summary>
		private IList<T> mapReaderRowsToList<T>(SqlDataReader reader)
		{
			var list = new List<T>();
			bool first = true;

			while (reader.Read())
			{
				// Verify result set on first iteration
				if (first)
				{
					if (reader.VisibleFieldCount > 1)
						throw new MoreThanOneColumnException();

					first = false;
				}

				object value = reader.GetValue(0);

				if (value == DBNull.Value)
					list.Add(default(T));
				else
					list.Add((T)value);
			}

			return list;
		}

		/// <summary>
		/// Executes the batch and returns all rows from the single result set.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		public IList<dynamic> GetRows(string sql, object parameters = null)
		{
			return getRows(sql, CommandBehavior.Default, parameters);
		}

		/// <summary>
		/// Executes the batch and returns all rows from the single result set. Each row is mapped into a new instance of T, mapping the columns
		/// to properties based on name matching.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		public IList<T> GetRows<T>(string sql, object parameters = null)
		{
			return getRows<T>(sql, CommandBehavior.Default, parameters);
		}

		/// <summary>
		/// Executes the batch and returns a single row of data. If more than one row is is returned from the database,
		/// all but the first will be discarded.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		public dynamic GetRow(string sql, object parameters = null)
		{
			return getRows(sql, CommandBehavior.SingleRow, parameters).FirstOrDefault();
		}

		/// <summary>
		/// Executes the batch and returns a single row of data. If more than one row is is returned from the database,
		/// all but the first will be discarded. The row will be mapped to the properties of the generic type.
		/// </summary>
		/// <param name="sql">The SQL statement to execute.</param>
		/// <param name="parameters">Anonymous object providing parameters for the query.</param>
		public T GetRow<T>(string sql, object parameters = null) where T : new()
		{
			return getRows<T>(sql, CommandBehavior.SingleResult | CommandBehavior.SingleRow, parameters).FirstOrDefault();
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

				try
				{
					return (T)result;
				}
				catch (InvalidCastException)
				{
					throw new InvalidCastException("Can't cast return type '" + result.GetType() + "' into '" + typeof(T) + "'");
				}
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
		/// Maps each row into a new instance of T, mapping columns to properties based on the name.
		/// </summary>
		private IEnumerable<T> mapReaderRowsToType<T>(SqlDataReader reader, Func<T> typeCreator)
		{
			int visibleFieldCount = reader.VisibleFieldCount;
			
			string schemaIdentifier = "";
			for (int i = 0; i < visibleFieldCount; i++)
				schemaIdentifier += reader.GetName(i) + ",";

			string typeWriterIdentifier = typeof(T) + ";" + schemaIdentifier;

			Action<object, object[]> typeWriter;
			if (!typeWriterCache.TryGetValue(typeWriterIdentifier, out typeWriter))
			{
				var type = typeof(T);
				PropertyInfo[] properties = type.GetProperties();

				var schema = reader.GetSchemaTable().AsEnumerable().Select(x => new {
					Name = x.Field<string>("ColumnName"),
					Ordinal = x.Field<int>("ColumnOrdinal")
				});
				 
				// If there are no properties on the type, it doesn't make sense to use it here
				if (properties.Length == 0)
					throw new ObjectHasNoPropertiesException("Can't use a type with no properties.");
		
				var dm = new DynamicMethod("SetProperties", typeof(void), new[] { typeof(object), typeof(object[]) }, true);
				var il = dm.GetILGenerator();

				// First we'll want to cast the object value into the type of the actual value and store it as a local variable
				il.DeclareLocal(type); // []
				il.Emit(OpCodes.Ldarg_0); // [object]
				il.Emit(OpCodes.Castclass, type); // [object]
				il.Emit(OpCodes.Stloc_0); // []
				
				foreach (var prop in properties)
				{
					var endLabel = il.DefineLabel();
					var propColumns = schema.Where(x => string.Equals(prop.Name, x.Name, StringComparison.InvariantCultureIgnoreCase)).ToList();

					// If property doesn't have a matching column, skip it
					if (propColumns.Count == 0)
						continue;

					// If more than one column shares the same property name, we have to throw
					if (propColumns.Count > 1)
						throw new DuplicateColumnException(prop.Name);

					// Get the single column
					var propColumn = propColumns.Single();

					// Load the values
					il.Emit(OpCodes.Ldarg_1); // [values[]]

					// Load the column ordinal
					il.Emit(OpCodes.Ldc_I4, propColumn.Ordinal); // [values[], ordinal]

					// Load a reference to the value element
					il.Emit(OpCodes.Ldelem_Ref); // [value_ref]

					// Load DBNull.Value
					il.Emit(OpCodes.Ldsfld, typeof(DBNull).GetField("Value")); // [values_ref, DBNull.Value]

					// Compare the values - resulting in 0 or 1 being pushed onto the stack
					il.Emit(OpCodes.Ceq); // [0/1]

					// If it's null , skip setting the value
					il.Emit(OpCodes.Brtrue, endLabel); // []

					// Begin a try {} block
					il.BeginExceptionBlock();

					// Load the type object
					il.Emit(OpCodes.Ldloc_0); // [object]

					// Load the element reference
					il.Emit(OpCodes.Ldarg_1); // [object, values[]]
					il.Emit(OpCodes.Ldc_I4, propColumn.Ordinal); // [object, values[], ordinal]
					il.Emit(OpCodes.Ldelem_Ref); // [object, value_ref]

					// Unbox the value & set the value
					il.Emit(OpCodes.Unbox_Any, prop.PropertyType); // [object, value]
					il.Emit(OpCodes.Callvirt, prop.GetSetMethod(true)); // []

					// If we succeeded in setting the value, skip to the end of the method
					il.Emit(OpCodes.Leave, endLabel);
					
					// Catch an InvalidCastException
					il.BeginCatchBlock(typeof(InvalidCastException)); // [ex]
					
					// Pop the actual exception
					il.Emit(OpCodes.Pop); // []

					// Throw TypeMismatchException with details
					string errorMessage = "Property '" + prop.Name + "' with type '" + prop.PropertyType + "' does not match column with the same name.";
					il.Emit(OpCodes.Ldstr, errorMessage); // [errorMessage]
					il.Emit(OpCodes.Newobj, typeof(TypeMismatchException).GetConstructor(new[] { typeof(string) })); // [ex]
					il.Emit(OpCodes.Throw); // []

					// Exit the catch block and fall through to the end of the method
					il.EndExceptionBlock();

					// Marks the end of this property's value setting
					il.MarkLabel(endLabel);
				}
				
				// Return
				il.Emit(OpCodes.Ret);

				// Add it to the cache
				typeWriter = (Action<object, object[]>)dm.CreateDelegate(typeof(Action<object, object[]>));
				typeWriterCache.AddOrUpdate(typeWriterIdentifier, typeWriter, (k, v) => v);
			}

			while (reader.Read())
			{
				var values = new object[visibleFieldCount];
				reader.GetValues(values);

				var row = typeCreator();
				typeWriter(row, values);

				yield return row;
			}
		}

		/// <summary>
		/// Maps any number of rows to dynamic objects
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
					var dm = new DynamicMethod("AddParameters", typeof(void), new[] { typeof(SqlParameterCollection), typeof(object) }, true);
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
							throw new ParameterTypeNotSupportedException(prop.Name, prop.PropertyType);

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
	}
}