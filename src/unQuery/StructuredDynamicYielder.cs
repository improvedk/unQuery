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

		/// <summary>
		/// Enumerates all the source data and yields populated SqlDataRecords based on the source data. Right now this is based off of
		/// standard reflection. This needs to be optimized to avoid the reflection hit.
		/// </summary>
		public IEnumerator<SqlDataRecord> GetEnumerator()
		{
			SqlDataRecord sdr = null;
			object[] recordValues = null;
			CachedType cachedType = null;

			foreach (object row in values)
			{
				// For the very first value, we'll first have to create the schema as an array of SqlMetaData
				if (sdr == null)
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
						cachedType = new CachedType {
							Properties = properties,
							Schema = schema
						};
						typeCache.AddOrUpdate(valueType, cachedType, (k, v) => v);

						// This is now the base SqlDataRecord that contains the schema without values. We'll be reusing this
						// while constantly overwriting the values, for each record.
						sdr = new SqlDataRecord(schema);
					}
					else
						sdr = new SqlDataRecord(cachedType.Schema);

					// This is the array that'll be used to efficiently set all values on the data record at once
					recordValues = new object[sdr.FieldCount];
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

				sdr.SetValues(recordValues);

				// Once the values have been overwritten, we can now yield it before rewriting the values again
				yield return sdr;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}