using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using unQuery.SqlTypes;

namespace unQuery
{
	internal class StructuredDynamicYielder : IEnumerable<SqlDataRecord>
	{
		private readonly IEnumerable<object> values;

		internal StructuredDynamicYielder(IEnumerable<object> values)
		{
			if (values == null)
				throw new ArgumentException("Values can't be null.");

			this.values = values;
		}

		public IEnumerator<SqlDataRecord> GetEnumerator()
		{
			SqlDataRecord sdr = null;
			SqlMetaData[] schema = null;
			PropertyInfo[] properties = null;

			foreach (dynamic value in values)
			{
				// Initialize schema
				if (sdr == null)
				{
					properties = value.GetType().GetProperties();
					schema = new SqlMetaData[properties.Length];

					// If no properties are found on the provided parameter object, then there's no schema & value to read
					if (schema.Length == 0)
						throw new ObjectHasNoPropertiesException("For an object to be used as a value for a Structured parameter, its properties need to match the SQL Server type. The provided object has no properties.");

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

					sdr = new SqlDataRecord(schema);
				}

				// Populate values & yield
				for (int i = 0; i < schema.Length; i++)
				{
					var col = schema[i];
					var prop = properties[i];
					var columnValue = prop.GetValue(value);
					var columnValueSqlType = columnValue as ISqlType;

					if (columnValueSqlType != null)
						columnValue = columnValueSqlType.GetRawValue();

					try
					{
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

				yield return sdr;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}