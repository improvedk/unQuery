using System;

namespace unQuery.SqlTypes
{
	public abstract class SqlType
	{
		internal static object GetDBNullableValue(object value)
		{
			return value ?? DBNull.Value;
		}
	}
}