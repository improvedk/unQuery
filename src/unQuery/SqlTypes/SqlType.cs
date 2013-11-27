using System;

namespace unQuery.SqlTypes
{
	internal static class TypeHelper
	{
		internal static object GetDBNullableValue(object value)
		{
			return value ?? DBNull.Value;
		}
	}
}