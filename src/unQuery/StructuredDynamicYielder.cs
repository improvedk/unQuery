using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

					int keyCount = 0;
					foreach (PropertyInfo prop in properties)
					{
						var propValue = prop.GetValue(value);
						var propSqlType = propValue as ISqlType;
						
						if (propSqlType != null)
							schema[keyCount++] = new SqlMetaData(prop.Name, propSqlType.GetSqlDbType());
						else
						{
							try
							{
								schema[keyCount++] = new SqlMetaData(prop.Name, unQueryDB.ClrToSqlDbTypeMap[prop.PropertyType]);
							}
							catch (KeyNotFoundException)
							{
								throw new TypeNotSupportedException(prop.PropertyType);
							}
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

					if (columnValue == null)
						sdr.SetDBNull(i);
					else
					{
						switch (col.SqlDbType)
						{
							case SqlDbType.Bit:
								sdr.SetBoolean(i, (bool)columnValue);
								break;

							case SqlDbType.TinyInt:
								sdr.SetByte(i, (byte)columnValue);
								break;

							case SqlDbType.Int:
								sdr.SetInt32(i, (int)columnValue);
								break;

							default:
								throw new TypeNotSupportedException(col.SqlDbType.ToString());
						}
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