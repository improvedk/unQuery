using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using unQuery.SqlTypes;

namespace unQuery
{
	internal class CachedType
	{
		internal SqlMetaData[] Schema;
		internal PropertyInfo[] Properties;
	}

	/// <summary>
	/// Datasource that yields SqlDataRecords for efficient streaming of table valued parameter data to SQL Server.
	/// </summary>
	internal class StructuredDynamicYielder : IEnumerable<SqlDataRecord>
	{
		private readonly IEnumerable<object> values;
		private static readonly ConcurrentDictionary<Type, CachedType> typeCache = new ConcurrentDictionary<Type, CachedType>();

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

		private IEnumerable<SqlDataRecord> processSqlTypes(object[] rows)
		{
			var record = new SqlDataRecord(((ITypeHandler)rows[0]).CreateMetaData("UnnamedColumn"));
			Type rowType = null;

			foreach (var row in rows)
			{
				// We have to detect potential type mismatches in case there are different ISqlTypes among the rows
				if (row != null)
					if (rowType == null)
						rowType = row.GetType();
					else if (rowType != row.GetType())
						throw new StructuredTypeMismatchException(row.GetType());

				record.SetValue(0, ((ISqlType)row).GetRawValue() ?? DBNull.Value);

				yield return record;
			}
		}

		private IEnumerable<SqlDataRecord> processImplicitTypes(object[] rows, ITypeHandler implicitTypeHandler)
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
			SqlDataRecord record = null;
			object[] recordValues = null;
			CachedType cachedType = null;

			foreach (object row in values)
			{
				// For the very first value, we'll first have to create the schema as an array of SqlMetaData
				if (record == null)
				{
					var valueType = row.GetType();

					// If type schema is not cached, we'll have to create it
					if (!typeCache.TryGetValue(valueType, out cachedType))
					{
						// To ensure we get properties in the declaration order, we need to sort by the MetaDataToken
						PropertyInfo[] properties = valueType.GetProperties().OrderBy(x => x.MetadataToken).ToArray();
						SqlMetaData[] schema = new SqlMetaData[properties.Length];

						// If no properties are found on the provided parameter object, then there's no schema & value to read
						if (schema.Length == 0)
							throw new ObjectHasNoPropertiesException("For an object to be used as a value for a Structured parameter, its properties need to match the SQL Server type. The provided object has no properties.");

						// For each property, we'll add it as a SqlMetaData to the schema array. We'll let the ISqlType create
						// the actual SqlMetaData value.
						int index = 0;
						foreach (PropertyInfo prop in properties)
						{
							try
							{
								if (typeof(ITypeHandler).IsAssignableFrom(prop.PropertyType))
								{
									ITypeHandler value = (ITypeHandler)prop.GetValue(row);
									schema[index++] = value.CreateMetaData(prop.Name);
								}
								else
									schema[index++] = unQueryDB.ClrTypeHandlers[prop.PropertyType].CreateMetaData(prop.Name);
							}
							catch (KeyNotFoundException)
							{
								throw new TypeNotSupportedException(prop.PropertyType);
							}
						}

						// Cache the new schema
						cachedType = new CachedType
						{
							Properties = properties,
							Schema = schema
						};
						typeCache.AddOrUpdate(valueType, cachedType, (k, v) => v);

						// This is now the base SqlDataRecord that contains the schema without values. We'll be reusing this
						// while constantly overwriting the values, for each record.
						record = new SqlDataRecord(schema);
					}
					else
						record = new SqlDataRecord(cachedType.Schema);

					// This is the array that'll be used to efficiently set all values on the data record at once
					recordValues = new object[record.FieldCount];
				}

				// Populate values & yield
				for (int i = 0; i < recordValues.Length; i++)
				{
					var prop = cachedType.Properties[i];
					var columnValue = prop.GetValue(row);
					var columnValueSqlType = columnValue as ISqlType;

					// If column value is an ISqlType, get the raw value rather than the ISqlType itself
					if (columnValueSqlType != null)
						recordValues[i] = columnValueSqlType.GetRawValue() ?? DBNull.Value;
					else
						recordValues[i] = columnValue ?? DBNull.Value;
				}

				record.SetValues(recordValues);

				// Once the values have been overwritten, we can now yield it before rewriting the values again
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

			// Based on the type of hte first value, we'll decide how to process the data
			var rowType = rows[0].GetType();

			// If the value is an ITypeHandler, we know it's an ISqlType and we can process it as such
			if (typeof(ITypeHandler).IsAssignableFrom(rowType))
			{
				foreach (var row in processSqlTypes(rows))
					yield return row;
				
				yield break;
			}

			// If the value type is supported as an implicit type, we can just send the raw values out directly
			ITypeHandler implicitTypeHandler;
			if (unQueryDB.ClrTypeHandlers.TryGetValue(rowType, out implicitTypeHandler))
			{
				foreach (var row in processImplicitTypes(rows, implicitTypeHandler))
					yield return row;

				yield break;
			}

			// At this point we'll utilize reflection to pull the property values from the provided objects
			foreach (var row in processObjects(rows))
				yield return row;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}