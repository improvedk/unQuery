using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using unQuery.SqlTypes;

namespace unQuery
{
	/// <summary>
	/// Datasource that yields SqlDataRecords for efficient streaming of table valued parameter data to SQL Server.
	/// </summary>
	internal class StructuredDynamicYielder : IEnumerable<SqlDataRecord>
	{
		private static ConcurrentDictionary<Type, TypeHandler> typeHandlers = new ConcurrentDictionary<Type, TypeHandler>();
		private static readonly Dictionary<Type, MethodInfo> sqlDataRecordSetters = new Dictionary<Type, MethodInfo> {
			{ typeof(int), typeof(SqlDataRecord).GetMethod("SetInt32", new[] { typeof(int), typeof(int) }) },
			{ typeof(string), typeof(SqlDataRecord).GetMethod("SetString", new[] { typeof(int), typeof(string) }) },
			{ typeof(bool), typeof(SqlDataRecord).GetMethod("SetBoolean", new[] { typeof(int), typeof(bool) }) },
			{ typeof(byte), typeof(SqlDataRecord).GetMethod("SetByte", new[] { typeof(int), typeof(byte)}) },
			{ typeof(short), typeof(SqlDataRecord).GetMethod("SetInt16", new[] { typeof(int), typeof(short)}) },
			{ typeof(long), typeof(SqlDataRecord).GetMethod("SetInt64", new[] { typeof(int), typeof(long)}) },
			{ typeof(Guid), typeof(SqlDataRecord).GetMethod("SetGuid", new[] { typeof(int), typeof(Guid)}) }
		};

		private readonly IEnumerable<object> values;

		private class TypeHandler
		{
			internal SqlMetaData[] Schema;
			internal Action<SqlDataRecord, object> SetRecordValues;
		}

		/// <summary>
		/// Instantiates a StructedDynamicYielder that will efficiently stream the values to SQL Server.
		/// </summary>
		/// <param name="values">The values to be streamed. Each object must be of the same type and must match the table type in SQL Server by ordinal position.</param>
		internal StructuredDynamicYielder(IEnumerable<object> values)
		{
			if (values == null)
				throw new ArgumentException("Values can't be null.");

			this.values = values;
		}

		internal static void ResetCache()
		{
			typeHandlers = new ConcurrentDictionary<Type, TypeHandler>();
		}

		private IEnumerable<SqlDataRecord> processSqlTypes(object[] rows)
		{
			var record = new SqlDataRecord(((SqlTypeHandler)rows[0]).CreateMetaData("UnnamedColumn"));
			Type rowType = null;

			foreach (var row in rows)
			{
				// We have to detect potential type mismatches in case there are different ISqlTypes among the rows
				if (row != null)
					if (rowType == null)
						rowType = row.GetType();
					else if (rowType != row.GetType())
						throw new StructuredTypeMismatchException(row.GetType());

				record.SetValue(0, ((SqlType)row).GetRawValue() ?? DBNull.Value);

				yield return record;
			}
		}

		private IEnumerable<SqlDataRecord> processImplicitTypes(object[] rows, SqlTypeHandler implicitTypeHandler)
		{
			var record = new SqlDataRecord(implicitTypeHandler.CreateMetaData("UnnamedColumn"));

			foreach (var row in rows)
			{
				try
				{
					record.SetValue(0, row ?? DBNull.Value);
				}
				catch (InvalidCastException)
				{
					throw new StructuredTypeMismatchException(row.GetType());
				}

				yield return record;
			}
		}

		private IEnumerable<SqlDataRecord> processObjects(object[] rows)
		{
			// We know there's at least one object, so we can safely pull the type of the first one
			Type objectType = rows[0].GetType();
			object objectValue = rows[0];
			TypeHandler typeHandler;

			// If we haven't already seen this type before, we'll have to extract its schema and create a type reader for it
			if (!typeHandlers.TryGetValue(objectType, out typeHandler))
			{
				PropertyInfo[] properties = objectType.GetProperties().OrderBy(x => x.MetadataToken).ToArray();

				// If no properties are found on the provided parameter object, then there's no schema & value to read
				if (properties.Length == 0)
					throw new ObjectHasNoPropertiesException("For an object to be used as a value for a Structured parameter, its properties need to match the SQL Server type. The provided object has no properties.");

				var schema = new SqlMetaData[properties.Length];
				var dm = new DynamicMethod("SetRecordValues", typeof(void), new[] { typeof(SqlDataRecord), typeof(object) }, true);
				var il = dm.GetILGenerator();

				// First we'll want to cast the object value into the type of the actual value and store it as a local variable
				il.DeclareLocal(objectType); // []
				il.Emit(OpCodes.Ldarg_1); // [object]
				il.Emit(OpCodes.Castclass, objectType); // [object]
				il.Emit(OpCodes.Stloc_0); // []

				// Loop object properties
				int propertyIndex = 0;
				int localIndex = 1;
				foreach (PropertyInfo prop in objectType.GetProperties().OrderBy(x => x.MetadataToken))
				{
					try
					{
						// Add property to schema
						if (typeof(SqlTypeHandler).IsAssignableFrom(prop.PropertyType))
							schema[propertyIndex] = ((SqlTypeHandler)prop.GetValue(rows[0], null)).CreateMetaData(prop.Name);
						else
							schema[propertyIndex] = unQueryDB.ClrTypeHandlers[prop.PropertyType].CreateMetaData(prop.Name);

						// Is the type a nullable, reference or value type?
						if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
						{
							// Nullable type []
							var valueLabel = il.DefineLabel();
							var endLabel = il.DefineLabel();

							// Load the record & ordinal onto the stack
							il.Emit(OpCodes.Ldarg_0); // Load the SqlDataRecord parameter [record]
							il.Emit(OpCodes.Ldc_I4, propertyIndex); // Load the ordinal index of the column [record, ordinal]

							// Is the nullable type null?
							il.Emit(OpCodes.Ldloc_0); // Load the object [record, ordinal, object]
							il.Emit(OpCodes.Callvirt, prop.GetGetMethod()); // Get the property value [record, ordinal, value]
							il.DeclareLocal(prop.PropertyType); // We'll have to store the value as a variable so we can pass it by ref
							il.Emit(OpCodes.Stloc, localIndex); // [record, ordinal]
							il.Emit(OpCodes.Ldloca, localIndex); // Load the address of the property value [record, ordinal, &value]
							il.Emit(OpCodes.Call, prop.PropertyType.GetMethod("get_HasValue")); // Check if nullable has value [record, ordinal, hasValue]
							il.Emit(OpCodes.Brtrue, valueLabel); // If non-null, skip setting a null value [record, ordinal]

							// Set a DBNull value
							il.Emit(OpCodes.Callvirt, typeof(SqlDataRecord).GetMethod("SetDBNull", new[] { typeof(int) })); // Set a DBNull value []
							il.Emit(OpCodes.Br, endLabel); // Jump past setting a value []

							// Set an actual value
							il.MarkLabel(valueLabel); // Marks the beginning where we're setting an actual value [record, ordinal]
							il.Emit(OpCodes.Ldloca, localIndex); // Load the value [record, ordinal, &value]
							il.Emit(OpCodes.Call, prop.PropertyType.GetMethod("get_Value")); // Get the underlying value [record, ordinal, value]
							il.Emit(OpCodes.Callvirt, sqlDataRecordSetters[Nullable.GetUnderlyingType(prop.PropertyType)]); // Set value []

							// End
							il.MarkLabel(endLabel);

							localIndex++;
						}
						else
						{
							// Reference & value types []
							if (typeof(SqlType).IsAssignableFrom(prop.PropertyType))
							{
								// Type is an ISqlType and we need to extract the value
								il.Emit(OpCodes.Ldloc_0); // Load the object [object]
								il.Emit(OpCodes.Call, prop.GetGetMethod()); // Load the property value [value]
								il.Emit(OpCodes.Ldarg_0); // Load the record [value, record]
								il.Emit(OpCodes.Ldc_I4, propertyIndex); // Load the ordinal [value, record, ordinal]
								il.Emit(OpCodes.Callvirt, prop.PropertyType.GetMethod("SetDataRecordValue", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(SqlDataRecord), typeof(int) }, null) ); // Let the ISqlType set the value itself []
							}
							else
							{
								// Load the record & ordinal onto the stack
								il.Emit(OpCodes.Ldarg_0); // Load the SqlDataRecord parameter [record]
								il.Emit(OpCodes.Ldc_I4, propertyIndex); // Load the ordinal index of the column [record, ordinal]

								// It's either a non-ISqlType or a value type
								il.Emit(OpCodes.Ldloc_0); // Load the object [record, ordinal, object]
								il.Emit(OpCodes.Callvirt, prop.GetGetMethod()); // Load the property value [record, ordinal, value]
								il.Emit(OpCodes.Callvirt, sqlDataRecordSetters[prop.PropertyType]); // Set value []
							}
						}

						propertyIndex++;
					}
					catch (KeyNotFoundException)
					{
						throw new TypeNotSupportedException(prop.PropertyType);
					}
				}

				// Return
				il.Emit(OpCodes.Ret);

				// Cache type handler
				typeHandler = new TypeHandler {
					Schema = schema,
					SetRecordValues = (Action<SqlDataRecord, object>)dm.CreateDelegate(typeof(Action<SqlDataRecord, object>))
				};
				typeHandlers.AddOrUpdate(objectType, typeHandler, (k, v) => v);
			}

			// This is now the base SqlDataRecord that contains the schema without values. We'll be reusing this
			// while constantly overwriting the values, for each record.
			var record = new SqlDataRecord(typeHandler.Schema);
			
			foreach (object row in rows)
			{
				typeHandler.SetRecordValues(record, row);
				yield return record;
			}
		}

		/// <summary>
		/// Enumerates all the source data and yields populated SqlDataRecords based on the source data. Right now this is based off of
		/// standard reflection. This needs to be optimized to avoid the reflection hit.
		/// </summary>
		public IEnumerator<SqlDataRecord> GetEnumerator()
		{
			// Cache rows aggressively to avoid the database waiting on us
			var rows = values.ToArray();

			if (rows.Length == 0)
				yield break;

			// Based on the type of the first value, we'll decide how to process the data
			var rowType = rows[0].GetType();

			// If the value is a SqlTypeHandler, we know it's a SqlType and we can process it as such
			if (typeof(SqlTypeHandler).IsAssignableFrom(rowType))
			{
				foreach (var row in processSqlTypes(rows))
					yield return row;

				yield break;
			}

			// If the value type is supported as an implicit type, we can just send the raw values out directly
			SqlTypeHandler implicitTypeHandler;
			if (unQueryDB.ClrTypeHandlers.TryGetValue(rowType, out implicitTypeHandler))
			{
				foreach (var row in processImplicitTypes(rows, implicitTypeHandler))
					yield return row;

				yield break;
			}

			// At this point we'll extract property values from the provided objects
			foreach (var row in processObjects(rows))
				yield return row;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}