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

					// If no properties are found on the provided parameter object, then there's no schema & value to read
					if (schema.Length == 0)
						throw new ObjectHasNoPropertiesException("For an object to be used as a value for a Structured parameter, its properties need to match the SQL Server type. The provided object has no properties.");

					int keyCount = 0;
					foreach (PropertyInfo prop in properties)
					{
						var propValue = prop.GetValue(value);
						var propSqlType = propValue as ISqlType;
						
						if (propSqlType != null)
						{
							if (propSqlType is SqlVarChar || propSqlType is SqlNVarChar)
								schema[keyCount++] = new SqlMetaData(prop.Name, propSqlType.GetSqlDbType(), -1);
							else
								schema[keyCount++] = new SqlMetaData(prop.Name, propSqlType.GetSqlDbType());
						}
						else
						{
							try
							{
								SqlDbType dbType = unQueryDB.ClrToSqlDbTypeMap[prop.PropertyType];

								switch (dbType)
								{
									case SqlDbType.VarChar:
									case SqlDbType.NVarChar:
										schema[keyCount++] = new SqlMetaData(prop.Name, dbType, -1);
										break;

									default:
										schema[keyCount++] = new SqlMetaData(prop.Name, dbType);
										break;
								}
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

							case SqlDbType.BigInt:
								sdr.SetInt64(i, (long)columnValue);
								break;

							case SqlDbType.SmallInt:
								sdr.SetInt16(i, (short)columnValue);
								break;

							case SqlDbType.UniqueIdentifier:
								sdr.SetGuid(i, (Guid)columnValue);
								break;

							case SqlDbType.NVarChar:
							case SqlDbType.VarChar:
								sdr.SetString(i, (string)columnValue);
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