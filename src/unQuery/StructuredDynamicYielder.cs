using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using unQuery.SqlTypes;

namespace unQuery
{
	/// <summary>
	/// Datasource that yields SqlDataRecords for efficient streaming of table valued parameter data to SQL Server.
	/// </summary>
	internal class StructuredDynamicYielder : IEnumerable<SqlDataRecord>
	{
		private readonly IEnumerable<object> values;

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
		/// Enumerates all the source data and yields populated SqlDataRecords based on the source data.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<SqlDataRecord> GetEnumerator()
		{
			SqlDataRecord sdr = null;
			SqlMetaData[] schema = null;
			PropertyInfo[] properties = null;

			foreach (dynamic value in values)
			{
				// For the very first value, we'll first have to create the schema as an array of SqlMetaData
				if (sdr == null)
				{
					properties = value.GetType().GetProperties();
					schema = new SqlMetaData[properties.Length];

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
							schema[index++] = unQueryDB.ClrTypeHandlers[prop.PropertyType].CreateSqlMetaData(prop.Name);
						}
						catch (KeyNotFoundException)
						{
							throw new TypeNotSupportedException(prop.PropertyType);
						}
					}

					// This is now the base SqlDataRecord that contains the schema without values. We'll be reusing this
					// while constantly overwriting the values, for each record.
					sdr = new SqlDataRecord(schema);
				}

				// Populate values & yield
				for (int i = 0; i < schema.Length; i++)
				{
					var col = schema[i];
					var prop = properties[i];
					var columnValue = prop.GetValue(value);
					var columnValueSqlType = columnValue as ISqlType;

					// If column value is an ISqlType, get the raw value rather than the ISqlType itself
					if (columnValueSqlType != null)
						columnValue = columnValueSqlType.GetRawValue();

					try
					{
						// If the value is null, we'll have to write a DBNull and if it's not, we'll let the TypeHandler set the value
						if (columnValue == null)
							sdr.SetDBNull(i);
						else
							unQueryDB.SqlDbTypeHandlers[col.SqlDbType].SetDataRecordValue(i, sdr, columnValue);
					}
					catch (KeyNotFoundException)
					{
						throw new TypeNotSupportedException(col.SqlDbType.ToString());
					}
				}

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